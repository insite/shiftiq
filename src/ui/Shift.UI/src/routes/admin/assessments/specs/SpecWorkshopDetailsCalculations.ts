import { SpecWorkshopDetails } from "@/contexts/workshop/models/SpecWorkshopDetails";
import { criterionWeightToFloat, criterionWeightToPercent } from "@/contexts/workshop/SpecWorkshopProviderContext";

export interface SpecWorkshopDetailsCalculations {
    totalWeight: number;
    totalQuestionCount: number;
    totalCompetencyCount: number;
    criteria: {
        criterionId: string;
        plannedQuestionCount: number;
        competencyCount: number;
        hasMismatch: boolean;
        competencies: {
            standardId: string;
            totalCount: number;
        }[];
    }[];
}

export function getSpecWorkshopDetailsCalculations(details: SpecWorkshopDetails): SpecWorkshopDetailsCalculations {
    const totalWeight = details.criteria.reduce((sum, criterion) => sum + criterionWeightToPercent(criterion.weight), 0);

    let totalQuestionCount = 0;
    let totalCompetencyCount = 0;

    const criteria = details.criteria.map(criterion => {
        const plannedQuestionCount = Math.round(details.questionLimit * criterionWeightToFloat(criterion.weight));
        const competencies = criterion.competencies.map(competency => ({
            standardId: competency.standard.standardId,
            totalCount: (competency.tax1Count ?? 0) + (competency.tax2Count ?? 0) + (competency.tax3Count ?? 0),
        }));
        const competencyCount = competencies.reduce((sum, competency) => sum + competency.totalCount, 0);

        totalQuestionCount += plannedQuestionCount;
        totalCompetencyCount += competencyCount;

        return {
            criterionId: criterion.criterionId,
            plannedQuestionCount,
            competencyCount,
            hasMismatch: plannedQuestionCount !== competencyCount,
            competencies,
        };
    });

    return {
        totalWeight,
        totalQuestionCount,
        totalCompetencyCount,
        criteria,
    };
}