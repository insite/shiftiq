export interface ApiWorkshopImage {
    FileName: string;
    Url: string;
    Environment: string;
    Attachment: {
        Title: string;
        Number: string;
        Condition: string | null | undefined;
        PublicationStatus: string;
        Dimension: string;
    } | null | undefined;
}