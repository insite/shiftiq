import FormCard from "@/components/form/FormCard";
import FormTab from "@/components/form/FormTab";
import FormTabs from "@/components/form/FormTabs";
import { translate } from "@/helpers/translate";
import FormWorkshop_FormDetails_Form from "./FormWorkshop_FormDetails_Form";
import FormWorkshop_FormDetails_Summaries from "./FormWorkshop_FormDetails_Summaries";

export default function FormWorkshop_FormDetails() {
    return (
        <FormCard>
            <FormTabs defaultTab="form">
                <FormTab tab="form" title={translate("Form")}>
                    <FormWorkshop_FormDetails_Form />
                </FormTab>
                <FormTab tab="questions" title={translate("Summaries")}>
                    <FormWorkshop_FormDetails_Summaries />
                </FormTab>
            </FormTabs>
        </FormCard>
    );
}