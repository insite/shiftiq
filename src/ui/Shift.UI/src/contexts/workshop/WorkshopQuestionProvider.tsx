import { ReactNode, useMemo, useReducer } from "react";
import { WorkshopQuestionState } from "./states/WorkshopQuestionState";
import { WorkshopStandard } from "./models/WorkshopStandard";
import { WorkshopQuestion } from "./models/WorkshopQuestion";
import { WorkshopQuestionProviderContext } from "./WorkshopQuestionProviderContext";
import { WorkshopComment } from "./models/WorkshopComment";
import { WorkshopFlag } from "./models/WorkshopEnums";
import { MultiLanguageText } from "@/helpers/language";
import { WorkshopQuestionOption } from "./models/WorkshopQuestionOption";
import { ListItem } from "@/models/listItem";
import { WorkshopQuestionChangeDates } from "./models/WorkshopQuestionChangeDates";

interface InitStateAction {
    type: "initState";
    state: WorkshopQuestionState;
}

interface SetSectionDataAction {
    type: "setSectionData";
    competencies: WorkshopStandard[];
    questions: WorkshopQuestion[];
}

interface SelectSectionAction {
    type: "selectSection";
    sectionId: string;
}

interface ModifyQuestionCommentsAction {
    type: "modifyQuestionComments";
    questionId: string;
    comments: WorkshopComment[];
    candidateCommentCount: number;
}

interface ModifyQuestionFlagAction {
    type: "modifyQuestionFlag";
    questionId: string;
    flag: WorkshopFlag;
}

interface ModifyQuestionTitleAction {
    type: "modifyQuestionTitle";
    questionId: string;
    title: MultiLanguageText;
    titleHtml: string | null;
}

interface ModifyQuestionCodeAction {
    type: "modifyQuestionCode";
    questionId: string;
    code: string | null;
}

interface ModifyQuestionLIGAction {
    type: "modifyQuestionLIG";
    questionId: string;
    lig: string | null;
}

interface ModifyQuestionReferenceAction {
    type: "modifyQuestionReference";
    questionId: string;
    reference: string | null;
}

interface ModifyQuestionTagAction {
    type: "modifyQuestionTag";
    questionId: string;
    tag: string | null;
}

interface ModifyQuestionTaxonomyAction {
    type: "modifyQuestionTaxonomy";
    questionId: string;
    taxonomy: number | null;
}

interface ModifyQuestionConditionAction {
    type: "modifyQuestionCondition";
    questionId: string;
    condition: string | null;
}

interface ModifyQuestionStandardAction {
    type: "modifyQuestionStandard";
    questionId: string;
    standardId: string | null;
}

interface ModifyQuestionColumnHeaderAction {
    type: "modifyQuestionColumnHeader";
    questionId: string;
    columnIndex: number;
    textMarkdown: string | null;
    textHtml: string | null;
}

interface ModifyQuestionOptionTitleAction {
    type: "modifyQuestionOptionTitle";
    questionId: string;
    optionNumber: number;
    titleMarkdown: string | null;
    titleHtml: string | null;
}

interface ModifyQuestionOptionPointsAction {
    type: "modifyQuestionOptionPoints";
    questionId: string;
    optionNumber: number;
    points: number;
}

interface ModifyQuestionOptionColumnTitleAction {
    type: "modifyQuestionOptionColumnTitle";
    questionId: string;
    optionNumber: number;
    columnIndex: number;
    textMarkdown: string | null;
    textHtml: string | null;
}

interface ShowHideCommentAction {
    type: "showHideComment";
    questionId: string;
    commentId: string;
    hidden: boolean;
}

interface SetQuestionChangeDatesAction {
    type: "setQuestionChangeDates";
    questionChangeDates: WorkshopQuestionChangeDates | null;
}

interface AddQuestionAction {
    type: "addQuestion";
    question: WorkshopQuestion;
}

type Action =
    InitStateAction
    | SetSectionDataAction
    | SelectSectionAction
    | ModifyQuestionCommentsAction
    | ModifyQuestionFlagAction
    | ModifyQuestionTitleAction
    | ModifyQuestionCodeAction
    | ModifyQuestionLIGAction
    | ModifyQuestionReferenceAction
    | ModifyQuestionTagAction
    | ModifyQuestionTaxonomyAction
    | ModifyQuestionConditionAction
    | ModifyQuestionStandardAction
    | ModifyQuestionColumnHeaderAction
    | ModifyQuestionOptionTitleAction
    | ModifyQuestionOptionPointsAction
    | ModifyQuestionOptionColumnTitleAction
    | ShowHideCommentAction
    | SetQuestionChangeDatesAction
    | AddQuestionAction
    ;

