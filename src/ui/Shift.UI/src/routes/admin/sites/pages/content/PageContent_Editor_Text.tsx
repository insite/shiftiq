import FormField from "@/components/form/FormField";
import { BlockFieldName } from "./models/BlockFieldName";
import { usePageContent_Provider } from "./PageContent_Provider";
import { BlockState } from "./models/BlockState";
import TextBox from "@/components/TextBox";

interface Props {
    block: BlockState;
    fieldName: BlockFieldName;
}

export default function PageContent_Editor_Text({ block, fieldName }: Props) {
    const field = block.contentFields.find(x => x.fieldName === fieldName);

    const { readOnly, modifyBlockField } = usePageContent_Provider();

    return (
        <FormField label={fieldName}>
            <TextBox
                maxLength={500}
                defaultValue={field?.fieldValue as string}
                readOnly={readOnly}
                onBlur={e => modifyBlockField(block.blockId, fieldName, e.target.value)}
            />
        </FormField>
    );
}