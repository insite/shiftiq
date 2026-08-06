import { MouseEvent, useRef, useState } from "react";
import { Button, Modal } from "react-bootstrap";
import FinderSearchGrid from "./FinderSearchGrid";
import LoadingPanel from "../LoadingPanel";
import FinderSearchInput from "./FinderSearchInput";
import { ListItem } from "@/models/listItem";
import { useStatusProvider } from "@/contexts/status/StatusProviderContext";
import LoadingOverlay from "@/components/LoadingOverlay";
import { FinderSearchWindowData } from "./FinderSearchWindowData";
import Icon from "../icon/Icon";

interface Criteria {
    keyword: string;
}

interface ContextData {
    isLoading: boolean;
    pageIndex: number;
    criteria: Criteria;
    items: ListItem[] | null;
    totalItemCount: number;
    itemsPerPage: number; 
}

interface Props {
    value: string | null;
    show: boolean;
    windowTitle: string;
    columnHeaderTitle: string;
    hideClearButton?: boolean;
    reloadOnShow?: boolean;
    onLoad: (pageIndex: number, keyword: string) => Promise<FinderSearchWindowData>;
    onChange: (item: ListItem) => void;
    onClose: () => void;
}

interface ButtonProps {
    disabled?: boolean;
    className?: string;
    onClick?: (e: MouseEvent<HTMLButtonElement>) => void;
}

const defaultContext: ContextData = {
    isLoading: false,
    pageIndex: -1,
    criteria: { keyword: "" },
    items: null,
    totalItemCount: 0,
    itemsPerPage: 0,
}

export default function FinderSearchWindow({
    value,
    show,
    windowTitle,
    columnHeaderTitle,
    hideClearButton = false,
    reloadOnShow = false,
    onLoad,
    onChange,
    onClose
}: Props) {
    const [contextData, setContextData] = useState<ContextData>(defaultContext);
    const [pageIndex, setPageIndex] = useState(0);
    const [criteria, setCriteria] = useState<Criteria>({ keyword: "" });

    const { addError, removeError } = useStatusProvider();

    const prevShowRef = useRef(false);

    if (show
        && !contextData.isLoading
        && (pageIndex !== contextData.pageIndex || criteria !== contextData.criteria)
    ) {
        prevShowRef.current = true;

        setContextData(prev => ({
            ...prev,
            isLoading: true
        }));

        const newPageIndex = criteria === contextData.criteria ? pageIndex : 0;

        load(newPageIndex , criteria);
    } else if (show
        && !contextData.isLoading
        && reloadOnShow
        && !prevShowRef.current
    ) {
        prevShowRef.current = true;

        setContextData(prev => ({
            ...prev,
            isLoading: true
        }));

        load(0, defaultContext.criteria);
    } else if (show !== prevShowRef.current) {
        prevShowRef.current = show;
    }

    async function load(newPageIndex: number, newCriteria: Criteria) {
        try {
            const data = await onLoad(newPageIndex, newCriteria.keyword);
            setContextData({
                isLoading: false,
                pageIndex: data.pageIndex,
                criteria: newCriteria,
                items: data.items,
                totalItemCount: data.totalItemCount,
                itemsPerPage: data.itemsPerPage,
            });
            setCriteria(newCriteria);
            setPageIndex(newPageIndex);
            removeError();
        } catch (err) {
            addError(err, "Loading error");
            onClose();
        }
    }

    return (
        <Modal
            show={show}
            onHide={onClose}
            className="insite-modal"
        >
            <Modal.Header closeButton>
                <Modal.Title as="h5">{windowTitle}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <FinderSearchInput
                    disabled={contextData.isLoading || !contextData.items}
                    keyword={contextData.criteria.keyword}
                    onFilter={(keyword) => setCriteria({ keyword })}
                />

                {contextData.items ? (
                    <LoadingOverlay isLoading={contextData.isLoading === true}>
                        <FinderSearchGrid
                            isLoading={contextData.isLoading}
                            value={value}
                            items={contextData.items}
                            columnHeaderTitle={columnHeaderTitle}
                            pageIndex={contextData.pageIndex}
                            totalItemCount={contextData.totalItemCount}
                            itemsPerPage={contextData.itemsPerPage}
                            onGoToPage={pageIndex => setPageIndex(pageIndex)}
                            onSelect={value => onChange(value)}
                        />
                    </LoadingOverlay>
                ) : (
                    <LoadingPanel />
                )}
            </Modal.Body>
            <Modal.Footer>
                {!hideClearButton && (
                    <FinderClearButton onClick={() => onChange({ value: "", text: "" })} className="me-2" disabled={contextData.isLoading || !contextData.items} />
                )}
                <FinderCancelButton onClick={() => onClose()} />
            </Modal.Footer>
        </Modal>
    );
}

function FinderCancelButton({ disabled, className, onClick }: ButtonProps) {
    return (
        <Button disabled={disabled} variant="default" size="sm" className={className} onClick={onClick}>
            <Icon style="solid" name="ban" className="me-1" />
            Cancel
        </Button>
    );
}

function FinderClearButton({ disabled, className, onClick }: ButtonProps) {
    return (
        <Button disabled={disabled} variant="default" size="sm" className={className} onClick={onClick}>
            <Icon style="solid" name="undo" className="me-1" />
            Clear
        </Button>
    );
}