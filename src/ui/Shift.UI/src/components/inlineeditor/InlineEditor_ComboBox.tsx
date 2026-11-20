import { ListItem } from "@/models/listItem";
import ComboBox from "../combobox/ComboBox";

interface Props {
    value: string;
    isSaving: boolean;
    items: ListItem[];
    onChange: (value: string) => void;
    onSave: () => Promise<void>;
    onCancel: () => void;
}

export default function InlineEditor_ComboBox({
    value,
    isSaving,
    items,
    onChange,
    onSave,
    onCancel,
}: Props) {
    function handleKeyDown(e: React.KeyboardEvent) {
        if (e.key === "Enter") {
            onSave();
        } else if (e.key === "Escape") {
            onCancel();
        }
    }

    return (
        <ComboBox
            autoFocus
            value={value}
            items={items}
            disabled={isSaving}
            onChange={newValue => onChange(newValue ?? "")}
            onKeyDown={handleKeyDown}
        />
    );
}