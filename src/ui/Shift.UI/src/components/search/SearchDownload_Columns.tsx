import { DndContext, closestCenter, KeyboardSensor, PointerSensor, useSensor, useSensors } from '@dnd-kit/core';
import { SortableContext, sortableKeyboardCoordinates } from '@dnd-kit/sortable';
import { restrictToParentElement, restrictToVerticalAxis } from "@dnd-kit/modifiers";
import { translate } from "@/helpers/translate";
import SearchDownload_SortableColumn, { SortableColumn } from "./SearchDownload_SortableColumn";
import { ReorderColumnsHandler, SelectAllColumnsHandler, SelectColumnHandler } from "./useSearchDownload";

import "./SearchDownload_Columns.css";

interface Props {
    visibleColumns: SortableColumn[];
    handleSelect: SelectColumnHandler;
    handleSelectAll: SelectAllColumnsHandler;
    handleReorder: ReorderColumnsHandler;
}

export default function SearchDownload_Columns({
    visibleColumns,
    handleSelect,
    handleSelectAll,
    handleReorder,
}: Props) {
    const sensors = useSensors(
        useSensor(PointerSensor),
        useSensor(KeyboardSensor, {
            coordinateGetter: sortableKeyboardCoordinates,
        })
    );

    return (
        <>
            <div className="search-download-columns">
                <div className="header">
                    <span>
                        <input
                            type="checkbox"
                            title={translate("Toggle Column Visibility")}
                            defaultChecked={!visibleColumns.find(x => !x.isVisible)}
                            onChange={e => handleSelectAll(e.target.checked)}
                        />
                    </span>
                </div>
                <div className="sortable">
                    <DndContext
                        sensors={sensors}
                        collisionDetection={closestCenter}
                        modifiers={[restrictToVerticalAxis, restrictToParentElement]}
                        onDragEnd={({ active, over }) => {
                            if (over && active.id !== over.id) {
                                handleReorder(String(active.id), String(over.id));
                            }
                        }}
                    >
                        <SortableContext items={visibleColumns}>
                            {visibleColumns.map(column => (
                                <SearchDownload_SortableColumn
                                    key={column.id}
                                    column={column}
                                    onSelect={isSelected => handleSelect(column.id, isSelected)}
                                />
                            ))}
                        </SortableContext>
                    </DndContext>
                </div>
            </div>
        </>
    );
}