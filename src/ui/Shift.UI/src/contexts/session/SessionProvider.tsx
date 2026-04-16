import { ReactNode, useEffect, useMemo, useState } from "react";
import { useSiteProvider } from "@/contexts/site/SiteProviderContext";
import { timerHelper } from "@/helpers/timerHelper";
import { sessionRefreshQueue } from "@/helpers/sessionRefreshQueue";
import { SessionProviderContext } from "./SessionProviderContext";

const BLINK_TIME_IN_MS = 5 * 60 * 1000;

interface TimeLeft {
    timeInMs: number;
    isBlinking: boolean;
}

interface Props {
    children?: ReactNode;
}

export default function SessionProvider({ children }: Props) {
    const { siteSetting: { SessionTimeoutMinutes: sessionTimeoutMinutes } } = useSiteProvider();

    const [timeLeft, setTimeLeft] = useState<TimeLeft>({ timeInMs: 0, isBlinking: false });
    const [sessionTimeoutInMs, setSessionTimeoutInMs] = useState(0);

    const contextData = useMemo(() => ({
        timeLeftInMs: timeLeft.timeInMs,
        isBlinking: timeLeft.isBlinking,
        sessionTimeoutInMs,
        setSessionTimeout: setSessionTimeoutInMs,
    }), [timeLeft.timeInMs, timeLeft.isBlinking, sessionTimeoutInMs]);

    useEffect(() => {
        timerHelper.syncWithStorage();

        window.addEventListener("storage", handleStorage);

        return () => window.removeEventListener("storage", handleStorage);

        function handleStorage() {
            timerHelper.syncWithStorage();
        }
    }, []);

    useEffect(() => {
        const newSessionTimeoutInMs = sessionTimeoutMinutes * 60 * 1000;

        setSessionTimeoutInMs(newSessionTimeoutInMs);

        if (!handleTimeLeft()) {
            return;
        }

        let handlerId: number | null = window.setInterval(handleInterval, 1000);

        return () => {
            if (handlerId !== null) {
                clearInterval(handlerId);
            }
        }

        function handleInterval() {
            if (!handleTimeLeft() && handlerId !== null) {
                clearInterval(handlerId);
                handlerId = null;
            }
        }

        function handleTimeLeft(): boolean {
            const startTimeInMs = timerHelper.getStartTimeInMs();
            const newTimeLeft = calcTimeLeft(startTimeInMs, newSessionTimeoutInMs);

            setTimeLeft(newTimeLeft);

            return newTimeLeft.timeInMs > 0;
        }
    }, [sessionTimeoutMinutes]);

    useEffect(() => {
        document.addEventListener("click", handleInteraction);
        document.addEventListener("keypress", handleInteraction);

        return () => {
            document.removeEventListener("click", handleInteraction);
            document.removeEventListener("keypress", handleInteraction);
        }

        function handleInteraction() {
            const startTimeInMs = timerHelper.getStartTimeInMs();
            const newTimeLeft = calcTimeLeft(startTimeInMs, sessionTimeoutInMs);

            sessionRefreshQueue.queue(newTimeLeft.timeInMs);
        }

    }, [sessionTimeoutInMs]);

    return (
        <SessionProviderContext.Provider value={contextData}>
            {children}
        </SessionProviderContext.Provider>
    )
}

function calcTimeLeft(startTimeInMs: number | null, sessionTimeoutInMs: number): TimeLeft {
    if (startTimeInMs === null) {
        return { timeInMs: 0, isBlinking: true };
    }

    const timeInMs = sessionTimeoutInMs - Date.now() + startTimeInMs;

    return {
        timeInMs: timeInMs < 0 ? 0 : timeInMs,
        isBlinking: timeInMs < BLINK_TIME_IN_MS,
    };
}