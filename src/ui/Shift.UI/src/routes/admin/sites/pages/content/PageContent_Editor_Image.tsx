import FileTextBox from "@/components/filetextbox/FileTextBox";
import TextBox from "@/components/TextBox";
import { translate } from "@/helpers/translate";
import { BlockFieldName } from "./models/BlockFieldName";
import { BlockState } from "./models/BlockState";
import { BlockImageValue } from "./models/BlockImageValue";
import { usePageContent_Provider } from "./PageContent_Provider";
import FormField from "@/components/form/FormField";

interface Props {
    block: BlockState;
    fieldName: BlockFieldName;
}

export default function PageContent_Editor_Image({ block, fieldName }: Props) {
    const field = block.contentFields.find(x => x.fieldName === fieldName);
    const fieldValue: BlockImageValue | undefined = field?.fieldValue as BlockImageValue;

    const { readOnly, modifyBlockField } = usePageContent_Provider();

    return (
        <FormField label={fieldName}>
            <div className="d-flex gap-2">
                <TextBox
                    placeholder={translate("Img Alt Text")}
                    className="w-25"
                    maxLength={64}
                    defaultValue={fieldValue?.alt ?? ""}
                    readOnly={readOnly}
                    onBlur={e => modifyBlockField(block.blockId, fieldName, {
                        key: 0,
                        url: fieldValue?.url ?? null,
                        alt: e.target.value,
                    })}
                />
                <FileTextBox
                    placeholder={translate("Img URL")}
                    className="w-75"
                    maxLength={512}
                    defaultValue={fieldValue?.url ?? ""}
                    readOnly={readOnly}
                    onBlur={value => modifyBlockField(block.blockId, fieldName, {
                        key: 0,
                        url: value,
                        alt: fieldValue?.alt ?? null,
                    })}
                />
            </div>
        </FormField>
    );
}