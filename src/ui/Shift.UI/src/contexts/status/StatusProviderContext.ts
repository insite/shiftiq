import { AlertType } from "@/components/Alert";
import { createContext, useContext, useEffect, useMemo, useState } from "react";

interface ContextData {
    addError: (key: string, error: unknown, details?: string | null) => void;
    removeError: (key: string) => void;
    clearErrors: () => void;
    addStatus: (key: string, alertType: AlertType, message: string) => void;
    removeStatus: (key: string) => void;
}

export const StatusProviderContext = createContext<ContextData>({
    addError() {},
    removeError() {},
    clearErrors() {},
    addStatus() {},
    removeStatus() {},
});

let _counter = 1;

export function useStatusProvider() {
    const { addError, removeError, addStatus, removeStatus } = useContext(StatusProviderContext);
    const [key] = useState(() => `sequential-status-key.${_counter++}`);

    const methods = useMemo(() => ({
        addError: (error: unknown, details?: string | null) => addError(key, error, details),
        removeError: () => removeError(key),
        addStatus: (alertType: AlertType, message: string) => addStatus(key, alertType, message),
        removeStatus: () => removeStatus(key),
    }), [addError, removeError, addStatus, removeStatus, key]);

    useEffect(() => () => {
        removeError(key);
        removeStatus(key);
    }, [removeError, removeStatus, key]);

    return methods;
}

export function getErrorDescription(error: unknown, details?: string | null) {
    let errorMessage = error instanceof Error && error.message
        ? error.message
        : typeof error === "string"
            ? error
            : "Unknown error";

    if (details) {
        errorMessage = details + ": " + errorMessage;
    }

    return errorMessage;
}