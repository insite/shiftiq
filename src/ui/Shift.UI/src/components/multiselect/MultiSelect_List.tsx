import { ListItem } from "@/models/listItem";
import PopupPanel from "../PopupPanel";
import "./MultiSelect_List.css";
import MultiSelect_Buttons from "./MultiSelect_Buttons";

interface Props {
    relative: HTMLElement,
    trigger: HTMLElement | null;
    maxHeight?: number;
    items: ListItem[];
    selectedValues: string[];
    showButtons?: boolean;
    onClose: (escPressed: boolean) => void;
    onSelect: (value: string, checked: boolean) => void;
    onSelectAll: () => void;
    onDeselectAll: () => void;
}

export default function MultiSelect_List({
    relative,
    trigger,
    maxHeight,
    items,
    selectedValues,
    showButtons,
    onClose,
    onSelect,
    onSelectAll,
    onDeselectAll
}: Props) {
    return (
        <PopupPanel
            relative={relative}
            trigger={trigger}
            maxHeight={maxHeight}
            className="dropdown-menu show multiselect-list"
            onClose={onClose}
        >
            {showButtons !== false && (
                <MultiSelect_Buttons onSelectAll={onSelectAll} onDeselectAll={onDeselectAll} />
            )}
            <div className="item-list">
                {items.map(({ value, text }) => (
                    <button
                        key={value}
                        type="button"
                        tabIndex={-1}
                        data-selected={selectedValues.includes(value) ? true : undefined}
                        onClick={() => onSelect(value, !selectedValues.includes(value))}
                    >
                        {text}
                    </button>
                ))}
            </div>
       </PopupPanel>
    );
}