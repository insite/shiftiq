import { DateTimeParts } from "@/helpers/date/dateTimeTypes";

export interface FileRow {
    organizationId: string;
    organizationCode: string;
    objectType: string;
    objectId: string;
    fileId: string;
    fileLocation: string;
    fileName: string;
    documentName: string;
    fileSize: number;
    fileUploaded: DateTimeParts;
    userId: string;
    userName: string;
    visibility: string;
}