export type Language = "ar" | "zh" | "nl" | "en" | "eo" | "fr" | "de" | "he" | "it" | "ja" | "ko" | "la" | "no" | "pa" | "pl" | "pt" | "ru" | "es" | "sv" | "uk";

export type MultiLanguageText = {
    [key in Language]?: string;
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