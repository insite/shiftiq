import { Control, Path, useController } from "react-hook-form";
import { errorHelper } from "@/helpers/errorHelper";
import IntegerTextBox, { IntegerTextBoxProps } from "./IntegerTextBox";

interface Props<Fields extends object>
    extends Omit<IntegerTextBoxProps, "ref" | "value" | "defaultValue" | "error">
{
    name: Path<Fields>;
    control: Control<Fields>;
    required?: boolean | string;
    validate?: (value: number | null) => string | undefined;
    onChange?: (value: number | null) => number | null | undefined | void;
}

export default function ControlledIntegerTextBox<Fields extends object>({
    name,
    control,
    required,
    validate,
    onBlur,
    onChange,
    ...integerTextBoxProps
}: Props<Fields>) {
    const { field, fieldState } = useController({
        name,
        control,
        rules: {
            validate: (value: number | null | undefined) => {
                if (required && (value === null || value === undefined)) {
                    return errorHelper.createFieldRequiredMessage(required);
                }
                return validate?.(value ?? null);
            },
        },
    });

    return (
        <IntegerTextBox
            {...integerTextBoxProps}
            ref={field.ref}
            value={field.value}
            error={fieldState.error}
            onChange={value => {
                let newValue = onChange?.(value);
                if (newValue === undefined) {
                    newValue = value;
                }
                field.onChange(newValue);
            }}
            onBlur={() => {
                field.onBlur();
                onBlur?.();
            }}
        />
    )
}