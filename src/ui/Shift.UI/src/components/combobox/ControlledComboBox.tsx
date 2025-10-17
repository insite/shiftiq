import { Control, Path, useController } from "react-hook-form";
import ComboBox, { ComboBoxProps } from "./ComboBox";
import { fieldRequiredMessage } from "@/helpers/errorHelper";

export interface ControlledComboBoxProps<Criteria extends object>
    extends Omit<ComboBoxProps, "buttonRef" | "name" | "value" | "defaultValue" | "error" | "onSelect" | "onBlur" | "onChange">
{
    name: Path<Criteria>;
    control: Control<Criteria>;
    required?: boolean;
    validate?: (value: string | null) => string | undefined;
}

export default function ControlledComboBox<Criteria extends object>({
    name,
    control,
    required,
    validate,
    ...comboBoxProps
}: ControlledComboBoxProps<Criteria>) {
    const { field, fieldState } = useController({
        name,
        control,
        rules: required || validate
            ? {
                validate: (value: string | null | undefined) => {
                    if (required && !value) {
                        return fieldRequiredMessage;
                    }
                    return validate?.(value ?? null);
                }
            } : undefined,
    });

    return (
        <ComboBox
            {...comboBoxProps}
            buttonRef={field.ref}
            name={field.name}
            value={field.value}
            error={fieldState.error}
            onChange={field.onChange}
            onBlur={field.onBlur}
        />
    )
}