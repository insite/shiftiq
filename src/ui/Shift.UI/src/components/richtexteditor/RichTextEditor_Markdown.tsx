import { ForwardedRef, useEffect, useImperativeHandle, useRef, useState } from "react";

import {
    BlockTypeSelect,
    BoldItalicUnderlineToggles,
    CreateLink,
    DiffSourceToggleWrapper,
    InsertImage,
    InsertTable,
    ListsToggle,
    MDXEditor,
    MDXEditorMethods,
    Separator,
    UndoRedo,
    diffSourcePlugin,
    headingsPlugin,
    imagePlugin,
    linkDialogPlugin,
    linkPlugin,
    listsPlugin,
    markdownShortcutPlugin,
    quotePlugin,
    tablePlugin,
    thematicBreakPlugin,
    toolbarPlugin
} from "@mdxeditor/editor";

import { ApiUploadFileInfo } from "@/api/controllers/file/ApiUploadFileInfo";
import { urlHelper } from "@/helpers/urlHelper";

import "@mdxeditor/editor/style.css";

export interface RichTextEditor_MarkdownRef {
    focus: () => void;
    insert: (text: string) => void;
}

interface Props {
    ref?: ForwardedRef<RichTextEditor_MarkdownRef>,
    isTranslating: boolean;
    markdown: string;
    disableUploadFile: boolean;
    onUploadFile(file: File): Promise<ApiUploadFileInfo | null>;
    onChange: (markdown: string) => void;
    onBlur?: () => void;
}

export default function RichTextEditor_Markdown({
    ref,
    isTranslating,
    markdown,
    disableUploadFile,
    onUploadFile,
    onChange,
    onBlur
}: Props) {
    const [uploadedFileIds, setUploadedFileIds] = useState<string[]>([]);

    const wrapperRef = useRef<HTMLDivElement>(null);

    const markdownRef = useRef<MDXEditorMethods>(null);

    useImperativeHandle(ref, () => ({
        focus() {
            markdownRef.current?.focus();
        },
        insert(text: string) {
            markdownRef.current?.insertMarkdown(text);
        },
    }), []);

    useEffect(() => {
        if (!wrapperRef.current) {
            return;
        }

        const editor = wrapperRef.current.querySelector(".mdxeditor-rich-text-editor") as HTMLDivElement;
        editor.addEventListener("drop", editorDrop);

        async function editorDrop(e: DragEvent) {
            if (!markdownRef.current
                || !e.dataTransfer?.items
                || e.dataTransfer.items.length !== 1
                || e.dataTransfer.items[0].kind !== "file"
            ) {
                return;
            }

            const file = e.dataTransfer.items[0].getAsFile();
            if (!file || file.type.toLowerCase().startsWith("image/")) {
                return;
            }

            const apiFile = await onUploadFile(file);
            if (!apiFile) {
                return;
            }

            const { FileIdentifier: fileId, FileName: fileName, DocumentName: documentName } = apiFile;
            const text = `[${documentName}](${urlHelper.getFileUrl(fileId, fileName)})`;

            markdownRef.current.insertMarkdown(text);
        }

        return () => editor.removeEventListener("drop", editorDrop);
    }, [onUploadFile]);

    useEffect(() => {
        if (markdownRef.current && markdownRef.current.getMarkdown() !== markdown) {
            markdownRef.current.setMarkdown(markdown);
        }
    }, [markdown]);

    async function uploadFile(file: File) {
        const result = await onUploadFile(file);
        if (!result) {
            return null as unknown as string;
        }

        const { FileIdentifier: fileId, FileName: fileName } = result;

        setUploadedFileIds([...uploadedFileIds, fileId]);

        return urlHelper.getFileUrl(fileId, fileName);
    }

    return (
        <div ref={wrapperRef} className="markdown">
            <MDXEditor
                ref={markdownRef}
                readOnly={isTranslating}
                markdown={markdown}
                plugins={[
                    headingsPlugin(),
                    listsPlugin(),
                    quotePlugin(),
                    thematicBreakPlugin(),
                    linkPlugin(),
                    linkDialogPlugin(),
                    imagePlugin({
                        imageUploadHandler: disableUploadFile ? null : uploadFile,
                    }),
                    tablePlugin(),
                    markdownShortcutPlugin(),
                    diffSourcePlugin(),
                    toolbarPlugin({
                        toolbarContents: () => (
                            <DiffSourceToggleWrapper options={["rich-text", "source"]}>
                                <UndoRedo />
                                <Separator />
                                <BoldItalicUnderlineToggles />
                                <Separator />
                                <BlockTypeSelect />
                                <Separator />
                                <ListsToggle options={["bullet", "number"]} />
                                <Separator />
                                <CreateLink />
                                {!disableUploadFile && <InsertImage />}
                                <InsertTable />
                            </DiffSourceToggleWrapper>
                        )
                    })
                ]}
                onChange={onChange}
                onBlur={onBlur}
            />
        </div>
    );
}