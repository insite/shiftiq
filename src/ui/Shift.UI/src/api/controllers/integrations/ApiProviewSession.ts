export interface ApiProviewSession {
    RedirectUrl: string | null | undefined;
    Data: {
        InitScriptUrl: string;
        ProviewToken: string;
        SessionTitle: string;
        ProfileId: string;
        Session: string;
        FirstName: string;
        LastName: string;
        MiddleName: string | null | undefined;
        Email: string;
    } | null | undefined;
}