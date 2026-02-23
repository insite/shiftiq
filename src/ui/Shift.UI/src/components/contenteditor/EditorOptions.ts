type EditorType = "markdown" | "html" | "markdownAndHtml" | "custom";

export interface EditorOptions {
    fieldName: string;
    type: EditorType;
    title: string;
    required?: boolean;
}