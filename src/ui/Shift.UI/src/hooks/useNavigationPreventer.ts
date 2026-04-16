import { useCallback, useEffect } from "react";
import { useBlocker } from "react-router";

export function useNavigationPreventer(prevent: boolean): void {
    const shouldBlock = useCallback(() => prevent, [prevent]);
    const blocker = useBlocker(shouldBlock);

    useEffect(() => {
        if (blocker.state === "blocked") {
            if (window.confirm("Changes you made may not be saved. Leave the page?")) {
                blocker.proceed();
            } else {
                blocker.reset();
            }
        }
    }, [blocker.state, blocker]);

    useEffect(() => {
        if (!prevent) {
            return;
        }

        window.addEventListener("beforeunload", handleBeforeUnload);

        return () => window.removeEventListener("beforeunload", handleBeforeUnload);

        function handleBeforeUnload(e: BeforeUnloadEvent) {
            e.preventDefault();
            e.returnValue = "";
        }
    }, [prevent]);
}