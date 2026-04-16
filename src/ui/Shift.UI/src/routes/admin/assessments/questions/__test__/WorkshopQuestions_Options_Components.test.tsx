import { WorkshopQuestionLayoutType, WorkshopQuestionType } from "@/contexts/workshop/models/WorkshopEnums";
import { WorkshopQuestion } from "@/contexts/workshop/models/WorkshopQuestion";
import { WorkshopQuestionOption } from "@/contexts/workshop/models/WorkshopQuestionOption";
import { renderToStaticMarkup } from "react-dom/server";
import { beforeEach, describe, expect, test, vi } from "vitest";

interface MockInlineEditorValue {
    [key: string]: string | undefined;
}

interface MockInlineEditorProps {
    type: "ComboBox" | "TextBox" | "TextArea" | "MarkdownEditor";
    value: string | MockInlineEditorValue | null;
    valueHtml: string | null;
    onSave?: (value: string | MockInlineEditorValue | null) => Promise<void>;
}

const inlineEditorMock = vi.hoisted(() => {
    const renderedProps: MockInlineEditorProps[] = [];

    function getVisibleValue(props: MockInlineEditorProps): string {
        if (props.valueHtml) {
            return stripHtml(props.valueHtml);
        }

        if (typeof props.value === "string") {
            return props.value;
        }

        if (props.value) {
            return Object.values(props.value).find(Boolean) ?? "";
        }

        return "";
    }

    function stripHtml(value: string): string {
        return value.replace(/<[^>]+>/g, "");
    }

    return {
        renderedProps,
        reset() {
            renderedProps.length = 0;
        },
        component: vi.fn((props: MockInlineEditorProps) => {
            renderedProps.push(props);
            return getVisibleValue(props);
        }),
    };
});

vi.mock("@/components/inlineeditor/InlineEditor", () => ({ default: inlineEditorMock.component }));

import WorkshopQuestions_Options_BooleanTable from "../WorkshopQuestions_Options_BooleanTable";
import WorkshopQuestions_Options_Matching from "../WorkshopQuestions_Options_Matching";
import WorkshopQuestions_Options_MultipleCorrect from "../WorkshopQuestions_Options_MultipleCorrect";
import WorkshopQuestions_Options_SingleCorrect from "../WorkshopQuestions_Options_SingleCorrect";

