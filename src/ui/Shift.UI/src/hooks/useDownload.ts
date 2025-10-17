import { useStatusProvider } from "@/contexts/StatusProvider";
import { downloadHelper } from "@/helpers/downloadHelper";
import { useState } from "react";

type DownloadHandler = () => Promise<{
    filename: string;
    data: Blob;
} | null>;

export function useDownload() {
    const [downloadKey, setDownloadKey] = useState<string | null>(null);    
    const { addError, removeError } = useStatusProvider();

    async function handleDownload(key: string, onDownload: DownloadHandler) {
        if (downloadKey) {
            return;
        }

        setDownloadKey(key);

        try {
            const result = await onDownload();
            if (result) {
                downloadHelper.download(result.filename, result.data);
            }
            removeError();
        } catch (err) {
            addError(err, "Failed to download data");
        }
        finally {
            setDownloadKey(null);
        }
    }

    return {
        downloadKey,
        handleDownload
    };
}