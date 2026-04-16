import FormSection from "@/components/form/FormSection";
import GradebookOutline_Setup from "./GradebookOutline_Setup";
import { GradebookOutlineModel } from "./GradebookOutlineModel";
import { shiftClient } from "@/api/shiftClient";
import { dateTimeHelper } from "@/helpers/date/dateTimeHelper";
import { useSiteProvider } from "@/contexts/site/SiteProviderContext";
import { TimeZoneId } from "@/helpers/date/timeZones";
import { ApiCommand } from "@/api/controllers/command/ApiCommand";
import { useSaveAction } from "@/hooks/useSaveAction";
import { useStatusProvider } from "@/contexts/status/StatusProviderContext";
import { ApiLockGradebook } from "@/api/controllers/command/gradebook/ApiLockGradebook";
import { ApiUnlockGradebook } from "@/api/controllers/command/gradebook/ApiUnlockGradebook";
import { useOutlineForm } from "@/hooks/useOutlineForm";
import FormTabs from "@/components/form/FormTabs";
import FormTab from "@/components/form/FormTab";
import Icon from "@/components/icon/Icon";

export default function GradebookOutline() {
    const { siteSetting } = useSiteProvider();
    const { id, model, reloadModel } = useOutlineForm(id => load(id, siteSetting.TimeZoneId));
    const { addStatus, removeStatus } = useStatusProvider();
    const { isSaving, runSave } = useSaveAction();

    async function handleLock(isLocked: boolean) {
        if (!model) {
            return;
        }

        const confirmMessage = isLocked
            ? "Locking this gradebook will prevent additional edits. Learners in any attached courses will no longer be able to progress. Manually granted achievements can no longer be granted. Are you sure you want to proceed?"
            : "Unlocking this gradebook will allow changes to be made to existing scores, learners, and other settings. Are you sure you want to proceed?";

        if (!window.confirm(confirmMessage)) {
            return;
        }

        const command: ApiCommand = isLocked
            ? new ApiLockGradebook(id)
            : new ApiUnlockGradebook(id)

        if (!await runSave(async () => {
            await shiftClient.command.send(command);
            await reloadModel();
        })) {
            removeStatus();
            return;
        }

        const message = isLocked
            ? "Gradebook is locked"
            : "Gradebook is unlocked";

        addStatus("success", message);
    }

    return (
        <FormTabs defaultTab="setup">
            <FormTab tab="gradeItems" icon={{ style: "regular", name: "list-ul" }} title="Grade Items">
                <FormSection title="Grade Items">
                    (Grade Items)
                </FormSection>
            </FormTab>
            <FormTab tab="setup" icon={{ style: "regular", name: "spell-check" }} title={
                <>
                    Gradebook Setup
                    {model?.isLocked === true && <Icon style="solid" name="lock" className="text-danger ms-2" title="Locked" />}
                </>
            }>
                <GradebookOutline_Setup model={model} isSaving={isSaving} onLock={handleLock} />
            </FormTab>
        </FormTabs>
    );
}

async function load(gradebookId: string, timeZoneId: TimeZoneId): Promise<GradebookOutlineModel> {
    const apiModel = await shiftClient.gradebook.retrieve(gradebookId);

    const [
        achievement,
        event
    ] = await Promise.all([
        apiModel.AchievementId ? shiftClient.achievement.retrieve(apiModel.AchievementId) : null,
        apiModel.EventId ? shiftClient.event.retrieve(apiModel.EventId) : null,
    ]);

    return {
        gradebookId,
        gradebookTitle: apiModel.GradebookTitle,
        gradebookType: apiModel.GradebookType,
        isLocked: apiModel.IsLocked,
        achievementId: apiModel.AchievementId ?? null,
        achievementTitle: achievement?.AchievementTitle ?? null,
        eventId: apiModel.EventId ?? null,
        eventTitle: event?.EventTitle ?? null,
        eventScheduledStart: dateTimeHelper.parseServerDateTime(event?.EventScheduledStart, timeZoneId),
        eventScheduledEnd: dateTimeHelper.parseServerDateTime(event?.EventScheduledEnd, timeZoneId),
        periodId: apiModel.PeriodId ?? null,
        periodTitle: null,
        frameworkId: apiModel.FrameworkId ?? null,
        frameworkTitle: null,
        reference: apiModel.Reference ?? null,
    }
}