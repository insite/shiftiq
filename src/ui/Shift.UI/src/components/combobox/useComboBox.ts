import { useStatusProvider } from "@/contexts/StatusProvider";
import { useLoadingProvider } from "@/contexts/LoadingProvider";
import { ListItem } from "@/models/listItem";
import { useEffect, useState } from "react";

export function useComboBox(
    items: ListItem[] | (() => ListItem[] | Promise<ListItem[]>),
    value: string | undefined | null,
    defaultValue: string | undefined | null,
) {
    const [storedValue, setStoredValue] = useState<string | null>(defaultValue ?? null);
    const [storedOnLoad] = useState(() => typeof items === "function" ? items : null);
    const [loadedItems, setLoadedItems] = useState<ListItem[] | null>(() => {
        return Array.isArray(items) ? items: null;
    });

    const { addError } = useStatusProvider();
    const { addLoading, removeLoading } = useLoadingProvider();

    const currentValue = value !== undefined ? value : storedValue;

    useEffect(() => {
        if (Array.isArray(items)) {
            setLoadedItems(items);
        }
    }, [items])

    useEffect(() => {
        if (!storedOnLoad) {
            return;
        }

        const itemsResult = storedOnLoad();

        if (Array.isArray(itemsResult)) {
            setLoadedItems(itemsResult);
        } else {
            addLoading();

            itemsResult.then(result => {
                setLoadedItems(result);
            }).catch(e => {
                addError(e, "Combobox failed to load");
            }).finally(() => {
                removeLoading();
            });
        }
    }, [addLoading, removeLoading, addError, storedOnLoad]);

    return {
        loadedItems,
        currentValue,
        currentText: loadedItems ? loadedItems.find(x => x.value === currentValue)?.text : null,
        setCurrentValue: setStoredValue,
    }
}