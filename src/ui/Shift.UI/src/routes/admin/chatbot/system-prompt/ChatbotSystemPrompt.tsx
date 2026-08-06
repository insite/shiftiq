import { shiftClient } from "@/api/shiftClient";
import Button from "@/components/Button";
import FormCard from "@/components/form/FormCard";
import FormField from "@/components/form/FormField";
import TextArea from "@/components/TextArea";
import ValidationSummary from "@/components/ValidationSummary";
import { translate } from "@/helpers/translate";
import { useLoadAction } from "@/hooks/useLoadAction";
import { useSaveAction } from "@/hooks/useSaveAction";
import { useForm } from "react-hook-form";
import { useNavigate } from "react-router";

interface FormValues {
    systemPrompt: string | null;
}

export default function ChatbotSystemPrompt() {
    const navigate = useNavigate();

    const { isLoaded, runLoad } = useLoadAction(async () => {
        const model = await shiftClient.chatbot.retrieveSystemPrompt();
        return {
            systemPrompt: model?.SystemPrompt ?? null
        }
    });

    const { isSaving, runSave } = useSaveAction();
    const { isSaving: isResetting, runSave: runReset } = useSaveAction();

    const { register, handleSubmit, formState: { errors } } = useForm({
        defaultValues: runLoad
    });

    async function handleSave(values: FormValues) {
        if (await runSave(() => shiftClient.chatbot.modifySystemPrompt(values.systemPrompt))) {
            navigate("/client/admin/chatbot");
        }
    }

    async function handleReset() {
        if (!window.confirm("Are you sure to reset system prompt to the original message?")) {
            return;
        }

        if (await runReset(() => shiftClient.chatbot.modifySystemPrompt(null))) {
            navigate("/client/admin/chatbot");
        }
    }

    return (
        <form autoComplete="off" onSubmit={handleSubmit(handleSave)}>
            <ValidationSummary errors={errors} />

            <FormCard>
                <FormField label={translate("System Prompt")} required>
                    <TextArea
                        {...register("systemPrompt", {
                            required: true
                        })}
                        autoFocus
                        readOnly={isSaving}
                        maxLength={4000}
                        rows={20}
                        error={errors.systemPrompt}
                    />
                </FormField>

                <FormField hasBottomMargin={false}>
                    <Button
                        variant="save"
                        className="me-2"
                        disabled={!isLoaded || isResetting}
                        isLoading={isSaving}
                    />
                    <Button
                        variant="reset"
                        className="me-2"
                        disabled={!isLoaded || isSaving}
                        isLoading={isResetting}
                        type="button"
                        onClick={handleReset}
                    />
                    <Button variant="cancel" href="/client/admin/chatbot" />
                </FormField>
            </FormCard>
        </form>
    );
}