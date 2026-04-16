import { ReactNode, useEffect, useMemo, useState } from "react";
import Alert from "@/components/Alert";
import LoadingPanel from "@/components/LoadingPanel";
import { shiftClient } from "@/api/shiftClient";
import { urlHelper } from "@/helpers/urlHelper";
import { cssHelper } from "@/helpers/cssHelper";
import { validateLanguage } from "@/helpers/language";
import { defaultSiteProviderContextData, SiteProviderContext } from "./SiteProviderContext";

interface Props {
    children?: ReactNode;
}

interface State {
    needToLoad: boolean;
    isLoading: boolean;
    error: string | null;
}

export default function SiteProvider({ children }: Props) {
    const [state, setState] = useState<State>({
        needToLoad: true,
        isLoading: true,
        error: null,
    });

    const [contextData, setContextData] = useState(defaultSiteProviderContextData);

    const methods = useMemo(()=> ({
        refreshSiteSetting() {
            setState({
                needToLoad: true,
                isLoading: false,
                error: null,
            });
        },
    }), []);

    useEffect(() => {
        if (!state.needToLoad) {
            return;
        }

        setState({
            needToLoad: false,
            isLoading: true,
            error: null
        });

        run();

        async function run() {
            try {
                const newSiteSetting = await shiftClient.me.context(false);
                validateLanguage(newSiteSetting.CurrentLanguage);
                validateLanguage(newSiteSetting.SupportedLanguages);
                setState({
                    needToLoad: false,
                    isLoading: false,
                    error: null,
                });
                setContextData({
                    siteSetting: newSiteSetting,
                    ...methods
                });
                urlHelper.setOrg(newSiteSetting.OrganizationCode);
                cssHelper.setShiftCssFile(newSiteSetting.StylePath);
            } catch (err) {
                setState({
                    needToLoad: false,
                    isLoading: false,
                    error: String(err),
                });
            }
        }
    }, [state.needToLoad, methods]);

    if (state.isLoading) {
        return <LoadingPanel />;
    }

    if (state.error) {
        return (
            <Alert alertType="error" className="my-4 mx-4">
                {state.error ?? "Unknown error while loading site settings"}
            </Alert>
        );
    }

    return (
        <SiteProviderContext.Provider value={contextData}>
            {children}
        </SiteProviderContext.Provider>
    )
}