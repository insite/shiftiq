import ContentEdior from "@/components/contenteditor/ContentEditor";
import { ContentEditorResult } from "@/components/contenteditor/ContentEditorResult";

export default function TestContentEditor() {
    async function handleSave(_: string, result: ContentEditorResult): Promise<boolean> {
        console.log("values", result);
        await new Promise<void>(resolve => setTimeout(() => resolve(), 1000));
        return true;
    }

    return (
        <div className="row">
            <div className="col-6">
                <ContentEdior
                    defaultValues={{
                        editors: [
                            {
                                value: {
                                    markdown: {
                                        en: "This is body",
                                        fr: "This is body (fr)",
                                    },
                                },
                                options: {
                                    fieldName: "Body",
                                    type: "markdownAndHtml",
                                    title: "Body",
                                }
                            },
                            {
                                value: {
                                    markdown: { en: "This is title" },
                                },
                                options: {
                                    fieldName: "Title",
                                    type: "markdown",
                                    title: "Title",
                                }
                            },
                            {
                                value: {},
                                options: {
                                    fieldName: "Required Field",
                                    type: "markdown",
                                    title: "Required Field",
                                    required: true
                                }
                            },
                        ]
                    }}
                    onSave={handleSave}
                />
            </div>
        </div>
    )
}