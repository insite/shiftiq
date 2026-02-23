import { RichTextEditorValue } from "@/components/richtexteditor/RichTextEditorValue";
import { BlockImageValue } from "./BlockImageValue";

export type BlockFieldValue = RichTextEditorValue | BlockImageValue | BlockImageValue[] | string | null;