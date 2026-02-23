import { Control, Path } from "react-hook-form";
import { ContentEditorValues } from "./ContentEditorValues";
import ControlledRichTextEditor from "../richtexteditor/ControlledRichTextEditor";
import { Language } from "@/helpers/language";
import { EditorOptions } from "./EditorOptions";

interface Props {
    control: Control<ContentEditorValues>;
    name: Path<ContentEditorValues>;
    options: EditorOptions;
    defaultLanguage: Language;
    disabled: boolean;
}

export default function ContentEdior_Editor({
    control,
    name,
    options,
    defaultLanguage,
    disabled,
}: Props) {
    return (
        <ControlledRichTextEditor
            control={control}
            name={name}
            defaultLanguage={defaultLanguage}
            defaultMode={options.type === "html" ? "html" : "markdown"}
            enableModeSwitch={options.type === "markdownAndHtml"}
            required={options.required ? options.title : false}
            htmlTitle="&nbsp;"
            markdownTitle="&nbsp;"
            disabled={disabled}
        />
    )
}