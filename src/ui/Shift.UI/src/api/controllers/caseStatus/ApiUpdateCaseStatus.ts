export interface ApiUpdateCaseStatus {
    StatusName: string;
    StatusSequence: number;
    StatusCategory: string;
    ReportCategory?: string | null;
    StatusDescription?: string | null;
}