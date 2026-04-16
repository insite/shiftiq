import { ApiWorkshopItem } from "./ApiWorkshopItem";
import { ApiWorkshopQuestion } from "./ApiWorkshopQuestion";
import { ApiWorkshopStandard } from "./ApiWorkshopStandard";

export interface ApiWorkshopQuestionData {
    TotalQuestionCount: number;
    Sections: ApiWorkshopItem[];
    Taxonomies: ApiWorkshopItem[];
    FirstSectionId: string;
    FirstSectionAreaId: string | null | undefined;
    FirstSectionStandards: ApiWorkshopStandard[] | null | undefined;
    FirstSectionQuestions: ApiWorkshopQuestion[];
}