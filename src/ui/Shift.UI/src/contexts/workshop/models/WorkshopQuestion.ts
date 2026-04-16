import { MultiLanguageText } from "@/helpers/language";
import { WorkshopComment } from "./WorkshopComment";
import { WorkshopFlag, WorkshopHorizontalAlignment, WorkshopQuestionLayoutType, WorkshopQuestionType } from "./WorkshopEnums";
import { WorkshopQuestionMatch } from "./WorkshopQuestionMatch";
import { WorkshopQuestionOption } from "./WorkshopQuestionOption";

export interface WorkshopQuestion {
    questionId: string;
    fieldId: string | null;
    parentStandardId: string | null;
    standardId:  string | null;
    questionBankIndex: number;
    questionFormSequence: number | null;
    questionFlag: WorkshopFlag;
    questionType: WorkshopQuestionType;
    questionTitle: MultiLanguageText;
    questionTitleHtml: string | null;
    rationale: string | null;
    rationaleOnCorrectAnswer: string | null;
    rationaleOnIncorrectAnswer: string | null;
    questionAssetNumber: number;
    questionAssetVersion: number;
    questionPublicationStatusDescription: string;
    questionCondition: string | null;
    questionTaxonomy: number | null;
    questionLikeItemGroup: string | null;
    questionReference: string | null;
    questionCode: string | null;
    questionTag: string | null;
    questionLayoutType: WorkshopQuestionLayoutType;
    questionPoints: number | null;
    questionCutScore: number | null;
    questionCalculationMethodDescription: string;
    questionClassificationDifficulty: number | null;
    questionRandomizationEnabled: boolean;
    layoutColumns: {
        alignment: WorkshopHorizontalAlignment;
        cssClass: string | null;
        textMarkdown: string | null;
        textHtml: string | null;
    }[] | null,
    candidateCommentCount: number;
    canEdit: boolean;
    canNavigateToChangePage: boolean;
    canCopyField: boolean;

    replaceButtons: {
        newVersion: boolean;
        newQuestionAndSurplus: boolean;
        newQuestionAndPurge: boolean;
        rollbackQuestion: boolean;
    };

    source: {
        questionId: string;
        questionAssetNumber: number;
    } | null;

    forms: {
        formId: string;
        formName: string;
        formSequence: number;
        formAssetNumber: number;
        formAssetversion: number;
    }[] | null;

    comments: WorkshopComment[];
    options: WorkshopQuestionOption[] | null;
    matches: WorkshopQuestionMatch[] | null;
    distractors: string[] | null;
}