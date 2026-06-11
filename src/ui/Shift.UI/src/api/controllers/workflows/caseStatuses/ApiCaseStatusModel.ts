export interface ApiCaseStatusModel {
    OrganizationId: string;
    CaseType: string;
    StatusId: string;
    StatusName: string;
    StatusSequence: number;
    StatusCategory: string;
    ReportCategory: string | null | undefined;
    StatusDescription: string | null | undefined;
}