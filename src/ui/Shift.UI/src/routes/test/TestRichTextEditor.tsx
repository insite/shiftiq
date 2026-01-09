import FormField from "@/components/form/FormField";
import RichTextEditor from "@/components/richtexteditor/RichTextEditor";

export default function TestRichTextEditor() {
    return (
        <>
            <div className="row">
                <div className="col-6">
                    <FormField>
                        <RichTextEditor
                            enableModeSwitch
                            defaultMode="markdown"
                            defaultValue={{
                                markdown: {
                                    en: `
                                        # Hello world
                                        > Quote
                                    `,
                                    fr: "Another test"
                                },
                                html: {
                                    en: "<p>Hello world</p>"
                                }
                            }}
                        />
                    </FormField>
                </div>
            </div>
        </>
    )
}