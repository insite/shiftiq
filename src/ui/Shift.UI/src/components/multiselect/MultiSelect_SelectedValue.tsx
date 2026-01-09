import { translate } from "@/helpers/translate";
import { ListItem } from "@/models/listItem";
import { ReactNode } from "react";

interface Props {
    placeholder?: string;
    items?: ListItem[] | undefined | null;
    selectedValues: string[];
    itemsSelectedText?: string | null;
    allItemsSelectedText?: string | null;
}

export default function MultiSelect_SelectedValue({
    placeholder,
    items,
    selectedValues,
    itemsSelectedText,
    allItemsSelectedText
}: Props) {
    let content: ReactNode;

    if (!items?.length || !selectedValues.length) {
        content = <span className="combobox-placeholder">{placeholder}</span>;
    }
    else if (selectedValues.length === 1) {
        const selectedItem = items.find(x => x.value === selectedValues[0]);
        if (!selectedItem) {
            throw new Error(`Selected item is not found: ${selectedValues[0]}`);
        }
        content = selectedItem.text;
    } else if (items.length === selectedValues.length) {
        content = allItemsSelectedText ?? translate("All Items Selected");
    } else {
        content = `${selectedValues.length} ${itemsSelectedText ?? translate("Items Selected")}`;
    }

    return (
        <span className="combobox-text overflow-hidden w-100 text-start">
            {content}
        </span>  
    );
}