import { BlockFieldName } from "./BlockFieldName";
import { BlockFieldValue } from "./BlockFieldValue";
import { BlockType } from "./BlockType";

export interface BlockState {
    blockId: string | number
    blockType: BlockType;
    contentDirty: boolean;
    otherDirty: boolean;
    blockTitle: string;
    hook: string | null;
    contentFields: {
        fieldName: BlockFieldName;
        fieldValue: BlockFieldValue;
    }[];
}