import { Environment } from "@/models/enums";

export interface WorkshopImage {
    fileName: string;
    url: string;
    environment: Environment;
    attachment: {
        title: string;
        number: string;
        condition: string | null;
        publicationStatus: string;
        dimension: string;
    } | null;
}