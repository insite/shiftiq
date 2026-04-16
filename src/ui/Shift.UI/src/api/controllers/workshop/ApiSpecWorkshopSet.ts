import { ApiWorkshopQuestion } from "./ApiWorkshopQuestion";

export interface ApiSpecWorkshopSet {
    AreaId: string | null | undefined;
    Questions: ApiWorkshopQuestion[];
}