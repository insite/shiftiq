import { FormWorkshopDetails } from "../models/FormWorkshopDetails";
import { FormWorkshopQuestionStatistics } from "../models/FormWorkshopQuestionStatistics";

export interface FormWorkshopState {
    bankId: string;
    formId: string;
    details: FormWorkshopDetails | null;
    statistics: FormWorkshopQuestionStatistics | null;
    readOnly: boolean;
}