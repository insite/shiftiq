export interface ApiCaseStatusModel {
    OrganizationIdentifier: string;
    CaseType: string;
    StatusIdentifier: string;
    StatusName: string;
    StatusSequence: number;
    StatusCategory: string;
    ReportCategory: string | null | undefined;
    StatusDescription: string | null | undefined;
}