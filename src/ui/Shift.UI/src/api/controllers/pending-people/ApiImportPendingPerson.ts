export interface ApiImportPendingPerson {
    PendingPersonId: string;
    MatchUserId: string | null;
    Action: "ignore" | "create" | "match";
}