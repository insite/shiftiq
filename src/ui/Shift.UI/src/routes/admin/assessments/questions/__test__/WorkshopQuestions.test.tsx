import { act, ReactNode, useMemo, useState } from "react";
import { createRoot } from "react-dom/client";
import { MemoryRouter } from "react-router";
import { afterEach, beforeEach, describe, expect, test, vi } from "vitest";
import { _workshopQuestionReducer } from "@/contexts/workshop/WorkshopQuestionProvider";
import { WorkshopQuestionProviderContext } from "@/contexts/workshop/WorkshopQuestionProviderContext";
import { WorkshopQuestionState } from "@/contexts/workshop/states/WorkshopQuestionState";
import { WorkshopQuestionChangeDates } from "@/contexts/workshop/models/WorkshopQuestionChangeDates";
import { WorkshopQuestion } from "@/contexts/workshop/models/WorkshopQuestion";
import { WorkshopStandard } from "@/contexts/workshop/models/WorkshopStandard";
import WorkshopQuestions from "@/routes/admin/assessments/questions/WorkshopQuestions";
import { WorkshopQuestionFilterState } from "../workshopQuestionFilter";

declare global {
    // eslint-disable-next-line no-var
    var IS_REACT_ACT_ENVIRONMENT: boolean | undefined;
}

globalThis.IS_REACT_ACT_ENVIRONMENT = true;

const retrieveSectionMock = vi.hoisted(() => vi.fn());

vi.mock("@/api/shiftClient", () => ({
    shiftClient: {
        workshop: {
            retrieveSection: retrieveSectionMock,
        }
    }
}));

vi.mock("@/routes/admin/assessments/questions/workshopQuestionAdapter", () => ({
    workshopQuestionAdapter: {
        getSectionData: (section: { competencies: WorkshopStandard[]; questions: WorkshopQuestion[] }) => section,
    }
}));

vi.mock("@/components/form/FormCard", () => ({
    default: ({ children, bodyClassName }: { children?: ReactNode; bodyClassName?: string }) => (
        <div className={bodyClassName}>{children}</div>
    )
}));

vi.mock("@/components/Alert", () => ({
    default: ({ children }: { children?: ReactNode }) => (
        <div role="alert">{children}</div>
    )
}));

vi.mock("@/components/Button", () => ({
    default: ({ text, variant, type, disabled, onClick }: {
        text?: string;
        variant?: string;
        type?: "button" | "submit" | "reset";
        disabled?: boolean;
        onClick?: (e: MouseEvent) => void;
    }) => (
        <button
            type={type ?? "button"}
            disabled={disabled}
            onClick={event => onClick?.(event.nativeEvent)}
        >
            {text ?? getButtonText(variant)}
        </button>
    )
}));

vi.mock("@/components/combobox/ComboBox", () => ({
    default: ({ value, items, disabled, onChange }: {
        value?: string | null;
        items: { value: string; text: string }[];
        disabled?: boolean;
        onChange?: (value: string | null) => void;
    }) => (
        <select
            disabled={disabled}
            value={value ?? ""}
            onChange={event => onChange?.(event.currentTarget.value || null)}
        >
            {items.map(item => (
                <option key={item.value} value={item.value}>{item.text}</option>
            ))}
        </select>
    )
}));

vi.mock("@/components/multiselect/MultiSelect", () => ({
    default: ({ values, items, disabled, onChange }: {
        values?: string[] | null;
        items?: { value: string; text: string }[] | null;
        disabled?: boolean;
        onChange?: (values: string[]) => void;
    }) => (
        <select
            multiple
            disabled={disabled}
            value={values ?? []}
            onChange={event => onChange?.(Array.from(event.currentTarget.selectedOptions).map(option => option.value))}
        >
            {(items ?? []).map(item => (
                <option key={item.value} value={item.value}>{item.text}</option>
            ))}
        </select>
    )
}));

vi.mock("@/components/date/DatePicker", () => ({
    default: ({ disabled, placeholder }: { disabled?: boolean; placeholder?: string }) => (
        <input disabled={disabled} placeholder={placeholder} />
    )
}));

vi.mock("@/routes/admin/assessments/questions/WorkshopQuestions_Row", () => ({
    default: ({ id, question }: { id: string; question: WorkshopQuestion }) => (
        <tr id={id} data-question-id={question.questionId}>
            <td>{question.questionId}</td>
        </tr>
    )
}));

