import { filePickerHelper } from "@/helpers/filePickerHelper";

interface Props {
    supportedFileTypes: string[];
    onUploadFile: (file: File) => void;
}

export default function RichTextEditor_FileTypes({
    supportedFileTypes,
    onUploadFile
}: Props) {
    async function handleClick(e: React.MouseEvent<HTMLAnchorElement, MouseEvent>) {
        e.preventDefault();

        const files = await filePickerHelper.pick(false, supportedFileTypes);
        if (files && files.length === 1) {
            onUploadFile(files[0]);
        }
    }

    return (
        <div className="form-text my-2">
            Attach files by dragging and dropping or
            {" "}
            <a href="#" onClick={handleClick}>selecting</a>
            {" "}
            them.
            File types supported: {supportedFileTypes.join(", ")}
        </div>
    );
}