import { translate } from "@/helpers/translate";

interface Props {
    onSelectAll: () => void;
    onDeselectAll: () => void;
}

export default function MultiSelect_Buttons({ onSelectAll, onDeselectAll }: Props) {
    return (
        <div className="btn-group btn-group-sm w-100 select-buttons">
            <button
                type="button"
                className="actions-btn bs-select-all btn btn-outline-secondary"
                onClick={onSelectAll}
            >
                {translate("Select All")}
            </button>
            <button
                type="button"
                className="actions-btn bs-deselect-all btn btn-outline-secondary"
                onClick={onDeselectAll}
            >
                {translate("Deselect All")}
            </button>
        </div>
    );
}