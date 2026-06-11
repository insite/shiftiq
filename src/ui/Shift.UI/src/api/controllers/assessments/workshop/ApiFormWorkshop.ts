import { ApiWorkshopComment } from "./ApiWorkshopComment";
import { ApiFormWorkshopQuestionStatistics } from "./ApiFormWorkshopQuestionStatistics";
import { ApiWorkshopStandard } from "./ApiWorkshopStandard";
import { ApiWorkshopAttachment } from "./ApiWorkshopAttachment";
import { ApiWorkshopProblemQuestion } from "./ApiWorkshopProblemQuestion";
import { ApiWorkshopQuestionData } from "./ApiWorkshopQuestionData";

export interface ApiFormWorkshop {
    BankId: string;
    Details: {
        SpecificationName: string;
        SpecificationType: "Dynamic" | "Static";
        Standard: ApiWorkshopStandard;
        FormName: string;
        FormAssetNumber: number;
        FormAssetVersion: number;
        FormCode: string;
        FormSource: string | null | undefined;
        FormOrigin: string | null | undefined;
        FormHook: string | null | undefined;
        PublicationStatus: string | null | undefined;
        ThirdPartyAssessmentIsEnabled: boolean;
        StaticQuestionOrderVerified: string | null | undefined;
        VerifiedQuestions: {
            Sequence: number,
            Code: string | null | undefined;
            Tag: string | null | undefined;
            Text: string;
        }[] | null | undefined;
        IsQuestionOrderMatch: boolean;
    };
    Statistics: ApiFormWorkshopQuestionStatistics;
    QuestionData: ApiWorkshopQuestionData;
    Comments: ApiWorkshopComment[];
    Attachments: ApiWorkshopAttachment[];
    ProblemQuestions: ApiWorkshopProblemQuestion[];
}