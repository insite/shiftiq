export interface ApiSearchCaseStatuses {
    CaseTypeExact?: string | null;
    CaseTypeContains?: string | null;
    StatusNameExact?: string | null;
    StatusNameContains?: string | null;
    StatusCategoryExact?: string | null;
    StatusCategoryContains?: string | null;
    ReportCategoryExact?: string | null;
    ReportCategoryContains?: string | null;
    StatusSequenceSince?: number | null;
    StatusSequenceBefore?: number | null;
}