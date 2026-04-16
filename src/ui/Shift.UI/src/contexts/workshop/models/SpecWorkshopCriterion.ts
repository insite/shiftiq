import { SpecWorkshopCriterionCompetency } from "./SpecWorkshopCriterionCompetency";
import { WorkshopStandard } from "./WorkshopStandard";

export interface SpecWorkshopCriterion {
    criterionId: string;
    title: string;
    weight: number;
    areaStandards: WorkshopStandard[];
    competencies: SpecWorkshopCriterionCompetency[];
}