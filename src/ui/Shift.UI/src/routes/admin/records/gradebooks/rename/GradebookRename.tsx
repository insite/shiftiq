import { ApiRenameGradebook } from "@/api/controllers/command/gradebook/ApiRenameGradebook";
import { shiftClient } from "@/api/shiftClient";
import Button from "@/components/Button";
import FormCard from "@/components/form/FormCard";
import FormField from "@/components/form/FormField";
import TextBox from "@/components/TextBox";
import ValidationSummary from "@/components/ValidationSummary";
import { translate } from "@/helpers/translate";
import { useEditForm } from "@/hooks/useEditForm";

interface FormValues {
    title: string | null;
}

export default function GradebookRename() {
    const {
        backUrl,
        isLoaded,
        isSaving,
        isDisabled,
        register,
        errors,
        handleSubmit,
    } = useEditForm(
        async (id: string) => {
            const model = await shiftClient.gradebook.retrieve(id);
            return {
                title: model.GradebookTitle
            }
        },
        async (id: string, values: FormValues) => {
            const command = new ApiRenameGradebook(id, values.title!);
            await shiftClient.command.send(command);
        }
    );

    return (
        <form autoComplete="off" onSubmit={handleSubmit}>
            <ValidationSummary errors={errors} />

            <FormCard>
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
                        readOnly={isDisabled}
                        maxLength={400}
                        className="w-50"
                        error={errors.title}
                    />
                </FormField>

                <FormField hasBottomMargin={false}>
                    <Button
                        variant="save"
                        className="me-2"
                        disabled={!isLoaded}
                        isLoading={isSaving}
                    />
                    <Button variant="cancel" href={backUrl} />
                </FormField>
            </FormCard>
        </form>
    );
}