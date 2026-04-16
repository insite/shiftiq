import { Environment } from "@/models/enums";
import { allFlags, WorkshopFlag, WorkshopHorizontalAlignment, WorkshopQuestionLayoutType, WorkshopQuestionType } from "./WorkshopEnums";

export const workshopValidation = {
    validateFlag(flag: string): WorkshopFlag {
        const result = allFlags.find(x => x === flag);
        if (!result) {
            throw new Error(`Unsupported flag: ${flag}`);
        }
        return result;
    },

    validateQuestionType(questionType: string): WorkshopQuestionType {
        switch (questionType) {
            case "SingleCorrect":
            case "TrueOrFalse":
            case "MultipleCorrect":
            case "BooleanTable":
            case "ComposedEssay":
            case "Matching":
            case "Likert":
            case "HotspotStandard":
            case "HotspotImageCaptcha":
            case "HotspotMultipleChoice":
            case "HotspotMultipleAnswer":
            case "HotspotCustom":
            case "ComposedVoice":
            case "Ordering":
                return questionType;
            default:
                throw new Error(`Unsupported question type: ${questionType}`);
        }
    },

    validateQuestionLayoutType(questionLayoutType: string): WorkshopQuestionLayoutType {
        switch (questionLayoutType) {
            case "None":
            case "List":
            case "Table":
                return questionLayoutType;
            default:
                throw new Error(`Unsupported question layout type: ${questionLayoutType}`);
        }
    },

    validateAlignment(alignment: string): WorkshopHorizontalAlignment {
        switch (alignment) {
            case "Left":
            case "Right":
            case "Center":
                return alignment;
            default:
                throw new Error(`Unsupported question column alignment: ${alignment}`);
        }
    },

    validateEnvironment(environment: string): Environment {
        switch (environment) {
            case "Local":
            case "Development":
            case "Sandbox":
            case "Production":
            case "External":
                return environment;
            default:
                throw new Error(`Unsupported environment: ${environment}`);
        }
    }
}