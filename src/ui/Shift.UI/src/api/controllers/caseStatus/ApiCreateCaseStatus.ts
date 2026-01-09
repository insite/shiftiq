export interface ApiCreateCaseStatus {
    CaseType: string;
    StatusName: string;
    StatusSequence: number;
    StatusCategory: string;
    ReportCategory?: string | null;
    StatusDescription?: string | null;
}