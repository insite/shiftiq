import { MultiLanguageText } from "@/helpers/language";
import { ApiWorkshopComment } from "./ApiWorkshopComment";
import { ApiWorkshopQuestionMatch } from "./ApiWorkshopQuestionMatch";
import { ApiWorkshopQuestionOption } from "./ApiWorkshopQuestionOption";

export interface ApiWorkshopQuestion {
    QuestionId: string;
    FieldId: string | null | undefined;
    ParentStandardId: string | null | undefined;
    StandardId:  string | null | undefined;
    QuestionBankIndex: number;
    QuestionFormSequence: number | null | undefined;
    QuestionFlag: string;
    QuestionType: string;
    QuestionTitle: MultiLanguageText;
    QuestionTitleHtml: string | null | undefined;
    Rationale: string | null | undefined;
    RationaleOnCorrectAnswer: string | null | undefined;
    RationaleOnIncorrectAnswer: string | null | undefined;
    QuestionAssetNumber: number;
    QuestionAssetVersion: number;
    QuestionPublicationStatusDescription: string;
    QuestionCondition: string | null | undefined;
    QuestionTaxonomy: number | null | undefined;
    QuestionLikeItemGroup: string | null | undefined;
    QuestionReference: string | null | undefined;
    QuestionCode: string | null | undefined;
    QuestionTag: string | null | undefined;
    QuestionLayoutType: string;
    QuestionPoints: number | null | undefined;
    QuestionCutScore: number | null | undefined;
    QuestionCalculationMethodDescription: string;
    QuestionClassificationDifficulty: number | null | undefined;
    QuestionRandomizationEnabled: boolean;
    LayoutColumns: {
        Alignment: string;
        CssClass: string | null | undefined;
        TextMarkdown: string | null | undefined;
        TextHtml: string | null | undefined;
    }[] | null,
    CandidateCommentCount: number;
    CanEdit: boolean;
    CanNavigateToChangePage: boolean;
    CanCopyField: boolean;

    ReplaceButtons: {
        NewVersion: boolean;
        NewQuestionAndSurplus: boolean;
        NewQuestionAndPurge: boolean;
        RollbackQuestion: boolean;
    };

    Source: {
        QuestionId: string;
        QuestionAssetNumber: number;
    } | null | undefined;

    Forms: {
        FormId: string;
        FormName: string;
        FormSequence: number;
        FormAssetNumber: number;
        FormAssetversion: number;
    }[] | null | undefined;

    Comments: ApiWorkshopComment[];
    Options: ApiWorkshopQuestionOption[] | null | undefined;
    Matches: ApiWorkshopQuestionMatch[] | null | undefined;
    Distractors: string[] | null | undefined;
}