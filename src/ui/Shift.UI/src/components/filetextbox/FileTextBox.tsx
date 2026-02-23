import "./FileTextBox.css";

import { errorHelper } from "@/helpers/errorHelper";
import { translate } from "@/helpers/translate";
import { ForwardedRef, useState } from "react";
import { Spinner } from "react-bootstrap";
import { FieldError } from "react-hook-form";
import Icon from "../icon/Icon";
import { filePickerHelper } from "@/helpers/filePickerHelper";
import { shiftClient } from "@/api/shiftClient";
import { useStatusProvider } from "@/contexts/StatusProvider";
import { ApiUploadFileInfo } from "@/api/controllers/file/ApiUploadFileInfo";
import { urlHelper } from "@/helpers/urlHelper";

interface Props {
    ref?: ForwardedRef<HTMLInputElement>,
    name?: string;
    autoFocus?: boolean;
    className?: string;
    maxLength?: number;
    placeholder?: string;
    disabled?: boolean;
    readOnly?: boolean;
    value?: string;
    defaultValue?: string;
    supportedFileTypes?: string[];
    error?: FieldError;
    onBlur?: (value: string) => void;
    onChange?: (value: string) => void;
}

export default function FileTextBox({
    ref,
    name,
    autoFocus,
    className,
    maxLength,
    placeholder,
    disabled,
    readOnly,
    value,
    defaultValue,
    supportedFileTypes,
    error,
    onBlur,
    onChange
}: Props) {
    const [isLoading, setIsLoading] = useState(false);
    const [storedValue, setStoredValue] = useState(value !== undefined ? value : defaultValue ?? null);

    const currentValue = value !== undefined ? value : storedValue;
    const setCurrentValue = value !== undefined
        ? (onChange ?? (() => {}))
        : (newValue: string) => {
            setStoredValue(newValue);
            onChange?.(newValue);
        };

    const { addError, removeError } = useStatusProvider();

    const errorTooltip = errorHelper.getErrorTooltip(error);

    function handleChange(e: React.ChangeEvent<HTMLInputElement>) {
        setCurrentValue(e.target.value);
    }

    async function handleClick(e: React.MouseEvent<HTMLButtonElement>) {
        e.preventDefault();

        if (disabled || readOnly) {
            return;
        }

        const files = await filePickerHelper.pick(false, supportedFileTypes);
        if (!files || files.length !== 1) {
            return;
        }

        let result: ApiUploadFileInfo[] | null;

        setIsLoading(true);

        try {
            result = await shiftClient.file.uploadTempFile(files[0]);
            removeError();
        } catch (err) {
            addError(err, "Failed to upload file");
            return null;
        } finally {
            setIsLoading(false);
        }

        if (!result || result.length === 0) {
            return;
        }

        const url = urlHelper.getFileUrl(result[0].FileId, result[0].FileName);
        setCurrentValue(url);
    }

    return (
        <div className={"filetextbox " + (className ?? "")}>
            <input
                ref={ref}
                autoFocus={autoFocus}
                name={name}
                className={`form-control ${error ? "is-invalid" : ""}`}
                maxLength={maxLength}
                placeholder={placeholder}
                value={currentValue ?? undefined}
                disabled={disabled}
                readOnly={isLoading || readOnly}
                type="text"
                title={errorTooltip ?? undefined}
                onBlur={() => onBlur?.(currentValue ?? "")}
                onChange={handleChange}
            />
            <button
                type="button"
                title={isLoading ? translate("Uploading...") : translate("Browse")}
                disabled={isLoading || disabled || readOnly}
                tabIndex={-1}
                onClick={handleClick}
            >
                {isLoading ? <Spinner animation="border" role="status" size="sm" /> : <Icon style="Regular" name="search" />}
            </button>
        </div>
    );
}