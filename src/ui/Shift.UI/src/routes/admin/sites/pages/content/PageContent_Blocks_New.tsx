import Button from "@/components/Button";
import FormField from "@/components/form/FormField";
import TextBox from "@/components/TextBox";
import { translate } from "@/helpers/translate";
import { ListItem } from "@/models/listItem";
import { blockList } from "./blockList";
import ComboBox from "@/components/combobox/ComboBox";
import { useRef, useState } from "react";
import { usePageContent_Provider } from "./PageContent_Provider";
import { BlockType } from "./models/BlockType";
import { blockTypeNameList } from "./blockTypeNameList";

const blockTypeItems: ListItem[] = [{
    value: "",
    text: "",
}].concat(blockList.map(({ blockType }) => ({
    value: blockType,
    text: blockTypeNameList[blockType]
})));

export default function PageContent_Blocks_New() {
    const [blockType, setBlockType] = useState("");
    const [blockTitle, setBlockTitle] = useState("");

    const { readOnly, addBlock } = usePageContent_Provider();

    const titleRef = useRef<HTMLInputElement>(null);

    function handleChangeBlockType(blockType: string | null) {
        setBlockType(blockType ?? "");

        if (blockType && titleRef.current) {
            titleRef.current.focus();
        }
    }

    function handleAdd() {
        if (!blockType) {
            throw new Error("blockType is empty");
        }

        addBlock(blockType as BlockType, blockTitle);

        setBlockType("");
        setBlockTitle("");
    }

    return (
        <>
            <div className="row">
                <div className="col-md-4">
                    <FormField label={translate("Block Type")} required>
                        <ComboBox
                            value={blockType}
                            items={blockTypeItems}
                            onChange={handleChangeBlockType}
                        />
                    </FormField>
                </div>
                <div className="col-md-4">
                    <FormField label={translate("Block Title")}>
                        <TextBox
                            ref={titleRef}
                            value={blockTitle}
                            maxLength={128}
                            onChange={e => setBlockTitle(e.target.value)}
                        />
                    </FormField>
                </div>
            </div>

            <Button
                type="button"
                variant="add"
                text={translate("Add Block")}
                disabled={!blockType || readOnly}
                onClick={handleAdd}
            />
        </>
    );
}