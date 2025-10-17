import { ApiCaseStatusMatch } from "@/api/controllers/caseStatus/ApiCaseStatusMatch";

export interface CaseStatusRow {
    statusIdentifier: string;
    statusName: string;
    caseType: string;
    statusCategory: string;
    statusSequence: number;
}

export function toCaseStatusRow(row: ApiCaseStatusMatch) {
    return {
        statusIdentifier: row.StatusIdentifier,
        statusName: row.StatusName,
        caseType: row.CaseType,
        statusCategory: row.StatusCategory,
        statusSequence: row.StatusSequence,
    };
}