import { createContext, ReactNode, useContext, useEffect, useMemo, useState } from "react";
import { ApiSiteSetting } from "@/api/models/ApiSiteSetting";
import Alert from "@/components/Alert";
import LoadingPanel from "@/components/LoadingPanel";
import { shiftClient } from "@/api/shiftClient";
import { urlHelper } from "@/helpers/urlHelper";
import { cssHelper } from "@/helpers/cssHelper";
import { validateLanguage } from "@/helpers/language";

interface ContextData {
    siteSetting: ApiSiteSetting;
    actionSubtitle: string | null;
    refreshSiteSetting: () => void;
    setActionSubtitle: (subtitle: string | null) => void;
}

const _defaultValue: ContextData = {
    siteSetting: {
        TimeZoneId: "UTC",
        OrganizationCode: "NA",
        CompanyName: "NA",
        IsCmds: false,
        Home: {
            Text: "N/A",
            Icon: "N/A",
            Image: "N/A",
            Url: "N/A",
        },
        IsAdministrator: false,
        IsOperator: false,
        IsMultiOrganization: false,
        PlatformSearchDownloadMaximumRows: 0,
        PartitionEmail: "NA",
        MyDashboard: {
            Url: "NA",
            Text: "NA"
        },
        Permissions: {
            Accounts: false,
            Integrations: false,
            Settings: false,
        },
        Environment: {
            Name: "NA",
            Version: "NA",
            Color: "NA",
        },
        StylePath: "NA",
        AdminNavigationLogo: "NA",
        UserHostAddress: "NA",
        SessionTimeoutMinutes: 0,
        NavigationGroups: [],
        AdminNavigationGroups: [],
        CurrentLanguage: "en",
        SupportedLanguages: ["en"],
    },
    actionSubtitle: null,
    refreshSiteSetting() {},
    setActionSubtitle() {},
};

const SiteProviderContext = createContext<ContextData>(_defaultValue);

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

    const [contextData, setContextData] = useState<ContextData>(_defaultValue);

    const methods = useMemo(()=> ({
        refreshSiteSetting() {
            setState({
                needToLoad: true,
                isLoading: false,
                error: null,
            });
        },
        setActionSubtitle(subtitle: string | null) {
            setContextData(prev => {
                if (prev.actionSubtitle === subtitle) {
                    return prev;
                }
                return {
                    ...prev,
                    actionSubtitle: subtitle,
                };
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
                    actionSubtitle: null,
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

// eslint-disable-next-line react-refresh/only-export-components
export function useSiteProvider() {
    return useContext(SiteProviderContext);
}