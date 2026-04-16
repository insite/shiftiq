import { MultiLanguageText } from "@/helpers/language";
import RichTextEditor from "../richtexteditor/RichTextEditor";

interface Props {
    value: MultiLanguageText;
    enableSelectFile: boolean;
    isSaving: boolean;
    onSelectFile?(insertFile: (fileUrl: string, documentName: string, isImage: boolean) => void): void;
    onChange: (value: MultiLanguageText) => void;
}

export default function InlineEditor_MarkdownEditor({
    value,
    enableSelectFile,
    isSaving,
    onSelectFile,
    onChange,
}: Props) {
    return (
        <RichTextEditor
            autoFocus
            value={{ markdown: value }}
            disabled={isSaving}
            enableSelectFile={enableSelectFile}
            onSelectFile={onSelectFile}
            onChange={newValue => onChange(newValue.markdown ?? {})}
        />
    );
}
