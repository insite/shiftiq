import { BlockFieldName } from "./models/BlockFieldName";
import { BlockFieldValue } from "./models/BlockFieldValue";
import { createContext, ReactNode, useContext, useMemo, useReducer } from "react";
import { BlockId } from "./models/BlockId";
import { BlockState } from "./models/BlockState";
import { BlockType } from "./models/BlockType";
import { blockList } from "./blockList";

interface ContextData {
    blocks: BlockState[];
    deletedBlockIds: string[];
    selectedBlockId: BlockId | null;
    readOnly: boolean;
    isDirty: boolean;
    initBlocks: (blocks: BlockState[], defaultSelectedBlockId: BlockId | null) => void;
    addBlock: (blockType: BlockType, blockTitle: string) => void;
    deleteBlock: (blockId: BlockId) => void;
    modifyBlockField: (blockId: BlockId, fieldName: BlockFieldName, fieldValue: BlockFieldValue) => void;
    modifyBlockTitle: (blockId: BlockId, blockTitle: string) => void;
    modifyBlockHook: (blockId: BlockId, hook: string) => void;
    selectBlock: (blockId: BlockId | null) => void;
    setReadOnly: (readOnly: boolean) => void;
    clearDirty: (replacedBlockIds: Record<number, string>) => void;
}

const PageContent_ProviderContext = createContext<ContextData>({
    blocks: [],
    deletedBlockIds: [],
    selectedBlockId: null,
    readOnly: false,
    isDirty: false,
    initBlocks() {},
    addBlock() {},
    deleteBlock() {},
    modifyBlockField() {},
    modifyBlockTitle() {},
    modifyBlockHook() {},
    selectBlock() {},
    setReadOnly() {},
    clearDirty() {},
});

interface InitBlocksAction {
    type: "initBlocks";
    blocks: BlockState[];
    defaultSelectedBlockId: BlockId | null;
}

interface AddBlockAction {
    type: "addBlock";
    blockType: BlockType;
    blockTitle: string;
}

interface DeleteBlockAction {
    type: "deleteBlock";
    blockId: BlockId;
}

interface ModifyBlockFieldAction {
    type: "modifyBlockField";
    blockId: BlockId;
    fieldName: BlockFieldName;
    fieldValue: BlockFieldValue;
}

interface ModifyBlockTitleAction {
    type: "modifyBlockTitle";
    blockId: BlockId;
    blockTitle: string;
}

interface ModifyBlockHookAction {
    type: "modifyBlockHook";
    blockId: BlockId;
    hook: string | null;
}

interface SelectBlockAction {
    type: "selectBlock";
    blockId: BlockId | null;
}

interface SetReadOnlyAction {
    type: "setReadOnly";
    readOnly: boolean;
}

interface ClearDirtyAction {
    type: "clearDirty";
    replacedBlockIds: Record<number, string>;
}

type Action = InitBlocksAction
    | AddBlockAction
    | DeleteBlockAction
    | ModifyBlockFieldAction
    | ModifyBlockTitleAction
    | ModifyBlockHookAction
    | SelectBlockAction
    | SetReadOnlyAction
    | ClearDirtyAction
    ;

interface State {
    blocks: BlockState[];
    deletedBlockIds: string[];
    selectedBlockId: BlockId | null;
    readOnly: boolean;
}

const _initialState: State = {
    blocks: [],
    deletedBlockIds: [],
    selectedBlockId: null,
    readOnly: false,
}

function getSelectedBlockIdOnRemove(blocks: BlockState[], deletedBlock: BlockState): BlockId | null {
    if (blocks.length === 1) {
        return null;
    }

    const deletedIndex = blocks.findIndex(x => x === deletedBlock);

    return deletedIndex === blocks.length - 1
        ? blocks[deletedIndex - 1].blockId
        : blocks[deletedIndex + 1].blockId;
}

