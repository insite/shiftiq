import { createContext, useContext } from "react";
import { WorkshopQuestionState } from "./states/WorkshopQuestionState";
import { WorkshopStandard } from "./models/WorkshopStandard";
import { WorkshopQuestion } from "./models/WorkshopQuestion";
import { WorkshopComment } from "./models/WorkshopComment";
import { WorkshopFlag } from "./models/WorkshopEnums";
import { MultiLanguageText } from "@/helpers/language";
import { WorkshopQuestionChangeDates } from "./models/WorkshopQuestionChangeDates";

type ContextData = WorkshopQuestionState & {
    initQuestionState: (state: WorkshopQuestionState) => void;
    setSectionData: (competencies: WorkshopStandard[], questions: WorkshopQuestion[]) => void;
    selectSection: (sectionId: string) => void;
    modifyQuestionComments: (questionId: string, comments: WorkshopComment[], candidateCommentCount: number) => void;
    modifyQuestionFlag: (questionId: string, flag: WorkshopFlag) => void;
    modifyQuestionTitle: (questionId: string, title: MultiLanguageText, titleHtml: string | null) => void;
    modifyQuestionCode: (questionId: string, code: string | null) => void;
    modifyQuestionLIG: (questionId: string, lig: string | null) => void;
    modifyQuestionReference: (questionId: string, reference: string | null) => void;
    modifyQuestionTag: (questionId: string, tag: string | null) => void;
    modifyQuestionTaxonomy: (questionId: string, taxonomy: number | null) => void;
    modifyQuestionCondition: (questionId: string, condition: string | null) => void;
    modifyQuestionStandard: (questionId: string, standardId: string | null) => void;
    modifyQuestionColumnHeader: (questionId: string, columnIndex: number, textMarkdown: string | null, textHtml: string | null) => void;
    modifyQuestionOptionTitle: (questionId: string, optionNumber: number, titleMarkdown: string | null, titleHtml: string | null) => void;
    modifyQuestionOptionPoints: (questionId: string, optionNumber: number, points: number) => void;
    modifyQuestionOptionColumnTitle: (questionId: string, optionNumber: number, columnIndex: number, textMarkdown: string | null, textHtml: string | null) => void;
    showHideComment: (questionId: string, commentId: string, hidden: boolean) => void;
    setQuestionChangeDates: (questionChangeDates: WorkshopQuestionChangeDates | null) => void;
    addQuestion: (question: WorkshopQuestion) => void;
}

export const WorkshopQuestionProviderContext = createContext<ContextData>({
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
    initQuestionState() {},
    setSectionData() {},
    selectSection() {},
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
    setQuestionChangeDates() {},
    addQuestion() {},
});

export function useWorkshopQuestionProvider() {
    return useContext(WorkshopQuestionProviderContext);
}
