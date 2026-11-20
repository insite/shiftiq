import { ApiSpecWorkshopDetails } from "./ApiSpecWorkshopDetails";
import { ApiWorkshopAttachment } from "./ApiWorkshopAttachment";
import { ApiWorkshopComment } from "./ApiWorkshopComment";
import { ApiWorkshopProblemQuestion } from "./ApiWorkshopProblemQuestion";
import { ApiWorkshopQuestionData } from "./ApiWorkshopQuestionData";
import { ApiWorkshopStandard } from "./ApiWorkshopStandard";

export interface ApiSpecWorkshop {
    BankId: string;
    Standards: ApiWorkshopStandard[];
    Details: ApiSpecWorkshopDetails;
    QuestionData: ApiWorkshopQuestionData;
    Comments: ApiWorkshopComment[];
    Attachments: ApiWorkshopAttachment[];
    ProblemQuestions: ApiWorkshopProblemQuestion[];
}