describe("WorkshopQuestions", () => {
    beforeEach(() => {
        retrieveSectionMock.mockReset();
        window.scrollTo = vi.fn();
    });

    afterEach(() => {
        document.body.innerHTML = "";
    });

    test("auto-selects the first competency and updates the filtered count", async () => {
        const state = createState({
            sectionCompetencies: [createCompetency("std1"), createCompetency("std2")],
            sections: [
                createSection("s1", "Section 1", [createCompetency("std1"), createCompetency("std2")], [
                    createQuestion("q1", { standardId: "std1" }),
                    createQuestion("q2", { standardId: "std2" }),
                ]),
            ],
            sectionQuestions: [
                createQuestion("q1", { standardId: "std1" }),
                createQuestion("q2", { standardId: "std2" }),
            ],
        });

        const view = await renderWorkshop(state);

        expect(getVisibleQuestionIds(view.container)).toEqual(["q1"]);
        expect(view.container.textContent).toContain("Questions(1)");
    });

    test("changes competency immediately without applying staged filters", async () => {
        const state = createState({
            sectionCompetencies: [createCompetency("std1"), createCompetency("std2")],
            sections: [
                createSection("s1", "Section 1", [createCompetency("std1"), createCompetency("std2")], [
                    createQuestion("q1", { standardId: "std1" }),
                    createQuestion("q2", { standardId: "std2" }),
                ]),
            ],
            sectionQuestions: [
                createQuestion("q1", { standardId: "std1" }),
                createQuestion("q2", { standardId: "std2" }),
            ],
        });

        const view = await renderWorkshop(state);

        await changeSingleSelect(getSelect(view.container, ".question-filter-competency"), "std2");

        expect(getVisibleQuestionIds(view.container)).toEqual(["q2"]);
    });

    test("stages non-section filters until Apply and Clear restores the section default competency", async () => {
        const state = createState({
            sectionCompetencies: [createCompetency("std1"), createCompetency("std2")],
            sections: [
                createSection("s1", "Section 1", [createCompetency("std1"), createCompetency("std2")], [
                    createQuestion("q1", { standardId: "std1", questionFlag: "Red" }),
                    createQuestion("q2", { standardId: "std1", questionFlag: "Blue" }),
                    createQuestion("q3", { standardId: "std2", questionFlag: "Red" }),
                ]),
            ],
            sectionQuestions: [
                createQuestion("q1", { standardId: "std1", questionFlag: "Red" }),
                createQuestion("q2", { standardId: "std1", questionFlag: "Blue" }),
                createQuestion("q3", { standardId: "std2", questionFlag: "Red" }),
            ],
        });

        const view = await renderWorkshop(state);

        await changeMultiSelect(getSelect(view.container, ".question-filter-flag"), ["Blue"]);
        expect(getVisibleQuestionIds(view.container)).toEqual(["q1", "q2"]);

        await clickButton(view.container, "Apply");
        expect(getVisibleQuestionIds(view.container)).toEqual(["q2"]);

        await changeSingleSelect(getSelect(view.container, ".question-filter-competency"), "std2");
        expect(view.container.textContent).toContain("No questions match the current filter.");

        await clickButton(view.container, "Clear");
        expect(getVisibleQuestionIds(view.container)).toEqual(["q1", "q2"]);
        expect(getSelect(view.container, ".question-filter-competency").value).toEqual("std1");
    });

    test("loads an unloaded section once and reuses cached data", async () => {
        retrieveSectionMock.mockResolvedValue({
            competencies: [createCompetency("std3")],
            questions: [createQuestion("q3", { standardId: "std3" })],
        });

        const state = createState({
            sections: [
                createSection("s1", "Section 1", [createCompetency("std1")], [createQuestion("q1", { standardId: "std1" })]),
                createSection("s2", "Section 2", null, null),
            ],
            sectionCompetencies: [createCompetency("std1")],
            sectionQuestions: [createQuestion("q1", { standardId: "std1" })],
        });

        const view = await renderWorkshop(state);

        await changeSingleSelect(getSelect(view.container, ".question-filter-section"), "s2");
        await expectVisibleQuestionIds(view.container, ["q3"]);
        expect(retrieveSectionMock).toHaveBeenCalledTimes(1);

        await changeSingleSelect(getSelect(view.container, ".question-filter-section"), "s1");
        expect(getVisibleQuestionIds(view.container)).toEqual(["q1"]);

        await changeSingleSelect(getSelect(view.container, ".question-filter-section"), "s2");
        expect(getVisibleQuestionIds(view.container)).toEqual(["q3"]);
        expect(retrieveSectionMock).toHaveBeenCalledTimes(1);
    });

    test("shows the empty section and no-match messages distinctly", async () => {
        const state = createState({
            sections: [
                createSection("s1", "Section 1", [createCompetency("std1")], [createQuestion("q1", { standardId: "std1", questionFlag: "Red" })]),
                createSection("s2", "Section 2", [], []),
            ],
            sectionCompetencies: [createCompetency("std1")],
            sectionQuestions: [createQuestion("q1", { standardId: "std1", questionFlag: "Red" })],
        });

        const view = await renderWorkshop(state);

        await changeMultiSelect(getSelect(view.container, ".question-filter-flag"), ["Blue"]);
        await clickButton(view.container, "Apply");
        expect(view.container.textContent).toContain("No questions match the current filter.");

        await changeSingleSelect(getSelect(view.container, ".question-filter-section"), "s2");
        expect(view.container.textContent).toContain("No questions found for the selected section.");
    });

    test("shows custom changed-on inputs only when needed and does not change results when applying", async () => {
        const state = createState({
            sectionCompetencies: [createCompetency("std1")],
            sections: [
                createSection("s1", "Section 1", [createCompetency("std1")], [createQuestion("q1", { standardId: "std1" })]),
            ],
            sectionQuestions: [createQuestion("q1", { standardId: "std1" })],
        });

        const view = await renderWorkshop(state);

        const changedShortcut = getSelect(view.container, ".question-filter-changed-shortcut");

        expect(changedShortcut.disabled).toBe(false);
        expect(view.container.querySelectorAll(".question-filter-changed-dates input")).toHaveLength(0);

        await changeSingleSelect(changedShortcut, "Custom");

        const changedInputs = Array.from(view.container.querySelectorAll(".question-filter-changed-dates input")) as HTMLInputElement[];
        expect(changedInputs).toHaveLength(2);
        expect(changedInputs.every(input => input.disabled === false)).toBe(true);

        await clickButton(view.container, "Apply");
        expect(getVisibleQuestionIds(view.container)).toEqual(["q1"]);
    });

    test("scrolls to the selected question when it remains visible", async () => {
        const state = createState({
            sectionCompetencies: [createCompetency("std1")],
            sections: [
                createSection("s1", "Section 1", [createCompetency("std1")], [
                    createQuestion("q1", { standardId: "std1" }),
                    createQuestion("q2", { standardId: "std1" }),
                ]),
            ],
            sectionQuestions: [
                createQuestion("q1", { standardId: "std1" }),
                createQuestion("q2", { standardId: "std1" }),
            ],
        });

        await renderWorkshop(state, "/workshop?question=q2", {
            selectedQuestionId: "q2",
        });

        expect(window.scrollTo).toHaveBeenCalled();
    });
});

