import { expect, test } from "vitest";
import { _workshopQuestionReducer } from "../WorkshopQuestionProvider";
import { WorkshopQuestionState } from "../states/WorkshopQuestionState";
import { WorkshopQuestion } from "../models/WorkshopQuestion";
import { WorkshopQuestionOption } from "../models/WorkshopQuestionOption";
import { WorkshopStandard } from "../models/WorkshopStandard";

test("modifyQuestionOptionTitle updates option title markdown and html", () => {
    const state = createState();

    const result = _workshopQuestionReducer(state, {
        type: "modifyQuestionOptionTitle",
        questionId: "q1",
        optionNumber: 1,
        titleMarkdown: "Updated option",
        titleHtml: "<p>Updated option</p>",
    });

    const option = result.sectionQuestions![0].options![0];
    expect(option.titleMarkdown).toEqual("Updated option");
    expect(option.titleHtml).toEqual("<p>Updated option</p>");
    expect(result.sections![0].questions![0].options![0].titleMarkdown).toEqual("Updated option");
});

test("modifyQuestionOptionPoints updates option points", () => {
    const state = createState();

    const result = _workshopQuestionReducer(state, {
        type: "modifyQuestionOptionPoints",
        questionId: "q1",
        optionNumber: 1,
        points: 8.5,
    });

    expect(result.sectionQuestions![0].options![0].points).toEqual(8.5);
    expect(result.sections![0].questions![0].options![0].points).toEqual(8.5);
});

test("modifyQuestionOptionColumnTitle updates option column markdown and html", () => {
    const state = createState();

    const result = _workshopQuestionReducer(state, {
        type: "modifyQuestionOptionColumnTitle",
        questionId: "q1",
        optionNumber: 1,
        columnIndex: 1,
        textMarkdown: "Updated column",
        textHtml: "<p>Updated column</p>",
    });

    const column = result.sectionQuestions![0].options![0].columns![1];
    expect(column.textMarkdown).toEqual("Updated column");
    expect(column.textHtml).toEqual("<p>Updated column</p>");
});

test("modifyQuestionOptionColumnTitle throws when columns are null", () => {
    const state = createState({ columns: null });

    expect(() => _workshopQuestionReducer(state, {
        type: "modifyQuestionOptionColumnTitle",
        questionId: "q1",
        optionNumber: 1,
        columnIndex: 0,
        textMarkdown: "Column",
        textHtml: "<p>Column</p>",
    })).toThrowError("columns is null");
});

test("modifyQuestionOptionColumnTitle throws when column index is out of range", () => {
    const state = createState({
        columns: [{ textMarkdown: "Only", textHtml: "<p>Only</p>" }],
    });

    expect(() => _workshopQuestionReducer(state, {
        type: "modifyQuestionOptionColumnTitle",
        questionId: "q1",
        optionNumber: 1,
        columnIndex: 2,
        textMarkdown: "Column",
        textHtml: "<p>Column</p>",
    })).toThrowError("out of range");
});

test("setSectionData updates the targeted selected section", () => {
    const state = createState();
    const updatedQuestion = createQuestion("q2");
    const competencies = [createCompetency("std1")];

    const result = _workshopQuestionReducer(state, {
        type: "setSectionData",
        competencies,
        questions: [updatedQuestion],
    });

    expect(result.sections![0].questions![0].questionId).toEqual("q2");
    expect(result.sectionQuestions![0].questionId).toEqual("q2");
    expect(result.sectionCompetencies).toEqual(competencies);
    expect(result.sectionCompetencyItems[1].value).toEqual("std1");
});

test("setSectionData updates only the selected section when other sections exist", () => {
    const state = {
        ...createState(),
        sectionCompetencies: [createCompetency("std-current")],
        sectionCompetencyItems: [{ value: "", text: "" }, { value: "std-current", text: "C1. Competency std-current" }],
        sections: [
            {
                sectionId: "s1",
                sectionTitle: "Section 1",
                competencies: [createCompetency("std-current")],
                questions: [createQuestion("q1")],
            },
            {
                sectionId: "s2",
                sectionTitle: "Section 2",
                competencies: [createCompetency("std-other")],
                questions: [createQuestion("q-other")],
            },
        ],
    };

    const cachedQuestion = createQuestion("q2");
    const cachedCompetencies = [createCompetency("std-cached")];

    const result = _workshopQuestionReducer(state, {
        type: "setSectionData",
        competencies: cachedCompetencies,
        questions: [cachedQuestion],
    });

    expect(result.sections![0].questions![0].questionId).toEqual("q2");
    expect(result.sections![0].competencies).toEqual(cachedCompetencies);
    expect(result.sections![1].questions![0].questionId).toEqual("q-other");
    expect(result.sections![1].competencies![0].standardId).toEqual("std-other");
    expect(result.sectionId).toEqual("s1");
    expect(result.sectionQuestions![0].questionId).toEqual("q2");
    expect(result.sectionCompetencies![0].standardId).toEqual("std-cached");
    expect(result.sectionCompetencyItems[1].value).toEqual("std-cached");
});

function createState(overrides?: { columns?: WorkshopQuestionOption["columns"] }): WorkshopQuestionState {
    const columnsOverride = overrides?.columns;
    const columns: WorkshopQuestionOption["columns"] = columnsOverride !== undefined
        ? columnsOverride
        : [
            { textMarkdown: "Column 1", textHtml: "<p>Column 1</p>" },
            { textMarkdown: "Column 2", textHtml: "<p>Column 2</p>" },
        ];

    const option: WorkshopQuestionOption = {
        number: 1,
        letter: "A",
        titleMarkdown: "Option 1",
        titleHtml: "<p>Option 1</p>",
        points: 2,
        isTrue: true,
        columns,
    };

    const question = createQuestion("q1", option);

    return {
        bankId: "bank1",
        formId: "form1",
        specificationId: null,
        totalQuestionCount: 1,
        taxonomyItems: [],
        areaCompetencies: null,
        sections: [{
            sectionId: "s1",
            sectionTitle: "Section 1",
            competencies: null,
            questions: [question],
        }],
        sectionItems: [{
            value: "s1",
            text: "Section 1",
        }],
        sectionId: "s1",
        sectionCompetencies: null,
        sectionCompetencyItems: [],
        sectionQuestions: [question],
        questionChangeDates: null,
        readOnly: false,
    };
}

function createQuestion(questionId: string, option?: WorkshopQuestionOption): WorkshopQuestion {
    return {
        questionId,
        fieldId: null,
        parentStandardId: null,
        standardId: null,
        questionBankIndex: 0,
        questionFormSequence: 1,
        questionFlag: "None",
        questionType: "SingleCorrect",
        questionTitle: { en: `Question ${questionId}` },
        questionTitleHtml: `<p>Question ${questionId}</p>`,
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
        options: option ? [option] : [],
        matches: null,
        distractors: null,
    };
}

function createCompetency(standardId: string): WorkshopStandard {
    return {
        standardId,
        assetNumber: 1,
        sequence: 1,
        code: "C1",
        label: "Competency",
        title: `Competency ${standardId}`,
        parent: null,
    };
}
