import ControlledComboBox, { ControlledComboBoxProps } from "@/components/combobox/ControlledComboBox";
import { translate } from "@/helpers/translate";

type Props<Criteria extends object> = Omit<ControlledComboBoxProps<Criteria>, "items">;

const _items = [
    { text: "", value: "" }, 
    { text: translate("Temporary"), value: "Temporary" },
    { text: translate("User"), value: "User" },
    { text: translate("Issue"), value: "Issue" },
    { text: translate("Response"), value: "Response" },
    { text: translate("Attempt"), value: "Attempt" },
    { text: translate("Badge"), value: "Badge" },
    { text: translate("Product"), value: "Product" },
    { text: translate("Logbook Experience"), value: "LogbookExperience" },
    { text: translate("Credential"), value: "Credential" }
];

export default function FileObjectTypeComboBox<Criteria extends object>(props: Props<Criteria>) {
    return (
        <ControlledComboBox
            {...props}
            items={_items}
        />
    )
}