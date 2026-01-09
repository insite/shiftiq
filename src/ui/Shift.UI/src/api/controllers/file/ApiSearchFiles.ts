export interface ApiSearchFiles {
    OrganizationIdentifier?: string | null;
    UserIdentifier?: string | null;
    ObjectIdentifier?: string | null;
    ObjectIdentifierContains?: string | null;

    ObjectTypeExact?: string | null;
    FileNameContains?: string | null;
    DocumentNameContains?: string | null;

    FileUploadedSince?: string | null;
    FileUploadedBefore?: string | null;

    HasClaims?: boolean | null;
}