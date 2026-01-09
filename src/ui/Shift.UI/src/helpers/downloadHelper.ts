export const downloadHelper = {
    download(filename: string, data: Blob) {
        const url = window.URL.createObjectURL(data);

        const a = document.createElement("a");
        a.href = url;
        a.download = filename;
        a.click();
        
        window.URL.revokeObjectURL(url);
    }
}