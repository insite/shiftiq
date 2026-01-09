import FormField from "@/components/form/FormField";
import Button from "@/components/Button";
import TextBox from "@/components/TextBox";
import ValidationSummary from "@/components/ValidationSummary";
import { useForm } from "react-hook-form";
import ControlledMultiSelect from "@/components/multiselect/ControlledMultiSelect";
import ControlledDatePicker from "@/components/date/ControlledDatePicker";
import { DateParts } from "@/helpers/date/dateTimeTypes";
import ControlledRichTextEditor from "@/components/richtexteditor/ControlledRichTextEditor";
import { RichTextEditorValue } from "@/components/richtexteditor/RichTextEditorValue";

interface FormFields {
    fullName: string | null;
    birthdate: DateParts | null;
    options: string[] | null;
    summary: RichTextEditorValue | null;
    template: RichTextEditorValue | null;
}

export default function TestValidation() {
    const { handleSubmit, register, formState: { errors }, control } = useForm<FormFields>({
        defaultValues: {
            fullName: null,
            birthdate: null,
            options: null,
            summary: null,
            template: {
                markdown: {
                    en: "English Template",
                    fr: "French Template"
                },
                html: {
                    en: "<p>Test</p>"
                }
            }
        }
    });

    function handleValidSubmit(fields: FormFields) {
        console.log(fields);
    }

    return (
        <form autoComplete="off" onSubmit={handleSubmit(handleValidSubmit)}>
            <ValidationSummary errors={errors} />

            <div className="row">
                <div className="col-3">
                    <FormField label="Full Name" required error={errors.fullName}>
                        <TextBox
                            {...register("fullName", {
                                required: true
                            })}
                            error={errors.fullName}
                        />
                    </FormField>
                    <FormField label="Birthdate" required error={errors.birthdate}>
                        <ControlledDatePicker
                            name="birthdate"
                            control={control}
                            required={true}
                            className="w-75"
                            placeholder="MMM d, yyyy"
                        />
                    </FormField>
                    <FormField label="Options" required error={errors.options}>
                        <ControlledMultiSelect
                            name="options"
                            control={control}
                            validate={values => {
                                return values?.length !== 2 ? "Two options must be selected" : undefined;
                            }}
                            items={[
                                { value: "Opt1", text: "Administrator" },
                                { value: "Opt2", text: "Supervisor" },
                                { value: "Opt3", text: "Manager" },
                            ]}
                            showButtons={false}
                        />
                    </FormField>
                </div>
                <div className="col-6">
                    <FormField label="Summary" required error={errors.summary}>
                        <ControlledRichTextEditor
                            control={control}
                            name="summary"
                            required
                            defaultMode="markdown"
                        />
                    </FormField>
                    <FormField label="Template">
                        <ControlledRichTextEditor
                            control={control}
                            name="template"
                            disableUploadFile
                            defaultMode="html"
                        />
                    </FormField>
                </div>
            </div>

            <div>
                <Button variant="save" text="Post" />
            </div>
        </form>
    );
}