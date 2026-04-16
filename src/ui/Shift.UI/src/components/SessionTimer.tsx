import "./SessionTimer.css";

import { timerHelper } from "@/helpers/timerHelper";
import { useSessionProvider } from "@/contexts/session/SessionProviderContext";

export default function SessionTimer() {
    const { timeLeftInMs, isBlinking } = useSessionProvider();

    return (
        <span className={`sessionTimer ${isBlinking ? "blink_me text-danger" : ""}`}>
            {timerHelper.getTimerText(timeLeftInMs)}
        </span>
    );
}