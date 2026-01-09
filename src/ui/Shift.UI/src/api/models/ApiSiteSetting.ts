import { Language } from "@/helpers/language";
import { TimeZoneId } from "@/helpers/date/timeZones";

export interface ApiSiteSetting {
    TimeZoneId: TimeZoneId;
    OrganizationCode?: string | null;
    CompanyName: string;
    IsCmds: boolean;
    CmdsHomeLink: string;
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
    };
    PlatformLogoSrc: string;
    StylePath?: string | null;
    AdminNavigationLogo: string;
    UserHostAddress: string;
    SessionTimeoutMinutes: number;
    NavigationGroups: {
        Title: string;
        MenuItems: {
            Url: string;
            Text: string;
            Icon: string;
        }[];
    }[];
    ShortcutGroups?: {
            Url: string;
            Text: string;
            Icon: string;
    }[];
    PlatformSearchDownloadMaximumRows: number;
    PartitionEmail: string;
    SupportedLanguages: Language[];
}