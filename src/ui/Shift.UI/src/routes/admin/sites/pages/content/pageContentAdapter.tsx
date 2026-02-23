import { ApiPageContentModel } from "@/api/controllers/pageContent/ApiPageContentModel";
import { ApiContentModel } from "@/api/models/ApiContentModel";
import { ContentEditorValues } from "@/components/contenteditor/ContentEditorValues";
import { EditorOptions } from "@/components/contenteditor/EditorOptions";
import { RichTextEditorValue } from "@/components/richtexteditor/RichTextEditorValue";
import { ReactNode } from "react";
import PageContent_Blocks from "./PageContent_Blocks";
import { ApiPageContentModifyModel } from "@/api/controllers/pageContent/ApiPageContentModifyModel";
import { ContentEditorResult } from "@/components/contenteditor/ContentEditorResult";

function getEditor(fieldName: string, content: ApiContentModel): {
    value: RichTextEditorValue;
    options: EditorOptions;
    control: ReactNode | undefined;
}
{
    const value = {
        html: content[fieldName]?.Html,
        markdown: content[fieldName]?.Text,
    };

    switch (fieldName) {
        case "HtmlHead":
            return {
                value,
                options: {
                    fieldName,
                    type: "html",
                    title: "<HEAD>",
                },
                control: undefined,
            };
        case "PageBlocks":
            return {
                value: {},
                options: {
                    fieldName,
                    type: "custom",
                    title: "Blocks",
                },
                control: <PageContent_Blocks />
            };
        default:
            return {
                value,
                options: {
                    fieldName,
                    type: "markdownAndHtml",
                    title: fieldName,
                },
                control: undefined,
            }
    }
}

export const pageContentAdapter = {
    getContentEditorValues(model: ApiPageContentModel): ContentEditorValues {
        const editors = model.ContentFields.map(fieldName => getEditor(fieldName, model.Content));
        return { editors };
    },

    addModifiedContent(model: ApiPageContentModifyModel, result: ContentEditorResult): void {
        if (result.fields.length === 0) {
            return;
        }

        model.Content = {};

        for (const { fieldName, value } of result.fields) {
            model.Content[fieldName] = {
                Text: value.markdown,
                Html: value.html,
            }
        }
    },
}