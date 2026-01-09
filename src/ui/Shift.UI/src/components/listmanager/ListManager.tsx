import { ListItem } from "@/models/listItem";
import ListManagerNewItem from "./ListManagerNewItem";
import ListManagerExistingItems from "./ListManagerExistingItems";

interface Props {
    items: ListItem[];
    selectedValue?: string | undefined | null;
    titlePlaceholder?: string | undefined | null;
    addTooltip?: string | undefined | null;
    confirmDeleteText?: string | undefined | null;
    saveTooltip?: string | undefined | null;
    deleteTooltip?: string | undefined | null;
    onSelect: (value: string | null) => void;
    onAdd: (title: string) => void;
    onSave: (value: string) => void;
    onDelete: (value: string) => void;
}

export default function ListManager({
    items,
    selectedValue,
    titlePlaceholder,
    addTooltip,
    confirmDeleteText,
    saveTooltip,
    deleteTooltip,
    onSelect,
    onAdd,
    onSave,
    onDelete
}: Props) {
    return (
        <>
            <ListManagerNewItem
                titlePlaceholder={titlePlaceholder}
                addTooltip={addTooltip}
                onAddItem={onAdd}
            />
            {items.length > 0 && (
                <ListManagerExistingItems
                    items={items}
                    selectedValue={selectedValue ?? null}
                    confirmDeleteText={confirmDeleteText}
                    saveTooltip={saveTooltip}
                    deleteTooltip={deleteTooltip}
                    onSelect={onSelect}
                    onSave={onSave}
                    onDelete={onDelete}
                />
            )}
        </>
    );
}