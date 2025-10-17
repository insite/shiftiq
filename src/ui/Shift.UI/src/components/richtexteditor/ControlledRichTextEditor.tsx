import { Control, Path, useController } from "react-hook-form";
import { fieldRequiredMessage } from "@/helpers/errorHelper";
import RichTextEditor, { RichTextEditorProps } from "./RichTextEditor";
import { MultiLanguageText } from "./language";

interface Props<Fields extends object>
    extends Omit<RichTextEditorProps, "ref" | "markdown" | "defaultMarkdown" | "error" | "onBlur" | "onChange">
{
    name: Path<Fields>;
    control: Control<Fields>;
    required?: boolean;
    validate?: (value: MultiLanguageText | null) => string | undefined;
}

export default function ControlledDatePicker<Fields extends object>({
    name,
    control,
    required,
    validate,
    ...richTextEditorProps
}: Props<Fields>) {
    const { field, fieldState } = useController({
        name,
        control,
        rules: {
            validate: (value: MultiLanguageText | null | undefined) => {
                if (required && !value?.en) {
                    return fieldRequiredMessage;
                }
                return validate?.(value ?? null);
            }
        },
    });

    return (
        <RichTextEditor
            {...richTextEditorProps}
            ref={field.ref}
            markdown={field.value}
            error={fieldState.error}
            onChange={field.onChange}
            onBlur={field.onBlur}
        />
    )
}