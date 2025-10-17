export interface ApiFileModel {
    ApprovedUserIdentifier: string | null | undefined;
    FileIdentifier: string;
    LastActivityUserIdentifier: string | null | undefined;
    ObjectIdentifier: string;
    OrganizationIdentifier: string;
    ReviewedUserIdentifier: string | null | undefined;
    UserIdentifier: string;
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