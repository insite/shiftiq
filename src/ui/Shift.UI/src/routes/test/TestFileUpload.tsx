import { useForm } from "react-hook-form";
import FormField from "@/components/form/FormField";
import ValidationSummary from "@/components/ValidationSummary";
import Button from "@/components/Button";
import { ApiUploadFileInfo } from "@/api/controllers/file/ApiUploadFileInfo";
import { ControlledFileUpload } from "@/components/fileupload/ControlledFileUpload";
import TextBox from "@/components/TextBox";

interface FormFields {
    files: ApiUploadFileInfo[] | null;
}

export default function TestFileUpload() {
    const { handleSubmit, formState: { errors }, control } = useForm<FormFields>({
        defaultValues: {
            files: null,
        }
    });

    function handleValidSubmit(fields: FormFields) {
        console.log(fields);
    }

    return (
        <>
            <div className="row">
                <div className="col-3">
                    <form autoComplete="off" onSubmit={handleSubmit(handleValidSubmit)}>
                        <ValidationSummary errors={errors} />
                        
                        <FormField>
                            <TextBox />
                        </FormField>
                        <FormField>
                           <ControlledFileUpload
                                control={control}
                                name="files"
                                required
                                allowedExtensions={[".csv", ".png"]}
                           /> 
                        </FormField>
                        <FormField>
                            <TextBox />
                        </FormField>
                        <FormField>
                            <Button
                                variant="save"
                                text="Submit to Console"
                            />
                        </FormField>
                    </form>
                </div>
            </div>
        </>
    )
}