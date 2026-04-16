import { SpecWorkshopCriterion } from "./SpecWorkshopCriterion";
import { WorkshopStandard } from "./WorkshopStandard";

export interface SpecWorkshopDetails {
    specName: string;
    assetNumber: number;
    frameworkStandard: WorkshopStandard | null,
    formLimit: number;
    questionLimit: number;
    criteria: SpecWorkshopCriterion[];
}