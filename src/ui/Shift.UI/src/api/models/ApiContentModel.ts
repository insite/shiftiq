import { Language } from "@/helpers/language";

export interface ApiContentModel {
    [itemName: string]: {
        [partName in "Text" | "Html" | "Snip"]?: {
            [languageName in Language]?: string | null
        } | null;
    };
}