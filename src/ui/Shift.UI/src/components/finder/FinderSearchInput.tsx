import { ChangeEvent, ForwardedRef, KeyboardEvent, useImperativeHandle, useRef, useState } from "react";
import { translate } from "@/helpers/translate";

import "./FinderSearchInput.css";

export interface FinderSearchInputType {
    focus: () => void;
}

interface Props {
    ref?: ForwardedRef<FinderSearchInputType>,
    disabled: boolean;
    keyword: string | null;
    onFilter: (keyword: string) => void;
}    

function FinderSearchInput({ ref, disabled, keyword, onFilter }: Props) {
    const [showClearButton, setShowClearButton] = useState(!!keyword);
    
    const inputRef = useRef<HTMLInputElement | null>(null);

    useImperativeHandle(ref, () => {
        return {
            focus() {
                inputRef.current?.focus();
            }
        }
    }, []);

    function handleFilterChange(e: ChangeEvent<HTMLInputElement>) {
        setShowClearButton(!!e.target.value);
    }

    function handleFilterKeyDown(e: KeyboardEvent) {
        if (e.key === "Enter") {
            handleFilter();
        }
    }

    function handleClearFilter() {
        if (!inputRef.current) {
            return;
        }
        inputRef.current.value = "";
        setShowClearButton(false);
        handleFilter();
    }

    function handleFilter() {
        if (inputRef.current) {
            onFilter(inputRef.current.value);
        }
    }

    return (
        <div className="finder-search-input w-100">
            <input
                ref={inputRef}
                autoFocus={true}
                className="form-control"
                readOnly={disabled}
                defaultValue={keyword ?? ""}
                placeholder={translate("Keyword")}
                maxLength={200}
                onKeyDown={handleFilterKeyDown}
                onChange={handleFilterChange}
            />
            {showClearButton && (
                <button
                    className="clear-filter"
                    title="Clear Filter"
                    onClick={handleClearFilter}
                    tabIndex={-1}
                    disabled={disabled}
                >
                    <i className="fas fa-times"></i>
                </button>
            )}
            <button
                className="apply-filter"
                title="Apply Filter"
                tabIndex={-1}
                disabled={disabled}
                onClick={handleFilter}
            >
                <i className="fas fa-filter"></i>
            </button>
        </div>
    );
};

export default FinderSearchInput;