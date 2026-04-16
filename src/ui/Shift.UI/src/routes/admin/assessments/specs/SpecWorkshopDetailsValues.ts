import { ApiSpecWorkshopInput } from "@/api/controllers/workshop/ApiSpecWorkshopInput";
import { SpecWorkshopDetails } from "@/contexts/workshop/models/SpecWorkshopDetails";
import { criterionPercentToWeight, criterionWeightToPercent } from "@/contexts/workshop/SpecWorkshopProviderContext";

export interface SpecWorkshopDetailsValues {
    formLimit: number | null;
    questionLimit: number | null;
    criteria: {
        weightPercent: number | null;
        competencies: {
            tax1Count: number | null;
            tax2Count: number | null;
            tax3Count: number | null;
        }[];
    }[];
}

export function toSpecWorkshopDetailsValues(details: SpecWorkshopDetails): SpecWorkshopDetailsValues {
    return {
        formLimit: details.formLimit,
        questionLimit: details.questionLimit,
        criteria: details.criteria.map(criterion => ({
            weightPercent: criterionWeightToPercent(criterion.weight),
            competencies: criterion.competencies.map(competency => ({
                tax1Count: competency.tax1Count,
                tax2Count: competency.tax2Count,
                tax3Count: competency.tax3Count,
            })),
        })),
    };
}

export function toApiSpecWorkshopInput(details: SpecWorkshopDetails, values: SpecWorkshopDetailsValues): ApiSpecWorkshopInput {
    return {
        FormLimit: values.formLimit ?? 0,
        QuestionLimit: values.questionLimit ?? 0,
        Criteria: details.criteria.map((criterion, criterionIndex) => ({
            CriterionId: criterion.criterionId,
            Weight: criterionPercentToWeight(values.criteria[criterionIndex]?.weightPercent ?? 0),
            Competencies: criterion.competencies.map((competency, competencyIndex) => ({
                StandardId: competency.standard.standardId,
                Tax1Count: values.criteria[criterionIndex]?.competencies[competencyIndex]?.tax1Count ?? null,
                Tax2Count: values.criteria[criterionIndex]?.competencies[competencyIndex]?.tax2Count ?? null,
                Tax3Count: values.criteria[criterionIndex]?.competencies[competencyIndex]?.tax3Count ?? null,
            })),
        })),
    };
}