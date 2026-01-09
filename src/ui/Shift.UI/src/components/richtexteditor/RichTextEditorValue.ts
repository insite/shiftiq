import { MultiLanguageText } from "../../helpers/language";

export interface RichTextEditorValue {
    markdown?: MultiLanguageText | null;
    html?: MultiLanguageText | null;
}