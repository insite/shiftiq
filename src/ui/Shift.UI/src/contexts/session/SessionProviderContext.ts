import { createContext, useContext } from "react";

interface ContextData {
    timeLeftInMs: number;
    isBlinking: boolean;
    sessionTimeoutInMs: number;
    setSessionTimeout: (sessionTimeoutInMs: number) => void;
}

export const SessionProviderContext = createContext<ContextData>({
    timeLeftInMs: 0,
    isBlinking: false,
    sessionTimeoutInMs: 0,
    setSessionTimeout() {},
});

export function useSessionProvider() {
    return useContext(SessionProviderContext);
}