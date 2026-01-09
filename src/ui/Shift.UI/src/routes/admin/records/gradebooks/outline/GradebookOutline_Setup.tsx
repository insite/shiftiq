import FormCard from "@/components/form/FormCard";
import FormSection from "@/components/form/FormSection";
import { GradebookOutlineModel } from "./GradebookOutlineModel";
import FormField from "@/components/form/FormField";
import { textHelper } from "@/helpers/textHelper";
import LinkOrNone from "@/components/LinkOrNone";
import { dateTimeHelper } from "@/helpers/date/dateTimeHelper";
import { translate } from "@/helpers/translate";
import Button from "@/components/Button";
import ButtonSpacer from "@/components/ButtonSpacer";

interface Props {
    model: GradebookOutlineModel | null;
    isSaving: boolean;
    onLock: (isLoked: boolean) => void;
}

export default function GradebookOutline_Setup({ model, isSaving, onLock }: Props) {
    const changeUrl = model?.isLocked === false ? `/ui/admin/records/gradebooks/change?gradebook=${model.gradebookId}` : null;
    const isDisabled = !model || isSaving;

    return (
        <FormSection title="Grade Items">
            <div className="mb-3 d-flex gap-1">
                <Button variant="new" href="/ui/admin/records/gradebooks/open" disabled={isDisabled} />
                <ButtonSpacer />
                {model?.isLocked === true && <Button variant="unlock" isLoading={isDisabled} onClick={() => onLock(false)} />}
                {model?.isLocked === false && <Button variant="lock" isLoading={isDisabled} onClick={() => onLock(true)} />}
                <ButtonSpacer />
                <Button variant="download" text="Download JSON" disabled={isDisabled} href={`/ui/admin/records/gradebooks/download?gradebook=${model?.gradebookId}`} />
                {model?.isLocked === false && <Button variant="delete" disabled={isDisabled} href={`/client/admin/records/gradebooks/delete/${model.gradebookId}`} />}
            </div>
            <div className="d-flex gap-6">

                <FormCard hasShadow={false} hasBottomMargin={false} className="w-100">
                    <FormField
                        label="Gradebook Title"
                        description="A descriptive title for this gradebook."
                        editHref={model?.isLocked === false ? `/client/admin/records/gradebooks/rename/${model.gradebookId}` : null}
                        editTitle="Rename Gradebook"
                    >
                        {textHelper.none(model?.gradebookTitle)}
                    </FormField>
                    <FormField
                        label="Achievement"
                        description="The achievement granted for successful completion of the items in this gradebook."
                        editHref={changeUrl}
                        editTitle="Change Achievement"
                    >
                        <LinkOrNone
                            href={`/ui/admin/records/achievements/outline?id=${model?.achievementId}`}
                            text={model?.achievementTitle}
                        />
                    </FormField>
                    <FormField
                        label="Class"
                        description="This class contains the registrations for learners tracked in this gradebook."
                        editHref={changeUrl}
                        editTitle="Change Class"
                    >
                        <LinkOrNone
                            href={`/ui/admin/events/classes/outline?event=${model?.eventId}`}
                            text={model?.eventTitle}
                        />
                        {model?.eventScheduledStart && (
                            <div>
                                {translate("Scheduled")}: {dateTimeHelper.formatDate(model.eventScheduledStart.date, "mmm d, yyyy")}
                                {model.eventScheduledEnd && (
                                    ` - ${dateTimeHelper.formatDate(model.eventScheduledEnd.date, "mmm d, yyyy")}`
                                )}
                            </div>
                        )}
                    </FormField>
                    <FormField label="Period">
                        {textHelper.none(model?.periodTitle)}
                    </FormField>
                    <FormField label="Reference">
                        {textHelper.none(model?.reference)}
                    </FormField>
                </FormCard>

                <FormCard hasShadow={false} hasBottomMargin={false} className="w-100">
                    <FormField label="Current Status" description="Changes to a locked gradebook are not permitted.">
                        {model ? (
                            model.isLocked ? (
                                <span className='badge bg-danger'><i className='far fa-lock'></i> Locked</span>
                            ) : (
                                <span className='badge bg-success'><i className='far fa-lock-open'></i> Unlocked</span>
                            )
                        ) : (
                            textHelper.none()
                        )}
                    </FormField>
                    <FormField label="Include" description="Track scores, standards, or both">
                        <label className="me-2">
                            <input type="checkbox" disabled checked={textHelper.in(model?.gradebookType, ["Scores", "ScoresAndStandards"])} />
                            Scores
                        </label>
                        <label>
                            <input type="checkbox" disabled checked={textHelper.in(model?.gradebookType, ["Standards", "ScoresAndStandards"])} />
                            Standards
                        </label>
                    </FormField>
                    {model && textHelper.in(model.gradebookType, ["Standards", "ScoresAndStandards"]) && (
                        <FormField label="Include" description="Track scores, standards, or both">
                            <LinkOrNone
                                href={`/ui/admin/standards/edit?id=${model.frameworkId}`}
                                text={model.frameworkTitle}
                            />
                        </FormField>
                    )}
                </FormCard>

            </div>
        </FormSection>
    );
}