import { ApiSearchFiles } from "@/api/controllers/file/ApiSearchFiles";
import { BaseCriteria } from "@/components/search/BaseCriteria";
import { dateTimeHelper } from "@/helpers/date/dateTimeHelper";
import { DateTimeInvalid, DateTimeParts } from "@/helpers/date/dateTimeTypes";

export interface FileCriteria extends BaseCriteria {
    organizationId: string;
    objectType: string;
    objectId: string;
    fileName: string;
    documentName: string;
    fileUploadedSince: DateTimeParts | DateTimeInvalid | null;
    fileUploadedBefore: DateTimeParts | DateTimeInvalid | null;
    fileUploadedBy: string;
    visibility: "public" | "private" | "";
}

export function toApiSearchFiles(criteria: FileCriteria): ApiSearchFiles {
    return {
        OrganizationId: criteria.organizationId,
        ObjectTypeExact: criteria.objectType,
        ObjectId: null,
        ObjectIdContains: criteria.objectId,
        FileNameContains: criteria.fileName,
        DocumentNameContains: criteria.documentName,
        FileUploadedSince: dateTimeHelper.formatServerDateTime(criteria.fileUploadedSince),
        FileUploadedBefore: dateTimeHelper.formatServerDateTime(criteria.fileUploadedBefore),
        UserId: criteria.fileUploadedBy,
        HasClaims: criteria.visibility === "public" ? false : (criteria.visibility === "private" ? true : null),
    }
}