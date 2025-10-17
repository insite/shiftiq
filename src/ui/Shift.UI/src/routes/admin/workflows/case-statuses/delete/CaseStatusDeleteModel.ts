export interface CaseStatusDeleteModel {
    statusIdentifier: string;
    statusName: string;
    caseType: string;
    statusSequence: number;
    statusCategory: string;
    reportCategory: string | null;
    statusDescription: string | null;
    consequences: Array<{ name: string; count: number }>;
}