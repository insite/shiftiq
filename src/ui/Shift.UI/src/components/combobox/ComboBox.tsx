import { ButtonGroup, Dropdown } from "react-bootstrap";
import { detectOverflow } from "@popperjs/core/lib/popper";
import { ForwardedRef, ReactNode, useEffect, useRef, useState } from "react";
import { ListItem } from "@/models/listItem";
import { translate } from "@/helpers/translate";
import { useComboBox } from "./useComboBox";
import { errorHelper } from "@/helpers/errorHelper";
import { FieldError } from "react-hook-form";

export interface ComboBoxProps {
    ref?: ForwardedRef<HTMLInputElement>,
    buttonRef?: ForwardedRef<HTMLButtonElement>,
    name?: string;
    value?: string | null;
    defaultValue?: string | null;
    isLoading?: boolean;
    className?: string;
    widthClassName?: string;
    items: ListItem[] | (() => ListItem[] | Promise<ListItem[]>);
    maxHeight?: number;
    placeholder?: string;
    disabled?: boolean;
    autoFocus?: boolean;
    error?: FieldError;
    onChange?: (value: string | null) => void;
    onBlur?: () => void;
    onKeyDown?: (e:React.KeyboardEvent) => void;
}

export default function ComboBox({
    ref,
    buttonRef,
    name,
    value,
    defaultValue,
    className,
    widthClassName,
    items,
    maxHeight,
    placeholder,
    disabled,
    autoFocus = false,
    error,
    onChange,
    onBlur,
    onKeyDown,
}: ComboBoxProps) {
    const menuRef = useRef<HTMLDivElement>(null);
    const focusActiveItem = useRef(false);

    const [opened, setOpened] = useState(false);

    const {
        loadedItems,
        currentValue,
        currentText,
        setCurrentValue,
    } = useComboBox(items, value, defaultValue);

    const errorTooltip = errorHelper.getErrorTooltip(error) ?? "";

    useEffect(() => {
        if (!menuRef.current || !opened || !focusActiveItem.current) {
            return;
        }

        focusActiveItem.current = false;

        const item = menuRef.current.querySelector("a.active.dropdown-item") as HTMLElement
            ?? menuRef.current.querySelector("a.dropdown-item") as HTMLElement;

        if (!item) {
            return;
        }

        const menuRect = menuRef.current.getBoundingClientRect();
        const itemRect = item.getBoundingClientRect();

        if (itemRect.top < menuRect.top) {
            menuRef.current.scrollTop -= menuRect.top - itemRect.top;
        } else if (itemRect.bottom > menuRect.bottom) {
            menuRef.current.scrollTop += itemRect.bottom - menuRect.bottom;
        }

        try {
            item.focus({ preventScroll: true });
        } catch {
            item.focus();
        }
    }, [opened]);

    function handleChange(eventKey: string | null): void {
        setCurrentValue(eventKey);        
        onChange?.(eventKey ?? null);
    }

    function handleToggle(nextShow: boolean) {
        focusActiveItem.current = nextShow;
        setOpened(nextShow);
    }

    function handleKeyDownCapture(e:React.KeyboardEvent) {
        if (e.key === "ArrowUp") {
            focusActiveItem.current = true;
            setOpened(true);
        } else if (loadedItems && /^[\p{L}\p{N}]$/u.test(e.key)) { // Check if the pressed key is alpha-numeric
            const c = e.key.toLowerCase();
            const currentIndex = currentValue ? loadedItems.findIndex(x => x.value === currentValue) : -1;
            let item = loadedItems.find((x, index) => currentIndex === -1 || currentIndex < index && x.text.toLowerCase().startsWith(c));

            if (!item && currentIndex !== -1) {
                item = loadedItems.find(x => x.text.toLowerCase().startsWith(c));
            }

            if (item && item.value !== currentValue) {
                handleChange(item.value);
            } else {
                return;
            }
        } else {
            return;
        }

        e.preventDefault();
        e.stopPropagation();
    }
       
    return (
        <>
            <input
                ref={ref}
                name={name}
                value={currentValue ?? ""}
                className="d-none"
                onBlur={onBlur}
                readOnly
            />

            <Dropdown
                as={ButtonGroup}
                className={`
                    insite-combobox bootstrap-select ${widthClassName ?? "w-100"} ${className ?? ""}
                    ${disabled ? "insite-combobox-disabled" : ""}
                `}
                show={opened}
                onSelect={handleChange}
                onToggle={handleToggle}
            >
                <Dropdown.Toggle
                    ref={buttonRef}
                    autoFocus={autoFocus}
                    variant="combobox"
                    className={`w-100 ${errorTooltip ? "is-invalid" : ""}`}
                    disabled={disabled || !loadedItems}
                    onKeyDownCapture={handleKeyDownCapture}
                    onKeyDown={onKeyDown}
                >
                    <span
                        className="combobox-text overflow-hidden w-100 text-start"
                        title={errorTooltip}
                    >
                        {!loadedItems ? (
                            <span className="combobox-placeholder">{translate("Loading...")}</span>
                        ) : currentText ? (
                            <>{currentText}</>
                        ) : (
                            <span className="combobox-placeholder">{placeholder}</span>
                        )}
                    </span>
                </Dropdown.Toggle>
                <CustomDropdownMenu ref={menuRef} maxHeight={maxHeight}>
                    {loadedItems &&(
                        <>
                            {loadedItems.map(item => (
                                <Dropdown.Item key={item.value} eventKey={item.value} active={!!item.value && item.value === currentValue}>
                                    {item.text || <>&nbsp;</>}
                                </Dropdown.Item>
                            ))}
                        </>
                    )}
                </CustomDropdownMenu>
            </Dropdown>
        </>
    );
}

