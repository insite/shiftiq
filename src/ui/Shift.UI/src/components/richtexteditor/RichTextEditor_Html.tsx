import { ForwardedRef, useEffect, useImperativeHandle, useRef, useState } from "react";
import { Editor } from "@tinymce/tinymce-react";
import { Editor as TinyMCEEditor } from "tinymce";
import { shiftConfig } from "@/helpers/shiftConfig";
import { ApiUploadFileInfo } from "@/api/controllers/file/ApiUploadFileInfo";
import { urlHelper } from "@/helpers/urlHelper";

export interface RichTextEditor_HtmlRef {
    focus: () => void;
    insert: (html: string) => void;
}

interface BlobInfo {
    id: () => string;
    name: () => string;
    filename: () => string;
    blob: () => Blob;
    base64: () => string;
    blobUri: () => string;
    uri: () => string | undefined;
}

type ProgressFn = (percent: number) => void;

interface Props {
    ref?: ForwardedRef<RichTextEditor_HtmlRef>;
    isTranslating: boolean;
    html: string;
    disableUploadFile: boolean;
    supportedImageFileTypes: string[];
    onUploadFile(file: File, progressCallback?: (percent: number) => void): Promise<ApiUploadFileInfo | null>;
    onChange: (html: string) => void;
    onBlur?: () => void;
}

export default function RichTextEditor_Html({
    ref,
    isTranslating,
    html,
    disableUploadFile,
    supportedImageFileTypes,
    onUploadFile,
    onChange,
    onBlur,
}: Props) {
    const [initialValue, setInitialValue] = useState(html);

    const editorRef = useRef<TinyMCEEditor>(null);

    useImperativeHandle(ref, () => ({
        focus() {
            editorRef.current?.focus();
        },
        insert(html: string) {
            editorRef.current?.insertContent(html);
        },
    }), []);

    useEffect(() => {
        if (editorRef.current !== null && editorRef.current.getContent() !== html) {
            setInitialValue(html);
        }
    }, [html])

    function handleFileDrop(e: DragEvent) {
        if (disableUploadFile) {
            e.preventDefault();
            return;
        }

        if (!e.dataTransfer?.items
            || e.dataTransfer.items.length !== 1
            || e.dataTransfer.items[0].kind !== "file"
        ) {
            return;
        }

        const file = e.dataTransfer.items[0].getAsFile();
        if (!file || file.type.toLowerCase().startsWith("image/")) {
            return;
        }

        e.preventDefault();

        onUploadFile(file)
            .then(apiFile => {
                if (!apiFile) {
                    return;
                }

                const { FileIdentifier: fileId, FileName: fileName, DocumentName: documentName } = apiFile;
                const html = `<a href="${urlHelper.getFileUrl(fileId, fileName)}">${documentName}</a>`;

                editorRef.current?.insertContent(html);
            });
    };

    async function handleImageUpload(blobInfo: BlobInfo, progress: ProgressFn): Promise<string> {
        const formData = new FormData();
        formData.append("file", blobInfo.blob(), blobInfo.filename());

        const file = formData.get("file") as File;

        const apiFile = await onUploadFile(file, progress);
        if (!apiFile) {
            return "";
        }

        return urlHelper.getFileUrl(apiFile.FileIdentifier, apiFile.FileName);
    }

    return (
        <Editor
            tinymceScriptSrc={shiftConfig.tinymceScript}
            licenseKey="gpl"
            initialValue={initialValue}
            disabled={isTranslating}
            init={{
                menubar: false,
                statusbar: false,
                plugins: [
                    "lists",
                    "link",
                    "autolink",
                    "image",
                    "code",
                    "fullscreen",
                    "table",
                ],
                toolbar: [
                    { name: "styles", items: ["styles", "bold", "italic", "underline", "additional_format", "forecolor", "backcolor"] },
                    { name: "functions", items: ["bullist", "numlist", "paragraph", "table", "link", "image"] },
                    { name: "mode", items: ["code", "fullscreen"] },
                ],
                toolbar_groups: {
                    additional_format: {
                        icon: "strike-through",
                        items: "strikethrough superscript subscript removeformat",
                    },
                    paragraph: {
                        icon: "align-left",
                        tooltip: "Paragraph",
                        items: "alignleft aligncenter alignright alignjustify outdent indent lineheight",
                    },
                },
                content_style: "body { font-family:Helvetica,Arial,sans-serif; font-size:14px }",
                images_reuse_filename: true,
                images_file_types: supportedImageFileTypes.map(x => x.substring(1)).join(","),
                images_upload_handler: disableUploadFile ? undefined : handleImageUpload,
            }}
            onInit={(_, editor) => editorRef.current = editor}
            onEditorChange={onChange}
            onDrop={handleFileDrop}
            onBlur={onBlur}
        />
    );
}