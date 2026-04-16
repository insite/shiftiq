import { ApiWorkshopAttachment } from "./ApiWorkshopAttachment";
import { ApiWorkshopComment } from "./ApiWorkshopComment";
import { ApiWorkshopProblemQuestion } from "./ApiWorkshopProblemQuestion";
import { ApiWorkshopQuestionData } from "./ApiWorkshopQuestionData";
import { ApiWorkshopStandard } from "./ApiWorkshopStandard";

interface Details {
    SpecName: string;
    AssetNumber: number;
    FrameworkId: string | null | undefined;
    FormLimit: number;
    QuestionLimit: number;
    Criteria: {
        CriterionId: string;
        Title: string;
        // Scaled decimal transport value: original weight * 10000.
        Weight: number;
        StandardIds: string[];
        Competencies: {
            StandardId: string;
            Tax1Count: number | null | undefined;
            Tax2Count: number | null | undefined;
            Tax3Count: number | null | undefined;
            QuestionCount: number | null | undefined;
            Tax1CountActual: number;
            Tax2CountActual: number;
            Tax3CountActual: number;
            UnassignedCount: number | null | undefined;
        }[];
    }[];
}

export interface ApiSpecWorkshop {
    BankId: string;
    Standards: ApiWorkshopStandard[];
    Details: Details;
    QuestionData: ApiWorkshopQuestionData;
    Comments: ApiWorkshopComment[];
    Attachments: ApiWorkshopAttachment[];
    ProblemQuestions: ApiWorkshopProblemQuestion[];
}
