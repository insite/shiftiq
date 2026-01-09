import ControlledComboBox, { ControlledComboBoxProps } from "@/components/combobox/ControlledComboBox";
import { translate } from "@/helpers/translate";

type Props<Criteria extends object> = Omit<ControlledComboBoxProps<Criteria>, "items">;

const _items = [{ text: "", value: "" }, { text: translate("Locked"), value: "locked" }, { text: translate("Unlocked"), value: "unlocked" }];

export default function GradebookStatusComboBox<Criteria extends object>(props: Props<Criteria>) {
    return (
        <ControlledComboBox
            {...props}
            items={_items}
        />
    )
}