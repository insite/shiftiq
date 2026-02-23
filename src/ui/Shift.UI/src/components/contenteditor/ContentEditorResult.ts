import { RichTextEditorValue } from "../richtexteditor/RichTextEditorValue";

export interface ContentEditorResult {
    fields: {
        fieldName: string;
        value: RichTextEditorValue;
    }[];
}