function CustomDropdownMenu ({ ref, maxHeight, children }: {
    ref?: ForwardedRef<HTMLDivElement>;
    maxHeight?: number;
    children?: ReactNode;
}) {
    return (
        <Dropdown.Menu
            ref={ref}
            className="overflow-y-auto overflow-x-hidden"
            popperConfig={{
                modifiers: [
                    {
                        name: "sameWidth",
                        enabled: true,
                        phase: "beforeWrite",
                        requires: ["computeStyles"],
                        fn: ({ state }) => {
                            state.styles.popper.width = `${state.rects.reference.width}px`;
                        },
                        effect: ({ state }) => {
                            state.elements.popper.style.width = `${(state.elements.reference as HTMLElement).offsetWidth}px`;
                        }
                    },
                    {
                        name: "maxSize",
                        enabled: true,
                        phase: "main",
                        requires: ["offset", "preventOverflow", "flip"],
                        fn({ state, name, options }) {
                            const overflow = detectOverflow(state, options);
                            const { x, y } = state.modifiersData.preventOverflow || { x: 0, y: 0 };
                            const { width, height } = state.rects.popper;
                            const basePlacement = state.placement.split("-")[0];
                            const widthProp = basePlacement === "left" ? "left" : "right";
                            const heightProp = basePlacement === "top" ? "top" : "bottom";
                            const heightOffset = basePlacement === "top" ? 48 : 0; {/* Fixed header offset */}

                            state.modifiersData[name] = {
                                width: width - overflow[widthProp] - x,
                                height: height - overflow[heightProp] - y - heightOffset
                            };
                        }
                    },
                    {
                        name: "applyMaxSize",
                        enabled: true,
                        phase: "beforeWrite",
                        requires: ["maxSize"],
                        fn: ({ state }) => {
                            const { height } = state.modifiersData.maxSize;
                            const popperMaxHeight = maxHeight != undefined && maxHeight < height ? maxHeight : height - 2;
                            state.styles.popper.maxHeight = `${popperMaxHeight}px`;
                        }
                    },
                ]
            }}
        >
            {children}
        </Dropdown.Menu>
    );
};
