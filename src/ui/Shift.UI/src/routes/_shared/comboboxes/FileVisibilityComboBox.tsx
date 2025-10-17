import ControlledComboBox, { ControlledComboBoxProps } from "@/components/combobox/ControlledComboBox";
import { translate } from "@/helpers/translate";

type Props<Criteria extends object> = Omit<ControlledComboBoxProps<Criteria>, "items">;

const _items = [
    { text: "", value: "" }, 
    { text: translate("Public"), value: "public" },
    { text: translate("Private"), value: "private" }
];

export default function FileVisibilityComboBox<Criteria extends object>(props: Props<Criteria>) {
    return (
        <ControlledComboBox
            {...props}
            items={_items}
        />
    )
}