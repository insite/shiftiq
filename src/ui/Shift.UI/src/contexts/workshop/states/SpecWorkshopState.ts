import { SpecWorkshopDetails } from "../models/SpecWorkshopDetails";

export interface SpecWorkshopState {
    bankId: string;
    specificationId: string;
    details: SpecWorkshopDetails | null;
    readOnly: boolean;
}