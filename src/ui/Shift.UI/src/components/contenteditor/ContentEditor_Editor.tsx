import { Control, Path } from "react-hook-form";
import { ContentEditorValues } from "./ContentEditorValues";
import ControlledRichTextEditor from "../richtexteditor/ControlledRichTextEditor";
import { Language } from "@/helpers/language";
import { EditorOptions } from "./EditorOptions";
import { RichTextEditorMode } from "../richtexteditor/RichTextEditorMode";

interface Props {
    control: Control<ContentEditorValues>;
    name: Path<ContentEditorValues>;
    options: EditorOptions;
    defaultLanguage: Language;
    defaultMode: RichTextEditorMode;
    disabled: boolean;
}

export default function ContentEdior_Editor({
    control,
    name,
    options,
    defaultLanguage,
    defaultMode,
    disabled,
}: Props) {
    return (
        <ControlledRichTextEditor
            control={control}
            name={name}
            defaultLanguage={defaultLanguage}
            defaultMode={defaultMode}
            enableModeSwitch={options.type === "markdownAndHtml"}
            required={options.required ? options.title : false}
            htmlTitle="&nbsp;"
            markdownTitle="&nbsp;"
            disabled={disabled}
        />
    )
}