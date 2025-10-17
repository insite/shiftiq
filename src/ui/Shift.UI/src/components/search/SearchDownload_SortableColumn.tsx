import { translate } from "@/helpers/translate";
import { useSortable } from "@dnd-kit/sortable";
import { CSS } from '@dnd-kit/utilities';

export interface SortableColumn {
    id: string;
    title: string;
    isVisible: boolean;
}

interface Props {
    column: SortableColumn;
    onSelect: (isSelected: boolean) => void;
}

export default function SearchDownload_SortableColumn({ column: { id, title, isVisible }, onSelect }: Props) {
    const { attributes, listeners, setNodeRef, transform, transition, isDragging, setActivatorNodeRef } = useSortable({ id });

    const style = {
        transform: CSS.Transform.toString(transform),
        transition,
    };

    return (
        <div
            ref={setNodeRef}
            style={style}
            data-dragging={isDragging ? true : undefined}
            {...attributes}
        >
            <span>
                <input
                    type="checkbox"
                    title={translate("Toggle Column Visibility")}
                    checked={isVisible}
                    onChange={e => onSelect(e.target.checked)}
                />
            </span>
            <span
                ref={setActivatorNodeRef} {...listeners}
                data-not-selected={!isVisible ? true : undefined}
            >
                {title}
            </span>
        </div>
    );
}