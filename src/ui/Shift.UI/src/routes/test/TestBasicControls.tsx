import ComboBox from "@/components/combobox/ComboBox";
import TextBox from "@/components/TextBox";
import TestFinder from "@/routes/test/TestFinder";
import FormField from "@/components/form/FormField";
import DatePicker from "@/components/date/DatePicker";
import { dateTimeHelper } from "@/helpers/date/dateTimeHelper";
import { useSiteProvider } from "@/contexts/SiteProvider";
import MultiSelect from "@/components/multiselect/MultiSelect";
import FileUpload from "@/components/fileupload/FileUpload";

const items: { value: string, text: string }[] = [];
for (let i = 0; i < 10; i++) {
    items.push({ value: String(i), text: `Option ${i}` });
}

export default function TestBasicControls() {
    const { siteSetting } = useSiteProvider();

    return (
        <>
            <div className="row">
                <div className="col-3">
                    <FormField label="Input Text:">
                        <TextBox name="Input1" />
                    </FormField>

                    <FormField label="InSiteDropdown:">
                        <ComboBox items={items} maxHeight={200} placeholder="Select option..." />
                    </FormField>

                    <FormField label="InSiteDropdown (preset):">
                        <ComboBox items={items} maxHeight={200} defaultValue="1" placeholder="Select option..." />
                    </FormField>

                    <FormField label="Input Text:">
                        <TextBox placeholder="Test Input" />
                    </FormField>

                    <FormField label="TestFinder:">
                        <TestFinder />
                    </FormField>

                    <FormField label="Input Text:">
                        <TextBox />
                    </FormField>

                </div>
                <div className="col-3">

                    <FormField label="Input Text:">
                        <TextBox />
                    </FormField>

                    <FormField label="Multi Dropdown:">
                        <MultiSelect
                            items={items}
                            placeholder="Select items..."
                            itemsSelectedText="Items Selected"
                            allItemsSelectedText="All Items Selected"
                            maxHeight={200}
                        />
                    </FormField>

                    <FormField label="Multi Dropdown (Preselected):">
                        <MultiSelect
                            items={items}
                            placeholder="Select columns..."
                            itemsSelectedText="Columns Visible"
                            allItemsSelectedText="All Columns Visible"
                            defaultValues={["1", "3", "9", "100"]}
                            maxHeight={200}
                        />
                    </FormField>

                    <FormField label="Multi Dropdown (FullHeight):">
                        <MultiSelect
                            items={items}
                            placeholder="Select columns..."
                            itemsSelectedText="Columns Visible"
                            allItemsSelectedText="All Columns Visible"
                        />
                    </FormField>

                    <FormField label="File Upload:">
                        <FileUpload allowMultiple />
                    </FormField>

                </div>
                <div className="col-3">

                    <FormField label="Date picker 1:">
                        <DatePicker name="Picker1" value={dateTimeHelper.today()} />
                    </FormField>

                    <FormField label="Date picker 2:">
                        <DatePicker name="Picker2" />
                    </FormField>

                    <FormField label="DateTime picker:">
                        <DatePicker
                            name="DateTimePicker1"
                            showTime
                            defaultTimeZoneId={siteSetting.TimeZoneId}
                        />
                    </FormField>

                    <FormField label="DateTime picker (preset):">
                        <DatePicker
                            name="DateTimePicker2"
                            showTime
                            value={dateTimeHelper.parseDateTime("Feb 28, 2025 11:45 AM MT")}
                        />
                    </FormField>

                    <FormField label="DateTime picker (current time):">
                        <DatePicker
                            name="DateTimePicker3"
                            value={dateTimeHelper.now(siteSetting.TimeZoneId)}
                            showTime
                        />
                    </FormField>
                </div>
            </div>
        </>
    )
}