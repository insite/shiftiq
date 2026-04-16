import { ApiFormWorkshopSection } from "@/api/controllers/workshop/ApiFormWorkshopSection";
import { ApiQuestionChangeDate } from "@/api/controllers/workshop/ApiQuestionChangeDate";
import { ApiSpecWorkshopSet } from "@/api/controllers/workshop/ApiSpecWorkshopSet";
import { ApiWorkshopComment } from "@/api/controllers/workshop/ApiWorkshopComment";
import { ApiWorkshopImage } from "@/api/controllers/workshop/ApiWorkshopImage";
import { ApiWorkshopItem } from "@/api/controllers/workshop/ApiWorkshopItem";
import { ApiWorkshopQuestion } from "@/api/controllers/workshop/ApiWorkshopQuestion";
import { ApiWorkshopQuestionData } from "@/api/controllers/workshop/ApiWorkshopQuestionData";
import { ApiWorkshopStandard } from "@/api/controllers/workshop/ApiWorkshopStandard";
import { WorkshopQuestionState } from "@/contexts/workshop/states/WorkshopQuestionState";
import { WorkshopAreaCompetencies } from "@/contexts/workshop/models/WorkshopAreaCompetencies";
import { WorkshopComment } from "@/contexts/workshop/models/WorkshopComment";
import { WorkshopImage } from "@/contexts/workshop/models/WorkshopImage";
import { WorkshopQuestion } from "@/contexts/workshop/models/WorkshopQuestion";
import { WorkshopQuestionChangeDates } from "@/contexts/workshop/models/WorkshopQuestionChangeDates";
import { WorkshopStandard } from "@/contexts/workshop/models/WorkshopStandard";
import { workshopValidation } from "@/contexts/workshop/models/workshopValidation";
import { dateTimeHelper } from "@/helpers/date/dateTimeHelper";
import { ListItem } from "@/models/listItem";

const emptyArea: WorkshopStandard = {
    standardId: "",
    assetNumber: 0,
    sequence: 0,
    code: "0",
    label: "0",
    title: "Unknown",
    parent: null
};

function getStandard(standard: ApiWorkshopStandard, parent: WorkshopStandard | null): WorkshopStandard {
    return {
        standardId: standard.StandardId.toLowerCase(),
        assetNumber: standard.AssetNumber,
        sequence: standard.Sequence,
        code: standard.Code,
        label: standard.Label,
        title: standard.Title,
        parent
    };
}

function getCompetencies(standards: ApiWorkshopStandard[], hasFramework: boolean): {
    framework: WorkshopStandard | null;
    areas: WorkshopStandard[];
    competencies: WorkshopStandard[];
} {
    if (standards.length === 0) {
        return {
            framework: null,
            areas: [],
            competencies: [],
        };
    }

    const framework = hasFramework ? getStandard(standards[0], null) : null;
    const frameworkId = framework?.standardId ?? null;

    const areas = standards
        .filter(x => x.ParentId === frameworkId)
        .map(x => getStandard(x, framework));

    const competencies = standards
        .filter(x => x.ParentId)
        .map(x => getStandard(x, areas.find(y => y.standardId === x.ParentId!.toLowerCase()) ?? emptyArea));

    return {
        framework,
        areas,
        competencies
    };
}

function getComments(comments: ApiWorkshopComment[]): WorkshopComment[] {
    return comments.map(c => ({
        commentId: c.CommentId,
        authorName: c.AuthorName,
        postedOn: c.PostedOn,
        subject: c.Subject ?? null,
        text: c.Text,
        category: c.Category ?? null,
        flag: workshopValidation.validateFlag(c.Flag),
        eventFormat: c.EventFormat ?? null,
        isHidden: c.IsHidden ?? null,
    }));
}

