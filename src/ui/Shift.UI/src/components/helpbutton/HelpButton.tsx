import "./HelpButton.css";

import { ReactNode, useRef, useState } from "react";
import IconButton from "../iconbutton/IconButton";
import PopupPanel from "../PopupPanel";

interface Props {
    className?: string;
    title?: string;
    children: ReactNode;
}

export default function HelpButton({
    className,
    title = "Click to see help",
    children
}: Props) {
    const [isHelpOpen, setIsHelpOpen] = useState(false);

    const helpButtonRef = useRef<HTMLButtonElement>(null);

    function handleHelpButtonClick(event: React.MouseEvent<HTMLButtonElement>) {
        helpButtonRef.current = event.currentTarget;
        setIsHelpOpen(value => !value);
    }

    function handleHelpClose(escPressed: boolean) {
        if (escPressed) {
            helpButtonRef.current?.focus();
        }
        setIsHelpOpen(false);
    }

    return (
        <div className={`HelpButton ${className ?? ""}`}>
            <IconButton
                iconName="circle-info"
                iconStyle="solid"
                iconClassName="text-secondary"
                title={title}
                onClick={handleHelpButtonClick}
            />
            {isHelpOpen && helpButtonRef.current && (
                <PopupPanel
                    relative={helpButtonRef.current}
                    trigger={helpButtonRef.current}
                    className="search-help-popup"
                    onClose={handleHelpClose}
                >
                    {children}
                </PopupPanel>
            )}
        </div>
    );
}