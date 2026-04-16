import { ListItem } from "@/models/listItem";
import ComboBox from "../combobox/ComboBox";

interface Props {
    value: string;
    isSaving: boolean;
    items: ListItem[];
    onChange: (value: string) => void;
}

export default function InlineEditor_ComboBox({
    value,
    isSaving,
    items,
    onChange,
}: Props) {
    return (
        <ComboBox
            value={value}
            items={items}
            disabled={isSaving}
            onChange={newValue => onChange(newValue ?? "")}
        />
    );
}