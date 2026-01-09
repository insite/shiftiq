export interface ApiFileMatch {
    OrganizationIdentifier: string;
    OrganizationCode: string;
    ObjectType: string;
    ObjectIdentifier: string;
    FileIdentifier: string;
    FileLocation: string;
    FileName: string;
    DocumentName: string;
    FileSize: number;
    FileUploaded: string;
    UserIdentifier: string;
    UserFullName: string;
    HasClaims: boolean;
}