import { WorkshopAttachment } from "../models/WorkshopAttachment";
import { WorkshopComment } from "../models/WorkshopComment";
import { WorkshopProblemQuestion } from "../models/WorkshopProblemQuestion";

export interface WorkshopOtherState {
    bankId: string;
    formId: string | null;
    specificationId: string | null;
    comments: WorkshopComment[] | null;
    attachments: WorkshopAttachment[] | null;
    problemQuestions: WorkshopProblemQuestion[] | null;
    readOnly: boolean;
}