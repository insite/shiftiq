import { BlockFieldName } from "./models/BlockFieldName";
import { BlockType } from "./models/BlockType";

export interface BlockInfo {
    blockType: BlockType,
    fields: BlockFieldName[];
}

export const blockList: BlockInfo[] = [
    {
        blockType: "HeadingAndParagraphs",
        fields: ["Heading", "Paragraphs"],
    },
    {
        blockType: "HeadingAndParagraphsWithImage",
        fields: ["Heading", "Paragraphs", "Image URL"],
    },
    {
        blockType: "ImageGallery",
        fields: ["Image List"],
    },
    {
        blockType: "TwoColumns",
        fields: ["Column 1", "Column 2"],
    },
    {
        blockType: "LinkToAchievement",
        fields: ["Heading"],
    },
    {
        blockType: "LinkToAssessment",
        fields: ["Link Target"],
    },
    {
        blockType: "LinkToCourse",
        fields: ["Link Target"],
    },
    {
        blockType: "LinkToForm",
        fields: ["Link Target"],
    },
    {
        blockType: "CourseSummary",
        fields: ["Title", "Description", "Time Required", "Start URL"],
    },
];