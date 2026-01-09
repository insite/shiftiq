import { errorHelper } from "@/helpers/errorHelper";
import { ReactNode } from "react";
import { FieldErrors } from "react-hook-form";
import Alert from "./Alert";

interface Props {
    errors: FieldErrors;
}

export default function ValidationSummary({ errors }: Props) {
    const fieldNames = Object.keys(errors);
    if (!fieldNames.length) {
        return;
    }

    fieldNames.sort();

    const errorMessages: ReactNode[] = [];

    for (const fieldName of fieldNames) {
        const errorMessage = errorHelper.getErrorMessage(fieldName, errors[fieldName]);
        if (errorMessage) {
            errorMessages.push(errorMessage);
        }
    }
    return (
        <Alert
            alertType="error"
            icon={null}
            className="validation-summary"
            messages={errorMessages}
        />
    );
}