const _initialState: WorkshopQuestionState = {
    bankId: "empty",
    formId: null,
    specificationId: null,
    totalQuestionCount: 0,
    taxonomyItems: [],
    areaCompetencies: null,
    sections: null,
    sectionItems: [],
    sectionId: null,
    sectionCompetencies: null,
    sectionCompetencyItems: [],
    sectionQuestions: null,
    questionChangeDates: null,
    readOnly: true,
}

function modifyQuestion(
    state: WorkshopQuestionState,
    questionId: string,
    action: (q: WorkshopQuestion) => WorkshopQuestion
): WorkshopQuestionState
{
    const section = state.sections?.find(x => x.sectionId === state.sectionId);
    if (!section) {
        throw new Error(`Section ${state.sectionId} does not exist`);
    }
    const question = state.sectionQuestions?.find(x => x.questionId === questionId);
    if (!question) {
        throw new Error(`Question ${questionId} does not exist`);
    }
    const questions = section.questions!.map(q => q.questionId === questionId ? action(q): q);
    return {
        ...state,
        sections: state.sections!.map(s => ({
            ...s,
            questions: s === section ? questions : s.questions,
        })),
        sectionQuestions: questions,
    };
}

function modifyOption(
    state: WorkshopQuestionState,
    questionId: string,
    optionNumber: number,
    action: (o: WorkshopQuestionOption) => WorkshopQuestionOption
): WorkshopQuestionState
{
    return modifyQuestion(state, questionId, q => {
        const option = q.options?.find(x => x.number === optionNumber);
        if (!option) {
            throw new Error(`Question ${questionId} does not exist`);
        }
        const options: WorkshopQuestionOption[] = q.options!.map(o => o.number === optionNumber ? action(o) : o);
        return {
            ...q,
            options,
        }
    });
}

function competenciesToItems(competencies: WorkshopStandard[] | null): ListItem[] {
    const items: ListItem[] = [{ value: "", text: "No Competency Assigned" }];
    if (!competencies) {
        return items;
    }
    for (const competency of competencies) {
        const title = competency.title.replace(/<(.|\n)*?>/gi, "").trim();
        items.push({
            value: competency.standardId,
            text: `${competency.parent ? competency.parent.code + " " : ""}${competency.code}. ${title}`,
        });
    }
    items.sort((a, b) => !a.value ? -1 : !b.value ? 1 : a.text.toLowerCase().localeCompare(b.text.toLowerCase()));
    return items;
}

function sectionsToItems(sections: { sectionId: string, sectionTitle: string }[] | null): ListItem[] {
    return sections?.map(x => ({
        value: x.sectionId,
        text: x.sectionTitle
    })) ?? [];
}

