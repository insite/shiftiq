export interface DownloadConfig {
    hiddenColumnIds: string[];
    orderedColumnIds: string[] | null;
    fileFormat: "csv" | "json";
    showHiddenColumns: boolean;
    removeSpaces: boolean;
}