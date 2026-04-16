import { ForwardedRef, useEffect, useImperativeHandle, useRef, useState } from "react";

import {
    BlockTypeSelect,
    BoldItalicUnderlineToggles,
    ButtonWithTooltip,
    CreateLink,
    DiffSourceToggleWrapper,
    InsertImage,
    InsertTable,
    ListsToggle,
    MDXEditor,
    MDXEditorMethods,
    Separator,
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
import Icon from "../icon/Icon";
import { translate } from "@/helpers/translate";

export interface RichTextEditor_MarkdownRef {
    focus: () => void;
    insert: (text: string) => void;
}

interface Props {
    ref?: ForwardedRef<RichTextEditor_MarkdownRef>;
    disabled: boolean;
    markdown: string;
    disableUploadFile: boolean;
    enableSelectFile: boolean;
    onUploadFile(file: File): Promise<ApiUploadFileInfo | null>;
    onSelectFile?(): void,
    onChange: (markdown: string) => void;
    onBlur?: () => void;
}

export default function RichTextEditor_Markdown({
    ref,
    disabled,
    markdown,
    disableUploadFile,
    enableSelectFile,
    onUploadFile,
    onSelectFile,
    onChange,
    onBlur
}: Props) {
    // MDXEditor trims the initial value and even if I set trim=false then it doesn't prevent trimming
    // This triggers the change event before the component is mounted
    // Therefore prevMarkdown is needed to prevent triggering the change event
    const prevMarkdown = useRef<string | undefined>(undefined);

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
            if (disableUploadFile
                || !markdownRef.current
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

            const { FileId: fileId, FileName: fileName, DocumentName: documentName } = apiFile;
            const text = `[${documentName}](${urlHelper.getFileUrl(fileId, fileName)})`;

            markdownRef.current.insertMarkdown(text);
        }

        return () => editor.removeEventListener("drop", editorDrop);
    }, [onUploadFile, disableUploadFile]);

    useEffect(() => {
        if (prevMarkdown.current === undefined) {
            prevMarkdown.current = markdown;
        }
        else if (markdownRef.current
            && prevMarkdown.current !== markdown
            && markdownRef.current.getMarkdown() !== markdown
        ) {
            prevMarkdown.current = markdown;

            markdownRef.current.setMarkdown(markdown);
        }
    }, [markdown]);

    async function uploadFile(file: File) {
        const result = await onUploadFile(file);
        if (!result) {
            return null as unknown as string;
        }

        const { FileId: fileId, FileName: fileName } = result;

        setUploadedFileIds([...uploadedFileIds, fileId]);

        return urlHelper.getFileUrl(fileId, fileName);
    }

    function handleChange(newMarkdown: string) {
        if (prevMarkdown.current !== undefined && onChange) {
            onChange(newMarkdown);
        }
    }

    return (
        <div ref={wrapperRef} className="markdown">
            <MDXEditor
                ref={markdownRef}
                className="light-theme"
                readOnly={disabled}
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
                                <BoldItalicUnderlineToggles />
                                <Separator />
                                <BlockTypeSelect />
                                <Separator />
                                <ListsToggle options={["bullet", "number"]} />
                                <Separator />
                                <CreateLink />
                                {!disableUploadFile && <InsertImage />}
                                {enableSelectFile && (
                                    <ButtonWithTooltip
                                        title={translate("Select Uploaded Image")}
                                        className="px-1"
                                        disabled={disabled}
                                        onClick={onSelectFile}
                                    >
                                        <Icon style="regular" name="images" className="text-dark" />
                                    </ButtonWithTooltip>
                                )}
                                <InsertTable />
                            </DiffSourceToggleWrapper>
                        )
                    })
                ]}
                onChange={handleChange}
                onBlur={onBlur}
            />
        </div>
    );
}
