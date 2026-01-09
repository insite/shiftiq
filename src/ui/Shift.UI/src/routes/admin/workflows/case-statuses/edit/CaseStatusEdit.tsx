import { ApiUpdateCaseStatus } from "@/api/controllers/caseStatus/ApiUpdateCaseStatus";
import { shiftClient } from "@/api/shiftClient";
import Button from "@/components/Button";
import FormCard from "@/components/form/FormCard";
import FormField from "@/components/form/FormField";
import TextBox from "@/components/TextBox";
import TextArea from "@/components/TextArea";
import ValidationSummary from "@/components/ValidationSummary";
import { translate } from "@/helpers/translate";
import { useEditForm } from "@/hooks/useEditForm";
import CaseStatusCategoryComboBox from "@/routes/_shared/comboboxes/CaseStatusCategoryComboBox";
import { useParams } from "react-router";
import { _searchCache } from "@/cache/_searchCache";

interface FormValues {
    statusName: string;
    statusSequence: number;
    statusCategory: string;
    reportCategory: string;
    statusDescription: string;
}

export default function CaseStatusEdit() {
    const params = useParams();
    const statusId = params["id"];

    const {
        backUrl,
        isLoaded,
        isSaving,
        isDisabled,
        control,
        register,
        errors,
        handleSubmit,
    } = useEditForm(
        async (id: string) => {
            const model = await shiftClient.caseStatus.retrieve(id);
            return {
                statusName: model.StatusName,
                statusSequence: model.StatusSequence,
                statusCategory: model.StatusCategory,
                reportCategory: model.ReportCategory || "",
                statusDescription: model.StatusDescription || "",
            }
        },
        async (id: string, values: FormValues) => {
            const command: ApiUpdateCaseStatus = {
                StatusName: values.statusName,
                StatusSequence: values.statusSequence,
                StatusCategory: values.statusCategory,
                ReportCategory: values.reportCategory || null,
                StatusDescription: values.statusDescription || null,
            };
            await shiftClient.caseStatus.update(id, command);
            _searchCache.clearRows('search.caseStatus');
        }
    );

    return (
        <form autoComplete="off" onSubmit={handleSubmit}>
            <ValidationSummary errors={errors} />

            <FormCard>
                <FormField
                    label={translate("Status Name")}
                    description={translate("The name of this status.")}
                    required
                >
                    <TextBox
                        {...register("statusName", {
                            required: true
                        })}
                        autoFocus
                        readOnly={isDisabled}
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
                        readOnly={isDisabled}
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
                        readOnly={isDisabled}
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
                        readOnly={isDisabled}
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
                        disabled={!isLoaded}
                        isLoading={isSaving}
                    />
                    <Button variant="cancel" href={backUrl} />
                    <Button
                        variant="delete" 
                        href={`/client/admin/workflows/case-statuses/delete/${statusId}`}
                        className="float-end"
                        disabled={isDisabled}
                    />
                </FormField>
            </FormCard>
        </form>
    );
}