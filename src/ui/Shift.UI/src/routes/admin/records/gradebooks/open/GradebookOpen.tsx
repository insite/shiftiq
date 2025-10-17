import { ApiCreateGradebook } from "@/api/controllers/command/gradebook/ApiCreateGradebook";
import { shiftClient } from "@/api/shiftClient";
import Button from "@/components/Button";
import FormCard from "@/components/form/FormCard";
import FormField from "@/components/form/FormField";
import TextBox from "@/components/TextBox";
import ValidationSummary from "@/components/ValidationSummary";
import { translate } from "@/helpers/translate";
import { useNewForm } from "@/hooks/useNewForm";
import AchievementFinder from "@/routes/_shared/finders/AchievementFinder";

interface FormValues {
    title: string | null;
    achievementId: string | null;
}

export default function GradebookOpen() {
    const {
        backUrl,
        isSaving,
        register,
        control,
        errors,
        handleSubmit
    } = useNewForm<FormValues>(
        {
            title: "",
            achievementId: "",
        },

        "/client/records/gradebooks/outline/",

        async (values: FormValues) => {
            const id = window.crypto.randomUUID();
            const command = new ApiCreateGradebook(id, window.crypto.randomUUID(), values.title!, "Scores", null, null, null);
            await shiftClient.command.send(command);
            return id;
        }
    )

    return (
        <form autoComplete="off" onSubmit={handleSubmit}>
            <ValidationSummary errors={errors} />

            <FormCard>
                <div className="w-50">
                    <FormField
                        label={translate("Title")}
                        description={translate("The descriptive title for this gradebook.")}
                        required
                    >
                        <TextBox
                            {...register("title", {
                                required: true
                            })}
                            autoFocus
                            readOnly={isSaving}
                            maxLength={400}
                            error={errors.title}
                        />
                    </FormField>

                    <FormField
                        label={translate("Achievement")}
                        description={translate("The course content for this class.")}
                    >
                        <AchievementFinder
                            control={control}
                            name="achievementId"
                            disabled={isSaving}
                        />
                    </FormField>

                    <FormField hasBottomMargin={false}>
                        <Button
                            variant="save"
                            className="me-2"
                            isLoading={isSaving}
                        />
                        <Button variant="cancel" href={backUrl} />
                    </FormField>
                </div>
            </FormCard>
        </form>
    );
}