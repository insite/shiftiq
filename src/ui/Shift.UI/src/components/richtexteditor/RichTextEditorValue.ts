import { MultiLanguageText } from "./language";

export interface RichTextEditorValue {
    markdown?: MultiLanguageText | null;
    html?: MultiLanguageText | null;
}