export interface ApiOrganizationModel {
    AdministratorUserIdentifier: string | null | undefined;
    GlossaryIdentifier: string | null | undefined;
    OrganizationIdentifier: string;
    ParentOrganizationIdentifier: string | null | undefined;
    AccountStatus: string;
    CompanyDomain: string | null | undefined;
    CompanyName: string | null | undefined;
    CompanySize: string | null | undefined;
    CompanySummary: string | null | undefined;
    CompanyTitle: string | null | undefined;
    CompanyWebSiteUrl: string | null | undefined;
    CompetencyAutoExpirationMode: string;
    OrganizationCode: string | null | undefined;
    OrganizationData: string | null | undefined;
    OrganizationLogoUrl: string | null | undefined;
    StandardContentLabels: string | null | undefined;
    TimeZone: string | null | undefined;
    CompetencyAutoExpirationDay: number | null | undefined;
    CompetencyAutoExpirationMonth: number | null | undefined;
    AccountClosed: string | null | undefined;
    AccountOpened: string | null | undefined;
}