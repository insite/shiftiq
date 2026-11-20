import { ApiSpecWorkshopDetails } from "./ApiSpecWorkshopDetails";
import { ApiWorkshopProblemQuestion } from "./ApiWorkshopProblemQuestion";

export interface ApiSpecWorkshopPartial {
    Details: ApiSpecWorkshopDetails;
    ProblemQuestions: ApiWorkshopProblemQuestion[];
}