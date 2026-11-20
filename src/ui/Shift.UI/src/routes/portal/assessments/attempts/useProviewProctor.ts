/* eslint-disable @typescript-eslint/no-explicit-any */

import { useCallback, useEffect, useRef } from "react";

declare global {
    interface Window {
        TalviewProctor: string;
        tv: (...args: any[]) => void;
        ProctorClient3: {
            stop: (callback: () => void) => void;
            on: (events: string, callback: (data: any) => void) => void;
        };
    }
}

export interface ProviewSettings {
    initScriptUrl: string;
    proviewToken: string;
    sessionTitle: string;
    sessionType: string;
    profileId: string;
    session: string;
    examUrl: string;
    first_name: string;
    last_name: string;
    middle_name: string | null;
    email: string;

    onProctoringStopped?: () => void;
    onProctoringInitialized: (uuid: string) => void;
}

export function useProviewProctor(settings: ProviewSettings | null) {
    const iframeRef = useRef<HTMLIFrameElement>(null);
    const initializedRef = useRef(false);

    const stopProctoringCallback = useCallback(() => {
        return new Promise<void>((resolve, reject) => {
            try {
                stopProctoring(() => {
                    settings?.onProctoringStopped?.();
                    resolve();
                });
            } catch (err) {
                reject(err);
            }
        });
    }, [settings]);

    useEffect(() => {
        if (!iframeRef.current || !settings || initializedRef.current) {
            return;
        }

        initializedRef.current = true;

        const onProctoringStopped = settings.onProctoringStopped;

        window.addEventListener("message", handleMessage);

        const script = loadProviewScript(settings, iframeRef.current);

        return () => {
            window.removeEventListener("message", handleMessage);
            script.remove();
        };

        function handleMessage(event: MessageEvent) {
            if (event.origin !== window.location.origin) {
                return;
            }

            const data = event.data;

            if (data
                && typeof data === "object"
                && data["stop-proctoring"]
                && data.exit
            ) {
                stopProctoring(onProctoringStopped);
            }
        }
    }, [settings]);

    return {
        iframeRef,
        stopProctoring: stopProctoringCallback
    };
}

function loadProviewScript(settings: ProviewSettings, iframe: HTMLIFrameElement) {
    window.TalviewProctor = "tv";

    window.tv = window.tv ||
        function (...args: any[]) {
            const tv = window.tv as any;
            tv.q = tv.q || [];
            tv.q.push(args);
        };

    (window.tv as any).l = Date.now();

    const script = document.createElement("script");

    script.async = true;
    script.src = settings.initScriptUrl;

    script.onload = () => {
        initializeProctoring(settings, iframe);
    };

    script.onerror = () => {
        alert("Error loading Proctoring init script");
    };

    document.head.appendChild(script);

    return script;
}

function initializeProctoring(settings: ProviewSettings, iframe: HTMLIFrameElement) {
    window.tv("init", settings.proviewToken, {
        sessionTitle: settings.sessionTitle,
        sessionType: settings.sessionType,
        profileId: settings.profileId,
        session: settings.session,
        previewStyle: "position: fixed; bottom: 0px;",
        first_name: settings.first_name,
        last_name: settings.last_name,
        middle_name: settings.middle_name ?? undefined,
        email: settings.email,

        initCallback: (err: unknown, uuid: string) => {
            if (err) {
                console.error("Talview initialization failed:", uuid, err);
                return;
            }

            iframe.src = settings.examUrl;

            initializeEventListeners();

            settings.onProctoringInitialized(uuid);
        }
    });
}

function initializeEventListeners() {
    window.ProctorClient3.on(
        "log:event:all log:event:type:[:alert_type_id:]",
        (data) => {
            switch (data.alert_type_id) {
                case 254:
                    // Pause the test
                    break;
                case 255:
                    // Resume the test
                    break;
                case 256:
                    // Extend the test
                    /*
                    data.payload: {
                        extend_time: Number of minutes
                        extend_reason: Reason provided by Proctor for extension
                    }
                    */
                    break;
                case 144:
                    // Terminate the test
                    break;
            }
        }
    );
}

function stopProctoring(onProctoringStopped: (() => void) | null | undefined) {
    window.ProctorClient3?.stop(() => {
        onProctoringStopped?.();
    });
}