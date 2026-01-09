import { ChangeEvent, KeyboardEvent, useRef, useState } from "react";
import { Form } from "react-bootstrap";

interface Props {
    titlePlaceholder?: string | undefined | null;
    addTooltip?: string | undefined | null;
    onAddItem: (title: string) => void;
}

export default function ListManagerNewItem({ titlePlaceholder, addTooltip, onAddItem }: Props) {
    const [hasTitle, setHasTitle] = useState(false);
    const titleRef = useRef<HTMLInputElement>(null);

    let buttonClass = "btn btn-default btn-icon";
    if (!hasTitle) {
        buttonClass += " disabled";
    }

    function handleTitleChange(e: ChangeEvent<HTMLInputElement>) {
        setHasTitle(!!e.target.value);
    }

    function handleKeyPress(e: KeyboardEvent<HTMLInputElement>) {
        if (e.code === "Enter") {
            handleAddClick();
            e.preventDefault();
        }
    }

    function handleAddClick() {
        if (titleRef.current?.value) {
            onAddItem(titleRef.current.value);
            titleRef.current.value = "";
            setHasTitle(false);
        }
    }

    return (
        <div className="d-flex gap-1">
            <Form.Control
                ref={titleRef}
                className="insite-text"
                maxLength={20}
                placeholder={titlePlaceholder ?? undefined}
                onChange={handleTitleChange}
                onKeyDown={handleKeyPress}
            />

            <button
                type="button"
                className={buttonClass}
                title={addTooltip ?? undefined}
                tabIndex={hasTitle ? undefined : -1}
                onClick={() => handleAddClick()}
            >
                <i className="fas fa-plus-circle"></i>
            </button>
        </div>
    );
}