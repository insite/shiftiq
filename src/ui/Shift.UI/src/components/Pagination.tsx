import { numberHelper } from "@/helpers/numberHelper";
import { translate } from "@/helpers/translate";
import { MouseEvent } from "react";

interface Props {
    pageIndex: number;
    rowsPerPage: number;
    pagesPerView: number;
    totalRowCount: number;
    hideDetails?: boolean;
    onGoToPage: (pageIndex: number) => void;
}

export default function Pagination({ pageIndex, rowsPerPage, pagesPerView, totalRowCount, hideDetails, onGoToPage }: Props) {
    const pageCount = Math.floor((totalRowCount - 1) / rowsPerPage) + 1;
    if (pageCount <= 1) {
        return;
    }

    if (pageIndex < 0) {
        pageIndex = 0;
    } else if (pageIndex >= pageCount) {
        pageIndex = pageCount - 1;
    }

    const firstRowNumber = pageIndex * rowsPerPage + 1;
    const lastRowNumber = firstRowNumber + rowsPerPage - 1 > totalRowCount ? totalRowCount : firstRowNumber + rowsPerPage - 1;

    const firstPageIndex = Math.floor(pageIndex / pagesPerView) * pagesPerView;

    let lastPageIndex = firstPageIndex + pagesPerView - 1;
    if (lastPageIndex >= pageCount) {
        lastPageIndex = pageCount - 1;
    }

    const pages = Array.from({ length: pagesPerView < pageCount - firstPageIndex ? pagesPerView : pageCount - firstPageIndex }, (_, i) => i + firstPageIndex);

    function handleGoToPage(e: MouseEvent<HTMLAnchorElement>, newPageIndex: number) {
        e.preventDefault();
        onGoToPage(newPageIndex);
    }

    return (
        <nav className="insite-pager d-flex justify-content-between" aria-label="Grid Navigation">
            <ul className="pagination">
                {firstPageIndex > 0 && (
                    <li className="page-item">
                        <a
                            className="page-link"
                            title={translate("Previous Pages")}
                            href="#"
                            onClick={e => handleGoToPage(e, firstPageIndex - 1)}
                        >
                            ...
                        </a>
                    </li>
                )}
                {pages.map(currentPageIndex => (
                    <li key={currentPageIndex} className={`page-item ${currentPageIndex === pageIndex ? "active" : ""}`}>
                        {currentPageIndex === pageIndex ? (
                            <span className="page-link" title={translate("Current Page")}>
                                {currentPageIndex + 1}
                            </span>
                        ) : (
                            <a
                                className="page-link"
                                title={translate("Go to Page") + ` #${currentPageIndex + 1}`}
                                href="#"
                                onClick={e => handleGoToPage(e, currentPageIndex)}
                            >
                                {currentPageIndex + 1}
                            </a>
                        )}
                    </li>
                ))}
                {(lastPageIndex < pageCount - 1) && (
                    <li className="page-item">
                        <a
                            className="page-link"
                            title={translate("Next Pages")}
                            href="#"
                            onClick={e => handleGoToPage(e, lastPageIndex + 1)}
                        >
                            ...
                        </a>
                    </li>
                )}
            </ul>
            {!hideDetails && (
                <div>
                    Page <b>{numberHelper.formatInt(pageIndex + 1)}</b> of <b>{numberHelper.formatInt(pageCount)}</b>
                    &nbsp;-
                    Rows <b>{numberHelper.formatInt(firstRowNumber)}</b> to <b>{numberHelper.formatInt(lastRowNumber)}</b> of <b>{numberHelper.formatInt(totalRowCount)}</b>
                </div>
            )}
        </nav>
    );
}