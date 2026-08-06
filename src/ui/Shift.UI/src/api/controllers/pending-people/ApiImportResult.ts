export interface ApiImportResult {
    ReportFileId: string | null | undefined;
    ReportFileName: string | null | undefined;
    ImportedPeople: {
        PersonCode: string;
        Status: string;
        PendingPersonId: string;
        UserId: string | null | undefined;
        Failure: {
            Errors: {
                Type: string;
                Title: string;
                Status: number | null | undefined;
                Detail: string | null | undefined;
                Instance: string | null | undefined;
            }[];
        } | null | undefined;
    }[];
}