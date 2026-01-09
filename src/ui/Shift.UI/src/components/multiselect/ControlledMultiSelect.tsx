import { Control, Path, useController } from "react-hook-form";
import MultiSelect, { MultiSelectProps } from "./MultiSelect";
import { fieldRequiredMessage } from "@/helpers/errorHelper";

interface Props<Criteria extends object>
    extends Omit<MultiSelectProps, "ref" | "values" | "defaultValues" | "error" | "onBlur" | "onChange">
{
    name: Path<Criteria>;
    control: Control<Criteria>;
    required?: boolean;
    validate?: (values: string[]) => string | undefined;
}

export default function ControlledMultiSelect<Criteria extends object>({
    name,
    control,
    required,
    validate,
    ...multiSelectProps
}: Props<Criteria>) {
    const { field, fieldState } = useController({
        name,
        control,
        rules: required || validate
            ? {
                validate: (values: string[] | null | undefined) => {
                    if (required && !values?.length) {
                        return fieldRequiredMessage;
                    }
                    return validate?.(values ?? []);
                }
            } : undefined,
    });

    return (
        <MultiSelect
            {...multiSelectProps}
            ref={field.ref}
            values={field.value}
            error={fieldState.error}
            onChange={field.onChange}
            onBlur={field.onBlur}
        />
    )
}