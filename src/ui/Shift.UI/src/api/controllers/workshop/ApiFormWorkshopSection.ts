import { ApiWorkshopQuestion } from "./ApiWorkshopQuestion";
import { ApiWorkshopStandard } from "./ApiWorkshopStandard";

export interface ApiFormWorkshopSection {
    Standards: ApiWorkshopStandard[];
    Questions: ApiWorkshopQuestion[];
}