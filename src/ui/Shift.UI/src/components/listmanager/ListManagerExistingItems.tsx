import { ListItem } from "@/models/listItem";
import ComboBox from "../combobox/ComboBox";
import { useMemo, useState } from "react";
import Button from "../Button";

interface Props {
    items: ListItem[];
    selectedValue: string | null;
    confirmDeleteText?: string | undefined | null;
    saveTooltip?: string | undefined | null;
    deleteTooltip?: string | undefined | null;
    onSelect: (value: string | null) => void;
    onSave: (value: string) => void;
    onDelete: (value: string) => void;
}

export default function ListManagerExistingItems({
    items,
    selectedValue,
    confirmDeleteText,
    saveTooltip,
    deleteTooltip,
    onSelect,
    onSave,
    onDelete
}: Props) {
    const [isSaving, setIsSaving] = useState(false);

    const hasSelectedValue = selectedValue && items.find(x => !!x.value && x.value === selectedValue);

    let buttonClass = "btn btn-sm btn-default btn-icon";
    if (!hasSelectedValue) {
        buttonClass += " disabled";
    }

    const listWithEmptyItem = useMemo(() => [
        { value: "", text: "" },
        ...items
    ], [items]);

    function handleChange(value: string | null) {
        onSelect(value);
    }

    function handleSave() {
        if (!selectedValue) {
            return;
        }

        onSave(selectedValue);

        setIsSaving(true);

        setTimeout(() => setIsSaving(false), 200);
    }

    function handleDelete() {
        if (selectedValue && (!confirmDeleteText || confirm(confirmDeleteText))) {
            onDelete(selectedValue);
        }
    }

    return (
        <div className="d-flex gap-1 align-items-center mt-3">
            <ComboBox value={selectedValue} items={listWithEmptyItem} onChange={handleChange} />
            <Button
                type="button"
                variant="icon-save"
                disabled={!hasSelectedValue}
                tabIndex={hasSelectedValue ? undefined : -1}
                title={saveTooltip ?? undefined}
                isLoading={isSaving}
                onClick={handleSave}
            />
            <button
                type="button"
                className={buttonClass}
                tabIndex={hasSelectedValue ? undefined : -1}
                title={deleteTooltip ?? undefined}
                onClick={handleDelete}
            >
                <i className="fas fa-trash-alt"></i>
            </button>
        </div>
    );
}