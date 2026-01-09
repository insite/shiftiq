import { ReactNode, useEffect, useRef, useState } from "react";
import "./PopupPanel.css";

interface Props {
    relative: HTMLElement,
    trigger?: HTMLElement | null;
    className?: string;
    maxHeight?: number;
    children?: ReactNode;
    onClose: (escPressed: boolean) => void;
}

export default function PopupPanel({ relative, trigger, className, maxHeight, children, onClose }: Props) {
    const [isInitialized, setIsInitialized] = useState(false);

    const panelRef = useRef<HTMLDivElement>(null);

    useEffect(() => {
        const { height, top, width } = relative.getBoundingClientRect();
        const style = window.getComputedStyle(relative);

        let panelTop: number | null = null;
        let panelBottom: number | null = null;

        if (top > window.innerHeight - top) {
            panelBottom = height + parseFloat(style.marginBottom);
        } else {
            panelTop = height + parseFloat(style.marginTop);
        }

        document.documentElement.style.setProperty("--popup-panel-relative-width",`${width}px`);
        document.documentElement.style.setProperty("--popup-panel-max-height", maxHeight ? `${maxHeight}px` : null);
        document.documentElement.style.setProperty("--popup-panel-top", panelTop !== null ? `${panelTop}px` : null);
        document.documentElement.style.setProperty("--popup-panel-bottom", panelBottom !== null ? `${panelBottom}px` : null);

        setIsInitialized(true);

        setTimeout(() => {
            if (panelRef.current) {
                panelRef.current.dataset.visible = "true";
            }
        });
    }, [relative, maxHeight]);

    useEffect(() => {
        document.body.addEventListener("focusin", handleFocusIn);
        document.body.addEventListener("click", handleClick);
        document.body.addEventListener("keydown", handleKeyDown)

        return () => {
            document.body.removeEventListener("click", handleClick);
            document.body.removeEventListener("focusin", handleFocusIn);
            document.body.removeEventListener("keydown", handleKeyDown);
        };

        function handleClick(e: MouseEvent) {
            if (e.target !== null
                && e.target !== trigger
                && (!trigger || !trigger.contains(e.target as Node))
                && (panelRef.current && !panelRef.current.contains(e.target as Node))
            ) {
                onClose(false);
            }
        }

        function handleFocusIn(e: FocusEvent) {
            if (e.target !== null
                && e.target !== trigger
                && (panelRef.current && !panelRef.current.contains(e.target as Node))
            ) {
                onClose(false);
            }
        }

        function handleKeyDown(e: KeyboardEvent) {
            if (e.key == "Escape") {
                onClose(true);
            }
        }
    }, [onClose, trigger]);

    if (!isInitialized) {
        return null;
    }

    return (
        <div
            ref={panelRef}
            className={`popup-panel shadow-lg ${className ?? ""}`}
        >
            <div className="focus-button">
                <button autoFocus={true} />
            </div>
            {children}
        </div>
    );
}