function reducer(state: WorkshopQuestionState, action: Action): WorkshopQuestionState {
    const { type } = action;
    switch (type) {
        case "initState":
        {
            return {
                ...action.state,
                sectionItems: sectionsToItems(action.state.sections),
                sectionCompetencyItems: competenciesToItems(action.state.sectionCompetencies),
            };
        }

        case "setSectionData":
        {
            if (!state.sectionId || !state.sections) {
                throw new Error("Sections are not initialized");
            }
            return {
                ...state,
                sections: state.sections.map(s => {
                    if (s.sectionId !== state.sectionId) {
                        return s;
                    }
                    return {
                        sectionId: s.sectionId,
                        sectionTitle: s.sectionTitle,
                        competencies: action.competencies,
                        questions: action.questions,
                    };
                }),
                sectionCompetencies: action.competencies,
                sectionCompetencyItems: competenciesToItems(action.competencies),
                sectionQuestions: action.questions,
            };
        }

        case "selectSection":
        {
            const section = state.sections?.find(x => x.sectionId === action.sectionId);
            if (!section) {
                throw new Error(`Section ${action.sectionId} does not exist`);
            }
            return {
                ...state,
                sectionId: action.sectionId,
                sectionCompetencies: section.competencies,
                sectionCompetencyItems: competenciesToItems(section.competencies),
                sectionQuestions: section.questions,
            };
        }

        case "modifyQuestionComments":
            return modifyQuestion(state, action.questionId, q => ({
                ...q,
                comments: action.comments,
                candidateCommentCount: action.candidateCommentCount,
            }));

        case "modifyQuestionFlag":
            return modifyQuestion(state, action.questionId, q => ({ ...q, questionFlag: action.flag, }));

        case "modifyQuestionTitle":
            return modifyQuestion(state, action.questionId, q => ({
                ...q,
                questionTitle: action.title,
                questionTitleHtml: action.titleHtml,
            }));

        case "modifyQuestionCode":
            return modifyQuestion(state, action.questionId, q => ({ ...q, questionCode: action.code, }));

        case "modifyQuestionLIG":
            return modifyQuestion(state, action.questionId, q => ({ ...q, questionLikeItemGroup: action.lig, }));

        case "modifyQuestionReference":
            return modifyQuestion(state, action.questionId, q => ({ ...q, questionReference: action.reference, }));

        case "modifyQuestionTag":
            return modifyQuestion(state, action.questionId, q => ({ ...q, questionTag: action.tag, }));

        case "modifyQuestionTaxonomy":
            return modifyQuestion(state, action.questionId, q => ({ ...q, questionTaxonomy: action.taxonomy, }));

        case "modifyQuestionCondition":
            return modifyQuestion(state, action.questionId, q => ({ ...q, questionCondition: action.condition, }));

        case "modifyQuestionStandard":
            return modifyQuestion(state, action.questionId, q => ({ ...q, standardId: action.standardId, }));

        case "modifyQuestionColumnHeader":
            return modifyQuestion(state, action.questionId, q => {
                if (!q.layoutColumns) {
                    throw new Error(`layoutColumns is null for the question: ${q.questionId}`);
                }
                const layoutColumns: WorkshopQuestion["layoutColumns"] =
                    q.layoutColumns.map((c, index) => index === action.columnIndex ? {
                        ...c,
                        textMarkdown: action.textMarkdown,
                        textHtml: action.textHtml,
                    } : c);
                return {
                    ...q,
                    layoutColumns
                };
            });

        case "modifyQuestionOptionTitle":
            return modifyOption(state, action.questionId, action.optionNumber, o => ({
                ...o,
                titleMarkdown: action.titleMarkdown,
                titleHtml: action.titleHtml,
            }));

        case "modifyQuestionOptionPoints":
            return modifyOption(state, action.questionId, action.optionNumber, o => ({
                ...o,
                points: action.points,
            }));

        case "modifyQuestionOptionColumnTitle":
            return modifyOption(state, action.questionId, action.optionNumber, o => {
                if (!o.columns) {
                    throw new Error(`columns is null for the option: ${o.number}`);
                }
                if (action.columnIndex < 0 || action.columnIndex >= o.columns.length) {
                    throw new Error(`column index ${action.columnIndex} is out of range for the option: ${o.number}`);
                }
                const columns: WorkshopQuestionOption["columns"] =
                    o.columns.map((c, index) => index === action.columnIndex ? {
                        ...c,
                        textMarkdown: action.textMarkdown,
                        textHtml: action.textHtml,
                    } : c);
                return {
                    ...o,
                    columns,
                };
            });

        case "showHideComment":
            return modifyQuestion(state, action.questionId, q => {
                const comments: WorkshopQuestion["comments"] =
                    q.comments.map(c => c.commentId === action.commentId ? {
                        ...c,
                        isHidden: action.hidden,
                    } : c);
                return {
                    ...q,
                    comments
                };
            });

        case "setQuestionChangeDates":
            return {
                ...state,
                questionChangeDates: action.questionChangeDates,
            };

        case "addQuestion": {
            const section = state.sections?.find(x => x.sectionId === state.sectionId);
            if (!section) {
                throw new Error(`Section ${state.sectionId} does not exist`);
            }
            const questions = [...section.questions!, action.question];
            return {
                ...state,
                totalQuestionCount: state.totalQuestionCount + 1,
                sections: state.sections!.map(s => ({
                    ...s,
                    questions: s === section ? questions : s.questions,
                })),
                sectionQuestions: questions,
            };
        }

        default:
            throw new Error(`Unknown action: ${type}`);
    }
}

