import "./WorkshopAttachments_Group.css";

import ActionLink from "@/components/ActionLink";
import Icon from "@/components/icon/Icon";
import { WorkshopAttachment } from "@/contexts/workshop/models/WorkshopAttachment";
import { dateTimeHelper } from "@/helpers/date/dateTimeHelper";
import { urlHelper } from "@/helpers/urlHelper";
import { Fragment, useEffect, useMemo, useRef, useState } from "react";

interface Props {
    bankId: string;
    attachments: WorkshopAttachment[];
    filterText: string;
    selectedAttachmentId: string | null;
}

export default function WorkshopAttachments_Group({ bankId, attachments, filterText, selectedAttachmentId }: Props) {
    const [imagesWithError, setImagesWithError] = useState<string[]>([]);
    const rowRef = useRef<HTMLDivElement>(null);

    const filteredAttachments = useMemo(() => {
        return !filterText
            ? attachments
            : attachments.filter(({ title, fileName }) =>
                (title && title.toLowerCase().includes(filterText.toLowerCase()))
                || (fileName && fileName.toLowerCase().includes(filterText.toLowerCase()))
            );
    }, [attachments, filterText]);

    useEffect(() => {
        window.addEventListener("resize", handleWindowResize);
        window.addEventListener("click", handleWindowResize);

        handleWindowResize();

        return () => {
            window.removeEventListener("resize", handleWindowResize);
            window.removeEventListener("click", handleWindowResize)
        };

        function handleWindowResize() {
            if (!rowRef.current || !rowRef.current.checkVisibility()) {
                return;
            }
            rowRef.current.querySelectorAll(".card-attachment > .file-preview").forEach(el => {
                const preview = el as HTMLDivElement;
                const maxWidth = preview.offsetWidth;
                if (maxWidth !== 0) {
                    preview.style.height = `${maxWidth / 1.777}px`;
                }
            });
        }
    }, []);

    useEffect(() => {
        if (!selectedAttachmentId) {
            return;
        }
        const el = document.getElementById(selectedAttachmentId);
        el?.scrollIntoView({
            behavior: "smooth"
        });
    }, [selectedAttachmentId]);

    function handleImageLoad(e: React.SyntheticEvent<HTMLImageElement, Event>) {
        const img = e.target as HTMLImageElement;
        img.style.opacity = "1";
    }

    function handleImageError(attachmentId: string) {
        setImagesWithError(prev => [...prev, attachmentId]);
    }

    return (
        <div ref={rowRef} className="row row-attachments">
            {filteredAttachments.map((a, index) => {
                const returnUrl = urlHelper.getInSiteReturnUrl(`tab=attachments&attachment=${a.attachmentId}`);

                return (
                    <Fragment key={a.attachmentId}>
                        {index > 0 && (index % 3) === 0 ? <div className="row-separator"></div> : null}

                        <div id={a.attachmentId} className="col-md-4">
                            <div className="card mb-3">
                                <div className="card-body card-attachment">
                                    {a.attachmentType === "Image" && (
                                        <>
                                            <div className="file-preview">
                                                {a.fileUrl && !imagesWithError.includes(a.attachmentId) ? (
                                                    <img
                                                        alt=""
                                                        src={a.fileUrl}
                                                        onLoad={handleImageLoad}
                                                        onError={() => handleImageError(a.attachmentId)}
                                                    />
                                                ) : (
                                                    <Icon style="light" name="file-xmark" className="d-block no-upload" />
                                                )}
                                            </div>
                                            <hr />
                                        </>
                                    )}
                                    <table className="table table-striped">
                                        <tbody>
                                            <tr>
                                                <td className="info-label">Asset Title</td>
                                                <td className="info-value" title={a.title ?? "(Untitled)"}>{a.title ?? "(Untitled)"}</td>
                                            </tr>
                                            <tr>
                                                <td className="info-label">Asset #</td>
                                                <td className="info-value">
                                                    {a.assetNumber}.{a.assetVersion}
                                                    <div className="d-inline-block float-end">
                                                        <ActionLink
                                                            title="Edit Attachment"
                                                            href={`/ui/admin/assessments/attachments/change?bank=${bankId}&attachment=${a.attachmentId}&${returnUrl}`}
                                                            className="me-1"
                                                            icon={{ style: "regular", name: "pencil" }}
                                                        />
                                                        <ActionLink
                                                            title="Delete Attachment"
                                                            href={`/admin/assessments/attachments/delete?bank=${bankId}&attachment=${a.attachmentId}&${returnUrl}`}
                                                            icon={{ style: "regular", name: "trash-alt", className: "text-danger" }}
                                                        />
                                                    </div>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td className="info-label">Condition</td>
                                                <td className="info-value">{a.condition ?? "Unassigned"}</td>
                                            </tr>
                                            <tr>
                                                <td className="info-label">Publication<br/>Status</td>
                                                <td className="info-value">{a.publicationStatus}</td>
                                            </tr>
                                            <tr>
                                                <td className="info-label">File Name</td>
                                                <td className="info-value" title={a.fileName ?? "N/A"}>
                                                    {a.fileUrl ? (
                                                        <a href={`${a.fileUrl}?download=1`}>
                                                            {a.fileName}
                                                        </a>
                                                    ) : (
                                                        <>
                                                            <span>N/A</span>
                                                            <span className="fas fa-exclamation-triangle text-danger"></span>
                                                        </>
                                                    )}
                                                </td>
                                            </tr>
                                            <tr>
                                                <td className="info-label">File Size</td>
                                                <td className="info-value">{a.fileSize}</td>
                                            </tr>
                                            {a.attachmentType === "Image" && (
                                                <>
                                                    {a.imageResolution && (
                                                        <tr>
                                                            <td className="info-label">Resolution</td>
                                                            <td className="info-value">{a.imageResolution}</td>
                                                        </tr>
                                                    )}
                                                    {a.imageDimensions && (
                                                        <tr>
                                                            <td className="info-label">Dimensions</td>
                                                            <td className="info-value">
                                                                {a.imageDimensions.map((x, index) => (
                                                                    <Fragment key={x}>
                                                                        {index > 0 ? <br/> : null}
                                                                        {x}
                                                                    </Fragment>
                                                                ))}
                                                            </td>
                                                        </tr>
                                                    )}
                                                    <tr>
                                                        <td className="info-label">Palette</td>
                                                        <td className="info-value">{a.color}</td>
                                                    </tr>
                                                </>
                                            )}
                                            <tr>
                                                <td className="info-label">Timestamp</td>
                                                <td className="info-value form-text">
                                                    Posted by {a.authorName ?? "N/A"}
                                                    <br />
                                                    on {dateTimeHelper.formatDateTime(a.postedOn, "mmm d, yyyy", " - ")}
                                                </td>
                                            </tr>
                                            <tr>
                                                <td className="info-label">Changes</td>
                                                <td className="info-value">
                                                    {a.changeCount}
                                                    <div className="d-inline-block ms-2">
                                                        <ActionLink
                                                            href={`/ui/admin/assessments/attachments/history?bank=${bankId}&attachment=${a.attachmentId}&version=all&${returnUrl}`}
                                                            title="View Change History"
                                                        >
                                                            <Icon style="solid" name="history" />
                                                        </ActionLink>
                                                    </div>
                                                </td>
                                            </tr>
                                            {a.questionCount > 0 && (
                                                <tr>
                                                    <td className="info-label">Questions</td>
                                                    <td className="info-value">
                                                        {a.questionCount}
                                                        <div className="d-inline-block ms-2">
                                                            <ActionLink
                                                                href={`/ui/admin/assessments/attachments/usage?bank=${bankId}&attachment=${a.attachmentId}&version=all&${returnUrl}`}
                                                                title="View Usage Statistic"
                                                            >
                                                                <Icon style="solid" name="chart-pie" />
                                                            </ActionLink>
                                                        </div>
                                                    </td>
                                                </tr>
                                            )}
                                        </tbody>
                                    </table>
                                </div>
                            </div>
                        </div>
                    </Fragment>
                );
            })}
        </div>
    );
}