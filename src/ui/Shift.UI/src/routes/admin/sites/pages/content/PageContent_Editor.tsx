import { BlockFieldName } from "./models/BlockFieldName";
import { blockFieldList } from "./blockFieldList";
import PageContent_Editor_Image from "./PageContent_Editor_Image";
import PageContent_Editor_ImageList from "./PageContent_Editor_ImageList";
import { BlockState } from "./models/BlockState";
import PageContent_Editor_Text from "./PageContent_Editor_Text";
import PageContent_Editor_Html from "./PageContent_Editor_Html";

interface Props {
    block: BlockState;
    fieldName: BlockFieldName;
}

export default function PageContent_Editor({ block, fieldName }: Props) {
    const editorType = blockFieldList[fieldName];
    switch (editorType) {
        case "Html":
            return <PageContent_Editor_Html block={block} fieldName={fieldName} />
        case "Text":
            return <PageContent_Editor_Text block={block} fieldName={fieldName} />
        case "Image":
            return <PageContent_Editor_Image block={block} fieldName={fieldName} />;
        case "ImageList":
            return <PageContent_Editor_ImageList block={block} fieldName={fieldName} />;
        default:
            throw new Error(`Unknown editorType: ${editorType}`);
    }
}