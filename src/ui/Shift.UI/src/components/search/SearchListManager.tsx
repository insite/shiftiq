import { useState } from "react";
import { useSearch } from "./Search";
import ListManager from "../listmanager/ListManager";
import { SearchSettingList, searchStorage } from "@/cache/searchStorage";

interface Props<T extends object> {
    listTypeKey: "criteria" | "download",
    dataForSave: object | (() => object) | null;
    titlePlaceholder: string;
    addTooltip: string;
    confirmDeleteText: string;
    saveTooltip: string;
    deleteTooltip: string;
    onChange: (data: T | null) => void;
}

export default function SearchListManager<T extends object>({
    listTypeKey,
    dataForSave,
    titlePlaceholder,
    addTooltip,
    confirmDeleteText,
    saveTooltip,
    deleteTooltip,
    onChange
}: Props<T>) {
    const { cacheKey } = useSearch();
    if (!cacheKey) {
        throw new Error("The component cannot be used without the cacheKey");
    }

    const manager = listTypeKey === "criteria" ? searchStorage.criteriaManager : searchStorage.downloadManager;

    const [state, setState] = useState(() => toState(manager.load(cacheKey)));

    function handleSave(title: string) {
        const finalData = !dataForSave ? ({} as T) : (dataForSave instanceof Function ? dataForSave() : dataForSave);
        const list = manager.save(cacheKey!, title, finalData);
        setState(toState(list));
    }

    function handleSelect(title: string | null) {
        const list = manager.select(cacheKey!, title);
        setState(toState(list));
        onChange(list.items.find(x => x.title === list.selectedId)?.data as T ?? null);
    }

    function handleDelete(title: string) {
        const list = manager.delete(cacheKey!, title);
        setState(toState(list));
    }

    return (
        <div>
            <ListManager
                items={state.items}
                selectedValue={state.selectedValue}
                titlePlaceholder={titlePlaceholder}
                addTooltip={addTooltip}
                confirmDeleteText={confirmDeleteText}
                saveTooltip={saveTooltip}
                deleteTooltip={deleteTooltip}
                onAdd={handleSave}
                onSelect={handleSelect}
                onSave={handleSave}
                onDelete={handleDelete}
            />
        </div>
    );
}

function toState(list: SearchSettingList | null) {
    return {
        list,
        items: list ? list.items.map(({ title }) => ({
            value: title,
            text: title
        })) : [],
        selectedValue: list?.selectedId
    }
}