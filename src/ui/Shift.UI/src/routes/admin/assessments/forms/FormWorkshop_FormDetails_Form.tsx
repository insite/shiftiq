import FormField from "@/components/form/FormField";
import { translate } from "@/helpers/translate";
import TextOrNone from "@/components/TextOrNone";
import { useFormWorkshopProvider } from "@/contexts/workshop/FormWorkshopProviderContext";
import FormWorkshop_FormDetails_Questions from "./FormWorkshop_FormDetails_Questions";
import { useSaveAction } from "@/hooks/useSaveAction";
import { shiftClient } from "@/api/shiftClient";
import WorkshopStandardDisplay from "../other/WorkshopStandardDisplay";

export default function FormWorkshop_FormDetails_Form() {
    const { formId, details, modifyThirdPartyAssessment } = useFormWorkshopProvider();

    const { isSaving, runSave } = useSaveAction();

    async function handleThirdPartyAssessmentClick() {
        if (!details) {
            return;
        }

        const newThirdPartyAssessmentEnabled = !details.thirdPartyAssessmentEnabled;

        modifyThirdPartyAssessment(newThirdPartyAssessmentEnabled);

        if (!await runSave(() => shiftClient.workshop.modifyThirdPartyAssessment(formId, newThirdPartyAssessmentEnabled))) {
            modifyThirdPartyAssessment(!newThirdPartyAssessmentEnabled);
        }
    }

    return (
        <div className="row">
            <div className="col-lg-6">
                <h3>{translate("Identification")}</h3>

                <FormField label={translate("Specification")} description={translate("The specification that contains and drives the form.")}>
                    <TextOrNone>{details?.specificationName}</TextOrNone>
                </FormField>

                <FormField label={translate("Standard")} description={translate("The standard evaluated by questions on the form.")}>
                    {details?.standard?.parent ? (
                        <>
                            <WorkshopStandardDisplay standard={details.standard.parent} />
                            <div className="mt-2 ms-3">
                                <WorkshopStandardDisplay standard={details.standard} />
                            </div>
                        </>
                    ) : (
                        <WorkshopStandardDisplay standard={details?.standard} />
                    )}
                </FormField>

                <FormField label={translate("Form Name")} description={translate("The internal name used to uniquely identify the form for filing purposes.")}>
                    <TextOrNone>{details?.formName}</TextOrNone>
                </FormField>

                <FormField label={translate("Asset Number and Version")} description={translate("The inventory asset number (and version) for this form.")}>
                    <TextOrNone>{details ? `${details.formAssetNumber}.${details.formAssetVersion}` : null}</TextOrNone>
                </FormField>

                <FormField label={translate("Code")} description={translate("Alpha numeric catalog reference code for the form (required for Exam event scheduling).")}>
                    <TextOrNone>{details?.formCode}</TextOrNone>
                </FormField>

                <FormField label={translate("Source")} description={translate("Reference to the source of the content and/or configuration for this form.")}>
                    <TextOrNone>{details?.formSource}</TextOrNone>
                </FormField>

                <FormField label={translate("Origin")} description={translate("Identifies the originating platform and/or record for this form. When this property is used, it should ideally contain a fully qualified URL or API path.")}>
                    <TextOrNone>{details?.formOrigin}</TextOrNone>
                </FormField>

                <FormField label={translate("Hook / Integration Code")} description={translate("Unique code for integration with internal toolkits and external systems.")}>
                    <TextOrNone>{details?.formHook}</TextOrNone>
                </FormField>
            </div>
            <div className="col-lg-6">
                <h3>{translate("Publication")}</h3>

                <FormField label={translate("Publication Status")} description={translate("The process to publish assessment forms has 3 steps. This is the current step in the process.")}>
                    <TextOrNone>{details?.publicationStatus}</TextOrNone>
                </FormField>

                <FormField
                    label={translate("Third-Party Assessment")}
                    description={translate("Allow a third party assessor to use this form in the evaluation of a learner.")}
                    editIconStyle="solid"
                    editIcon={details?.thirdPartyAssessmentEnabled ? "toggle-on" : "toggle-off"}
                    editTitle={details?.thirdPartyAssessmentEnabled ? translate("Disable third-party assessment") : translate("Enable third-party assessment")}
                    editDisabled={!details || isSaving}
                    onEditClick={handleThirdPartyAssessmentClick}
                >
                    <TextOrNone>{details ? details.thirdPartyAssessmentEnabled ? translate("Enabled") : translate("Disabled") : null}</TextOrNone>
                </FormField>

                <FormWorkshop_FormDetails_Questions />
            </div>
        </div>
    );
}