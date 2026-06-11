export interface ApiSearchFiles {
    OrganizationId?: string | null;
    UserId?: string | null;
    ObjectId?: string | null;
    ObjectIdContains?: string | null;

    ObjectTypeExact?: string | null;
    FileNameContains?: string | null;
    DocumentNameContains?: string | null;

    FileUploadedSince?: string | null;
    FileUploadedBefore?: string | null;

    HasClaims?: boolean | null;
}