import FormField from "@/components/form/FormField";
import { BlockFieldName } from "./models/BlockFieldName";
import { usePageContent_Provider } from "./PageContent_Provider";
import { BlockState } from "./models/BlockState";
import RichTextEditor from "@/components/richtexteditor/RichTextEditor";
import { RichTextEditorValue } from "@/components/richtexteditor/RichTextEditorValue";
import { useSiteProvider } from "@/contexts/SiteProvider";

interface Props {
    block: BlockState;
    fieldName: BlockFieldName;
}

export default function PageContent_Editor_Html({ block, fieldName }: Props) {
    const field = block.contentFields.find(x => x.fieldName === fieldName);

    const { readOnly, modifyBlockField } = usePageContent_Provider();
    const { siteSetting: { CurrentLanguage: defaultLanguage } } = useSiteProvider();

    return (
        <FormField>
            <RichTextEditor
                defaultLanguage={defaultLanguage}
                defaultValue={field?.fieldValue as RichTextEditorValue}
                defaultMode="html"
                htmlTitle={fieldName}
                markdownTitle={fieldName}
                enableModeSwitch={true}
                disabled={readOnly}
                onBlur={value => modifyBlockField(block.blockId, fieldName, value)}
            />
        </FormField>
    );
}