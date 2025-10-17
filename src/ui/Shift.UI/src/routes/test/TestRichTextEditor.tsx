import FormField from "@/components/form/FormField";
import RichTextEditor from "@/components/richtexteditor/RichTextEditor";

export default function TestRichTextEditor() {
    return (
        <>
            <div className="row">
                <div className="col-6">
                    <FormField>
                        <RichTextEditor defaultMarkdown={{
                            en: `
                                # Hello world
                                > Quote
                            `,
                            fr: "Another test"
                        }} />
                    </FormField>
                </div>
            </div>
        </>
    )
}