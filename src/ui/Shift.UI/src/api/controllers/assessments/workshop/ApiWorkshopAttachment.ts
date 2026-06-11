export interface ApiWorkshopAttachment {
    AttachmentId: string;
    AttachmentType: string;
    AssetNumber: number;
    AssetVersion: number;
    Title: string | null | undefined;
    Condition: string | null | undefined;
    PublicationStatus: string;
    QuestionCount: number;
    PostedOn: string;
    FileName: string | null | undefined;
    FileUrl: string | null | undefined;
    FileSize: string | null | undefined;
    AuthorName: string | null | undefined;
    ChangeCount: number;
    ImageResolution: string | null | undefined;
    ImageDimensions: string[] | null | undefined;
    Color: string | null | undefined;
}