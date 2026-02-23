import { Language } from "@/helpers/language";
import { TimeZoneId } from "@/helpers/date/timeZones";

interface NavigationGroup {
    Title: string;
    MenuItems: {
        Url: string;
        Text: string;
        Icon: string;
    }[];
}

export interface ApiSiteSetting {
    TimeZoneId: TimeZoneId;
    OrganizationCode?: string | null;
    CompanyName: string;
    IsCmds: boolean;
    Home: {
        Text: string;
        Icon: string;
        Image: string | null | undefined;
        Url: string;
    },
    UserName?: string;
    IsAdministrator: boolean;
    IsOperator: boolean;
    IsMultiOrganization: boolean;
    ImpersonatorName?: string;
    MyDashboard: {
        Url: string;
        Text: string;
    };
    Permissions: {
        Accounts: boolean;
        Integrations: boolean;
        Settings: boolean;
    },
    Environment: {
        Name: string;
        Version: string;
        Color: string;
    };
    StylePath: string;
    AdminNavigationLogo: string;
    UserHostAddress: string;
    SessionTimeoutMinutes: number;
    NavigationGroups: NavigationGroup[];
    ShortcutGroups?: {
            Url: string;
            Text: string;
            Icon: string;
    }[];
    AdminNavigationGroups: NavigationGroup[];
    PlatformSearchDownloadMaximumRows: number;
    PartitionEmail: string;
    CurrentLanguage: Language;
    SupportedLanguages: Language[];
}