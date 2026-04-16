import { ListItem } from "@/models/listItem";
import { WorkshopQuestion } from "../models/WorkshopQuestion";
import { WorkshopStandard } from "../models/WorkshopStandard";
import { WorkshopQuestionChangeDates } from "../models/WorkshopQuestionChangeDates";
import { WorkshopAreaCompetencies } from "../models/WorkshopAreaCompetencies";

export interface WorkshopQuestionState {
    bankId: string;
    formId: string | null;
    specificationId: string | null;
    totalQuestionCount: number;
    taxonomyItems: ListItem[];
    areaCompetencies: WorkshopAreaCompetencies | null; // Used by specification
    sections: {
        sectionId: string;
        sectionTitle: string;
        competencies: WorkshopStandard[] | null;
        questions: WorkshopQuestion[] | null;
    }[] | null;
    sectionItems: ListItem[];
    sectionId: string | null;
    sectionCompetencies: WorkshopStandard[] | null;
    sectionCompetencyItems: ListItem[];
    sectionQuestions: WorkshopQuestion[] | null;
    questionChangeDates: WorkshopQuestionChangeDates | null;
    readOnly: boolean;
}