import ControlledIntegerTextBox from "@/components/textbox/ControlledIntegerTextBox";
import { numberHelper } from "@/helpers/numberHelper";
import WorkshopStandardDisplay from "../other/WorkshopStandardDisplay";
import { criterionPercentToWeight, useSpecWorkshopProvider } from "@/contexts/workshop/SpecWorkshopProviderContext";
import { Control } from "react-hook-form";
import { SpecWorkshopDetailsCalculations } from "./SpecWorkshopDetailsCalculations";
import { textHelper } from "@/helpers/textHelper";
import { SpecWorkshopDetailsValues } from "./SpecWorkshopDetailsValues";

interface Props {
    control: Control<SpecWorkshopDetailsValues>;
    disabled: boolean;
    calculations: SpecWorkshopDetailsCalculations;
}

export default function SpecWorkshop_Details_Criteria({
    control,
    disabled,
    calculations
}: Props)
{
    const {
        details,
        modifyCriterionWeight,
    } = useSpecWorkshopProvider();

    function handleWeightChange(criterionId: string, weightPercent: number | null) {
        if (weightPercent === null || weightPercent < 0) {
            weightPercent = 0;
        } else if (weightPercent > 100) {
            weightPercent = 100;
        }
        modifyCriterionWeight(criterionId, criterionPercentToWeight(weightPercent ?? 0));
        return weightPercent;
    }

    return (
        <table className="table table-sm table-criterion">
            <thead>
                <tr>
                    <th>Criterion</th>
                    <th>GAC</th>
                    <th className="text-end text-nowrap" title="Set Weight">GAC %</th>
                    <th className="text-end text-nowrap" title="Required Number of Questions">GAC #</th>
                </tr>
            </thead>
            <tbody>
                {details && details.criteria.map((criterion, criterionIndex) => (
                    <tr key={criterion.criterionId}>
                        <td>{criterion.title}</td>
                        <td>
                            {criterion.areaStandards.length > 0 ? criterion.areaStandards.map(standard => (
                                <div key={standard.standardId}>
                                    <WorkshopStandardDisplay standard={standard} />
                                </div>
                            )) : textHelper.none()}
                        </td>
                        <td className="text-end align-middle cell-weight">
                            <ControlledIntegerTextBox
                                control={control}
                                name={`criteria.${criterionIndex}.weightPercent`}
                                readOnly={disabled}
                                className="spec-workshop-input-sm text-end form-control-sm d-inline px-2"
                                onChange={value => handleWeightChange(criterion.criterionId, value)}
                            />
                        </td>
                        <td className="text-end align-middle cell-criterion">
                            {formatNumber(calculations.criteria[criterionIndex].plannedQuestionCount)}
                        </td>
                    </tr>
                ))}
            </tbody>
            <tfoot>
                <tr>
                    <th colSpan={2}></th>
                    <th className={`text-end cell-weight ${calculations.totalWeight === 100 ? "" : "table-danger"}`}>
                        {formatNumber(calculations.totalWeight)}
                    </th>
                    <th className="text-end cell-criterion">
                        {formatNumber(calculations.totalQuestionCount)}
                    </th>
                </tr>
            </tfoot>
        </table>
    );
}

function formatNumber(value: number | null | undefined): string {
    return value !== null && value !== undefined
        ? numberHelper.formatInt(value)
        : "";
}