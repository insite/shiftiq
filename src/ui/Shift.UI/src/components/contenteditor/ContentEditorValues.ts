import { ReactNode } from "react";
import { RichTextEditorValue } from "../richtexteditor/RichTextEditorValue";
import { EditorOptions } from "./EditorOptions";

export interface ContentEditorValues {
    editors: {
        value: RichTextEditorValue;
        options: EditorOptions;
        control?: ReactNode;
    }[];
}