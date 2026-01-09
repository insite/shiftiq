import { ForwardedRef, KeyboardEvent, useEffect, useState } from "react";
import { Button } from "react-bootstrap";
import { ListItem } from "@/models/listItem";
import { useStatusProvider } from "@/contexts/StatusProvider";
import { useLoadingProvider } from "@/contexts/LoadingProvider";
import { FieldError } from "react-hook-form";
import { errorHelper } from "@/helpers/errorHelper";
import { cache } from "@/cache/cache";
import { FinderSearchWindowData } from "./FinderSearchWindowData";

import "./Finder.css";
import FinderSearchWindow from "./FinderSearchWindow";

export interface FinderProps {
    ref?: ForwardedRef<HTMLButtonElement>,
    disabled?: boolean;
    value?: string | null;
    defaultValue?: string | null;
    placeholder?: string | null;
    windowTitle: string;
    columnHeaderTitle: string;
    className?: string;
    error?: FieldError;
    onChange?: (value: string) => void;
    onBlur?: () => void;
    onLoadText: (value: string) => string | Promise<string>;
    onLoadItems: (pageIndex: number, keyword: string) => Promise<FinderSearchWindowData>;
}

export default function Finder({
    ref,
    value,
    defaultValue,
    disabled,
    placeholder,
    windowTitle,
    columnHeaderTitle,
    className,
    error,
    onChange,
    onBlur,
    onLoadText,
    onLoadItems
}: FinderProps) {
    const [show, setShow] = useState(false);
    const [isInitialized, setIsInitialized] = useState(false);
    const [isLoading, setIsLoading] = useState(false);
    const [storedValue, setStoredValue] = useState((value !== undefined ? value : defaultValue) ?? "");
    const [displayedItem, setDisplayedItem] = useState<ListItem | null>(() => {
        if (!storedValue) {
            return null;
        }
        const text = cache.textById.getText(storedValue);
        return text
            ? {
                value: storedValue,
                text
            } : null;
    });

    const currentValue = value !== undefined ? (value ?? "") : storedValue;
    const setCurrentValue = value !== undefined
        ? (onChange ?? (() => {}))
        : ((newValue: string) => {
            setStoredValue(newValue);
            onChange?.(newValue);
        });

    const { addError, removeError } = useStatusProvider();
    const { addLoading, removeLoading } = useLoadingProvider();

    const errorTooltip = errorHelper.getErrorTooltip(error) ?? "";

    useEffect(() => {
        if (currentValue === displayedItem?.value || isLoading) {
            return;
        }

        if (!currentValue) {
            setDisplayedItem({
                value: currentValue,
                text: ""
            });
            setIsInitialized(true);
            return;
        }

        const result = onLoadText(currentValue);
        if (typeof result === "string") {
            cache.textById.setText(currentValue, result);
            setDisplayedItem({
                value: currentValue,
                text: result
            });
            setIsInitialized(true);
            return;
        }

        setIsLoading(true);
        
        if (!isInitialized) {
            addLoading();
        }

        result
            .then(newText => {
                cache.textById.setText(currentValue, newText);
                setDisplayedItem({
                    value: currentValue,
                    text: newText
                });
                removeError();
            })
            .catch(err => addError(err, "Loading failed"))
            .finally(() => {
                setIsLoading(false);
                if (!isInitialized) {
                    setIsInitialized(true);
                    removeLoading();
                }
            });
    }, [addLoading, removeLoading, addError, removeError, onLoadText, currentValue, displayedItem, isInitialized, isLoading]);

    function handleChange(item: ListItem) {
        setDisplayedItem(item);
        setCurrentValue(item.value);
        setShow(false);
    }

    function handleClose() {
        setShow(false);
    }

    function handleDownArrow(e: KeyboardEvent) {
        if (e.key === "ArrowDown") {
            setShow(true);
        }
    }

    return (
        <>
            <div className={`insite-combobox bootstrap-select w-100 dropdown btn-group ${className ?? ""}`}>
                <Button
                    ref={ref}
                    variant="combobox"
                    disabled={disabled || isLoading || !displayedItem}
                    className={`w-100 dropdown-toggle ${errorTooltip ? "is-invalid" : ""}`}
                    title={errorTooltip}
                    onClick={() => setShow(true)}
                    onKeyDown={handleDownArrow}
                    onBlur={onBlur}
                >
                    <span className="combobox-text overflow-hidden w-100 text-start">
                        {displayedItem?.text ? (
                            <>{displayedItem.text}</>
                        ) : (
                            <span className="combobox-placeholder">{placeholder}</span>
                        )}
                    </span>
                </Button>
            </div>

            <FinderSearchWindow
                value={currentValue}
                show={show}
                windowTitle={windowTitle}
                columnHeaderTitle={columnHeaderTitle}
                onLoad={onLoadItems}
                onChange={handleChange}
                onClose={handleClose}
            />
        </>
    );
}
