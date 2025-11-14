import { ForwardedRef, useImperativeHandle, useRef, useState } from "react";
import { FieldError } from "react-hook-form";
import { useSaveAction } from "@/hooks/useSaveAction";
import { useSiteProvider } from "@/contexts/SiteProvider";
import { useStatusProvider } from "@/contexts/StatusProvider";
import { ApiUploadFileInfo } from "@/api/controllers/file/ApiUploadFileInfo";
import { shiftClient } from "@/api/shiftClient";
import { Language, MultiLanguageText } from "./language";
import RichTextEditor_Translate from "./RichTextEditor_Translate";
import RichTextEditor_Markdown, { RichTextEditor_MarkdownRef } from "./RichTextEditor_Markdown";
import RichTextEditor_FileTypes from "./RichTextEditor_FileTypes";
import RichTextEditor_Languages from "./RichTextEditor_Languages";

import "./RichTextEditor.css";
import { urlHelper } from "@/helpers/urlHelper";

const _supportedFileTypes = [".png", ".gif", ".jpg", ".jpeg", ".doc", ".docx", ".ppt", ".pptx", ".xls", ".xlsx", ".txt", ".pdf", ".zip"];

interface EditorRef {
    focus: () => void;
}

export interface RichTextEditorProps {
    ref?: ForwardedRef<EditorRef>,
    markdown?: MultiLanguageText | null;
    defaultMarkdown?: MultiLanguageText | null;
    defaultLanguage?: Language;
    disableUploadFile?: boolean;
    error?: FieldError;
    onChange?: (value: MultiLanguageText) => void;
    onBlur?: () => void;
}

export default function RichTextEditor ({
    ref,
    markdown,
    defaultMarkdown,
    defaultLanguage,
    disableUploadFile = false,
    error,
    onChange,
    onBlur
}: RichTextEditorProps) {
    const [currentLanguage, setCurrentLanguage] = useState<Language>(defaultLanguage ?? "en");
    const [storedMarkdown, setStoredMarkdown] = useState<MultiLanguageText | undefined | null>(defaultMarkdown);

    const { isSaving: isTranslating, runSave } = useSaveAction();
    const { siteSetting } = useSiteProvider();

    const currentMarkdown = markdown !== undefined ? markdown : storedMarkdown;

    const setCurrentMarkdown = markdown !== undefined
        ? (newMarkdown: MultiLanguageText) => onChange?.(newMarkdown)
        : (newMarkdown: MultiLanguageText) => {
            setStoredMarkdown(newMarkdown);
            onChange?.(newMarkdown);
        };

    const { addError, removeError } = useStatusProvider();

    const markdownRef = useRef<RichTextEditor_MarkdownRef>(null);

    useImperativeHandle(ref, () => ({
        focus() {
            markdownRef.current?.focus();
        }
    }), []);

    function handleMarkdownChange(markdownText: string) {
        if (currentMarkdown?.[currentLanguage] === markdownText) {
            return;
        }

        const newMarkdown: MultiLanguageText = {...currentMarkdown};
        newMarkdown[currentLanguage] = markdownText;

        setCurrentMarkdown(newMarkdown);
    }

    function handleTranslate() {
        runSave(async () => {
            await new Promise(resolve => setTimeout(resolve, 1000));

            const newMarkdown: MultiLanguageText = {...currentMarkdown};

            for (const language of siteSetting.SupportedLanguages) {
                if (language !== "en") {
                    newMarkdown[language] = newMarkdown.en + `\n(translated ${language})`;
                }
            }

            setCurrentMarkdown(newMarkdown);
        });
    }

    function handleSelectLanguage(language: Language) {
        setCurrentLanguage(language);
    }

    async function handleUploadFile(file: File): Promise<ApiUploadFileInfo | null> {
        const fileExtIndex = file.name.lastIndexOf(".");
        const fileExt = fileExtIndex > 0 ? file.name.substring(fileExtIndex).toLowerCase() : null;

        if (!fileExt || !_supportedFileTypes.find(x => fileExt === x.toLowerCase())) {
            window.alert(`Unsupported file extension: ${fileExt}`);
            return null;
        }

        let result: ApiUploadFileInfo[] | null;

        try {
            result = await shiftClient.file.uploadTempFile(file);
            removeError();
        } catch (err) {
            addError(err, "Failed to upload file");
            return null;
        }

        if (!result || result.length === 0) {
            return null;
        }

        return result[0];
    }

    async function handleUploadFileAndInsert(file: File) {
        if (!markdownRef.current) {
            return;
        }

        const apiFile = await handleUploadFile(file);
        if (!apiFile) {
            return;
        }

        const { FileIdentifier: fileId, FileName: fileName, DocumentName: documentName } = apiFile;

        let text = `[${documentName}](${urlHelper.getFileUrl(fileId, fileName)})`;

        if (file.type.toLowerCase().startsWith("image/")) {
            text = "!" + text;
        }

        markdownRef.current.focus();
        markdownRef.current.insert(text);
    }

    return (
        <div className={`richtexteditor ${error ? "is-invalid" : ""}`}>
            <RichTextEditor_Markdown
                ref={markdownRef}
                isTranslating={isTranslating}
                markdown={currentMarkdown?.[currentLanguage] ?? ""}
                disableUploadFile={disableUploadFile}
                onUploadFile={handleUploadFile}
                onChange={handleMarkdownChange}
                onBlur={onBlur}
            />

            <RichTextEditor_Translate
                isTranslating={isTranslating}
                language={currentLanguage}
                onTranslate={handleTranslate}
            />

            {!disableUploadFile && (
                <RichTextEditor_FileTypes
                    supportedFileTypes={_supportedFileTypes}
                    onUploadFile={handleUploadFileAndInsert}
                />
            )}

            <RichTextEditor_Languages
                markdown={currentMarkdown}
                excludeLanguage={currentLanguage}
                onSelect={handleSelectLanguage}
            />
        </div>
    );
}