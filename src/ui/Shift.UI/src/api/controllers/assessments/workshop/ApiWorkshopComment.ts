export interface ApiWorkshopComment {
    CommentId: string;
    AuthorName: string;
    PostedOn: string;
    Subject: string | null | undefined;
    Text: string;
    Category: string | null | undefined;
    Flag: string;
    EventFormat: string | null | undefined;
    IsHidden: boolean;
}