import { ReactNode } from "react";
import { translate } from "@/helpers/translate";
import Button from "../Button";
import SearchDownload_Columns from "./SearchDownload_Columns";
import { ListItem } from "@/models/listItem";
import { useSearchDownload } from "./useSearchDownload";
import SearchDownload_Settings from "./SearchDownload_Settings";
import SearchDownload_List from "./SearchDownload_List";
import { useDownload } from "@/hooks/useDownload";

interface Props {
    columns: ListItem[];
    children?: ReactNode;
    onDownload?: (format: "csv" | "json", visibleColumns: string[]) => Promise<{
        filename: string;
        data: Blob;
    } | null>
}

export default function SearchDownload({ columns, children, onDownload }: Props) {
    const state = useSearchDownload(columns);

    const { downloadKey, handleDownload } = useDownload();

    return (
        <div className="row">
            <div className="col-md-4">
                <h4 className="mb-0">
                    {translate("Columns")}
                </h4>
                <div className="text-body-secondary fs-sm mb-2">
                    {translate("Drag and drop to reorder")}
                </div>
                <SearchDownload_Columns
                    visibleColumns={state.visibleColumns}
                    handleSelect={state.handleSelectColumn}
                    handleSelectAll={state.handleSelectAllColumns}
                    handleReorder={state.handleReorderColumns}
                />
            </div>

            <div className="col-md-4">
                <h4>
                    {translate("Settings")}
                </h4>
                <SearchDownload_Settings
                    fileFormat={state.download.fileFormat}
                    showHiddenColumns={state.download.showHiddenColumns}
                    removeSpaces={state.download.removeSpaces}
                    handleChangeFileFormat={state.handleChangeFileFormat}
                    handleChangeHiddenColumns={state.handleChangeHiddenColumns}
                    handleChangeRemoveSpaces={state.handleChangeRemoveSpaces}
                />
            </div>

            <div className="col-md-4">
                <h4>
                    {translate("Download Templates")}
                </h4>
                <SearchDownload_List
                    download={state.download}
                    onChange={state.handleChangeDownload}
                />
                {children}
            </div>

            <div className="mt-5">
                <div className="search-download-buttons">
                    <div className="border-1 shadow-lg">
                        {onDownload && (
                            <Button
                                variant="download"
                                type="button"
                                className="me-2"
                                isLoading={!!downloadKey}
                                onClick={() => handleDownload("download", () =>
                                    {
                                        const visibleColumns = state.visibleColumns
                                            .filter(x => x.isVisible)
                                            .map(({ id }) => id);

                                        return onDownload(state.download.fileFormat, visibleColumns)
                                    })
                                }
                            />
                        )}
                        <Button variant="reset" onClick={state.handleReset} />
                    </div>
                </div>
            </div>

        </div>
    );
}