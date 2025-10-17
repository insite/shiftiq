import { ReactNode } from "react";
import { FieldError, FieldErrorsImpl, Merge } from "react-hook-form";
import { translate } from "./translate";

export const fieldRequiredMessage = translate("The field is required");

function getErrorMessage<T extends object>(
    fieldName: string,
    error: FieldError | Merge<FieldError, FieldErrorsImpl<T>> | null | undefined
): ReactNode | null {
    if (!error) {
        return null;
    }

    const fieldNameTitle = getFieldNameTitle(fieldName);

    if (error.message) {
        return fieldNameTitle ? `${fieldNameTitle}: ${error.message}` : String(error.message);
    }

    switch (error.type) {
        case "required":
            return fieldNameTitle
                ? `${fieldNameTitle}: ${fieldRequiredMessage}`
                : fieldRequiredMessage;
    }

    throw new Error(`Unknown error type: ${error.type}`);
}

function getErrorTooltip<T extends object>(error: FieldError | Merge<FieldError, FieldErrorsImpl<T>> | null | undefined): string | null {
    if (!error) {
        return null;
    }
    if (error.message) {
        return String(error.message);
    }

    switch (error.type) {
        case "required":
            return fieldRequiredMessage;
    }

    throw new Error(`Unknown error type: ${error.type}`);
}

function getFieldNameTitle(fieldName: string | undefined) {
    if (!fieldName?.length) {
        return null;
    }

    let result = "";
    let start = 0;
    
    for (let i = 1; i < fieldName.length; i++) {
        const c = fieldName[i];

        if (c === c.toUpperCase()) {
            result = addNamePart(result, fieldName, start, i);
            start = i;
        }
    }

    result = addNamePart(result, fieldName, start, fieldName.length);

    return result;
}

function addNamePart(result: string, text: string, start: number, end: number) {
    const part = !start
        ? (
            start === end - 1
                ? text[start].toUpperCase()
                : text[start].toUpperCase() + text.substring(start + 1, end)
        )
        : text.substring(start, end);

    if (result) {
        result += " ";
    }
    return result + part;
}

export const errorHelper = {
    getErrorMessage,
    getErrorTooltip,
}