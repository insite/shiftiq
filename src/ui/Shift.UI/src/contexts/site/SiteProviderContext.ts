import { ApiSiteSetting } from "@/api/models/ApiSiteSetting";
import { createContext, useContext } from "react";

interface ContextData {
    siteSetting: ApiSiteSetting;
    refreshSiteSetting: () => void;
}

export const defaultSiteProviderContextData: ContextData = {
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
    refreshSiteSetting() {},
}

export const SiteProviderContext = createContext<ContextData>(defaultSiteProviderContextData);

export function useSiteProvider() {
    return useContext(SiteProviderContext);
}