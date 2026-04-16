import { translate } from "@/helpers/translate";
import { RichTextEditorMode } from "./RichTextEditorMode";
import Icon from "../icon/Icon";

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
                    <Icon style="brands" name="markdown" className="me-1" />
                    MD
                </span>
                <span className="html-switch">
                    <Icon style="regular" name="code" className="me-1" />
                    HTML
                </span>
                <span className="handle">
                </span>
            </div>
        </div>
    );
}