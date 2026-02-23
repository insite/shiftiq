import { ApiSearchCaseStatuses } from "@/api/controllers/caseStatus/ApiSearchCaseStatuses";
import { BaseCriteria } from "@/components/search/BaseCriteria";

export interface CaseStatusCriteria extends BaseCriteria {
    caseTypeExact: string;
    caseTypeContains: string;
    statusNameExact: string;
    statusNameContains: string;
    statusCategoryExact: string;
    statusCategoryContains: string;
    reportCategoryExact: string;
    reportCategoryContains: string;
}

export function defaultCaseStatusCriteria(): CaseStatusCriteria {
    return {
        caseTypeExact: "",
        caseTypeContains: "",
        statusNameExact: "",
        statusNameContains: "",
        statusCategoryExact: "",
        statusCategoryContains: "",
        reportCategoryExact: "",
        reportCategoryContains: "",
        visibleColumns: [],
        sortByColumn: null
    }
}

export function toApiSearchCaseStatuses(criteria: CaseStatusCriteria): ApiSearchCaseStatuses {
    return {
        CaseTypeExact: criteria.caseTypeExact || null,
        CaseTypeContains: criteria.caseTypeContains || null,
        StatusNameExact: criteria.statusNameExact || null,
        StatusNameContains: criteria.statusNameContains || null,
        StatusCategoryExact: criteria.statusCategoryExact || null,
        StatusCategoryContains: criteria.statusCategoryContains || null,
        ReportCategoryExact: criteria.reportCategoryExact || null,
        ReportCategoryContains: criteria.reportCategoryContains || null,
    }
}