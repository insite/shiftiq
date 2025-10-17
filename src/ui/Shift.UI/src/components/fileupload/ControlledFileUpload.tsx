import { Control, Path, useController } from "react-hook-form";
import { fieldRequiredMessage } from "@/helpers/errorHelper";
import FileUpload, { FileUploadProps } from "@/components/fileupload/FileUpload";
import { ApiUploadFileInfo } from "@/api/controllers/file/ApiUploadFileInfo";

interface Props<Criteria extends object>
    extends Omit<FileUploadProps, "ref" | "name" | "onUploadSucceed" | "onUploadFailed">
{
    name: Path<Criteria>;
    control: Control<Criteria>;
    required?: boolean;
    validate?: (value: ApiUploadFileInfo[] | null) => string | undefined;
}

export function ControlledFileUpload<Criteria extends object>({
    name,
    control,
    required,
    validate,
    ...fileUploadProps
}: Props<Criteria>) {
    const { field, fieldState } = useController({
        name,
        control,
        rules: {
            validate: (value: ApiUploadFileInfo[] | null | undefined) => {
                if (required && (!value || value.length === 0)) {
                    return fieldRequiredMessage;
                }
                return validate?.(value ?? null);
            }
        },
    });

    return (
        <FileUpload
            {...fileUploadProps}
            ref={field.ref}
            name={field.name}
            error={fieldState.error}
            onUploadSucceed={field.onChange}
            onUploadFailed={field.onChange}
            onBlur={field.onBlur}
        />
    )
}