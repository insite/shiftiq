import { shiftClient } from "@/api/shiftClient";
import DeleteForm from "@/routes/_shared/forms/DeleteForm";
import FormField from "@/components/form/FormField";
import FormCard from "@/components/form/FormCard";
import { translate } from "@/helpers/translate";
import { CaseStatusDeleteModel } from "./CaseStatusDeleteModel";
import { _searchCache } from "@/cache/_searchCache";

export default function CaseStatusDelete() {
    async function handleDelete(id: string) {
        await shiftClient.caseStatus.delete(id);
        _searchCache.clearRows('search.caseStatus');
        return true;
    }

    return (
        <DeleteForm
            entityName="case status"
            onLoad={load}
            onDelete={handleDelete}
        >
            {model => (
                <FormCard hasShadow={false} title={translate("Case Status")}>
                    <FormField label={translate("Status Name")}>
                        <div className="form-control-plaintext">
                            {model?.statusName}
                        </div>
                    </FormField>

                    <FormField label={translate("Case Type")}>
                        <div className="form-control-plaintext">
                            {model?.caseType}
                        </div>
                    </FormField>

                    <FormField label={translate("Status Sequence")}>
                        <div className="form-control-plaintext">
                            {model?.statusSequence}
                        </div>
                    </FormField>

                    <FormField label={translate("Status Category")}>
                        <div className="form-control-plaintext">
                            {model?.statusCategory}
                        </div>
                    </FormField>

                    {model?.reportCategory && (
                        <FormField label={translate("Report Category")}>
                            <div className="form-control-plaintext">
                                {model.reportCategory}
                            </div>
                        </FormField>
                    )}

                    {model?.statusDescription && (
                        <FormField label={translate("Status Description")} hasBottomMargin={false}>
                            <div className="form-control-plaintext">
                                {model.statusDescription}
                            </div>
                        </FormField>
                    )}
                </FormCard>
            )}
        </DeleteForm>
    );
}

async function load(statusIdentifier: string): Promise<CaseStatusDeleteModel> {
    const apiModel = await shiftClient.caseStatus.retrieve(statusIdentifier);

    return {
        statusIdentifier,
        statusName: apiModel.StatusName,
        caseType: apiModel.CaseType,
        statusSequence: apiModel.StatusSequence,
        statusCategory: apiModel.StatusCategory,
        reportCategory: apiModel.ReportCategory ?? null,
        statusDescription: apiModel.StatusDescription ?? null,
        consequences: [
            { name: "Case Status", count: 1 },
        ]
    }
}