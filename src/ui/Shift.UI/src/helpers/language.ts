export type Language = "ar" | "zh" | "nl" | "en" | "eo" | "fr" | "de" | "he" | "it" | "ja" | "ko" | "la" | "no" | "pa" | "pl" | "pt" | "ru" | "es" | "sv" | "uk";

export type MultiLanguageText = {
    [key in Language]?: string | null;
};

type LanguageNames = {
    [key in Language]: string;
};

export const languageNames: LanguageNames = {
    "ar": "Arabic",
    "zh": "Chinese",
    "nl": "Dutch",
    "en": "English",
    "eo": "Esperanto",
    "fr": "French",
    "de": "German",
    "he": "Hebrew",
    "it": "Italian",
    "ja": "Japanese",
    "ko": "Korean",
    "la": "Latin",
    "no": "Norwegian",
    "pa": "Punjabi",
    "pl": "Polish",
    "pt": "Portuguese",
    "ru": "Russian",
    "es": "Spanish",
    "sv": "Swedish",
    "uk": "Ukrainian",
};

export const localizedLanguageNames: LanguageNames = {
    "ar": "عربي",
    "zh": "中国人",
    "nl": "Nederlands",
    "en": "English",
    "eo": "Esperanto",
    "fr": "Français",
    "de": "Deutsch",
    "he": "עִברִית",
    "it": "Italiana",
    "ja": "日本語",
    "ko": "한국인",
    "la": "Latina",
    "no": "Norsk",
    "pa": "ਪੰਜਾਬੀ",
    "pl": "Polski",
    "pt": "Português",
    "ru": "Русский",
    "es": "Española",
    "sv": "Suwet",
    "uk": "Українська",
};

function validateOneLanguage(language: string): void {
    if (!languageNames[language as Language]) {
        throw new Error(`Unknown language: ${language}`);
    }
}

export function validateLanguage(language: string | string[]): void {
    if (typeof language === "string") {
        validateOneLanguage(language);
    } else {
        for (const l of language) {
            validateLanguage(l);
        }
    }
}