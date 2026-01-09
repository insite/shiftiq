import { Control, Path, useController } from "react-hook-form";
import Finder, { FinderProps } from "./Finder";
import { fieldRequiredMessage } from "@/helpers/errorHelper";

export interface ControlledFinderProps<Criteria extends object>
    extends Omit<FinderProps, "ref" | "value" | "error" | "onChange" | "onBlur">
{
    name: Path<Criteria>;
    control: Control<Criteria>;
    required?: boolean;
    validate?: (value: string | null) => string | undefined;
}

export default function ControlledFinder<Criteria extends object>({
    name,
    control,
    required,
    validate,
    ...finderProps
}: ControlledFinderProps<Criteria>) {
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
        <Finder
            {...finderProps}
            ref={field.ref}
            value={field.value}
            error={fieldState.error}
            onChange={field.onChange}
            onBlur={field.onBlur}
        />
    )
}