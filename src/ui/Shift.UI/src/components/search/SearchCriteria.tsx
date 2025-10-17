import { FormEvent, ReactNode, useRef } from "react";
import { Control, UseFormReset } from "react-hook-form";
import { translate } from "@/helpers/translate";
import { useSearch } from "./Search";
import SearchCriteria_Buttons from "./SearchCriteria_Buttons";
import { ListItem } from "@/models/listItem";
import { BaseCriteria, cleanCriteria, hydrateCriteria } from "./BaseCriteria";

import "./SearchCriteria.css";
import SearchCriteria_List from "./SearchCriteria_List";
import ControlledMultiSelect from "../multiselect/ControlledMultiSelect";
import ControlledComboBox from "../combobox/ControlledComboBox";

type Size = "size-1" | "size-2" | "size-3";

interface Props<Criteria extends BaseCriteria> {
    className?: string;
    contentSize: Size;
    sortByColumns?: ListItem[];
    children? : ReactNode;
    control: Control<Criteria>;
    onGetCriteria: () => Criteria;
    onSubmit: (e: FormEvent<HTMLFormElement>) => void;
    onReset: UseFormReset<Criteria>,
}

export default function SearchCriteria<Criteria extends BaseCriteria>({
    className,
    contentSize,
    sortByColumns,
    children,
    control,
    onGetCriteria,
    onSubmit,
    onReset,
}: Props<Criteria>) {
    const { isLoading, columns, defaultCriteria } = useSearch<Criteria, object>();

    const formRef = useRef<HTMLFormElement>(null);

    const columnItems: ListItem[] | null = columns
        ? columns.map(({ key, title }) => (
                {
                    value: key,
                    text: typeof title === "string" ? title : key
                }
            ))
            .filter(x => x.text) as ListItem[]
        : null;

    function handleChangeCriteria(newCriteria: Criteria | null) {
        const { visibleColumns, sortByColumn } = onGetCriteria();
        const onlyFields = {
            ...hydrateCriteria(newCriteria, defaultCriteria),
            visibleColumns,
            sortByColumn
        };
        onReset(onlyFields);
        formRef.current?.requestSubmit();
    }

    return (
        <form
            ref={formRef}
            className={`search-criteria-section ${contentSize} ${className ?? ""}`}
            noValidate
            autoComplete="off"
            onSubmit={onSubmit}
        >
            <div className="field-section">
                <h4>{translate("Criteria")}</h4>

                {children}

                <SearchCriteria_Buttons
                    isLoading={isLoading}
                    onClear={() => handleChangeCriteria(null)}
                />
            </div>
            <div>
                <h4>{translate("Settings")}</h4>
                <ControlledMultiSelect<BaseCriteria>
                    name="visibleColumns"
                    control={control as unknown as Control<BaseCriteria>}
                    className="mb-2"
                    disabled={isLoading || !columnItems}
                    items={columnItems}
                    itemsSelectedText={translate("Columns Visible")}
                    allItemsSelectedText={translate("All Columns Visible")}
                    placeholder={!columnItems ? translate("Loading...") : translate("All Columns Visible")}
                    maxHeight={400}
                />
                {sortByColumns?.length && (
                    <ControlledComboBox
                        name="sortByColumn"
                        control={control as unknown as Control<BaseCriteria>}
                        className="mb-2"
                        disabled={isLoading}
                        items={sortByColumns}
                    />
                )}
            </div>
            <div>
                <h4>{translate("Saved Filters")}</h4>
                <SearchCriteria_List
                    onGetDataForSave={() => cleanCriteria(onGetCriteria(), defaultCriteria)}
                    onChange={handleChangeCriteria}
                />
            </div>
        </form>
    );
}