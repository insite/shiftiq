import TextArea from "../TextArea";

interface Props {
    value: string;
    maxLength?: number;
    isSaving: boolean;
    onChange: (value: string) => void;
    onCancel: () => void;
}

export default function InlineEditor_TextBox({
    value,
    maxLength,
    isSaving,
    onChange,
    onCancel,
}: Props) {
    function handleKeyDown(e: React.KeyboardEvent<HTMLTextAreaElement>) {
        if (e.key === "Escape") {
            onCancel();
        }
    }

    return (
        <TextArea
            autoFocus
            className="form-control-sm"
            value={value}
            maxLength={maxLength}
            rows={7}
            disabled={isSaving}
            onChange={e => onChange(e.target.value)}
            onKeyDown={handleKeyDown}
        />
    );
}