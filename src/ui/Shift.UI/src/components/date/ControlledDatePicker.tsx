import { Control, Path, useController } from "react-hook-form";
import DatePicker, { DatePickerProps } from "./DatePicker";
import { DateOrDateTime, isDateTimeInvalid } from "@/helpers/date/dateTimeTypes";
import { fieldRequiredMessage } from "@/helpers/errorHelper";
import { translate } from "@/helpers/translate";

interface Props<Criteria extends object>
    extends Omit<DatePickerProps, "ref" | "name" | "value" | "error" | "onBlur" | "onChange">
{
    name: Path<Criteria>;
    control: Control<Criteria>;
    required?: boolean;
    validate?: (value: DateOrDateTime) => string | undefined;
}

export default function ControlledDatePicker<Criteria extends object>({
    name,
    control,
    required,
    validate,
    showTime,
    ...datePickerProps
}: Props<Criteria>) {
    const { field, fieldState } = useController({
        name,
        control,
        rules: {
            validate: (value: DateOrDateTime | undefined) => {
                if (isDateTimeInvalid(value)) {
                    return showTime ? translate("Incorrect date and time format") : translate("Incorrect date format");
                }
                if (required && !value) {
                    return fieldRequiredMessage;
                }
                return validate?.(value ?? null);
            }
        },
    });

    return (
        <DatePicker
            {...datePickerProps}
            ref={field.ref}
            name={field.name}
            value={field.value}
            error={fieldState.error}
            showTime={showTime}
            onChange={field.onChange}
            onBlur={field.onBlur}
        />
    )
}