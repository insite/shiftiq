import "./SpecWorkshop_Details.css";

import { useSpecWorkshopProvider } from "@/contexts/workshop/SpecWorkshopProviderContext";
import { Control } from "react-hook-form";
import { useMemo } from "react";
import { getSpecWorkshopDetailsCalculations } from "./SpecWorkshopDetailsCalculations";
import FormField from "@/components/form/FormField";
import WorkshopStandardDisplay from "../other/WorkshopStandardDisplay";
import ControlledIntegerTextBox from "@/components/textbox/ControlledIntegerTextBox";
import SpecWorkshop_Details_Criteria from "./SpecWorkshop_Details_Criteria";
import SpecWorkshop_Details_Competencies from "./SpecWorkshop_Details_Competencies";
import { SpecWorkshopDetailsValues } from "./SpecWorkshopDetailsValues";
import FormCard from "@/components/form/FormCard";

interface Props {
    control: Control<SpecWorkshopDetailsValues>;
    isSaving: boolean;
}

export default function SpecWorkshop_Details({ control, isSaving }: Props) {
    const {
        details,
        readOnly,
        modifyFormLimit,
        modifyQuestionLimit,
    } = useSpecWorkshopProvider();

    const calculations = useMemo(() => {
        return details ? getSpecWorkshopDetailsCalculations(details) : null;
    }, [details]);

    if (!details || !calculations) {
        return null;
    }

    const disabled = readOnly || isSaving;

    function handleFormLimitChange(value: number | null) {
        if (value !== null && value >= 0) {
            modifyFormLimit(value);
        }
    }

    function handleFormQuestionChange(value: number | null) {
        if (value !== null && value >= 0) {
            modifyQuestionLimit(value);
        }
    }

    return (
        <FormCard>
            <div className="row">
                <div className="col-lg-4">
                    <FormField label="Framework">
                        <WorkshopStandardDisplay standard={details.frameworkStandard} />
                    </FormField>
                </div>

                <div className="col-lg-4">
                    <FormField label="Required Number of Forms" required>
                        <ControlledIntegerTextBox
                            control={control}
                            name="formLimit"
                            readOnly={disabled}
                            required="Required Number of Forms"
                            validate={value => value === null || value < 0 ? "Required Number of Forms: Must be 0 or greater" : undefined}
                            onChange={handleFormLimitChange}
                        />
                    </FormField>
                </div>

                <div className="col-lg-4">
                    <FormField label="Required Number of Questions per Form" required>
                        <ControlledIntegerTextBox
                            control={control}
                            name="questionLimit"
                            readOnly={disabled}
                            required="Required Number of Questions per Form"
                            validate={value => value !== null && value < 0 ? "Required Number of Questions per Form: Must be 0 or greater" : undefined}
                            onChange={handleFormQuestionChange}
                        />
                    </FormField>
                </div>
            </div>

            <div className="row">
                <div className="col-xxl-6 mb-3 mb-xxl-0">
                    <SpecWorkshop_Details_Criteria
                        control={control}
                        calculations={calculations}
                        disabled={disabled}
                    />
                </div>

                <div className="col-xxl-6">
                    <SpecWorkshop_Details_Competencies
                        control={control}
                        calculations={calculations}
                        disabled={disabled}
                    />
                </div>
            </div>
        </FormCard>
    );
}