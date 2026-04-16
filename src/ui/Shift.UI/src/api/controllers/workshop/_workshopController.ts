import { fetchHelper } from "@/api/fetchHelper";
import { ApiFormWorkshop } from "./ApiFormWorkshop";
import { ApiFormWorkshopQuestions } from "./ApiFormWorkshopQuestions";
import { ApiFormWorkshopSection } from "./ApiFormWorkshopSection";
import { WorkshopFlag } from "@/contexts/workshop/models/WorkshopEnums";
import { ApiWorkshopQuestionComments } from "./ApiWorkshopQuestionComments";
import { ApiQuestionChangeDate } from "./ApiQuestionChangeDate";
import { Param } from "@/api/requestHelper";
import { ApiWorkshopImage } from "./ApiWorkshopImage";
import { ApiSpecWorkshop } from "./ApiSpecWorkshop";
import { ApiSpecWorkshopSet } from "./ApiSpecWorkshopSet";
import { ApiSpecWorkshopInput } from "./ApiSpecWorkshopInput";
import { WorkshopQuestion } from "@/contexts/workshop/models/WorkshopQuestion";

type WorkshopQuestionField = "Title" | "Code" | "Flag" | "LIG" | "Reference" | "Tag" | "Taxonomy" | "Condition" | "Standard" | "ColumnHeader";

type WorkshopQuestionOptionField = "Title" | "ColumnTitle" | "Points";

export type WorkshopReplaceQuestionCommand = "NewVersion" | "NewQuestionAndSurplus" | "NewQuestionAndPurge" | "RollbackQuestion";

type WorkshopNewQuestionCommand = "QuickMultipleChoice" | "QuickComposedEssay" | "QuickComposedVoice" | "QuickMultipleCorrect" | "QuickBooleanTable" | "QuickMatching";

interface ModifyResult {
    Html: string | null | undefined;
}

export const _workshopController = {
    async retrieveForm(formId: string, sectionId: string | null, questionId: string | null): Promise<ApiFormWorkshop | null> {
        const params: Param[] = [];
        if (sectionId) {
            params.push({ name: "sectionId", value: sectionId });
        }
        if (questionId) {
            params.push({ name: "questionId", value: questionId });
        }
        return await fetchHelper.get<ApiFormWorkshop>(`/api/evaluation/workshops/forms/${formId}`, params, true);
    },

    async retrieveSection(formId: string, sectionId: string): Promise<ApiFormWorkshopSection | null> {
        return await fetchHelper.get<ApiFormWorkshopSection>(`/api/evaluation/workshops/forms/${formId}/sections/${sectionId}`, null, true);
    },

    async modifyThirdPartyAssessment(formId: string, enabled: boolean): Promise<void> {
        const params = [{ name: "enabled", value: String(enabled) }];
        await fetchHelper.put(`/api/evaluation/workshops/forms/${formId}/third-party-assessment`, {}, params);
    },

    async verifyStaticQuestionOrder(formId: string): Promise<ApiFormWorkshopQuestions | null> {
        return await fetchHelper.post<ApiFormWorkshopQuestions>(`/api/evaluation/workshops/forms/${formId}/verify-static-question-order`, {});
    },

    async retrieveSpec(specificationId: string, setId: string | null, questionId: string | null): Promise<ApiSpecWorkshop | null> {
        const params: Param[] = [];
        if (setId) {
            params.push({ name: "setId", value: setId });
        }
        if (questionId) {
            params.push({ name: "questionId", value: questionId });
        }
        return await fetchHelper.get<ApiSpecWorkshop>(`/api/evaluation/workshops/specs/${specificationId}`, params, true);
    },

    async retrieveSpecSet(specificationId: string, setId: string): Promise<ApiSpecWorkshopSet | null> {
        return await fetchHelper.get<ApiSpecWorkshopSet>(`/api/evaluation/workshops/specs/${specificationId}/sets/${setId}`, null, true);
    },

    async modifySpec(specificationId: string, input: ApiSpecWorkshopInput): Promise<boolean> {
        return await fetchHelper.put(`/api/evaluation/workshops/specs/${specificationId}`, input, null, true) != null;
    },

    async collectQuestionChangeDates(bankId: string): Promise<ApiQuestionChangeDate[] | null> {
        return await fetchHelper.get<ApiQuestionChangeDate[]>(`/api/evaluation/workshops/banks/${bankId}/question-change-dates`);
    },

    async collectImages(bankId: string): Promise<ApiWorkshopImage[] | null> {
        return await fetchHelper.get<ApiWorkshopImage[]>(`/api/evaluation/workshops/banks/${bankId}/images`);
    },

    async addQuestion(bankId: string, setId: string, standardId: string | null, command: WorkshopNewQuestionCommand): Promise<WorkshopQuestion | null> {
        const params: Param[] = [{ name: "command", value: command }];
        if (standardId) {
            params.push({ name: "standardId", value: standardId });
        }
        return await fetchHelper.post<WorkshopQuestion>(`/api/evaluation/workshops/banks/${bankId}/sets/${setId}/questions`, null, params);
    },

    async modifyQuestion(
        bankId: string,
        questionId: string,
        field: WorkshopQuestionField,
        columnIndex: number | null,
        value: string | null
    ): Promise<string | null>
    {
        const body = {
            Field: field,
            ColumnIndex: columnIndex,
            Value: value
        };
        const result = await fetchHelper.put<ModifyResult>(`/api/evaluation/workshops/banks/${bankId}/questions/${questionId}`, body);
        return result.Html ?? null;
    },

    async modifyOption(
        bankId: string,
        questionId: string,
        optionNumber: number,
        field: WorkshopQuestionOptionField,
        columnIndex: number | null,
        value: string | null
    ): Promise<string | null>
    {
        const body = {
            Field: field,
            ColumnIndex: columnIndex,
            Value: value
        };
        const result = await fetchHelper.put<ModifyResult>(`/api/evaluation/workshops/banks/${bankId}/questions/${questionId}/options/${optionNumber}`, body);
        return result.Html ?? null;
    },

    async postFieldComment(bankId: string, fieldId: string, authorType: string, flag: WorkshopFlag, text: string): Promise<ApiWorkshopQuestionComments | null> {
        const body = {
            AuthorType: authorType,
            Flag: flag,
            text: text
        };
        return await fetchHelper.post<ApiWorkshopQuestionComments>(`/api/evaluation/workshops/banks/${bankId}/fields/${fieldId}/comments`, body);
    },

    async replaceFieldQuestion(
        bankId: string,
        fieldId: string,
        command: WorkshopReplaceQuestionCommand
    ) : Promise<ApiFormWorkshopSection | null>
    {
        return await fetchHelper.post<ApiFormWorkshopSection>(`/api/evaluation/workshops/banks/${bankId}/fields/${fieldId}/replace?command=${command}`, null, null, true);
    },

    async showHideComment(bankId: string, commentId: string, hidden: boolean): Promise<void> {
        await fetchHelper.put<ApiWorkshopQuestionComments>(`/api/evaluation/workshops/banks/${bankId}/comments/${commentId}/showhide?hidden=${hidden}`, null);
    },
}