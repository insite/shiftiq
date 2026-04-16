import "./InlineEditor.css";
import { useCallback, useEffect, useState } from "react";
import { useSaveAction } from "@/hooks/useSaveAction";
import InlineEditor_TextBox from "./InlineEditor_TextBox";
import { ListItem } from "@/models/listItem";
import InlineEditor_ComboBox from "./InlineEditor_ComboBox";
import { textHelper } from "@/helpers/textHelper";
import { translate } from "@/helpers/translate";
import { Spinner } from "react-bootstrap";
import Icon from "../icon/Icon";
import InlineEditor_TextArea from "./InlineEditor_TextArea";
import { MultiLanguageText } from "@/helpers/language";
import InlineEditor_MarkdownEditor from "./InlineEditor_MarkdownEditor";

interface ComboBoxBasedProps {
    type: "ComboBox";
    value: string | null;
    items: ListItem[];
    onSave: (value: string) => Promise<void>;
}

interface TextBasedProps {
    type: "TextBox" | "TextArea";
    value: string | null;
    maxLength?: number;
    onSave: (value: string) => Promise<void>;
}

interface MultiLanguageBasedProps {
    type: "MarkdownEditor";
    value: MultiLanguageText;
    enableSelectFile?: boolean;
    onSelectFile?(insertFile: (fileUrl: string, documentName: string, isImage: boolean) => void): void;
    onSave: (value: MultiLanguageText) => Promise<void>;
}

type Props = (ComboBoxBasedProps | TextBasedProps | MultiLanguageBasedProps) & {
    className?: string;
    textClassName?: string;
    editorClassName?: string;
    valueHtml: string | null;
    disabled?: boolean;
}

let _stopEditorCallback: (() => void) | false | null = null;

export default function InlineEditor(props: Props) {
    const {
        type,
        className,
        textClassName,
        editorClassName,
        value,
        valueHtml,
        disabled = false,
        onSave,
    } = props;

    const [editorVisible, setEditorVisible] = useState(false);
    const [editorValue, setEditorValue] = useState(value ?? "");

    const { isSaving, runSave } = useSaveAction();

    const cancelEditor = useCallback(() => {
        setEditorVisible(false);
        setEditorValue(value ?? "");
    }, [value]);

    useEffect(() => {
        setEditorValue(value ?? "");
    }, [value]);

    useEffect(() => {
        if (!editorVisible) {
            return;
        }

        const newStopEditorCallback = isSaving ? false : cancelEditor;

        _stopEditorCallback = newStopEditorCallback;

        return () => {
            if (newStopEditorCallback === _stopEditorCallback) {
                _stopEditorCallback = null;
            }
        };
    }, [editorVisible, isSaving, cancelEditor]);

    function handleLabelClick() {
        if (!disabled && _stopEditorCallback !== false) {
            _stopEditorCallback?.();
            setEditorVisible(true);
        }
    }

    async function handleSave() {
        // eslint-disable-next-line @typescript-eslint/no-explicit-any
        if (await runSave(() => onSave(editorValue as any))) {
            setEditorVisible(false);
        }
    }

    return (
        <div
            className={`InlineEditor ${className ?? ""}`}
            data-editor-visible={editorVisible ? true : undefined}
        >
            {editorVisible ? (
                <div className={editorClassName}>
                    {type === "TextBox" ? (
                        <InlineEditor_TextBox
                            value={editorValue as string}
                            maxLength={props.maxLength}
                            isSaving={isSaving}
                            onChange={setEditorValue}
                            onSave={handleSave}
                            onCancel={cancelEditor}
                        />
                    ) : type === "ComboBox" ? (
                        <InlineEditor_ComboBox
                            value={editorValue as string}
                            isSaving={isSaving}
                            items={props.items}
                            onChange={setEditorValue}
                        />
                    ) : type === "TextArea" ? (
                        <InlineEditor_TextArea
                            value={editorValue as string}
                            maxLength={props.maxLength}
                            isSaving={isSaving}
                            onChange={setEditorValue}
                            onCancel={cancelEditor}
                        />
                    ) : type === "MarkdownEditor" ? (
                        <InlineEditor_MarkdownEditor
                            value={editorValue as MultiLanguageText}
                            enableSelectFile={props.enableSelectFile ?? false}
                            isSaving={isSaving}
                            onSelectFile={props.onSelectFile}
                            onChange={setEditorValue}
                        />
                    ) : (
                        <>Invalid type: {type}</>
                    )}

                    <div className="edit-buttons">
                        <button
                            type="button"
                            className="btn btn-primary btn-sm editable-submit btn-icon"
                            disabled={isSaving}
                            title={isSaving ? translate("Saving...") : translate("Save")}
                            onClick={handleSave}
                        >
                            {isSaving ? (
                                <Spinner animation="border" role="status" size="sm" />
                            ) : (
                                <Icon style="regular" name="check" />
                            )}
                        </button>
                        <button
                            type="button"
                            className="btn btn-default btn-sm editable-submit btn-icon"
                            disabled={isSaving}
                            title={translate("Cancel")}
                            onClick={cancelEditor}
                        >
                            <Icon style="regular" name="times" />
                        </button>
                    </div>
                </div>
            ) : (
                <span
                    className={textClassName}
                    dangerouslySetInnerHTML={{
                        __html: value && (typeof value === "string" || value.en)
                            ? valueHtml || `<i>${textHelper.none()}</i>`
                            : `<i>${textHelper.none()}</i>`
                    }}
                    data-disabled={disabled ? true : undefined}
                    onClick={handleLabelClick}
                />
            )}
        </div>
    );
}