interface RenderWorkshopOptions {
    selectedQuestionId?: string | null;
    defaultFilter?: WorkshopQuestionFilterState | null;
}

function QuestionWorkshopTestProvider({
    initialState,
    children,
}: {
    initialState: WorkshopQuestionState;
    children?: ReactNode;
}) {
    const [state, setState] = useState(initialState);

    const value = useMemo(() => ({
        ...state,
        initQuestionState(nextState: WorkshopQuestionState) {
            setState(nextState);
        },
        setSectionData(competencies: WorkshopStandard[], questions: WorkshopQuestion[]) {
            setState(prev => _workshopQuestionReducer(prev, {
                type: "setSectionData",
                competencies,
                questions,
            }));
        },
        selectSection(sectionId: string) {
            setState(prev => _workshopQuestionReducer(prev, {
                type: "selectSection",
                sectionId,
            }));
        },
        modifyQuestionComments() {},
        modifyQuestionFlag() {},
        modifyQuestionTitle() {},
        modifyQuestionCode() {},
        modifyQuestionLIG() {},
        modifyQuestionReference() {},
        modifyQuestionTag() {},
        modifyQuestionTaxonomy() {},
        modifyQuestionCondition() {},
        modifyQuestionStandard() {},
        modifyQuestionColumnHeader() {},
        modifyQuestionOptionTitle() {},
        modifyQuestionOptionPoints() {},
        modifyQuestionOptionColumnTitle() {},
        showHideComment() {},
        setQuestionChangeDates(questionChangeDates: WorkshopQuestionChangeDates | null) {
            setState(prev => _workshopQuestionReducer(prev, {
                type: "setQuestionChangeDates",
                questionChangeDates,
            }));
        },
        addQuestion() {},
    }), [state]);

    return (
        <WorkshopQuestionProviderContext.Provider value={value}>
            {children}
        </WorkshopQuestionProviderContext.Provider>
    );
}

async function renderWorkshop(
    initialState: WorkshopQuestionState,
    entry = "/workshop",
    options?: RenderWorkshopOptions
): Promise<{
    container: HTMLDivElement;
    unmount: () => Promise<void>;
}> {
    const container = document.createElement("div");
    document.body.appendChild(container);

    const root = createRoot(container);

    await act(async () => {
        root.render(
            <MemoryRouter initialEntries={[entry]}>
                <QuestionWorkshopTestProvider initialState={initialState}>
                    <WorkshopQuestions
                        selectedQuestionId={options?.selectedQuestionId ?? null}
                        defaultFilter={options?.defaultFilter ?? null}
                    />
                </QuestionWorkshopTestProvider>
            </MemoryRouter>
        );
        await flushPromises();
    });

    return {
        container,
        async unmount() {
            await act(async () => {
                root.unmount();
                await flushPromises();
            });
        }
    };
}

