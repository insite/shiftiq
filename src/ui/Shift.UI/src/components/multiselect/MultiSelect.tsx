import { ForwardedRef, KeyboardEvent, useImperativeHandle, useRef, useState } from "react";
import { FieldError } from "react-hook-form";
import { ListItem } from "@/models/listItem";
import { errorHelper } from "@/helpers/errorHelper";
import MultiSelect_SelectedValue from "./MultiSelect_SelectedValue";
import MultiSelect_List from "./MultiSelect_List";

import "./MultiSelect.css";

export interface MultiSelectProps {
    ref?: ForwardedRef<HTMLButtonElement>,
    className?: string;
    disabled?: boolean;
    items?: ListItem[] | null;
    values?: string[] | null;
    defaultValues?: string[] | null;
    itemsSelectedText?: string;
    allItemsSelectedText?: string;
    placeholder?: string;
    maxHeight?: number;
    showButtons?: boolean;
    error?: FieldError;
    onChange?: (newValues: string[]) => void;
    onBlur?: () => void,
}

export default function MultiSelect({
    ref,
    className,
    disabled,
    items,
    values,
    defaultValues,
    itemsSelectedText,
    allItemsSelectedText,
    placeholder,
    maxHeight,
    showButtons,
    error,
    onChange,
    onBlur,
}: MultiSelectProps) {
    const [storedValues, setStoredValues] = useState<string[]>(defaultValues ?? []);
    const [isListOpen, setIsListOpen] = useState(false);

    const internalButtonRef = useRef<HTMLButtonElement>(null);
    useImperativeHandle<HTMLButtonElement | null, HTMLButtonElement | null>(ref, () => internalButtonRef.current);

    const currentValues = values !== undefined ? (values ?? []) : storedValues;
    const setCurrentValues = values !== undefined
        ? (onChange ?? (() => {}))
        : ((newValues: string[]) => {
            setStoredValues(newValues);
            onChange?.(newValues);
        })

    const errorTooltip = errorHelper.getErrorTooltip(error);

    function handleOpenList() {
        setIsListOpen(v => !v);
    }

    function handleKeyDown(e: KeyboardEvent) {
        if (e.key === "ArrowDown") {
            setIsListOpen(true);
        }
    }

    function handleCloseList(escPressed: boolean) {
        if (escPressed) {
            internalButtonRef.current?.focus();
        }
        setIsListOpen(false);
    }

    function handleSelect(value: string, checked: boolean) {
        if (checked) {
            if (!currentValues.includes(value)) {
                setCurrentValues([...currentValues, value]);
            }
        } else {
            setCurrentValues(currentValues.filter(x => x !== value));
        }
    }

    function handleSelectAll() {
        if (items) {
            setCurrentValues(items.map(x => x.value));
        }
    }

    function handleDeselectAll() {
        setCurrentValues([]);
    }

    return (
        <div className="insite-combobox bootstrap-select w-100 dropdown btn-group multiselect">
            <button
                ref={internalButtonRef}
                type="button"
                disabled={disabled}
                className={`dropdown-toggle btn btn-combobox w-100 ${className ?? ""} ${errorTooltip ? "is-invalid" : ""}`}
                title={errorTooltip ?? ""}
                onClick={handleOpenList}
                onKeyDown={handleKeyDown}
                onBlur={onBlur}
            >
                <MultiSelect_SelectedValue
                    placeholder={placeholder}
                    items={items}
                    selectedValues={currentValues}
                    itemsSelectedText={itemsSelectedText}
                    allItemsSelectedText={allItemsSelectedText}
                />
            </button>
            {!disabled && isListOpen && items && internalButtonRef.current && (
                <MultiSelect_List
                    relative={internalButtonRef.current}
                    trigger={internalButtonRef.current}
                    maxHeight={maxHeight}
                    items={items}
                    selectedValues={currentValues}
                    showButtons={showButtons}
                    onClose={handleCloseList}
                    onSelect={handleSelect}
                    onSelectAll={handleSelectAll}
                    onDeselectAll={handleDeselectAll}
                />
            )}
        </div>
    )
}