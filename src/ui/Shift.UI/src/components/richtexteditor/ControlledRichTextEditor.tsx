import { Control, Path, useController } from "react-hook-form";
import { fieldRequiredMessage } from "@/helpers/errorHelper";
import RichTextEditor, { RichTextEditorProps } from "./RichTextEditor";
import { RichTextEditorValue } from "./RichTextEditorValue";

interface Props<Fields extends object>
    extends Omit<RichTextEditorProps, "ref" | "value" | "defaultValue" | "error" | "onBlur" | "onChange">
{
    name: Path<Fields>;
    control: Control<Fields>;
    required?: boolean;
    validate?: (value: RichTextEditorValue | null) => string | undefined;
}

export default function ControlledRichTextEditor<Fields extends object>({
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
            validate: (value: RichTextEditorValue | null | undefined) => {
                if (required && !value?.html?.en && !value?.markdown?.en) {
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
            value={field.value}
            error={fieldState.error}
            onChange={field.onChange}
            onBlur={field.onBlur}
        />
    )
}