import { RefObject, useState } from "react";
import { useSaveAction } from "@/hooks/useSaveAction";
import { useStatusProvider } from "@/contexts/StatusProvider";
import { ApiUploadFileInfo } from "@/api/controllers/file/ApiUploadFileInfo";
import { shiftClient } from "@/api/shiftClient";
import { urlHelper } from "@/helpers/urlHelper";
import { Language, MultiLanguageText } from "../../helpers/language";
import { RichTextEditorValue } from "./RichTextEditorValue";
import { RichTextEditor_MarkdownRef } from "./RichTextEditor_Markdown";
import { RichTextEditor_HtmlRef } from "./RichTextEditor_Html";
import { RichTextEditorMode } from "./RichTextEditorMode";

export function useRichTextEditor(
    markdownRef: RefObject<RichTextEditor_MarkdownRef | null>,
    htmlRef: RefObject<RichTextEditor_HtmlRef | null>,
    mode: RichTextEditorMode,
    value: RichTextEditorValue | null | undefined,
    defaultValue: RichTextEditorValue | null | undefined,
    defaultLanguage: Language,
    supportedFileTypes: string[],
    onChange: ((value: RichTextEditorValue) => void) | undefined,
) {
    const [currentLanguage, setCurrentLanguage] = useState<Language>(defaultLanguage);
    const [storedValue, setStoredValue] = useState<RichTextEditorValue | undefined | null>(defaultValue);

    const { isSaving: isTranslating, runSave } = useSaveAction();

    const currentValue = value !== undefined ? value : storedValue;

    const setCurrentMarkdown = value !== undefined
        ? (newMarkdown: MultiLanguageText) => onChange?.({ markdown: newMarkdown, html: currentValue?.html ?? null })
        : (newMarkdown: MultiLanguageText) => {
            const newValue = { markdown: newMarkdown, html: currentValue?.html ?? null };
            setStoredValue(newValue);
            onChange?.(newValue);
        };

    const setCurrentHtml = value !== undefined
        ? (newHtml: MultiLanguageText) => onChange?.({ markdown: currentValue?.markdown ?? null, html: newHtml })
        : (newHtml: MultiLanguageText) => {
            const newValue = { markdown: currentValue?.markdown ?? null, html: newHtml };
            setStoredValue(newValue);
            onChange?.(newValue);
        };

    const setCurrentValue = value !== undefined
        ? (newValue: RichTextEditorValue) => onChange?.(newValue)
        : (newValue: RichTextEditorValue) => {
            setStoredValue(newValue);
            onChange?.(newValue);
        };
    const { addError, removeError } = useStatusProvider();

    function handleMarkdownChange(markdownText: string) {
        if (currentValue?.markdown?.[currentLanguage] === markdownText) {
            return;
        }

        const newMarkdown: MultiLanguageText = {...currentValue?.markdown};
        newMarkdown[currentLanguage] = markdownText;

        setCurrentMarkdown(newMarkdown);
    }

    function handleHtmlChange(html: string) {
        if (currentValue?.html?.[currentLanguage] === html) {
            return;
        }

        const newHtml: MultiLanguageText = {...currentValue?.html};
        newHtml[currentLanguage] = html;

        setCurrentHtml(newHtml);
    }

    function handleTranslate() {
        runSave(async () => {
            const englishTexts: string[] = [];
            let markdownIndex: number | null = null;
            let htmlIndex: number | null = null;

            if (currentValue?.markdown?.en) {
                markdownIndex = englishTexts.length;
                englishTexts.push(currentValue.markdown.en);
            }

            if (currentValue?.html?.en) {
                htmlIndex = englishTexts.length;
                englishTexts.push(currentValue.html.en);
            }

            if (englishTexts.length === 0) {
                return;
            }

            const translatedTexts = await shiftClient.translation.translate(englishTexts);
            if (!translatedTexts) {
                return;
            }

            const newMarkdown = markdownIndex !== null ? translatedTexts[markdownIndex] : currentValue?.markdown;
            const newHtml = htmlIndex !== null ? translatedTexts[htmlIndex] : currentValue?.html;

            setCurrentValue({ markdown: newMarkdown, html: newHtml });
        });
    }

    function handleSelectLanguage(language: Language) {
        setCurrentLanguage(language);
    }

    async function handleUploadFile(file: File, progressCallback?: (percent: number) => void): Promise<ApiUploadFileInfo | null> {
        const fileExtIndex = file.name.lastIndexOf(".");
        const fileExt = fileExtIndex > 0 ? file.name.substring(fileExtIndex).toLowerCase() : null;

        if (!fileExt || !supportedFileTypes.find(x => fileExt === x.toLowerCase())) {
            window.alert(`Unsupported file extension: ${fileExt}`);
            return null;
        }

        let result: ApiUploadFileInfo[] | null;

        try {
            result = await shiftClient.file.uploadTempFile(file, null, progressCallback);
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
        if (!markdownRef.current && !htmlRef.current) {
            return;
        }

        const apiFile = await handleUploadFile(file);
        if (!apiFile) {
            return;
        }

        const { FileIdentifier: fileId, FileName: fileName, DocumentName: documentName } = apiFile;
        const isImage = file.type.toLowerCase().startsWith("image/");
        const fileUrl = urlHelper.getFileUrl(fileId, fileName);

        if (markdownRef.current && mode === "markdown") {
            let text = `[${documentName}](${fileUrl})`;

            if (isImage) {
                text = "!" + text;
            }

            markdownRef.current.focus();
            markdownRef.current.insert(text);

        } else if (htmlRef.current && mode === "html") {
            const html = isImage
                ? `<img src="${fileUrl}" alt="${documentName}">`
                : `<a href="${fileUrl}">${documentName}</a>`;

            htmlRef.current.focus();
            htmlRef.current.insert(html);
        }
    }

    return {
        isTranslating,
        currentValue,
        currentLanguage,
        handleUploadFile,
        handleMarkdownChange,
        handleHtmlChange,
        handleTranslate,
        handleUploadFileAndInsert,
        handleSelectLanguage,
    };
}