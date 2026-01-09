import { MouseEvent, useEffect, useRef, useState } from "react";
import { Button, Modal } from "react-bootstrap";
import FinderSearchGrid from "./FinderSearchGrid";
import LoadingPanel from "../LoadingPanel";
import FinderSearchInput, { FinderSearchInputType } from "./FinderSearchInput";
import { ListItem } from "@/models/listItem";
import { useStatusProvider } from "@/contexts/StatusProvider";
import LoadingOverlay from "@/components/LoadingOverlay";
import { FinderSearchWindowData } from "./FinderSearchWindowData";

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
    onLoad: (pageIndex: number, keyword: string) => Promise<FinderSearchWindowData>;
    onChange: (item: ListItem) => void;
    onClose: () => void;
}    

interface ButtonProps {
    disabled?: boolean;
    className?: string;
    onClick?: (e: MouseEvent<HTMLButtonElement>) => void;
}

export default function FinderSearchWindow({
    value,
    show,
    windowTitle,
    columnHeaderTitle,
    onLoad,
    onChange,
    onClose
}: Props) {
    const [contextData, setContextData] = useState<ContextData>({
        isLoading: false,
        pageIndex: -1,
        criteria: { keyword: "" },
        items: null,
        totalItemCount: 0,
        itemsPerPage: 0,
    });

    const [pageIndex, setPageIndex] = useState(0);
    const [criteria, setCriteria] = useState<Criteria>({ keyword: "" });

    const filterRef = useRef<FinderSearchInputType | null>(null);

    const { addError, removeError } = useStatusProvider();

    useEffect(() => {
        if (!show
            || contextData.isLoading
            || pageIndex === contextData.pageIndex && criteria === contextData.criteria
        ) {
            return;
        }

        const newPageIndex = criteria === contextData.criteria ? pageIndex : 0;

        setContextData(prev => ({
            ...prev,
            isLoading: true
        }));

        run();

        async function run() {
            try {
                const data = await onLoad(newPageIndex, criteria.keyword);
                setContextData({
                    isLoading: false,
                    pageIndex: data.pageIndex,
                    criteria,
                    items: data.items,
                    totalItemCount: data.totalItemCount,
                    itemsPerPage: data.itemsPerPage,
                });
                setPageIndex(newPageIndex);
                removeError();
            } catch (err) {
                addError(err, "Loading error");
                onClose();
            }
        }
    }, [addError, removeError, onLoad, onClose, show, contextData, pageIndex, criteria]);

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
                    ref={filterRef}
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
                <FinderClearButton onClick={() => onChange({ value: "", text: "" })} className="me-2" disabled={contextData.isLoading || !contextData.items} />
                <FinderCancelButton onClick={() => onClose()} />
            </Modal.Footer>
        </Modal>
    );
}

function FinderCancelButton({ disabled, className, onClick }: ButtonProps) {
    return (
        <Button disabled={disabled} variant="default" size="sm" className={className} onClick={onClick}>
            <i className="fas fa-ban me-1"></i>
            Cancel
        </Button>
    );
}

function FinderClearButton({ disabled, className, onClick }: ButtonProps) {
    return (
        <Button disabled={disabled} variant="default" size="sm" className={className} onClick={onClick}>
            <i className="fas fa-undo me-1"></i>
            Clear
        </Button>
    );
}