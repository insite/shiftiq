import { criterionWeightToFloat } from "@/contexts/workshop/SpecWorkshopProviderContext";
import { SpecWorkshopDetails } from "@/contexts/workshop/models/SpecWorkshopDetails";
import { WorkshopStandard } from "@/contexts/workshop/models/WorkshopStandard";
import { numberHelper } from "@/helpers/numberHelper";
import { textHelper } from "@/helpers/textHelper";

export interface SpecWorkshopBankViewCalculations {
    criteria: {
        criterionId: string;
        title: string;
        plannedCriterion: number;
        competencies: {
            standard: WorkshopStandard;
            plannedCompetency: number;
            totalActual: number | null;
            variance: number;
            varianceState: VarianceState;
            t1Planned: number;
            t1Actual: number;
            isT1Shortfall: boolean;
            t2Planned: number;
            t2Actual: number;
            isT2Shortfall: boolean;
            t3Planned: number;
            t3Actual: number;
            isT3Shortfall: boolean;
            unassigned: number | null;
        }[];
    }[];
    totals: {
        plannedCriterion: number;
        plannedCompetency: number;
        totalActual: number;
        variance: number;
        varianceState: VarianceState;
        t1Planned: number;
        t1Actual: number;
        t2Planned: number;
        t2Actual: number;
        t3Planned: number;
        t3Actual: number;
        unassigned: number;
        completedPercent: number;
    };
}

export type VarianceState = "negative" | "positive" | "none";

export function getSpecWorkshopBankViewCalculations(details: SpecWorkshopDetails): SpecWorkshopBankViewCalculations {
    let totalPlannedCriterion = 0;
    let totalPlannedCompetency = 0;
    let totalActual = 0;
    let totalVariance = 0;
    let totalT1Planned = 0;
    let totalT1Actual = 0;
    let totalT2Planned = 0;
    let totalT2Actual = 0;
    let totalT3Planned = 0;
    let totalT3Actual = 0;
    let totalUnassigned = 0;

    const criteria = details.criteria.map(criterion => {
        const plannedQuestionCount = Math.round(details.questionLimit * criterionWeightToFloat(criterion.weight));
        const plannedCriterion = details.formLimit * plannedQuestionCount;

        totalPlannedCriterion += plannedCriterion;

        const competencies = criterion.competencies.map(competency => {
            const t1PerForm = competency.tax1Count ?? 0;
            const t2PerForm = competency.tax2Count ?? 0;
            const t3PerForm = competency.tax3Count ?? 0;

            const plannedCompetency = details.formLimit * (t1PerForm + t2PerForm + t3PerForm);
            const totalActualValue = competency.questionCount ?? 0;
            const variance = totalActualValue - plannedCompetency;
            const t1Planned = details.formLimit * t1PerForm;
            const t2Planned = details.formLimit * t2PerForm;
            const t3Planned = details.formLimit * t3PerForm;
            const unassignedValue = competency.unassignedCount ?? 0;

            totalPlannedCompetency += plannedCompetency;
            totalActual += totalActualValue;
            totalVariance += variance;
            totalT1Planned += t1Planned;
            totalT1Actual += competency.tax1CountActual;
            totalT2Planned += t2Planned;
            totalT2Actual += competency.tax2CountActual;
            totalT3Planned += t3Planned;
            totalT3Actual += competency.tax3CountActual;
            totalUnassigned += unassignedValue;

            return {
                standard: competency.standard,
                plannedCompetency,
                totalActual: competency.questionCount,
                variance,
                varianceState: getVarianceState(variance),
                t1Planned,
                t1Actual: competency.tax1CountActual,
                isT1Shortfall: t1Planned > competency.tax1CountActual,
                t2Planned,
                t2Actual: competency.tax2CountActual,
                isT2Shortfall: t2Planned > competency.tax2CountActual,
                t3Planned,
                t3Actual: competency.tax3CountActual,
                isT3Shortfall: t3Planned > competency.tax3CountActual,
                unassigned: competency.unassignedCount,
            };
        });

        return {
            criterionId: criterion.criterionId,
            title: criterion.title,
            plannedCriterion,
            competencies,
        };
    });

    return {
        criteria,
        totals: {
            plannedCriterion: totalPlannedCriterion,
            plannedCompetency: totalPlannedCompetency,
            totalActual,
            variance: totalVariance,
            varianceState: getVarianceState(totalVariance),
            t1Planned: totalT1Planned,
            t1Actual: totalT1Actual,
            t2Planned: totalT2Planned,
            t2Actual: totalT2Actual,
            t3Planned: totalT3Planned,
            t3Actual: totalT3Actual,
            unassigned: totalUnassigned,
            completedPercent: Math.round(totalActual / totalPlannedCompetency * 100),
        },
    };
}

export function formatBankViewNumber(value: number | null | undefined, postfix?: string): string {
    if (value === null || value === undefined) {
        return "";
    }

    if (!isFinite(value) || isNaN(value)) {
        return textHelper.none();
    }

    const formattedValue = numberHelper.formatInt(value);

    return postfix ? formattedValue + postfix : formattedValue;
}

function getVarianceState(value: number): VarianceState {
    if (value < 0) {
        return "negative";
    }

    if (value > 0) {
        return "positive";
    }

    return "none";
}
