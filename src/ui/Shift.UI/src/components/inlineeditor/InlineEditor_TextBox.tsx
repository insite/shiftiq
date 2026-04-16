import TextBox from "../TextBox";

interface Props {
    value: string;
    maxLength?: number;
    isSaving: boolean;
    onChange: (value: string) => void;
    onSave: () => Promise<void>;
    onCancel: () => void;
}

export default function InlineEditor_TextBox({
    value,
    maxLength,
    isSaving,
    onChange,
    onSave,
    onCancel,
}: Props) {
    function handleTextBoxKeyDown(e: React.KeyboardEvent<HTMLInputElement>) {
        if (e.key === "Enter") {
            onSave();
        } else if (e.key === "Escape") {
            onCancel();
        }
    }

    return (
        <TextBox
            autoFocus
            className="form-control-sm"
            value={value}
            maxLength={maxLength}
            disabled={isSaving}
            onChange={e => onChange(e.target.value)}
            onKeyDown={handleTextBoxKeyDown}
        />
    );
}