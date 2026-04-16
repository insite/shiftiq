import { ApiSpecWorkshop } from "@/api/controllers/workshop/ApiSpecWorkshop";
import { WorkshopStandard } from "@/contexts/workshop/models/WorkshopStandard";
import { SpecWorkshopState } from "@/contexts/workshop/states/SpecWorkshopState";

function findStandard(standardId: string, standards: Map<string, WorkshopStandard>): WorkshopStandard {
    const standard = standards.get(standardId);
    if (!standard) {
        throw new Error(`Standard ${standardId} is not found`);
    }
    return standard;
}

export const specWorkshopAdapter = {
    getState(
        specificationId: string, apiModel: ApiSpecWorkshop,
        framework: WorkshopStandard | null,
        areas: WorkshopStandard[],
        competencies: WorkshopStandard[]
    ): SpecWorkshopState
    {
        const { BankId: bankId, Details: details } = apiModel;

        const areasMap = new Map(areas.map(x => [x.standardId, x]));
        const competenciesMap = new Map(competencies.map(x => [x.standardId, x]));

        return {
            bankId,
            specificationId,
            details: {
                specName: details.SpecName,
                assetNumber: details.AssetNumber,
                frameworkStandard: framework,
                formLimit: details.FormLimit,
                questionLimit: details.QuestionLimit,
                criteria: details.Criteria.map(x => ({
                    criterionId: x.CriterionId,
                    title: x.Title,
                    weight: x.Weight,
                    areaStandards: x.StandardIds.map(standardId => findStandard(standardId, areasMap)),
                    competencies: x.Competencies.map(c => ({
                        standard: findStandard(c.StandardId, competenciesMap),
                        tax1Count: c.Tax1Count ?? null,
                        tax2Count: c.Tax2Count ?? null,
                        tax3Count: c.Tax3Count ?? null,
                        questionCount: c.QuestionCount ?? null,
                        tax1CountActual: c.Tax1CountActual,
                        tax2CountActual: c.Tax2CountActual,
                        tax3CountActual: c.Tax3CountActual,
                        unassignedCount: c.UnassignedCount ?? null,
                    }))
                })),
            },
            readOnly: false,
        }
    },
}
