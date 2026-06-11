export interface ApiFileMatch {
    OrganizationId: string;
    OrganizationCode: string;
    ObjectType: string;
    ObjectId: string;
    FileId: string;
    FileLocation: string;
    FileName: string;
    DocumentName: string;
    FileSize: number;
    FileUploaded: string;
    UserId: string;
    UserFullName: string;
    HasClaims: boolean;
}