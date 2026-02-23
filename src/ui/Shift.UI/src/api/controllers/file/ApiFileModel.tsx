export interface ApiFileModel {
    ApprovedUserId: string | null | undefined;
    FileId: string;
    LastActivityUserId: string | null | undefined;
    ObjectId: string;
    OrganizationId: string;
    ReviewedUserId: string | null | undefined;
    UserId: string;
    DocumentName: string;
    FileCategory: string | null | undefined;
    FileContentType: string;
    FileDescription: string | null | undefined;
    FileLocation: string;
    FileName: string;
    FilePath: string | null | undefined;
    FileStatus: string;
    FileSubcategory: string | null | undefined;
    FileUrl: string | null | undefined;
    ObjectType: string;
    FileSize: number;
    ApprovedTime: string | null | undefined;
    FileAlternated: string | null | undefined;
    FileExpiry: string | null | undefined;
    FileReceived: string | null | undefined;
    FileUploaded: string;
    LastActivityTime: string | null | undefined;
    ReviewedTime: string | null | undefined;
}