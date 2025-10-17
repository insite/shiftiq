import { shiftClient } from "@/api/shiftClient";
import { dateTimeHelper } from "@/helpers/date/dateTimeHelper";
import { useSiteProvider } from "@/contexts/SiteProvider";
import { TimeZoneId } from "@/helpers/date/timeZones";
import { GradebookDeleteModel } from "./GradebookDeleteModel";
import DeleteForm from "@/routes/_shared/forms/DeleteForm";
import FormField from "@/components/form/FormField";
import LinkOrNone from "@/components/LinkOrNone";
import FormCard from "@/components/form/FormCard";
import { translate } from "@/helpers/translate";
import { textHelper } from "@/helpers/textHelper";
import { ApiDeleteGradebook } from "@/api/controllers/command/gradebook/ApiDeleteGradebook";

export default function GradebookDelete() {
    const { siteSetting } = useSiteProvider();

    async function handelDelete(id: string) {
        const command = new ApiDeleteGradebook(id);
        await shiftClient.command.send(command);
        return true;
    }

    return (
        <DeleteForm
            entityName="gradebook"
            onLoad={id => load(id, siteSetting.TimeZoneId)}
            onDelete={handelDelete}
        >
            {model => (
                <FormCard hasShadow={false} title="Gradebook">
                    <FormField
                        label="Title"
                        description={`Created: ${dateTimeHelper.formatDateTime(model?.created, "mmm d, yyyy", " - ") ?? ""}`}
                    >
                        <LinkOrNone
                            href={`/client/admin/records/gradebooks/outline/${model?.gradebookId}`}
                            text={model?.gradebookTitle}
                        />
                    </FormField>
                    <FormField
                        label="Class"
                        description={
                            `${translate("Scheduled")}: ${dateTimeHelper.formatDate(model?.eventScheduledStart?.date, "mmm d, yyyy")}` +
                            `${model?.eventScheduledEnd ? " - " + dateTimeHelper.formatDate(model.eventScheduledEnd.date, "mmm d, yyyy") : ""}`
                        }
                    >
                        <LinkOrNone
                            href={`/ui/admin/events/classes/outline?event=${model?.eventId}`}
                            text={model?.eventTitle}
                        />
                    </FormField>
                    <FormField label="Achievement">
                        <LinkOrNone
                            href={`/ui/admin/records/achievements/outline?id=${model?.achievementId}`}
                            text={model?.achievementTitle}
                        />
                    </FormField>
                    <FormField label="Include" hasBottomMargin={false}>
                        <label className="me-2">
                            <input type="checkbox" disabled checked={textHelper.in(model?.gradebookType, ["Scores", "ScoresAndStandards"])} />
                            Scores
                        </label>
                        <label>
                            <input type="checkbox" disabled checked={textHelper.in(model?.gradebookType, ["Standards", "ScoresAndStandards"])} />
                            Standards
                        </label>
                    </FormField>
                </FormCard>
            )}
        </DeleteForm>
    );
}

async function load(gradebookId: string, timeZoneId: TimeZoneId): Promise<GradebookDeleteModel> {
    const apiModel = await shiftClient.gradebook.retrieve(gradebookId);

    const [
        achievement,
        event
    ] = await Promise.all([
        apiModel.AchievementIdentifier ? shiftClient.achievement.retrieve(apiModel.AchievementIdentifier) : null,
        apiModel.EventIdentifier ? shiftClient.event.retrieve(apiModel.EventIdentifier) : null,
    ]);

    return {
        gradebookId,
        gradebookTitle: apiModel.GradebookTitle,
        gradebookType: apiModel.GradebookType,
        created: dateTimeHelper.parseServerDateTime(apiModel.GradebookCreated, timeZoneId)!,
        achievementId: apiModel.AchievementIdentifier ?? null,
        achievementTitle: achievement?.AchievementTitle ?? null,
        eventId: apiModel.EventIdentifier ?? null,
        eventTitle: event?.EventTitle ?? null,
        eventScheduledStart: dateTimeHelper.parseServerDateTime(event?.EventScheduledStart, timeZoneId),
        eventScheduledEnd: dateTimeHelper.parseServerDateTime(event?.EventScheduledEnd, timeZoneId),
        consequences: [
            { name: "Gradebook", count: 1 },
            { name: "Something else", count: 0 },
        ]
    }
}