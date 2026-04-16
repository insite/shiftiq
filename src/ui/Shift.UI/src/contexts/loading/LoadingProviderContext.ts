import { createContext, useContext, useEffect, useMemo, useState } from "react";

interface ContextData {
    addLoading: (actionKey: string) => void;
    removeLoading: (actionKey: string) => void;
    clearLoading: (actionKey: string) => void;
}

export const LoadingProviderContext = createContext<ContextData>({
    addLoading: () => {},
    removeLoading: () => {},
    clearLoading: () => {},
});

let _counter = 1;

export function useLoadingProvider() {
    const { addLoading, removeLoading, clearLoading } = useContext(LoadingProviderContext);
    const [actionKey] = useState(() => `sequential-action-key.${_counter++}`);

    const methods = useMemo(() => ({
        addLoading: () => addLoading(actionKey),
        removeLoading: () => removeLoading(actionKey),
    }), [addLoading, removeLoading, actionKey]);

    useEffect(() => () => clearLoading(actionKey), [clearLoading, actionKey]);

    return methods;
}