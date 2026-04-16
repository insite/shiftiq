import FormCard from "@/components/form/FormCard";
import InlineEditor from "@/components/inlineeditor/InlineEditor";
import { MultiLanguageText } from "@/helpers/language";
import { useState } from "react";

export function TestInlineEditor() {
    const [item1, setItem1] = useState({
        value: "Question 1",
        valueHtml: "<p>Question 1</p>"
    });

    const [item2, setItem2] = useState({
        value: "ListItem1",
        valueHtml: "<p>ListItem1</p>"
    });

    const [item3, setItem3] = useState({
        value: "Option 1",
        valueHtml: "<p>Option 1</p>"
    });

    const [item4, setItem4] = useState<{ value: MultiLanguageText, valueHtml: string }>({
        value: { en: "Rich text 1" },
        valueHtml: "<p>Rich text 1</p>"
    });

    async function handleSaveItem1(value: string) {
        await new Promise<void>(resolve => setTimeout(() => resolve(), 1000));

        setItem1({
            value,
            valueHtml: `<p><b>${value}</b></p>`,
        });
    }

    async function handleSaveItem2(value: string) {
        await new Promise<void>(resolve => setTimeout(() => resolve(), 500));

        setItem2({
            value,
            valueHtml: `<p><b>${value}</b></p>`,
        });
    }

    async function handleSaveItem3(value: string) {
        await new Promise<void>(resolve => setTimeout(() => resolve(), 500));

        setItem3({
            value,
            valueHtml: `<p><b>${value}</b></p>`,
        });
    }

    async function handleSaveItem4(value: MultiLanguageText) {
        await new Promise<void>(resolve => setTimeout(() => resolve(), 500));

        setItem4({
            value,
            valueHtml: `<p><b>${value.en}</b></p>`,
        });
    }

    return (
        <div className="row">
            <div className="col-6">
                <FormCard hasBottomMargin={false}>
                    <div style={{ width: "400px" }}>
                        <div className="mb-3">
                            <InlineEditor
                                type="TextBox"
                                value={item1.value}
                                valueHtml={item1.valueHtml}
                                onSave={handleSaveItem1}
                            />
                        </div>
                        <div className="mb-3">
                            <InlineEditor
                                type="ComboBox"
                                value={item2.value}
                                valueHtml={item2.valueHtml}
                                items={[
                                    { value: "", text: "" },
                                    { value: "ListItem1", text: "List Item 1" },
                                    { value: "ListItem2", text: "List Item 2" },
                                    { value: "ListItem3", text: "List Item 3" },
                                ]}
                                onSave={handleSaveItem2}
                            />
                        </div>
                        <div className="mb-3">
                            <InlineEditor
                                type="TextArea"
                                value={item3.value}
                                valueHtml={item3.valueHtml}
                                onSave={handleSaveItem3}
                            />
                        </div>
                    </div>
                    <div>
                        <InlineEditor
                            type="MarkdownEditor"
                            value={item4.value}
                            valueHtml={item4.valueHtml}
                            onSave={handleSaveItem4}
                        />
                    </div>
                </FormCard>
            </div>
        </div>
    );
}