async function changeSingleSelect(select: HTMLSelectElement, value: string) {
    await act(async () => {
        select.value = value;
        select.dispatchEvent(new Event("change", { bubbles: true }));
        await flushPromises();
    });
}

async function changeMultiSelect(select: HTMLSelectElement, values: string[]) {
    await act(async () => {
        for (const option of Array.from(select.options)) {
            option.selected = values.includes(option.value);
        }
        select.dispatchEvent(new Event("change", { bubbles: true }));
        await flushPromises();
    });
}

async function clickButton(container: HTMLElement, text: string) {
    const button = Array.from(container.querySelectorAll("button")).find(element => element.textContent === text) as HTMLButtonElement | undefined;
    if (!button) {
        throw new Error(`Button is not found: ${text}`);
    }

    await act(async () => {
        button.dispatchEvent(new MouseEvent("click", { bubbles: true }));
        await flushPromises();
    });
}

function getSelect(container: HTMLElement, selector: string): HTMLSelectElement {
    const element = container.querySelector(`${selector} select`) as HTMLSelectElement | null;
    if (!element) {
        throw new Error(`Select is not found for selector: ${selector}`);
    }

    return element;
}

function getVisibleQuestionIds(container: HTMLElement): string[] {
    return Array.from(container.querySelectorAll("[data-question-id]"))
        .map(element => element.getAttribute("data-question-id"))
        .filter((value): value is string => !!value);
}

async function expectVisibleQuestionIds(container: HTMLElement, expectedQuestionIds: string[]) {
    for (let attempt = 0; attempt < 10; attempt++) {
        if (JSON.stringify(getVisibleQuestionIds(container)) === JSON.stringify(expectedQuestionIds)) {
            return;
        }

        await act(async () => {
            await flushPromises();
            await new Promise(resolve => setTimeout(resolve, 0));
        });
    }

    expect(getVisibleQuestionIds(container)).toEqual(expectedQuestionIds);
}

async function flushPromises() {
    await Promise.resolve();
    await Promise.resolve();
}

function createState(overrides?: Partial<WorkshopQuestionState>): WorkshopQuestionState {
    const competencies = overrides?.sectionCompetencies ?? [createCompetency("std1")];
    const questions = overrides?.sectionQuestions ?? [createQuestion("q1", { standardId: "std1" })];
    const sections = overrides?.sections ?? [createSection("s1", "Section 1", competencies, questions)];

    return {
        bankId: "bank1",
        formId: "form1",
        specificationId: null,
        totalQuestionCount: questions.length,
        taxonomyItems: [{ value: "", text: "" }, { value: "1", text: "Taxonomy 1" }],
        areaCompetencies: null,
        sections,
        sectionItems: overrides?.sectionItems ?? sections.map(section => ({
            value: section.sectionId,
            text: section.sectionTitle,
        })),
        sectionId: overrides?.sectionId ?? sections[0]?.sectionId ?? null,
        sectionCompetencies: overrides?.sectionCompetencies ?? competencies,
        sectionCompetencyItems: overrides?.sectionCompetencyItems ?? [
            { value: "", text: "" },
            ...competencies.map(competency => ({ value: competency.standardId, text: competency.title })),
        ],
        sectionQuestions: overrides?.sectionQuestions ?? questions,
        questionChangeDates: overrides?.questionChangeDates ?? null,
        readOnly: false,
    };
}

function createSection(
    sectionId: string,
    sectionTitle: string,
    competencies: WorkshopStandard[] | null,
    questions: WorkshopQuestion[] | null
): NonNullable<WorkshopQuestionState["sections"]>[number] {
    return {
        sectionId,
        sectionTitle,
        competencies,
        questions,
    };
}

function getButtonText(variant?: string) {
    if (variant === "apply-filter") {
        return "Apply";
    }

    if (variant === "clear") {
        return "Clear";
    }

    return variant;
}

function createCompetency(standardId: string): WorkshopStandard {
    return {
        standardId,
        assetNumber: 1,
        sequence: 1,
        code: standardId.toUpperCase(),
        label: standardId.toUpperCase(),
        title: `Competency ${standardId}`,
        parent: null,
    };
}

function createQuestion(questionId: string, overrides?: Partial<WorkshopQuestion>): WorkshopQuestion {
    return {
        questionId,
        fieldId: null,
        parentStandardId: null,
        standardId: null,
        questionBankIndex: 0,
        questionFormSequence: 1,
        questionFlag: "None",
        questionType: "SingleCorrect",
        questionTitle: { en: questionId },
        questionTitleHtml: `<p>${questionId}</p>`,
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
        options: null,
        matches: null,
        distractors: null,
        ...overrides,
    };
}
