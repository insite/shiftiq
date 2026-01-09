export type ApiDownloadFormat = "csv" | "json";

export interface ApiDownload {
    filename: string;
    data: Blob;
}