import { WorkshopOtherState } from "@/contexts/workshop/states/WorkshopOtherState";
import { workshopValidation } from "@/contexts/workshop/models/workshopValidation";
import { dateTimeHelper } from "@/helpers/date/dateTimeHelper";
import { TimeZoneId } from "@/helpers/date/timeZones";
import { urlHelper } from "@/helpers/urlHelper";
import { ApiWorkshopComment } from "@/api/controllers/workshop/ApiWorkshopComment";
import { ApiWorkshopAttachment } from "@/api/controllers/workshop/ApiWorkshopAttachment";
import { ApiWorkshopProblemQuestion } from "@/api/controllers/workshop/ApiWorkshopProblemQuestion";

function validateAttachmentType(attachmentType: string) {
    switch (attachmentType) {
        case "Unknown":
        case "Archive":
        case "Document":
        case "Image":
            return attachmentType;
        default:
            throw new Error(`Unsupported attachment type: ${attachmentType}`);
    }
}

export const workshopOtherAdapter = {
    getState(
        bankId: string,
        formId: string | null,
        specificationId: string | null,
        comments: ApiWorkshopComment[],
        attachments: ApiWorkshopAttachment[],
        problemQuestions: ApiWorkshopProblemQuestion[],
        timeZoneId: TimeZoneId
    ): WorkshopOtherState
    {
        return {
            bankId: bankId,
            formId,
            specificationId,
            comments: comments.map(c => ({
                commentId: c.CommentId,
                authorName: c.AuthorName,
                postedOn: c.PostedOn,
                subject: c.Subject ?? null,
                text: c.Text,
                category: c.Category ?? null,
                flag: workshopValidation.validateFlag(c.Flag),
                eventFormat: c.EventFormat ?? null,
                isHidden: c.IsHidden ?? null,
            })),
            attachments: attachments.map(a => ({
                attachmentId: a.AttachmentId,
                attachmentType: validateAttachmentType(a.AttachmentType),
                assetNumber: a.AssetNumber,
                assetVersion: a.AssetVersion,
                title: a.Title ?? null,
                condition: a.Condition ?? null,
                publicationStatus: a.PublicationStatus,
                questionCount: a.QuestionCount,
                postedOn: dateTimeHelper.parseServerDateTime(a.PostedOn, timeZoneId)!,
                fileName: a.FileName ?? null,
                fileUrl: a.FileUrl
                    ? a.FileUrl.toLowerCase().startsWith("/files/")
                        ? urlHelper.getResourceUrl(a.FileUrl)
                        : urlHelper.getFileUrlByNavigateUrl(a.FileUrl)
                            : null,
                fileSize: a.FileSize ?? null,
                authorName: a.AuthorName ?? null,
                changeCount: a.ChangeCount,
                imageResolution: a.ImageResolution ?? null,
                imageDimensions: a.ImageDimensions ?? null,
                color: a.Color ?? null,
            })),
            problemQuestions: problemQuestions.map(q => ({
                questionId: q.QuestionId,
                questionBankIndex: q.QuestionBankIndex,
                questionAssetNumber: q.QuestionAssetNumber,
                questionAssetVersion: q.QuestionAssetVersion,
                questionSetName: q.QuestionSetName,
                questionTitle: q.QuestionTitle ?? null,
                canDelete: q.CanDelete,
                problemDescription: q.ProblemDescription,
                options: q.Options.map(o => ({
                    number: o.Number,
                    title: o.Title ?? null,
                    letter: o.Letter,
                    points: o.Points,
                })),
            })),
            readOnly: false,
        }
    },
}