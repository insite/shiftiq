import { DateTimeParts } from "@/helpers/date/dateTimeTypes";
import { WorkshopStandard } from "./WorkshopStandard";
import { FormWorkshopVerifiedQuestion } from "./FormWorkshopVerifiedQuestion";

export interface FormWorkshopDetails {
    specificationName: string;
    specificationType: "Dynamic" | "Static";
    standard: WorkshopStandard;
    formName: string;
    formAssetNumber: number;
    formAssetVersion: number;
    formCode: string;
    formSource: string | null;
    formOrigin: string | null;
    formHook: string | null;
    publicationStatus: string | null;
    thirdPartyAssessmentEnabled: boolean;
    questionOrderVerified: DateTimeParts | null;
    verifiedQuestions: FormWorkshopVerifiedQuestion[] | null;
    questionOrderMatch: boolean;
}