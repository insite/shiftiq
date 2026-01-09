import { translate } from "@/helpers/translate";
import { RichTextEditorMode } from "./RichTextEditorMode";

interface Props {
    mode: RichTextEditorMode;
    htmlTitle: string;
    markdownTitle: string;
    onChange: (mode: RichTextEditorMode) => void;
}

export default function RichTextEditor_ModeSwitch({
    mode,
    htmlTitle,
    markdownTitle,
    onChange
}: Props) {
    return (
        <div className={`mode-switch ${mode === "markdown" ? "markdown-mode" : "html-mode"}`}>
            <label className="form-label">{mode === "markdown" ? markdownTitle : htmlTitle}</label>
            <div
                className="btn btn-primary btn-xs"
                onClick={() => onChange(mode === "markdown" ? "html" : "markdown")}
                title={translate("Change Mode")}
            >
                <span className="markdown-switch">
                    <i className="fab fa-markdown me-1"></i>
                    MD
                </span>
                <span className="html-switch">
                    <i className="far fa-code me-1 me-1"></i>
                    HTML
                </span>
                <span className="handle">
                </span>
            </div>
        </div>
    );
}