import { translate } from "@/helpers/translate"
import { BlockType } from "./models/BlockType"

type BlockTypeNameList = {
    [blockType in BlockType]: string;
}

export const blockTypeNameList: BlockTypeNameList = {
    "HeadingAndParagraphs": translate("Heading and Paragraphs"),
    "HeadingAndParagraphsWithImage": translate("Heading and Paragraphs with Image"),
    "ImageGallery": translate("Image Gallery"),
    "TwoColumns": translate("Two Columns"),
    "LinkToAchievement": translate("Link to Achievement"),
    "LinkToAssessment": translate("Link to Assessment"),
    "LinkToCourse": translate("Link to Course"),
    "LinkToForm": translate("Link to Form"),
    "CourseSummary": translate("Course Summary"),
}