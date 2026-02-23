import { FieldError, FieldErrorsImpl, Merge } from "react-hook-form";
import { translate } from "./translate";

const _fieldRequiredMessage = translate("The field is required");

function getErrorMessageFromField(fieldName: string, error: FieldError): string | null {
    const fieldNameTitle = getFieldNameTitle(fieldName);

    if (error.message) {
        return error.message.startsWith("\0")
            ? error.message.substring(1)
            : fieldNameTitle
                ? `${fieldNameTitle}: ${error.message}`
                : error.message;
    }

    switch (error.type) {
        case "required":
            return fieldNameTitle
                ? `${fieldNameTitle}: ${_fieldRequiredMessage}`
                : _fieldRequiredMessage;
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
    createFieldRequiredMessage(fieldName: boolean | string): string {
        return typeof fieldName === "boolean"
            ? _fieldRequiredMessage
            : `\0${fieldName}: ${_fieldRequiredMessage}`;
    },

    getErrorMessage<T extends object>(
        fieldName: string,
        error: FieldError | Merge<FieldError, FieldErrorsImpl<T>> | null | undefined
    ): string | string[] | null {
        if (!error) {
            return null;
        }

        if (!(error instanceof Array)) {
            return getErrorMessageFromField(fieldName, error as FieldError);
        }

        const result: string[] = [];

        for (const fieldError of error) {
            if (!fieldError) {
                continue;
            }
            const errorMessage = getErrorMessageFromField(fieldName, fieldError.value);
            if (errorMessage) {
                result.push(errorMessage);
            }
        }

        return result.length > 0 ? result : null;
    },

    getErrorTooltip<T extends object>(error: FieldError | Merge<FieldError, FieldErrorsImpl<T>> | null | undefined): string | null {
        if (!error) {
            return null;
        }
        if (error.message) {
            return String(error.message);
        }

        switch (error.type) {
            case "required":
                return _fieldRequiredMessage;
        }

        throw new Error(`Unknown error type: ${error.type}`);
    },
}