describe("WorkshopQuestions_Options components", () => {
    beforeEach(() => {
        inlineEditorMock.reset();
        inlineEditorMock.component.mockClear();
    });

    test.each(["SingleCorrect", "TrueOrFalse"] as const)("SingleCorrect component renders for %s", questionType => {
        const question = createQuestion(questionType);

        const html = renderToStaticMarkup(
            <WorkshopQuestions_Options_SingleCorrect
                question={question}
                isEditable={true}
                onSaveColumnHeader={async () => { }}
                onSaveOptionTitle={async () => { }}
                onSaveOptionPoints={async () => { }}
            />
        );

        expect(html).toContain("option-grid");
        expect(html).toContain("A.");
    });

    test("SingleCorrect component wires onSave callbacks with option number", async () => {
        const question = createQuestion("SingleCorrect");
        const onSaveColumnHeader = vi.fn(async () => { });
        const onSaveOptionTitle = vi.fn(async () => { });
        const onSaveOptionPoints = vi.fn(async () => { });

        renderToStaticMarkup(
            <WorkshopQuestions_Options_SingleCorrect
                question={question}
                isEditable={true}
                onSaveColumnHeader={onSaveColumnHeader}
                onSaveOptionTitle={onSaveOptionTitle}
                onSaveOptionPoints={onSaveOptionPoints}
            />
        );

        const titleField = inlineEditorMock.renderedProps.find(x => x.type === "TextArea");
        const pointsField = inlineEditorMock.renderedProps.find(x => x.type === "TextBox");

        expect(titleField).toBeDefined();
        expect(pointsField).toBeDefined();

        await titleField!.onSave?.("Updated option");
        await pointsField!.onSave?.("3.5");

        expect(onSaveOptionTitle).toHaveBeenCalledWith(1, null, "Updated option");
        expect(onSaveOptionPoints).toHaveBeenCalledWith(1, "3.5");
    });

    test("Matching component renders pairs and distractors", () => {
        const html = renderToStaticMarkup(
            <WorkshopQuestions_Options_Matching
                question={createQuestion("Matching")}
            />
        );

        expect(html).toContain("Matching Pairs");
        expect(html).toContain("Left");
        expect(html).toContain("Right");
        expect(html).toContain("1.00 points");
        expect(html).toContain("Matching Distractors");
        expect(html).toContain("Distractor");
    });

    test("table-layout components render column headers and cell values", () => {
        const question = createTableLayoutQuestion("SingleCorrect");
        const matchingHtml = renderToStaticMarkup(
            <WorkshopQuestions_Options_Matching
                question={createQuestion("Matching", "Table")}
            />
        );

        const singleCorrectHtml = renderToStaticMarkup(
            <WorkshopQuestions_Options_SingleCorrect
                question={{ ...question, questionType: "SingleCorrect" }}
                isEditable={true}
                onSaveColumnHeader={async () => { }}
                onSaveOptionTitle={async () => { }}
                onSaveOptionPoints={async () => { }}
            />
        );

        const multipleCorrectHtml = renderToStaticMarkup(
            <WorkshopQuestions_Options_MultipleCorrect
                question={{ ...question, questionType: "MultipleCorrect" }}
                isEditable={true}
                onSaveColumnHeader={async () => { }}
                onSaveOptionTitle={async () => { }}
            />
        );

        const booleanTableHtml = renderToStaticMarkup(
            <WorkshopQuestions_Options_BooleanTable
                question={{ ...question, questionType: "BooleanTable" }}
                isEditable={true}
                onSaveColumnHeader={async () => { }}
                onSaveOptionTitle={async () => { }}
            />
        );

        expect(matchingHtml).toContain("Matching Pairs");
        expect(matchingHtml).toContain("Matching Distractors");

        expect(singleCorrectHtml).toContain("Column 1");
        expect(singleCorrectHtml).toContain("Column 2");
        expect(singleCorrectHtml).toContain("Cell 1");
        expect(singleCorrectHtml).toContain("Cell 2");

        expect(multipleCorrectHtml).toContain("Column 1");
        expect(multipleCorrectHtml).toContain("Column 2");
        expect(multipleCorrectHtml).toContain("Cell 1");
        expect(multipleCorrectHtml).toContain("Cell 2");

        expect(booleanTableHtml).toContain("Column 1");
        expect(booleanTableHtml).toContain("Column 2");
        expect(booleanTableHtml).toContain("Cell 1");
        expect(booleanTableHtml).toContain("Cell 2");
    });
});

function createQuestion(
    questionType: WorkshopQuestionType,
    questionLayoutType: WorkshopQuestionLayoutType = "None",
): WorkshopQuestion {
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
        questionLayoutType,
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

function createTableLayoutQuestion(questionType: WorkshopQuestionType): WorkshopQuestion {
    const question = createQuestion(questionType, "Table");

    return {
        ...question,
        layoutColumns: [
            {
                alignment: "Left",
                cssClass: null,
                textMarkdown: "Column 1",
                textHtml: "<p>Column 1</p>",
            },
            {
                alignment: "Center",
                cssClass: "text-center",
                textMarkdown: "Column 2",
                textHtml: "<p>Column 2</p>",
            },
        ],
        options: question.options?.map(option => ({
            ...option,
            titleMarkdown: null,
            titleHtml: null,
            columns: [
                {
                    textMarkdown: "Cell 1",
                    textHtml: "<p>Cell 1</p>",
                },
                {
                    textMarkdown: "Cell 2",
                    textHtml: "<p>Cell 2</p>",
                },
            ],
        })) ?? null,
    };
}
