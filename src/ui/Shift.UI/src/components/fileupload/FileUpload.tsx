import { ForwardedRef, useMemo, useImperativeHandle, useRef, useState } from "react";
import { FieldError } from "react-hook-form";
import { Spinner } from "react-bootstrap";
import { numberHelper } from "@/helpers/numberHelper";
import { translate } from "@/helpers/translate";
import { useStatusProvider } from "@/contexts/StatusProvider";
import { shiftClient } from "@/api/shiftClient";
import { ApiUploadFileInfo } from "@/api/controllers/file/ApiUploadFileInfo";
import { errorHelper } from "@/helpers/errorHelper";
import ProgressBar, { ProgressBarIndicator } from "../ProgressBar";

import "./FileUpload.css";
import { filePickerHelper } from "@/helpers/filePickerHelper";

interface FileUploadRef {
    focus: () => void;
    clearFileInput: () => void;
}

export interface FileUploadProps {
    ref?: ForwardedRef<FileUploadRef>,
    name?: string;
    responseId?: string;
    allowMultiple?: boolean;
    showProgress?: boolean;
    className?: string;
    maxFileSize?: number;
    maxFileNameLength?: number;
    allowedExtensions?: string[]; // [".jpg", ".png"]
    error?: FieldError;
    onUploadSucceed?: (files: ApiUploadFileInfo[]) => void;
    onUploadFailed?: () => void;
    onBlur?: () => void;
}

interface ProgressState {
    value: number;
    indicator: ProgressBarIndicator;
}

interface FileUploadSettings {
    maxFileSize?: number;
    maxFileNameLength?: number;
    allowedExtensions?: string[];
    allowedExtensionsRegex?: RegExp;
}

export default function FileUpload({
    ref,
    responseId,
    allowMultiple = false,
    showProgress = true,
    className,
    maxFileSize,
    maxFileNameLength,
    allowedExtensions,
    error,
    onUploadSucceed,
    onUploadFailed,
    onBlur,
}: FileUploadProps) {
    const textInputRef = useRef<HTMLInputElement | null>(null);

    const [inputValue, setInputValue] = useState("");
    const [progress, setProgress] = useState<ProgressState>({
        value: 0,
        indicator: ProgressBarIndicator.default
    });
    const [isBusy, setIsBusy] = useState(false);

    const { addError, removeError } = useStatusProvider();

    const errorTooltip = errorHelper.getErrorTooltip(error) ?? "";

    const settings: FileUploadSettings = useMemo(() => {
        const result: FileUploadSettings = {
            maxFileSize: maxFileSize && !isNaN(maxFileSize) && maxFileSize > 0 ? maxFileSize : undefined,
            maxFileNameLength: maxFileNameLength && !isNaN(maxFileNameLength) && maxFileNameLength > 0 ? maxFileNameLength : undefined
        };

        if (allowedExtensions && allowedExtensions.length > 0) {
            const array = [...new Set(allowedExtensions.filter(x => /^\.[^.]+$/.test(x)).map(x => x.toLowerCase()))];
            if (array.length > 0) {
                result.allowedExtensions = array;
                result.allowedExtensionsRegex = new RegExp(`\\.(${array.map(x => x.substring(1)).join("|")})$`, 'i');
            }
        }

        return result;
    }, [maxFileSize, maxFileNameLength, allowedExtensions]);

    useImperativeHandle(ref, () => {
        return {
            focus() {
                textInputRef.current?.focus();
            },
            clearFileInput() {
                if (isBusy) {
                    return;
                }

                setInputValue("");
                setProgress({
                    value: 0,
                    indicator: ProgressBarIndicator.default
                });
            }
        }
    }, [isBusy]);

    async function handleButtonClick() {
        if (isBusy) {
            return;
        }

        const files = await filePickerHelper.pick(allowMultiple, allowedExtensions);
        if (files) {
            await uploadFiles(files);
        }

        textInputRef.current!.focus();
    }

    async function uploadFiles(files: FileList) {
        const names = validateFiles(files, settings);
        if (!names) {
            return;
        }

        setIsBusy(true);
        setProgress({
            value: 0,
            indicator: ProgressBarIndicator.default
        });

        setInputValue(names);

        let uploadedFiles: ApiUploadFileInfo[] | null;

        try {
            uploadedFiles = await shiftClient.file.uploadTempFile(files, responseId, (percent) => {
                setProgress({
                    value: percent,
                    indicator: ProgressBarIndicator.default
                });
            });
            removeError();
        } catch (err) {
            uploadedFiles = null;
            addError(err);
        } finally {
            setIsBusy(false);
        }

        if (uploadedFiles) {
            setProgress({
                value: 100,
                indicator: ProgressBarIndicator.success
            });
            onUploadSucceed?.(uploadedFiles);
        } else {
            setInputValue("");
            setProgress({ ...progress, indicator: ProgressBarIndicator.danger });
            onUploadFailed?.();
        }
    }

    return (
        <div className={"fileupload " + (className ?? "")}>
            <div>
                <input
                    ref={textInputRef}
                    type="text"
                    className={`form-control ${errorTooltip ? "is-invalid" : ""}`}
                    readOnly
                    value={inputValue}
                    onBlur={onBlur}
                />
                <button
                    type="button"
                    title={isBusy ? translate("Loading...") : translate("Browse")}
                    disabled={isBusy}
                    tabIndex={-1}
                    onClick={handleButtonClick}
                >
                    {isBusy ? <Spinner animation="border" role="status" size="sm" /> : <i className="far fa-search"></i>}
                </button>
            </div>
            {showProgress && (
                <div className="progressbar">
                    <ProgressBar value={progress.value} indicator={progress.indicator} indicatorAsBg />
                </div>
            )}
        </div>
    );
};

function validateFiles(files: FileList | null, settings: FileUploadSettings) {
    if (!files || files.length === 0) {
        return "";
    }

    let names = "";

    for (const file of files) {
        if (!checkFileSize(file, settings) || !checkFileExtension(file, settings) || !checkFileNameLength(file, settings)) {
            return "";
        }
        names += (names ? "; " : "") + file.name;
    }

    return names;
}

function checkFileSize(file: File, settings: FileUploadSettings) {
    if (file.size === 0) {
        alert(`The file ${file.name} is empty.`);
        return false;
    }

    if (settings.maxFileSize && file.size > settings.maxFileSize) {
        alert(`The file ${file.name} exceeds max allowed size. Max size: ${numberHelper.formatBytes(settings.maxFileSize)}.`);
        return false;
    }

    return true;
}

function checkFileExtension(file: File, settings: FileUploadSettings) {
    if (!settings.allowedExtensionsRegex || settings.allowedExtensionsRegex.test(file.name)) {
        return true;
    }

    alert(`${file.name} has a file name extension that is not permitted here. Allowed extensions: ${settings.allowedExtensions!.join(", ")}`);

    return false;
}

function checkFileNameLength(file: File, settings: FileUploadSettings) {
    if (!settings.maxFileNameLength || file.name.length <= settings.maxFileNameLength) {
        return true;
    }

    alert(`File name is too long. Max allowed length is ${settings.maxFileNameLength} characters.`);

    return false;
}