interface Props {
    children?: ReactNode;
}

export default function WorkshopQuestionProvider({ children }: Props) {
    const [state, dispatch] = useReducer(reducer, _initialState);

    const methods = useMemo(() => ({
        initQuestionState(state: WorkshopQuestionState): void {
            dispatch({ type: "initState", state });
        },
        setSectionData(competencies: WorkshopStandard[], questions: WorkshopQuestion[]): void {
            dispatch({ type: "setSectionData", competencies, questions });
        },
        selectSection(sectionId: string): void {
            dispatch({ type: "selectSection", sectionId: sectionId.toLowerCase() });
        },
        modifyQuestionComments(questionId: string, comments: WorkshopComment[], candidateCommentCount: number): void {
            dispatch({ type: "modifyQuestionComments", questionId, comments, candidateCommentCount });
        },
        modifyQuestionFlag(questionId: string, flag: WorkshopFlag): void {
            dispatch({ type: "modifyQuestionFlag", questionId, flag });
        },
        modifyQuestionTitle(questionId: string, title: MultiLanguageText, titleHtml: string | null): void {
            dispatch({ type: "modifyQuestionTitle", questionId, title, titleHtml });
        },
        modifyQuestionCode(questionId: string, code: string | null): void {
            dispatch({ type: "modifyQuestionCode", questionId, code });
        },
        modifyQuestionLIG(questionId: string, lig: string | null): void {
            dispatch({ type: "modifyQuestionLIG", questionId, lig });
        },
        modifyQuestionReference(questionId: string, reference: string | null): void {
            dispatch({ type: "modifyQuestionReference", questionId, reference });
        },
        modifyQuestionTag(questionId: string, tag: string | null): void {
            dispatch({ type: "modifyQuestionTag", questionId, tag });
        },
        modifyQuestionTaxonomy(questionId: string, taxonomy: number | null): void {
            dispatch({ type: "modifyQuestionTaxonomy", questionId, taxonomy });
        },
        modifyQuestionCondition(questionId: string, condition: string | null): void {
            dispatch({ type: "modifyQuestionCondition", questionId, condition });
        },
        modifyQuestionStandard(questionId: string, standardId: string | null): void {
            dispatch({ type: "modifyQuestionStandard", questionId, standardId });
        },
        modifyQuestionColumnHeader(questionId: string, columnIndex: number, textMarkdown: string | null, textHtml: string | null): void {
            dispatch({ type: "modifyQuestionColumnHeader", questionId, columnIndex, textMarkdown, textHtml });
        },
        modifyQuestionOptionTitle(questionId: string, optionNumber: number, titleMarkdown: string | null, titleHtml: string | null): void {
            dispatch({ type: "modifyQuestionOptionTitle", questionId, optionNumber, titleMarkdown, titleHtml });
        },
        modifyQuestionOptionPoints(questionId: string, optionNumber: number, points: number): void {
            dispatch({ type: "modifyQuestionOptionPoints", questionId, optionNumber, points });
        },
        modifyQuestionOptionColumnTitle(questionId: string, optionNumber: number, columnIndex: number, textMarkdown: string | null, textHtml: string | null): void {
            dispatch({ type: "modifyQuestionOptionColumnTitle", questionId, optionNumber, columnIndex, textMarkdown, textHtml });
        },
        showHideComment(questionId: string, commentId: string, hidden: boolean): void {
            dispatch({ type: "showHideComment", questionId, commentId, hidden });
        },
        setQuestionChangeDates(questionChangeDates: WorkshopQuestionChangeDates | null): void {
            dispatch({ type: "setQuestionChangeDates", questionChangeDates });
        },
        addQuestion(question: WorkshopQuestion): void {
            dispatch({ type: "addQuestion", question });
        },
    }), [dispatch]);

    const providerValue = useMemo(() => ({
        ...state,
        ...methods,
    }), [methods, state]);

    return (
        <WorkshopQuestionProviderContext.Provider value={providerValue}>
            {children}
        </WorkshopQuestionProviderContext.Provider>
    );
}

// Used only for unit tests
// eslint-disable-next-line react-refresh/only-export-components
export const _workshopQuestionReducer = reducer;