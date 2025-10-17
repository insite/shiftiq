import Button from "@/components/Button";
import DatePicker from "@/components/date/DatePicker";
import FormField from "@/components/form/FormField";
import MultiSelect from "@/components/multiselect/MultiSelect";
import TextBox from "@/components/TextBox";
import { useStatusProvider } from "@/contexts/StatusProvider";
import { useLoadingProvider } from "@/contexts/LoadingProvider";
import { ListItem } from "@/models/listItem";

const _items: ListItem[] = [];
for (let i = 0; i < 20; i++) {
    _items.push({ value: String(i + 1), text: `Option ${i + 1}` })
}

export default function TestNewFeatures() {
    const { addError, removeError } = useStatusProvider();
    const { addLoading, removeLoading } = useLoadingProvider();

    return (
        <>
            <div className="row">
                <div className="col-3">
                    <FormField>
                        <Button
                            variant="request"
                            className="w-100"
                            text="Try Loading"
                            onClick={() => {
                                addLoading();
                                setTimeout(() => removeLoading(), 2000)
                            }}
                        />
                    </FormField>
                    <FormField>
                        <Button
                            variant="request"
                            className="w-100"
                            text="Show Custom Error"
                            onClick={() => addError(new Error("My error"), "Custom Details")}
                        />
                    </FormField>
                    <FormField>
                        <Button
                            variant="request"
                            className="w-100"
                            text="Hide Custom Error"
                            onClick={() => removeError()}
                        />
                    </FormField>
                </div>
                <div className="col-3">
                    <FormField label="Custom Date Picker:">
                        <DatePicker />
                    </FormField>
                    <FormField label="Custom DateTime Picker:">
                        <DatePicker showTime />
                    </FormField>
                    <FormField label="TextBox:">
                        <TextBox />
                    </FormField>
                </div>
                <div className="col-3">
                    <FormField label="Custom Multiselect:">
                        <MultiSelect
                            maxHeight={200}
                            items={_items}
                            itemsSelectedText="Columns Selected"
                            allItemsSelectedText="All Columns Selected"
                            placeholder="Select Columns"
                        />
                    </FormField>
                </div>
            </div>
        </>
    )
}