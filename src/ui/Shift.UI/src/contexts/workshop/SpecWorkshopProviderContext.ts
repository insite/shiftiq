import { createContext, useContext } from "react";
import { SpecWorkshopState } from "./states/SpecWorkshopState";

type ContextData = SpecWorkshopState & {
    initState(state: SpecWorkshopState): void;
    modifyFormLimit(formLimit: number): void;
    modifyQuestionLimit(questionLimit: number): void;
    modifyCriterionWeight(criterionId: string, weight: number): void;
    modifyTax1Count(criterionId: string, standardId: string, tax1Count: number | null): void;
    modifyTax2Count(criterionId: string, standardId: string, tax2Count: number | null): void;
    modifyTax3Count(criterionId: string, standardId: string, tax3Count: number | null): void;
}

export const SpecWorkshopProviderContext = createContext<ContextData>({
    bankId: "empty",
    specificationId: "empty",
    details: null,
    readOnly: true,
    initState() {},
    modifyFormLimit() {},
    modifyQuestionLimit() {},
    modifyCriterionWeight() {},
    modifyTax1Count() {},
    modifyTax2Count() {},
    modifyTax3Count() {},
});

export function criterionWeightToPercent(weight: number): number {
    return Math.round(weight / 100);
}

export function criterionPercentToWeight(percent: number): number {
    return percent * 100;
}

export function criterionWeightToFloat(weight: number): number {
    return weight / 10000;
}

export function useSpecWorkshopProvider() {
    return useContext(SpecWorkshopProviderContext);
}