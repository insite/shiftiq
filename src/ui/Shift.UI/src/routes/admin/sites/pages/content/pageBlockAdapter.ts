import { ApiPageContentModel } from "@/api/controllers/pageContent/ApiPageContentModel";
import { BlockType } from "./models/BlockType";
import { ApiContentModel } from "@/api/models/ApiContentModel";
import { blockFieldList } from "./blockFieldList";
import { BlockFieldName } from "./models/BlockFieldName";
import { BlockFieldValue } from "./models/BlockFieldValue";
import { blockList } from "./blockList";
import { BlockImageValue } from "./models/BlockImageValue";
import { BlockState } from "./models/BlockState";
import { ApiPageContentModifyModel } from "@/api/controllers/pageContent/ApiPageContentModifyModel";
import { blockTypeNameList } from "./blockTypeNameList";
import { RichTextEditorValue } from "@/components/richtexteditor/RichTextEditorValue";

function getApiContent(block: BlockState) : ApiContentModel {
    const content: ApiContentModel = {};
    for (const { fieldName, fieldValue } of block.contentFields) {
        addApiContentItem(content, fieldName, fieldValue);
    }
    return content;
}

function addApiContentItem(content: ApiContentModel, fieldName: BlockFieldName, fieldValue: BlockFieldValue): void {
    if (!fieldValue) {
        return;
    }
    const editorType = blockFieldList[fieldName];
    if (!editorType) {
        throw new Error(`Non-supported block field: ${fieldName}`);
    }
    switch (editorType) {
        case "Html":
            content[fieldName] = {
                Text: (fieldValue as RichTextEditorValue).markdown,
                Html: (fieldValue as RichTextEditorValue).html,
            };
            break;
        case "Text":
            content[fieldName] = {
                Text: {
                    en: String(fieldValue)
                }
            };
            break;
        case "Image":
            if ((fieldValue as BlockImageValue).alt || (fieldValue as BlockImageValue).url) {
                content[fieldName + ":Alt"] = {
                    Text: {
                        en: (fieldValue as BlockImageValue).alt
                    }
                };
                content[fieldName + ":Url"] = {
                    Text: {
                        en: (fieldValue as BlockImageValue).url
                    }
                };
            }
            break;
        case "ImageList":
            addApiImageList(content, fieldName, fieldValue);
            break;
        default:
            throw new Error(`Non-supported editor type: ${editorType}`);
    }
}

function addApiImageList(content: ApiContentModel, fieldName: BlockFieldName, fieldValue: BlockFieldValue): void {
    const nonEmptyUrls = (fieldValue as BlockImageValue[]).filter(({ alt, url }) => alt || url);
    if (nonEmptyUrls.length === 0) {
        return;
    }

    for (let i = 0; i < nonEmptyUrls.length; i++) {
        const { alt, url } = nonEmptyUrls[i];
        content[`${fieldName}:${i}.Alt`] = {
            Text: {
                en: alt,
            }
        };
        content[`${fieldName}:${i}.Url`] = {
            Text: {
                en: url,
            }
        };
    }
}

function getContentFields(blockType: BlockType, content: ApiContentModel): {
    fieldName: BlockFieldName;
    fieldValue: BlockFieldValue;
}[]
{
    const fields = blockList.find(x => x.blockType === blockType)?.fields;
    if (!fields) {
        throw new Error(`Non-supported block type: ${blockType}`);
    }

    const result: {
        fieldName: BlockFieldName;
        fieldValue: BlockFieldValue;
    }[] = [];

    for (const fieldName of fields) {
        result.push({
            fieldName: fieldName,
            fieldValue: getBlockFieldValue(fieldName, content),
        });
    }

    return result;
}

function getBlockFieldValue(fieldName: BlockFieldName, content: ApiContentModel): BlockFieldValue {
    const editorType = blockFieldList[fieldName];
    if (!editorType) {
        throw new Error(`Non-supported block field: ${fieldName}`);
    }
    switch (editorType) {
        case "Html":
            return {
                markdown: content[fieldName]?.Text,
                html: content[fieldName]?.Html
            };
        case "Text":
            return content[fieldName]?.Text?.en ?? null;
        case "Image":
            return {
                key: 0,
                alt: content[fieldName + ":Alt"]?.Text?.en ?? null,
                url: content[fieldName + ":Url"]?.Text?.en ?? null,
            };
        case "ImageList":
            return getBlockImageList(fieldName, content);
        default:
            throw new Error(`Non-supported editor type: ${editorType}`);
    }
}

function getBlockImageList(fieldName: BlockFieldName, content: ApiContentModel): BlockImageValue[] {
    const result: BlockImageValue[] = [];
    let index = 0;

    for (let i = 0; i < 10000; i++) {
        const alt = content[`${fieldName}:${index}.Alt`]?.Text?.en ?? null;
        const url = content[`${fieldName}:${index}.Url`]?.Text?.en ?? null;

        if (!alt && !url) {
            return result;
        }

        index++;

        result.push({
            key: index,
            alt,
            url
        })
    }

    throw Error("Invalid ImageList");
}

export const pageBlockAdapter = {
    getBlocks(model: ApiPageContentModel): BlockState[] {
        const blocks: BlockState[] = [];

        for (const apiBlock of model.Blocks) {
            const contentFields = getContentFields(apiBlock.BlockType as BlockType, apiBlock.Content);
            blocks.push({
                blockId: apiBlock.BlockId.toLowerCase(),
                blockType: apiBlock.BlockType as BlockType,
                contentDirty: false,
                otherDirty: false,
                blockTitle: apiBlock.Title,
                hook: apiBlock.Hook ?? null,
                contentFields
            });
        }

        return blocks;
    },

    addModifiedBlocks(model: ApiPageContentModifyModel, blocks: BlockState[]): void {
        for (const block of blocks) {
            if (!block.contentDirty && !block.otherDirty) {
                continue;
            }
            if (!model.Blocks) {
                model.Blocks = [];
            }
            model.Blocks.push({
                BlockId: typeof block.blockId === "string" ? block.blockId : null,
                BlockIdNumber: typeof block.blockId === "number" ? block.blockId : null,
                BlockType: block.blockType,
                Title: block.blockTitle || blockTypeNameList[block.blockType],
                Hook: block.hook,
                Content: block.contentDirty ? getApiContent(block) : null,
            });
        }
    },
}