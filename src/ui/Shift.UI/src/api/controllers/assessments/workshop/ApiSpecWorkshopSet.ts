import { ApiWorkshopQuestion } from "./ApiWorkshopQuestion";

export interface ApiSpecWorkshopSet {
    AreaId: string | null | undefined;
    QuestionId: string | null | undefined;
    Questions: ApiWorkshopQuestion[];
}