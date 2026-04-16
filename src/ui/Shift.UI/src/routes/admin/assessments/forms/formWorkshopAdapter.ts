import { ApiFormWorkshop } from "@/api/controllers/workshop/ApiFormWorkshop";
import { ApiWorkshopStandard } from "@/api/controllers/workshop/ApiWorkshopStandard";
import { WorkshopStandard } from "../../../../contexts/workshop/models/WorkshopStandard";
import { dateTimeHelper } from "@/helpers/date/dateTimeHelper";
import { TimeZoneId } from "@/helpers/date/timeZones";
import { ApiFormWorkshopQuestions } from "@/api/controllers/workshop/ApiFormWorkshopQuestions";
import { DateTimeParts } from "@/helpers/date/dateTimeTypes";
import { FormWorkshopVerifiedQuestion } from "@/contexts/workshop/models/FormWorkshopVerifiedQuestion";
import { FormWorkshopState } from "@/contexts/workshop/states/FormWorkshopState";

function toStandard(standard: ApiWorkshopStandard): WorkshopStandard {
    return {
        standardId: standard.StandardId,
        assetNumber: standard.AssetNumber,
        sequence: standard.Sequence,
        code: standard.Code,
        label: standard.Label,
        title: standard.Title,
        parent: null,
    }
}

export const formWorkshopAdapter = {
    getState(formId: string, apiModel: ApiFormWorkshop, timeZoneId: TimeZoneId): FormWorkshopState {
        const {
            Details: formDetails,
            Statistics: qs,
        } = apiModel;

        return {
            bankId: apiModel.BankId,
            formId,
            details: {
                specificationName: formDetails.SpecificationName,
                specificationType: formDetails.SpecificationType,
                standard: {
                    ...toStandard(formDetails.Standard),
                    parent: formDetails.Standard.Parent ? toStandard(formDetails.Standard.Parent) : null,
                },
                formName: formDetails.FormName,
                formAssetNumber: formDetails.FormAssetNumber,
                formAssetVersion: formDetails.FormAssetVersion,
                formCode: formDetails.FormCode,
                formSource: formDetails.FormSource || null,
                formOrigin: formDetails.FormOrigin || null,
                formHook: formDetails.FormHook || null,
                publicationStatus: formDetails.PublicationStatus || null,
                thirdPartyAssessmentEnabled: formDetails.ThirdPartyAssessmentIsEnabled,
                questionOrderVerified: dateTimeHelper.parseServerDateTime(formDetails.StaticQuestionOrderVerified, timeZoneId),
                verifiedQuestions: formDetails.VerifiedQuestions ? formDetails.VerifiedQuestions.map(x => ({
                    sequence: x.Sequence,
                    code: x.Code || null,
                    tag: x.Tag || null,
                    text: x.Text,
                })) : null,
                questionOrderMatch: formDetails.IsQuestionOrderMatch,
            },
            statistics: {
                questionPerTagAndTaxonomy: qs.QuestionPerTagAndTaxonomyArray.map(x => ({ tag: x.Tag, taxonomy: x.Taxonomy, count: x.Count })),
                questionPerTaxonomy: qs.QuestionPerTaxonomyArray.map(x => ({ item: x.Item, count: x.Count })),
                questionPerDifficulty: qs.QuestionPerDifficultyArray.map(x => ({ item: x.Item, count: x.Count })),
                questionPerGAC: qs.QuestionPerGACArray.map(x => ({ item: x.Item, count: x.Count })),
                questionPerCode: qs.QuestionPerCodeArray.map(x => ({ item: x.Item, count: x.Count })),
                questionPerLIG: qs.QuestionPerLIGArray.map(x => ({ item: x.Item, count: x.Count })),
                taxonomies: qs.Taxonomies,
                standards: qs.Standards.map(x => ({
                    setStandardCode: x.SetStandardCode ?? null,
                    questionStandardCode: x.QuestionStandardCode ?? null,
                    questions: x.Questions,
                    taxonomies: x.Taxonomies.map(x => x ?? null),
                })),
                subCompetencies: qs.SubCompetencies.map(x => ({
                    setStandardCode: x.SetStandardCode ?? null,
                    questionStandardCode: x.QuestionStandardCode ?? null,
                    questionSubCode: x.QuestionSubCode ?? null,
                    questions: x.Questions,
                })),
                tagAndTaxonomy: qs.TagAndTaxonomyArray.map(x => ({
                    tag: x.Tag,
                    countPerTaxonomy: x.CountPerTaxonomy,
                    questions: x.CountPerTaxonomy.reduce((sum, value) => sum + value, 0)
                })),
            },
            readOnly: false,
        }
    },

    getVerifiedQuestions(apiResult: ApiFormWorkshopQuestions, timeZoneId: TimeZoneId): {
        questionOrderMatch: boolean;
        questionOrderVerified: DateTimeParts | null;
        verifiedQuestions: FormWorkshopVerifiedQuestion[] | null;
    } {
        return {
            questionOrderMatch: apiResult.IsQuestionOrderMatch,
            questionOrderVerified: dateTimeHelper.parseServerDateTime(apiResult.StaticQuestionOrderVerified, timeZoneId),
            verifiedQuestions: apiResult.VerifiedQuestions ? apiResult.VerifiedQuestions.map(x => ({
                sequence: x.Sequence,
                code: x.Code || null,
                tag: x.Tag || null,
                text: x.Text,
            })) : null,
        }
    }
}