import FormField from "@/components/form/FormField"
import TextBox from "@/components/TextBox";
import Icon from "@/components/icon/Icon";
import PageContent_Editor from "./PageContent_Editor";
import { BlockState } from "./models/BlockState";
import { blockList } from "./blockList";
import { usePageContent_Provider } from "./PageContent_Provider";
import { translate } from "@/helpers/translate";
import { BlockId } from "./models/BlockId";
import { blockTypeNameList } from "./blockTypeNameList";
import { useEffect, useRef } from "react";

interface Props {
    block: BlockState;
    selected: boolean;
}

export default function PageContent_Blocks_Edit({ block, selected }: Props) {
    const { readOnly, modifyBlockTitle, modifyBlockHook, deleteBlock } = usePageContent_Provider();

    const blockTitleRef = useRef<HTMLInputElement>(null);

    const blockInfo = blockList.find(x => x.blockType === block.blockType);
    if (!blockInfo) {
        throw new Error(`Block type ${block.blockType} doe snot exist`);
    }

    useEffect(() => {
        if (selected && blockTitleRef.current) {
            blockTitleRef.current.focus();
        }
    }, [selected])

    function handleDeleteBlock(blockId: BlockId) {
        if (window.confirm(translate("Are you sure to delete this block?"))) {
            deleteBlock(blockId);
        }
    }

    function handleTitleKeyDown(e: React.KeyboardEvent<HTMLInputElement>) {
        if (e.key === "Enter" && blockTitleRef.current) {
            modifyBlockTitle(block.blockId, blockTitleRef.current.value);
        }
    }

    return (
        <>
            <div className="row">
                <div className="col-md-4">
                    <FormField label={translate("Block Type")}>
                        <TextBox defaultValue={blockTypeNameList[blockInfo.blockType]} readOnly />
                    </FormField>
                </div>
                <div className="col-md-4">
                    <FormField
                        label={translate("Block Title")}
                        editTitle={translate("Delete Block")}
                        editIcon="trash-alt"
                        editDisabled={readOnly}
                        onEditClick={() => handleDeleteBlock(block.blockId)}
                    >
                        <TextBox
                            ref={blockTitleRef}
                            maxLength={128}
                            defaultValue={block.blockTitle}
                            readOnly={readOnly}
                            onBlur={e => modifyBlockTitle(block.blockId, e.target.value)}
                            onKeyDown={handleTitleKeyDown}
                        />
                    </FormField>
                </div>
                <div className="col-md-4">
                    <a
                        className="btn-scroll-bottom show"
                        href="#bottom"
                        data-scroll
                        data-fixed-element
                        tabIndex={-1}
                    >
                        <span className="btn-scroll-bottom-tooltip text-body-secondary fs-sm me-2">{translate("Bottom")}</span>
                        <Icon style="regular" name="arrow-down" className="btn-scroll-bottom-icon" />
                    </a>
                </div>
            </div>

            <div className="row mt-3">
                <div className="col-lg-12">
                    
                    <h3>{translate("Block Content")}</h3>

                    {blockInfo.fields.map(f => (
                        <PageContent_Editor key={f} block={block} fieldName={f} />
                    ))}

                    <FormField label={translate("Hook / Integration Code")}>
                        <TextBox
                            maxLength={100}
                            defaultValue={block.hook ?? ""}
                            readOnly={readOnly}
                            onBlur={e => modifyBlockHook(block.blockId, e.target.value)}
                        />
                    </FormField>

                </div>
            </div>
        </>
    )
}