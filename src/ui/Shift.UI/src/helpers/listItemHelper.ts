import { ListItem } from "@/models/listItem";

export function getListItemText(value?: string | null, listItems?: ListItem[] | null): string | null {
    if (!value || !listItems) {
        return null;
    }
    return listItems.find(x => x.value.toLocaleLowerCase() === value.toLocaleLowerCase())?.text ?? null;
}