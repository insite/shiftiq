import { useEffect, useState } from "react";
import { ProviewSettings, useProviewProctor } from "./useProviewProctor";
import { urlHelper } from "@/helpers/urlHelper";
import LoadingPanel from "@/components/LoadingPanel";
import { useSearchParams } from "react-router";
import { shiftClient } from "@/api/shiftClient";
import Alert from "@/components/Alert";
import { ApiError } from "@/api/apiError";
import ActionLink from "@/components/ActionLink";

export default function AttemptAnswer() {
    const [searchParams] = useSearchParams();
    const form = searchParams.get("form");
    const resource = searchParams.get("resource");
    const attempt = searchParams.get("attempt");

    const [isLoadingExam, setIsLoadingExam] = useState(true);
    const [errorMessage, setErrorMessage] = useState<string | null>(null);
    const [settings, setSettings] = useState<ProviewSettings | null>(null);
    const [completeRedirect, setCompleteRedirect] = useState<string | null>(null);

    const { iframeRef, stopProctoring } = useProviewProctor(settings);

    useEffect(() => {
        if (!attempt) {
            setErrorMessage("Invalid URL");
            return;
        }

        run();

        async function run() {
            try {
                const apiSession = await shiftClient.proview.retrieveSession(form, resource, attempt!);
                if (!apiSession) {
                    return;
                }

                if (apiSession.RedirectUrl) {
                    window.location.href = urlHelper.getActionUrl(apiSession.RedirectUrl);
                    return;
                } else if (!apiSession.Data) {
                    throw new Error();
                }

                const data = apiSession.Data;

                const examUrl = form
                    ? `/ui/portal/assessments/attempts/answer?form=${form}&attempt=${attempt}`
                    : `/ui/portal/assessments/attempts/answer?resource=${resource}&attempt=${attempt}`;

                const newSettings = {
                    initScriptUrl: data.InitScriptUrl,
                    proviewToken: data.ProviewToken,
                    sessionTitle: data.SessionTitle,
                    sessionType: "ai_proctor",
                    profileId: data.ProfileId,
                    session: data.Session,
                    examUrl: urlHelper.getActionUrl(examUrl),
                    first_name: data.FirstName,
                    last_name: data.LastName,
                    middle_name: data.MiddleName ?? null,
                    email: data.Email,

                    onProctoringInitialized: async (uuid: string) => {
                        try {
                            await shiftClient.proview.modifyProviewSessionId(data.Session, uuid);
                        } catch {
                            setErrorMessage("Failed to initialize proctoring session. Please refresh the page and try again.");
                        }
                    },
                };

                setSettings(newSettings);
            } catch (err) {
                const message = err instanceof ApiError && err.status === 401
                    ? "Your session has expired. Please sign in again."
                    : "Unexpected error while requesting exam data.";

                setErrorMessage(message);
            }
        }
    }, [form, resource, attempt, iframeRef]);

    useEffect(() => {
        window.addEventListener("message", handleMessage);

        return () => {
            window.removeEventListener("message", handleMessage);
        };

        async function handleMessage(event: MessageEvent) {
            if (event.origin !== window.location.origin
                && event.origin !== urlHelper.getActionUrl("")
            ) {
                return;
            }

            if (event.data?.type === "attempt-complete") {
                await stopProctoring();
                setCompleteRedirect(event.data.redirectUrl);
                window.location.href = urlHelper.getActionUrl(event.data.redirectUrl);
            }
        }
    }, [iframeRef, stopProctoring]);

    function handleIframeLoad() {
        setIsLoadingExam(false);
    };

    if (errorMessage) {
        return (
            <div className="m-4">
                <Alert alertType="error">{errorMessage}</Alert>
            </div>
        );
    }

    if (completeRedirect) {
        return (
            <div className="m-4">
                <ActionLink href={completeRedirect}>{completeRedirect}</ActionLink>
            </div>
        );
    }

    return (
        <>
            {isLoadingExam && <LoadingPanel text=" " />}

            <iframe
                ref={iframeRef}
                src={null as unknown as string}
                style={{
                    width: "100vw",
                    height: "100vh",
                    border: 0,
                    display: isLoadingExam ? "none" : "block",
                }}
                onLoad={handleIframeLoad}
            />
        </>
    );
}