import { DateTimeParts } from "@/helpers/date/dateTimeTypes";

export type AttachmentType = "Unknown" | "Archive" | "Document" | "Image";

export interface WorkshopAttachment {
    attachmentId: string;
    attachmentType: AttachmentType;
    assetNumber: number;
    assetVersion: number;
    title: string | null;
    condition: string | null;
    publicationStatus: string;
    questionCount: number;
    postedOn: DateTimeParts;
    fileName: string | null;
    fileUrl: string | null;
    fileSize: string | null;
    authorName: string | null;
    changeCount: number;
    imageResolution: string | null;
    imageDimensions: string[] | null;
    color: string | null;
}