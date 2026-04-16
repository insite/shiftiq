import { WorkshopStandard } from "./WorkshopStandard";

export interface SpecWorkshopCriterionCompetency {
    standard: WorkshopStandard;
    tax1Count: number | null;
    tax2Count: number | null;
    tax3Count: number | null;
    questionCount: number | null;
    tax1CountActual: number;
    tax2CountActual: number;
    tax3CountActual: number;
    unassignedCount: number | null;
}