function taxonomiesToItems(taxonomies: ApiWorkshopItem[] | null): ListItem[] {
    const items: ListItem[] = [
        { value: "", text: "" }
    ];

    if (!taxonomies) {
        return items;
    }

    for (const { Value, Text } of taxonomies) {
        items.push({ value: Value, text: Text });
    }

    return items;
}

function getQuestions(questions: ApiWorkshopQuestion[]): WorkshopQuestion[] {
    return questions.map(q => ({
        questionId: q.QuestionId.toLowerCase(),
        fieldId: q.FieldId?.toLowerCase() ?? null,
        parentStandardId: q.ParentStandardId?.toLowerCase() ?? null,
        standardId:  q.StandardId?.toLowerCase() ?? null,
        questionBankIndex: q.QuestionBankIndex,
        questionFormSequence: q.QuestionFormSequence ?? null,
        questionFlag: workshopValidation.validateFlag(q.QuestionFlag),
        questionType: workshopValidation.validateQuestionType(q.QuestionType),
        questionTitle: q.QuestionTitle,
        questionTitleHtml: q.QuestionTitleHtml ?? null,
        rationale: q.Rationale ?? null,
        rationaleOnCorrectAnswer: q.RationaleOnCorrectAnswer ?? null,
        rationaleOnIncorrectAnswer: q.RationaleOnIncorrectAnswer ?? null,
        questionAssetNumber: q.QuestionAssetNumber,
        questionAssetVersion: q.QuestionAssetVersion,
        questionPublicationStatusDescription: q.QuestionPublicationStatusDescription,
        questionCondition: q.QuestionCondition ?? null,
        questionTaxonomy: q.QuestionTaxonomy ?? null,
        questionLikeItemGroup: q.QuestionLikeItemGroup ?? null,
        questionReference: q.QuestionReference ?? null,
        questionCode: q.QuestionCode ?? null,
        questionTag: q.QuestionTag ?? null,
        questionLayoutType: workshopValidation.validateQuestionLayoutType(q.QuestionLayoutType),
        questionPoints: q.QuestionPoints ?? null,
        questionCutScore: q.QuestionCutScore ?? null,
        questionCalculationMethodDescription: q.QuestionCalculationMethodDescription,
        questionClassificationDifficulty: q.QuestionClassificationDifficulty ?? null,
        questionRandomizationEnabled: q.QuestionRandomizationEnabled,
        layoutColumns: q.LayoutColumns ? q.LayoutColumns.map(x => ({
            alignment: workshopValidation.validateAlignment(x.Alignment),
            cssClass: x.CssClass ?? null,
            textMarkdown: x.TextMarkdown ?? null,
            textHtml: x.TextHtml ?? null,
        })) : null,
        candidateCommentCount: q.CandidateCommentCount,
        canEdit: q.CanEdit,
        canNavigateToChangePage: q.CanNavigateToChangePage,
        canCopyField: q.CanCopyField,

        replaceButtons: {
            newVersion: q.ReplaceButtons.NewVersion,
            newQuestionAndSurplus: q.ReplaceButtons.NewQuestionAndSurplus,
            newQuestionAndPurge: q.ReplaceButtons.NewQuestionAndPurge,
            rollbackQuestion: q.ReplaceButtons.RollbackQuestion,
        },

        source: q.Source ? {
            questionId: q.Source.QuestionId,
            questionAssetNumber: q.Source.QuestionAssetNumber,
        } : null,

        forms: q.Forms ? q.Forms.map(f => ({
            formId: f.FormId,
            formName: f.FormName,
            formSequence: f.FormSequence,
            formAssetNumber: f.FormAssetNumber,
            formAssetversion: f.FormAssetversion,
        })) : null,

        comments: getComments(q.Comments),
        options: q.Options ? q.Options.map(o => ({
            number: o.Number,
            letter: o.Letter,
            titleMarkdown: o.TitleMarkdown ?? null,
            titleHtml: o.TitleHtml ?? null,
            points: o.Points,
            isTrue: o.IsTrue ?? null,
            columns: o.Columns ? o.Columns.map(x => ({
                textMarkdown: x.TextMarkdown ?? null,
                textHtml: x.TextHtml ?? null,
            })) : null,
        })) : null,
        matches: q.Matches ? q.Matches.map(m => ({
            left: m.Left,
            right: m.Right,
            points: m.Points
        })) : null,
        distractors: q.Distractors ?? null,
    }));
}

