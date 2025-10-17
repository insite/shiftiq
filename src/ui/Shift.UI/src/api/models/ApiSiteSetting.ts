import { Language } from "@/components/richtexteditor/language";
import { TimeZoneId } from "@/helpers/date/timeZones";

export interface ApiSiteSetting {
    TimeZoneId: TimeZoneId;
    OrganizationCode: string;
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