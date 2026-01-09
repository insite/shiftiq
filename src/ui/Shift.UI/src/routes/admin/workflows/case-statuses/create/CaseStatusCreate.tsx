import { ApiCreateCaseStatus } from "@/api/controllers/caseStatus/ApiCreateCaseStatus";
import { shiftClient } from "@/api/shiftClient";
import Button from "@/components/Button";
import FormCard from "@/components/form/FormCard";
import FormField from "@/components/form/FormField";
import TextBox from "@/components/TextBox";
import TextArea from "@/components/TextArea";
import ValidationSummary from "@/components/ValidationSummary";
import { translate } from "@/helpers/translate";
import { useNewForm } from "@/hooks/useNewForm";
import CaseStatusCategoryComboBox from "@/routes/_shared/comboboxes/CaseStatusCategoryComboBox";
import { _searchCache } from "@/cache/_searchCache";

interface FormValues {
    caseType: string;
    statusName: string;
    statusSequence: number | null;
    statusCategory: string;
    reportCategory: string;
    statusDescription: string;
}

export default function CaseStatusCreate() {
    const {
        backUrl,
        isSaving,
        register,
        control,
        errors,
        handleSubmit
    } = useNewForm<FormValues>(
        {
            caseType: "",
            statusName: "",
            statusSequence: null,
            statusCategory: "",
            reportCategory: "",
            statusDescription: "",
        },

        "/client/admin/workflows/case-statuses/edit/",

        async (values: FormValues) => {
            const query: ApiCreateCaseStatus = {
                CaseType: values.caseType,
                StatusName: values.statusName,
                StatusSequence: values.statusSequence || 0,
                StatusCategory: values.statusCategory,
                ReportCategory: values.reportCategory || null,
                StatusDescription: values.statusDescription || null,
            };
            const result = await shiftClient.caseStatus.create(query);
            _searchCache.clearRows('search.caseStatus');
            return result.StatusIdentifier;
        }
    )

    return (
        <form autoComplete="off" onSubmit={handleSubmit}>
            <ValidationSummary errors={errors} />

            <FormCard>
                <FormField
                    label={translate("Case Type")}
                    description={translate("The type of case this status applies to.")}
                    required
                >
                    <TextBox
                        {...register("caseType", {
                            required: true
                        })}
                        autoFocus
                        readOnly={isSaving}
                        maxLength={50}
                        className="w-50"
                        error={errors.caseType}
                    />
                </FormField>

                <FormField
                    label={translate("Status Name")}
                    description={translate("The name of this status.")}
                    required
                >
                    <TextBox
                        {...register("statusName", {
                            required: true
                        })}
                        readOnly={isSaving}
                        maxLength={50}
                        className="w-50"
                        error={errors.statusName}
                    />
                </FormField>

                <FormField
                    label={translate("Status Sequence")}
                    description={translate("The order in which this status appears.")}
                    required
                >
                    <TextBox
                        {...register("statusSequence", {
                            required: true,
                            valueAsNumber: true
                        })}
                        type="number"
                        readOnly={isSaving}
                        className="w-25"
                        error={errors.statusSequence}
                    />
                </FormField>

                <FormField
                    label={translate("Status Category")}
                    description={translate("The category this status belongs to.")}
                    required
                >
                    <CaseStatusCategoryComboBox
                        {...register("statusCategory", {
                            required: true
                        })}
                        control={control}
                        disabled={isSaving}
                        widthClassName="w-50"
                    />
                </FormField>

                <FormField
                    label={translate("Report Category")}
                    description={translate("The category used for reporting purposes.")}
                >
                    <TextBox
                        {...register("reportCategory")}
                        readOnly={isSaving}
                        maxLength={10}
                        className="w-25"
                        error={errors.reportCategory}
                    />
                </FormField>

                <FormField
                    label={translate("Status Description")}
                    description={translate("A detailed description of this status.")}
                >
                    <TextArea
                        {...register("statusDescription")}
                        readOnly={isSaving}
                        maxLength={200}
                        className="w-50"
                        rows={3}
                        error={errors.statusDescription}
                    />
                </FormField>

                <FormField hasBottomMargin={false}>
                    <Button
                        variant="save"
                        className="me-2"
                        isLoading={isSaving}
                    />
                    <Button variant="cancel" href={backUrl} />
                </FormField>
            </FormCard>
        </form>
    );
}