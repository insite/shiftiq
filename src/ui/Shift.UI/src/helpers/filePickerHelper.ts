export const filePickerHelper = {
    pick(multiple?: boolean, accept?: string[]): Promise<FileList | null> {
        const input = document.createElement("input");
        input.type = "file";

        if (multiple) {
            input.multiple = true;
        }

        if (accept && accept.length) {
            input.accept = accept.join(",");
        }

        input.click();

        return new Promise<FileList | null>(resolve => {
            input.addEventListener("change", () => {
                const result = input.files?.length
                    ? input.files
                    : null;

                resolve(result);
            });

            input.addEventListener("cancel", () => {
                resolve(null);
            });
        });
    }
}