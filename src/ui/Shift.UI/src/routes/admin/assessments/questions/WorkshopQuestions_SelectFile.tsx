import "./WorkshopQuestions_SelectFile.css";
import { shiftClient } from "@/api/shiftClient";
import type { WorkshopImage } from "@/contexts/workshop/models/WorkshopImage";
import { useWorkshopQuestionProvider } from "@/contexts/workshop/WorkshopQuestionProviderContext";
import { translate } from "@/helpers/translate";
import type { SyntheticEvent } from "react";
import { useEffect, useRef, useState } from "react";
import { Modal, Spinner } from "react-bootstrap";
import { workshopQuestionAdapter } from "./workshopQuestionAdapter";
import { getErrorDescription } from "@/contexts/status/StatusProviderContext";
import Alert from "@/components/Alert";
import Icon from "@/components/icon/Icon";
import { Environment } from "@/models/enums";

interface Props {
    onSelect(fileUrl: string, documentName: string, isImage: boolean): void;
    onClose(): void;
}

export default function WorkshopQuestions_SelectFile({
    onSelect,
    onClose,
}: Props)
{
    const { bankId } = useWorkshopQuestionProvider();

    const [files, setFiles] = useState<WorkshopImage[] | null>(null);
    const [error, setError] = useState<string | null>(null);
    const [brokenImages, setBrokenImages] = useState<Record<string, boolean>>({});

    const listRef = useRef<HTMLDivElement>(null);

    useEffect(() => {
        run();

        async function run() {
            try {
                const apiImages = await shiftClient.workshop.collectImages(bankId);
                if (apiImages) {
                    setFiles(workshopQuestionAdapter.getImages(apiImages));
                } else {
                    setFiles([]);
                }
                setError(null);
            } catch (err) {
                setError(getErrorDescription(err));
            }
        }
    }, [bankId]);

    useEffect(() => {
        setBrokenImages({});
    }, [files]);

    useEffect(() => {
        window.addEventListener("resize", handleWindowResize);

        handleWindowResize();

        return () => {
            window.removeEventListener("resize", handleWindowResize);
        };

        function handleWindowResize() {
            if (!listRef.current || !listRef.current.checkVisibility()) {
                return;
            }
            listRef.current.querySelectorAll(".file-thumbnail > .file-preview").forEach(el => {
                const preview = el as HTMLDivElement;
                const maxWidth = preview.offsetWidth;
                if (maxWidth !== 0) {
                    preview.style.height = `${maxWidth / 3}px`;
                }
            });
        }
    }, []);

    function handleSelect(file: WorkshopImage) {
        onSelect(file.url, file.fileName, true);
    }

    function handleImageLoad(e: SyntheticEvent<HTMLImageElement, Event>) {
        const img = e.target as HTMLImageElement;
        img.style.opacity = "1";
    }

    function handleImageError(file: WorkshopImage) {
        setBrokenImages(prev => ({
            ...prev,
            [file.url]: true,
        }));
    }

    return (
        <Modal
            show
            className="insite-modal"
            dialogClassName="modal-dialog WorkshopQuestions_SelectFile"
            onHide={onClose}
        >
            <Modal.Header closeButton>
                <Modal.Title as="h5">{translate("Select Image")}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                {error ? (
                    <Alert alertType="error">
                        {error}
                    </Alert>
                ) : !files ? (
                    <div className="d-flex align-items-center gap-3 justify-content-center">
                        <Spinner animation="border" role="status" /> Loading...
                    </div>
                ) : files.length === 0 ? (
                    <div className="text-center fs-2 my-4">
                        No Images
                    </div>
                ) : (
                    <div ref={listRef} className="WorkshopQuestions_SelectFile_List">
                        <h3 className="pb-3">{translate("Images")}</h3>

                        <div className="row g-3">
                            {files.map(file => (
                                <div key={file.url} className="col-lg-3 col-md-4 col-sm-6 mt-0">
                                    <div className="file-thumbnail">
                                        <div className="file-preview">
                                            {!brokenImages[file.url] ? (
                                                <img
                                                    alt={file.fileName}
                                                    src={file.url}
                                                    onLoad={handleImageLoad}
                                                    onError={() => handleImageError(file)}
                                                    onClick={() => handleSelect(file)}
                                                />
                                            ) : (
                                                <Icon style="light" name="file-xmark" className="d-block no-upload" />
                                            )}
                                        </div>

                                        <div className="icon-wrapper">
                                            <span className={`badge ${getEnvironmentTheme(file.environment)}`}>
                                                {`${file.attachment ? "Bank " : ""}${file.environment}`}
                                            </span>
                                        </div>

                                        {file.attachment ? (
                                            <>
                                                <label>{translate("Asset Title")}</label>
                                                <span className="file-title" title={file.attachment.title}>{file.attachment.title}</span>

                                                <label>{translate("Asset #")}</label>
                                                <span className="file-title">{file.attachment.number}</span>

                                                <label>{translate("Condition")}</label>
                                                <span className="file-title">{file.attachment.condition ?? "Unassigned"}</span>

                                                <label>{translate("Publication Status")}</label>
                                                <span className="file-title">{file.attachment.publicationStatus}</span>

                                                <label>{translate("File Name")}</label>
                                                <span className="file-title" title={file.fileName}>{file.fileName}</span>

                                                <label>{translate("Dimension")}</label>
                                                <span className="file-title">{file.attachment?.dimension ?? "0 x 0"}</span>
                                            </>
                                        ) : (
                                            <>
                                                <label>{translate("File Name")}</label>
                                                <span className="file-title" title={file.fileName}>{file.fileName}</span>

                                                <label>{translate("Dimension")}</label>
                                                <span className="file-title">0 x 0</span>
                                            </>
                                        )}
                                    </div>
                                </div>
                            ))}
                        </div>
                    </div>
                )}
            </Modal.Body>
        </Modal>
    );
}

function getEnvironmentTheme(environment: Environment): string {
    switch (environment) {
        case "Production":
            return "bg-success";
        case "Sandbox":
            return "bg-warning text-dark";
        case "Development":
            return "bg-danger";
        case "Local":
            return "bg-primary";
        case "External":
            return "bg-custom-default";
    }
}