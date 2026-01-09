import ControlledComboBox, { ControlledComboBoxProps } from "@/components/combobox/ControlledComboBox";
import { translate } from "@/helpers/translate";

type Props<Criteria extends object> = Omit<ControlledComboBoxProps<Criteria>, "items">;

const _items = [{ text: "", value: "" }, { text: translate("Open"), value: "Open" }, { text: translate("Closed"), value: "Closed" }];

export default function CaseStatusCategoryComboBox<Criteria extends object>(props: Props<Criteria>) {
    return (
        <ControlledComboBox
            {...props}
            items={_items}
        />
    )
}