function reducer(state: State, action: Action): State {
    const { type } = action;
    switch (type) {
        case "initBlocks":
        {
            const selectedBlockId = action.defaultSelectedBlockId && action.blocks.find(x => x.blockId === action.defaultSelectedBlockId)
                ? action.defaultSelectedBlockId
                : action.blocks.length > 0 ? action.blocks[0].blockId : null;

            return {
                ...state,
                blocks: action.blocks,
                selectedBlockId,
            };
        }

        case "addBlock":
        {
            const blockId = state.blocks.reduce((id, b) => typeof(b.blockId) === "number" && b.blockId > id ? b.blockId : id, 0) + 1;
            return {...state, blocks: [...state.blocks, {
                    blockId,
                    blockType: action.blockType,
                    contentDirty: true,
                    otherDirty: true,
                    blockTitle: action.blockTitle,
                    hook: null,
                    contentFields: []
                }],
                selectedBlockId: blockId,
            };
        }

        case "deleteBlock":
        {
            const block = state.blocks.find(x => x.blockId === action.blockId);
            if (!block) {
                throw new Error(`Block ${action.blockId} does not exist`);
            }
            const deletedBlockIds = [...state.deletedBlockIds];
            if (typeof block.blockId === "string") {
                deletedBlockIds.push(block.blockId);
            }
            return {
                ...state,
                blocks: state.blocks.filter(x => x !== block),
                deletedBlockIds,
                selectedBlockId: getSelectedBlockIdOnRemove(state.blocks, block),
            };
        }

        case "modifyBlockField":
        {
            const blockIndex = state.blocks.findIndex(x => x.blockId === action.blockId)!;
            if (blockIndex < 0) {
                throw new Error(`Block ${action.blockId} does not exist`);
            }
            const block = state.blocks[blockIndex];
            if (!blockList.find(x => x.blockType == block.blockType)?.fields?.includes?.(action.fieldName)) {
                throw new Error(`Field ${action.fieldName} is not specified for the block type ${block.blockType}`);
            }
            const existing = block.contentFields.find(x => x.fieldName === action.fieldName);
            if (existing?.fieldValue === action.fieldValue) {
                return state;
            }

            const blocks = [...state.blocks];
            blocks[blockIndex] = {
                ...block,
                contentDirty: true,
                contentFields: [
                    ...block.contentFields.filter(x => x.fieldName !== action.fieldName),
                    {
                        fieldName: action.fieldName,
                        fieldValue: action.fieldValue,
                    }
                ]
            };
            return {...state, blocks};
        }

        case "modifyBlockTitle": {
            const blockIndex = state.blocks.findIndex(x => x.blockId === action.blockId)!;
            if (blockIndex < 0) {
                throw new Error(`Block ${action.blockId} does not exist`);
            }
            const block = state.blocks[blockIndex];
            if (block.blockTitle === action.blockTitle) {
                return state;
            }

            const blocks = [...state.blocks];
            blocks[blockIndex] = {
                ...block,
                otherDirty: true,
                blockTitle: action.blockTitle,
            };
            return {...state, blocks};
        }

        case "modifyBlockHook": {
            const blockIndex = state.blocks.findIndex(x => x.blockId === action.blockId)!;
            if (blockIndex < 0) {
                throw new Error(`Block ${action.blockId} does not exist`);
            }
            const block = state.blocks[blockIndex];
            if (block.hook === action.hook) {
                return state;
            }

            const blocks = [...state.blocks];
            blocks[blockIndex] = {
                ...block,
                otherDirty: true,
                hook: action.hook,
            };
            return {...state, blocks};
        }

        case "selectBlock": {
            const selectedBlockId = typeof action.blockId === "string" && !isNaN(Number(action.blockId)) 
                ? Number(action.blockId)
                : action.blockId;

            return {...state, selectedBlockId};
        }

        case "setReadOnly": {
            return {...state, readOnly: action.readOnly};
        }

        case "clearDirty": {
            const blocks = state.blocks.map(block => ({
                ...block,
                blockId: typeof block.blockId === "number" ? action.replacedBlockIds[block.blockId] : block.blockId,
                contentDirty: false,
                otherDirty: false,
            }));
            return {
                ...state,
                blocks,
                deletedBlockIds: [],
                selectedBlockId: typeof state.selectedBlockId === "number" ? action.replacedBlockIds[state.selectedBlockId] : state.selectedBlockId,
            };
        }

        default:
            throw new Error(`Unknown action: ${type}`);
    }
}

interface Props {
    children?: ReactNode;
}

export default function PageContent_Context({ children }: Props) {
    const [{ blocks, deletedBlockIds, selectedBlockId, readOnly }, dispatch] = useReducer(reducer, _initialState);

    const methods = useMemo(() => ({
        initBlocks(blocks: BlockState[], defaultSelectedBlockId: BlockId | null): void {
            dispatch({ type: "initBlocks", blocks, defaultSelectedBlockId });
        },

        addBlock(blockType: BlockType, blockTitle: string): void {
            dispatch({ type: "addBlock", blockType, blockTitle });
        },

        deleteBlock(blockId: BlockId): void {
            dispatch({ type: "deleteBlock", blockId });
        },

        modifyBlockField(blockId: BlockId, fieldName: BlockFieldName, fieldValue: BlockFieldValue): void {
            dispatch({ type: "modifyBlockField", blockId, fieldName, fieldValue });
        },

        modifyBlockTitle(blockId: BlockId, blockTitle: string): void {
            dispatch({ type: "modifyBlockTitle", blockId, blockTitle });
        },

        modifyBlockHook(blockId: BlockId, hook: string): void {
            dispatch({ type: "modifyBlockHook", blockId, hook });
        },

        selectBlock(blockId: BlockId | null): void {
            dispatch({ type: "selectBlock", blockId });
        },

        setReadOnly(readOnly: boolean): void {
            dispatch({ type: "setReadOnly", readOnly });
        },

        clearDirty(replacedBlockIds: Record<number, string>): void {
            dispatch({ type: "clearDirty", replacedBlockIds });
        },
    }), [dispatch]);

    const providerValue = useMemo(() => ({
        blocks,
        deletedBlockIds,
        selectedBlockId,
        readOnly,
        isDirty: !!blocks.find(x => x.contentDirty || x.otherDirty),
        ...methods,
    }), [methods, blocks, deletedBlockIds, selectedBlockId, readOnly]);

    return (
        <PageContent_ProviderContext.Provider value={providerValue}>
            {children}
        </PageContent_ProviderContext.Provider>
    );
}

export function usePageContent_Provider() {
    return useContext(PageContent_ProviderContext);
}