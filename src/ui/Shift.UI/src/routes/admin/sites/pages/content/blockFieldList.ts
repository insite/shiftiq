import { BlockEditorType } from "./models/BlockEditorType"
import { BlockFieldName } from "./models/BlockFieldName"

type BlockFieldList = {
    [key in BlockFieldName]: BlockEditorType;
}

export const blockFieldList: BlockFieldList = {
    "Body": "Html",
    "Column 1": "Html",
    "Column 2": "Html",
    "Description": "Html",
    "Paragraphs": "Html",
    "Image URL": "Image",
    "Image List": "ImageList",
    "Heading": "Text",
    "Link Target": "Text",
    "Title": "Text",
    "Time Required": "Text",
    "Start URL": "Text",
}