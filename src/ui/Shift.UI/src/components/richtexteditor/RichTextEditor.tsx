import { ForwardedRef, useEffect, useImperativeHandle, useRef, useState } from "react";
import { FieldError } from "react-hook-form";
import { Language } from "../../helpers/language";
import RichTextEditor_Translate from "./RichTextEditor_Translate";
import RichTextEditor_Markdown, { RichTextEditor_MarkdownRef } from "./RichTextEditor_Markdown";
import RichTextEditor_FileTypes from "./RichTextEditor_FileTypes";
import RichTextEditor_Languages from "./RichTextEditor_Languages";
import RichTextEditor_Html, { RichTextEditor_HtmlRef } from "./RichTextEditor_Html";
import { RichTextEditorValue } from "./RichTextEditorValue";
import { RichTextEditorMode } from "./RichTextEditorMode";
import { RichTextEditorRef } from "./RichTextEditorRef";
import { useRichTextEditor } from "./useRichTextEditor";
import RichTextEditor_ModeSwitch from "./RichTextEditor_ModeSwitch";

import "./RichTextEditor.css";

const _supportedImageFileTypes = [".png", ".gif", ".jpg", ".jpeg", ".txt"];
const _supportedFileTypes = [".png", ".gif", ".jpg", ".jpeg", ".doc", ".docx", ".ppt", ".pptx", ".xls", ".xlsx", ".txt", ".pdf", ".zip"];

export interface RichTextEditorProps {
    ref?: ForwardedRef<RichTextEditorRef>,
    defaultMode?: RichTextEditorMode;
    value?: RichTextEditorValue | null;
    defaultValue?: RichTextEditorValue | null;
    defaultLanguage?: Language;
    disableUploadFile?: boolean;
    enableSelectFile?: boolean;
    enableModeSwitch?: boolean;
    htmlTitle?: string;
    markdownTitle?: string;
    autoFocus?: boolean;
    disabled?: boolean;
    error?: FieldError;
    onTranslate?: (value: RichTextEditorValue) => void;
    onSelectFile?(insertFile: (fileUrl: string, documentName: string, isImage: boolean) => void): void;
    onChange?: (value: RichTextEditorValue) => void;
    onBlur?: (value: RichTextEditorValue) => void;
}

export default function RichTextEditor ({
    ref,
    defaultMode = "markdown",
    value,
    defaultValue,
    defaultLanguage = "en",
    disableUploadFile = false,
    enableSelectFile = false,
    enableModeSwitch = false,
    htmlTitle = "Body HTML",
    markdownTitle = "Body Text (Markdown)",
    autoFocus = false,
    disabled = false,
    error,
    onTranslate,
    onSelectFile,
    onChange,
    onBlur
}: RichTextEditorProps) {
    const [mode, setMode] = useState(defaultMode);

    const markdownRef = useRef<RichTextEditor_MarkdownRef>(null);
    const htmlRef = useRef<RichTextEditor_HtmlRef>(null);

    useImperativeHandle(ref, () => ({
        focus() {
            markdownRef.current?.focus();
            htmlRef.current?.focus();
        }
    }), []);

    const {
        isTranslating,
        currentValue,
        currentLanguage,
        handleUploadFile,
        handleMarkdownChange,
        handleHtmlChange,
        handleTranslate,
        handleUploadFileAndInsert,
        handleInsertFileUrl,
        handleSelectLanguage,
    } = useRichTextEditor(
        markdownRef,
        htmlRef,
        mode,
        value,
        defaultValue,
        defaultLanguage,
        _supportedFileTypes,
        onTranslate,
        onChange,
    );

    useEffect(() => {
        if (autoFocus) {
            markdownRef.current?.focus();
            htmlRef.current?.focus();
        }
    }, [autoFocus]);

    function handleSelectFile() {
        onSelectFile?.(handleInsertFileUrl);
    }

    return (
        <div className={`richtexteditor ${error ? "is-invalid" : ""}`}>
            {enableModeSwitch && (
                <RichTextEditor_ModeSwitch
                    markdownTitle={markdownTitle}
                    htmlTitle={htmlTitle}
                    mode={mode}
                    onChange={setMode}
                />
            )}

            {mode === "markdown" && (
                <RichTextEditor_Markdown
                    ref={markdownRef}
                    disabled={isTranslating || disabled}
                    markdown={currentValue?.markdown?.[currentLanguage] ?? ""}
                    disableUploadFile={disableUploadFile}
                    enableSelectFile={enableSelectFile}
                    onUploadFile={handleUploadFile}
                    onSelectFile={handleSelectFile}
                    onChange={handleMarkdownChange}
                    onBlur={() => onBlur?.(currentValue ?? {})}
                />
            )}
            {mode === "html" && (
                <RichTextEditor_Html
                    ref={htmlRef}
                    disabled={isTranslating || disabled}
                    html={currentValue?.html?.[currentLanguage] ?? ""}
                    disableUploadFile={disableUploadFile}
                    supportedImageFileTypes={_supportedImageFileTypes}
                    onUploadFile={handleUploadFile}
                    onChange={handleHtmlChange}
                    onBlur={() => onBlur?.(currentValue ?? {})}
                />
            )}

            <RichTextEditor_Translate
                disabled={disabled}
                isTranslating={isTranslating}
                language={currentLanguage}
                onTranslate={handleTranslate}
            />

            {!disableUploadFile && (
                <RichTextEditor_FileTypes
                    disabled={disabled}
                    supportedFileTypes={_supportedFileTypes}
                    onUploadFile={handleUploadFileAndInsert}
                />
            )}

            <RichTextEditor_Languages
                text={mode === "markdown" ? currentValue?.markdown : currentValue?.html}
                mode={mode}
                excludeLanguage={currentLanguage}
                onSelect={handleSelectLanguage}
            />
        </div>
    );
}