import { translate } from "@/helpers/translate";
import "./LoadingOverlay.css";
import { ReactNode } from "react";
import { Spinner } from "react-bootstrap";

interface Props {
    isLoading: boolean;
    loadingText?: string | undefined | null;
    children: ReactNode;
}

export default function LoadingOverlay({ isLoading, loadingText, children }: Props) {
    return (
        <div className={isLoading ? "insite-loading-overlay" : ""}>
            {children}

            {isLoading && (
                <div className="d-flex justify-content-center align-items-center insite-loading-overlay-panel">
                    <Spinner animation="border" role="status" className="me-3" />
                    {loadingText ?? translate("Loading...")}
                </div>
            )}
        </div>
    );
}