import { ApiSearchFiles } from "@/api/controllers/file/ApiSearchFiles";
import { BaseCriteria } from "@/components/search/BaseCriteria";
import { dateTimeHelper } from "@/helpers/date/dateTimeHelper";
import { DateTimeInvalid, DateTimeParts } from "@/helpers/date/dateTimeTypes";

export interface FileCriteria extends BaseCriteria {
    organizationIdentifier: string;
    objectType: string;
    objectIdentifier: string;
    fileName: string;
    documentName: string;
    fileUploadedSince: DateTimeParts | DateTimeInvalid | null;
    fileUploadedBefore: DateTimeParts | DateTimeInvalid | null;
    fileUploadedBy: string;
    visibility: "public" | "private" | "";
}

export function toApiSearchFiles(criteria: FileCriteria): ApiSearchFiles {
    return {
        OrganizationIdentifier: criteria.organizationIdentifier,
        ObjectTypeExact: criteria.objectType,
        ObjectIdentifier: null,
        ObjectIdentifierContains: criteria.objectIdentifier,
        FileNameContains: criteria.fileName,
        DocumentNameContains: criteria.documentName,
        FileUploadedSince: dateTimeHelper.formatServerDateTime(criteria.fileUploadedSince),
        FileUploadedBefore: dateTimeHelper.formatServerDateTime(criteria.fileUploadedBefore),
        UserIdentifier: criteria.fileUploadedBy,
        HasClaims: criteria.visibility === "public" ? false : (criteria.visibility === "private" ? true : null),
    }
}