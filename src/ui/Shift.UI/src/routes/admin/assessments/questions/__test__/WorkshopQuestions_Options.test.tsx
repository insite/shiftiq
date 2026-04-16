import { WorkshopQuestion } from "@/contexts/workshop/models/WorkshopQuestion";
import { WorkshopQuestionType } from "@/contexts/workshop/models/WorkshopEnums";
import { WorkshopQuestionOption } from "@/contexts/workshop/models/WorkshopQuestionOption";
import { ReactElement } from "react";
import { describe, expect, test, vi } from "vitest";

const componentMocks = vi.hoisted(() => ({
    matching: vi.fn(() => null),
    singleCorrect: vi.fn(() => null),
    multipleCorrect: vi.fn(() => null),
    booleanTable: vi.fn(() => null),
}));

vi.mock("../WorkshopQuestions_Options_Matching", () => ({ default: componentMocks.matching }));
vi.mock("../WorkshopQuestions_Options_SingleCorrect", () => ({ default: componentMocks.singleCorrect }));
vi.mock("../WorkshopQuestions_Options_MultipleCorrect", () => ({ default: componentMocks.multipleCorrect }));
vi.mock("../WorkshopQuestions_Options_BooleanTable", () => ({ default: componentMocks.booleanTable }));

import WorkshopQuestions_Options from "../WorkshopQuestions_Options";

interface MockComponentProps {
    question: WorkshopQuestion;
    isEditable?: boolean;
    onSaveColumnHeader?: (columnIndex: number, title: string) => Promise<void>;
    onSaveOptionTitle?: (optionNumber: number, columnIndex: number | null, title: string) => Promise<void>;
    onSaveOptionPoints?: (optionNumber: number, points: string) => Promise<void>;
}

describe("WorkshopQuestions_Options", () => {
    test.each([
        ["Matching", componentMocks.matching, "matching"],
        ["SingleCorrect", componentMocks.singleCorrect, "singleCorrect"],
        ["TrueOrFalse", componentMocks.singleCorrect, "singleCorrect"],
        ["MultipleCorrect", componentMocks.multipleCorrect, "gridWithoutPoints"],
        ["BooleanTable", componentMocks.booleanTable, "gridWithoutPoints"],
    ] as const)("dispatches %s to the expected component", (questionType, expectedComponent, expectedProps) => {
        const question = createQuestion(questionType);
        const onSaveColumnHeader = vi.fn(async () => { });
        const onSaveOptionTitle = vi.fn(async () => { });
        const onSaveOptionPoints = vi.fn(async () => { });

        const result = WorkshopQuestions_Options({
            question,
            isEditable: true,
            onSaveColumnHeader,
            onSaveOptionTitle,
            onSaveOptionPoints,
        });

        const element = result as ReactElement<MockComponentProps>;

        expect(result).not.toBeNull();
        expect(element.type).toBe(expectedComponent);
        expect(element.props.question).toBe(question);

        switch (expectedProps) {
            case "matching":
                expect(element.props.isEditable).toBeUndefined();
                expect(element.props.onSaveColumnHeader).toBeUndefined();
                expect(element.props.onSaveOptionTitle).toBeUndefined();
                expect(element.props.onSaveOptionPoints).toBeUndefined();
                break;

            case "singleCorrect":
                expect(element.props.isEditable).toBe(true);
                expect(element.props.onSaveColumnHeader).toBe(onSaveColumnHeader);
                expect(element.props.onSaveOptionTitle).toBe(onSaveOptionTitle);
                expect(element.props.onSaveOptionPoints).toBe(onSaveOptionPoints);
                break;

            case "gridWithoutPoints":
                expect(element.props.isEditable).toBe(true);
                expect(element.props.onSaveColumnHeader).toBe(onSaveColumnHeader);
                expect(element.props.onSaveOptionTitle).toBe(onSaveOptionTitle);
                expect(element.props.onSaveOptionPoints).toBeUndefined();
                break;
        }
    });

    test.each([
        "ComposedEssay",
        "Likert",
        "HotspotStandard",
        "HotspotImageCaptcha",
        "HotspotMultipleChoice",
        "HotspotMultipleAnswer",
        "HotspotCustom",
        "ComposedVoice",
        "Ordering",
    ] as const)("returns null for unsupported type %s", questionType => {
        const question = createQuestion(questionType);

        const result = WorkshopQuestions_Options({
            question,
            isEditable: true,
            onSaveColumnHeader: async () => { },
            onSaveOptionTitle: async () => { },
            onSaveOptionPoints: async () => { },
        });

        expect(result).toBeNull();
    });
});

function createQuestion(questionType: WorkshopQuestionType): WorkshopQuestion {
    const option: WorkshopQuestionOption = {
        number: 1,
        letter: "A",
        titleMarkdown: "Option 1",
        titleHtml: "<p>Option 1</p>",
        points: 20000,
        isTrue: true,
        columns: null,
    };

    return {
        questionId: "q1",
        fieldId: null,
        parentStandardId: null,
        standardId: null,
        questionBankIndex: 0,
        questionFormSequence: 1,
        questionFlag: "None",
        questionType,
        questionTitle: { en: "Question 1" },
        questionTitleHtml: "<p>Question 1</p>",
        rationale: null,
        rationaleOnCorrectAnswer: null,
        rationaleOnIncorrectAnswer: null,
        questionAssetNumber: 1,
        questionAssetVersion: 1,
        questionPublicationStatusDescription: "Published",
        questionCondition: null,
        questionTaxonomy: null,
        questionLikeItemGroup: null,
        questionReference: null,
        questionCode: null,
        questionTag: null,
        questionLayoutType: "None",
        questionPoints: null,
        questionCutScore: null,
        questionCalculationMethodDescription: "Default",
        questionClassificationDifficulty: null,
        questionRandomizationEnabled: false,
        layoutColumns: null,
        candidateCommentCount: 0,
        canEdit: true,
        canNavigateToChangePage: true,
        canCopyField: false,
        replaceButtons: {
            newVersion: false,
            newQuestionAndSurplus: false,
            newQuestionAndPurge: false,
            rollbackQuestion: false,
        },
        source: null,
        forms: null,
        comments: [],
        options: [option],
        matches: [{ left: "Left", right: "Right", points: 10000 }],
        distractors: ["Distractor"],
    };
}
