import { useCallback, useMemo, useState } from "react";
import { SortableColumn } from "./SearchDownload_SortableColumn";
import { ListItem } from "@/models/listItem";
import { useSearch } from "./Search";
import { DownloadConfig } from "@/models/DownloadConfig";
import { searchStorage } from "@/cache/searchStorage";

const _defaultDownload: DownloadConfig = {
    hiddenColumnIds: [],
    orderedColumnIds: null,
    fileFormat: "csv",
    showHiddenColumns: true,
    removeSpaces: false,
};

export type ChangeDownloadHandler = (download: DownloadConfig | null) => void;
export type SelectColumnHandler = (id: string, isVisible: boolean) => void;
export type SelectAllColumnsHandler = (isVisible: boolean) => void;
export type ReorderColumnsHandler = (sourceId: string, destId: string) => void;
export type ChangeFileFormatHandler = (fileFormat: "csv" | "json") => void;
export type ChangeHiddenColumnsHandler = (showHiddenColumns: boolean) => void;
export type ChangeRemoveSpacesHandler = (removeSpaces: boolean) => void;

interface Result {
    download: DownloadConfig;
    visibleColumns: SortableColumn[];
    handleReset: () => void;
    handleChangeDownload: ChangeDownloadHandler;
    handleSelectColumn: SelectColumnHandler;
    handleSelectAllColumns: SelectAllColumnsHandler;
    handleReorderColumns: ReorderColumnsHandler;
    handleChangeFileFormat: ChangeFileFormatHandler;
    handleChangeHiddenColumns: ChangeHiddenColumnsHandler;
    handleChangeRemoveSpaces: ChangeRemoveSpacesHandler;
}

interface State {
    download: DownloadConfig;
    visibleColumns: SortableColumn[];
}

export function useSearchDownload(columns: ListItem[]): Result {
    const { cacheKey } = useSearch();

    const [state, setState] = useState(() => {
        const download = searchStorage.downloadManager.getDefaultItem<DownloadConfig>(cacheKey) ?? _defaultDownload;
        return loadState(columns, download);
    });

    const refreshState = useCallback((func: (prevDownload: DownloadConfig) => DownloadConfig) => {
        const newDownload = func(state.download);

        if (cacheKey) {
            searchStorage.downloadManager.save(cacheKey, null, newDownload);
        }

        const newState = loadState(columns, newDownload);
        
        setState(newState);

    }, [cacheKey, columns, state]);

    const handlers = useMemo(() => ({
        handleReset() {
            if (window.confirm("Are you sure to reset current download settings?")) {
                refreshState(() => _defaultDownload);
            }
        },

        handleChangeDownload(download: DownloadConfig | null) {
            const newDownload = download ? { ...download } : _defaultDownload;
            refreshState(() => newDownload);
        },

        handleSelectColumn(id: string, isVisible: boolean) {
            refreshState(prev => {
                const hiddenColumnIds = prev.hiddenColumnIds.filter(x => x !== id);
                return {
                    ...prev,
                    hiddenColumnIds: isVisible
                        ? hiddenColumnIds
                        : [...hiddenColumnIds, id],
                }
            });
        },

        handleSelectAllColumns(isVisible: boolean) {
            refreshState(prev => {
                return {
                    ...prev,
                    hiddenColumnIds: isVisible
                        ? []
                        : columns.map(x => x.value),
                }
            });
        },

        handleReorderColumns(sourceId: string, destId: string) {
            refreshState(prev => {
                const orderedColumnIds = prev.orderedColumnIds
                        && prev.orderedColumnIds.length === columns.length
                        && !prev.orderedColumnIds.find(id => !columns.find(c => c.value === id))
                    ? prev.orderedColumnIds
                    : columns.map(x => x.value);

                const sourceIndex = orderedColumnIds.findIndex(id => id === sourceId);
                const destIndex = orderedColumnIds.findIndex(id => id === destId);

                orderedColumnIds.splice(sourceIndex, 1);
                orderedColumnIds.splice(destIndex, 0, sourceId);

                return {
                    ...prev,
                    orderedColumnIds
                };
            });
        },

        handleChangeFileFormat(fileFormat: "csv" | "json") {
            refreshState(prev => {
                return {
                    ...prev,
                    fileFormat
                };
            });
        },

        handleChangeHiddenColumns(showHiddenColumns: boolean) {
            refreshState(prev => {
                return {
                    ...prev,
                    showHiddenColumns
                };
            });
        },

        handleChangeRemoveSpaces(removeSpaces: boolean) {
            refreshState(prev => {
                return {
                    ...prev,
                    removeSpaces
                };
            });
        },
    }), [refreshState, columns]);

    const result = useMemo(() => ({
        download: state.download,
        visibleColumns: state.visibleColumns,
        handleReset: handlers.handleReset,
        handleChangeDownload: handlers.handleChangeDownload,
        handleSelectColumn: handlers.handleSelectColumn,
        handleSelectAllColumns: handlers.handleSelectAllColumns,
        handleReorderColumns: handlers.handleReorderColumns,
        handleChangeFileFormat: handlers.handleChangeFileFormat,
        handleChangeHiddenColumns: handlers.handleChangeHiddenColumns,
        handleChangeRemoveSpaces: handlers.handleChangeRemoveSpaces,
    }), [state, handlers]);

    return result;
}

function loadState(columns: ListItem[], download: DownloadConfig): State {
    const orderedColumns = download.orderedColumnIds
            && download.orderedColumnIds.length === columns.length
            && !download.orderedColumnIds.find(id => !columns.find(c => c.value === id))
        ? download.orderedColumnIds.map(id => columns.find(c => c.value === id)!)
        : columns;

    const filteredColumns = !download.showHiddenColumns
        ? orderedColumns.filter(x => !download.hiddenColumnIds.includes(x.value))
        : orderedColumns;

    const visibleColumns = filteredColumns
        .map(({ value, text }) => ({
            id: value,
            title: text,
            isVisible: !download.hiddenColumnIds.includes(value),
        }));

    return {
        download,
        visibleColumns
    };
}