function getQuestionChangeDates(list: ApiQuestionChangeDate[] | null): WorkshopQuestionChangeDates | null {
    if (!list) {
        return null;
    }

    const result: WorkshopQuestionChangeDates = {};
    for (const { QuestionId, LastChangeTime } of list) {
        result[QuestionId] = dateTimeHelper.toDate(dateTimeHelper.parseServerDateTime(LastChangeTime, "UTC"))!
    }

    return result;
}

export const workshopQuestionAdapter = {
    getState(
        bankId: string,
        formId: string | null,
        specificationId: string | null,
        apiModel: ApiWorkshopQuestionData,
        apiQuestionChangeDates: ApiQuestionChangeDate[] | null,
        areaCompetencies: WorkshopAreaCompetencies | null,
    ): WorkshopQuestionState
    {
        const competencies = apiModel.FirstSectionStandards
            ? getCompetencies(apiModel.FirstSectionStandards, false).competencies
            : apiModel.FirstSectionAreaId && areaCompetencies
                ? areaCompetencies[apiModel.FirstSectionAreaId] ?? []
                : [];

        const questions = getQuestions(apiModel.FirstSectionQuestions);

        return {
            bankId,
            formId,
            specificationId,
            totalQuestionCount: apiModel.TotalQuestionCount,
            taxonomyItems: taxonomiesToItems(apiModel.Taxonomies),
            areaCompetencies,
            sections: apiModel.Sections.map(x => ({
                sectionId: x.Value.toLowerCase(),
                sectionTitle: x.Text,
                competencies: apiModel.FirstSectionId.toLowerCase() === x.Value.toLowerCase() ? competencies : null,
                questions: apiModel.FirstSectionId.toLowerCase() === x.Value.toLowerCase() ? questions : null,
            })),
            sectionItems: [],
            sectionId: apiModel.FirstSectionId.toLowerCase(),
            sectionCompetencies: competencies,
            sectionCompetencyItems: [],
            sectionQuestions: questions,
            questionChangeDates: apiQuestionChangeDates ? getQuestionChangeDates(apiQuestionChangeDates) : null,
            readOnly: apiModel.Sections.length === 0,
        }
    },

    getSectionData(apiModel: ApiFormWorkshopSection): {
        competencies: WorkshopStandard[];
        questions: WorkshopQuestion[];
    } {
        const { competencies } = getCompetencies(apiModel.Standards, false);
        const questions = getQuestions(apiModel.Questions);

        return {
            competencies,
            questions
        };
    },

    getSetData(apiModel: ApiSpecWorkshopSet, areaCompetencies: WorkshopAreaCompetencies): {
        competencies: WorkshopStandard[];
        questions: WorkshopQuestion[];
    } {
        const competencies = apiModel.AreaId ? areaCompetencies[apiModel.AreaId] ?? [] : [];
        const questions = getQuestions(apiModel.Questions);

        return {
            competencies,
            questions
        };
    },

    getComments,

    getQuestionChangeDates,

    getImages(apiModels: ApiWorkshopImage[]): WorkshopImage[] {
        return apiModels.map(x => ({
            fileName: x.FileName,
            url: x.Url,
            environment: workshopValidation.validateEnvironment(x.Environment),
            attachment: x.Attachment ? {
                title: x.Attachment.Title,
                number: x.Attachment.Number,
                condition: x.Attachment.Condition ?? null,
                publicationStatus: x.Attachment.PublicationStatus,
                dimension: x.Attachment.Dimension,
            } : null,
        }))
    },

    getCompetencies,
}