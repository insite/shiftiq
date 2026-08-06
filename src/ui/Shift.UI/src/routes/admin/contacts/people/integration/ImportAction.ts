export interface ImportAction {
    name: "ignore" | "create" | "match";
    match: {
        userId: string;
        userFullName: string;
    } | null;
}