import { ListItem } from "@/models/listItem";

export type WorkshopFlag = "None" | "Black" | "Blue" | "Cyan" | "Red" | "Gray" | "Green" | "Yellow" | "White";

type WorkshopFlagClasses = {
    [key in WorkshopFlag]: string;
};

const flagClasses: WorkshopFlagClasses = {
    "Black": "text-dark",
    "Blue": "text-primary",
    "Cyan": "text-info",
    "Red": "text-danger",
    "Gray": "text-default",
    "Green": "text-success",
    "Yellow": "text-warning",
    "White": "text-light",
    "None": "",
};

export const allFlags = Object.keys(flagClasses) as WorkshopFlag[];

export const allFlagItems: ListItem[] = allFlags.map(x => ({ value: x as string, text: x as string }));

export function flagEnumToTextClass(flag: WorkshopFlag) {
    return flagClasses[flag];
}

export type WorkshopQuestionType = 
    "SingleCorrect"
    | "TrueOrFalse"
    | "MultipleCorrect"
    | "BooleanTable"
    | "ComposedEssay"
    | "Matching"
    | "Likert"
    | "HotspotStandard"
    | "HotspotImageCaptcha"
    | "HotspotMultipleChoice"
    | "HotspotMultipleAnswer"
    | "HotspotCustom"
    | "ComposedVoice"
    | "Ordering"
;

type WorkshopQuestionTypeDescriptions = {
    [key in WorkshopQuestionType]: string;
};

export const questionTypeDescriptions: WorkshopQuestionTypeDescriptions = {
    "SingleCorrect": "Multiple Choice",
    "TrueOrFalse": "True or False",
    "MultipleCorrect": "Multiple Correct",
    "BooleanTable": "Multiple True/False List",
    "ComposedEssay": "Composed Essay",
    "Matching": "Matching",
    "Likert": "Likert",
    "HotspotStandard": "Hotspot Standard",
    "HotspotImageCaptcha": "Hotspot Image Captcha",
    "HotspotMultipleChoice": "Hotspot Multiple Choice",
    "HotspotMultipleAnswer": "Hotspot Multiple Answer",
    "HotspotCustom": "Hotspot",
    "ComposedVoice": "Composed Voice",
    "Ordering": "Ordering",
};

export type WorkshopQuestionLayoutType = "None" | "List" | "Table";

export type WorkshopHorizontalAlignment = "Left" | "Right" | "Center";

export const allQuestionConditions: string[] = [
    "Copy",
    "Edit",
    "New",
    "Purge",
    "Surplus",
    "Unassigned"
];

export const allQuestionConditionItems: ListItem[] = allQuestionConditions.map(x => ({ value: x as string, text: x as string }));