import { useSpecWorkshopProvider } from "@/contexts/workshop/SpecWorkshopProviderContext";
import { numberHelper } from "@/helpers/numberHelper";
import { Control } from "react-hook-form";
import { SpecWorkshopDetailsCalculations } from "./SpecWorkshopDetailsCalculations";
import WorkshopStandardDisplay from "../other/WorkshopStandardDisplay";
import ControlledIntegerTextBox from "@/components/textbox/ControlledIntegerTextBox";
import { Fragment } from "react";
import { SpecWorkshopDetailsValues } from "./SpecWorkshopDetailsValues";

interface Props {
    control: Control<SpecWorkshopDetailsValues>;
    disabled: boolean;
    calculations: SpecWorkshopDetailsCalculations;
}

export default function SpecWorkshop_Details_Competencies({
    control,
    disabled,
    calculations
}: Props) {
    const {
        details,
        modifyTax1Count,
        modifyTax2Count,
        modifyTax3Count,
    } = useSpecWorkshopProvider();

    function handleTax1CountChange(criterionId: string, standardId: string, tax1Count: number | null) {
        if (tax1Count !== null && tax1Count < 0) {
            tax1Count = 0;
        }
        modifyTax1Count(criterionId, standardId, tax1Count);
        return tax1Count;
    }

    function handleTax2CountChange(criterionId: string, standardId: string, tax2Count: number | null) {
        if (tax2Count !== null && tax2Count < 0) {
            tax2Count = 0;
        }
        modifyTax2Count(criterionId, standardId, tax2Count);
        return tax2Count;
    }

    function handleTax3CountChange(criterionId: string, standardId: string, tax3Count: number | null) {
        if (tax3Count !== null && tax3Count < 0) {
            tax3Count = 0;
        }
        modifyTax3Count(criterionId, standardId, tax3Count);
        return tax3Count;
    }

    return (
        <table className="table table-sm table-competency">
            <thead>
                <tr>
                    <th>Competency</th>
                    <th className="text-end text-nowrap">GAC #</th>
                    <th className="text-end text-nowrap">Competency</th>
                    <th className="text-end text-nowrap">T1</th>
                    <th className="text-end text-nowrap">T2</th>
                    <th className="text-end text-nowrap">T3</th>
                </tr>
            </thead>
            <tbody>
                {details && details.criteria.map((criterion, criterionIndex) => {
                    const criterionCalculation = calculations.criteria[criterionIndex];

                    return (
                        <Fragment key={criterion.criterionId}>
                            <tr className="fw-bold">
                                <td>{criterion.title}</td>
                                <td className="text-end cell-criterion-total">
                                    {formatNumber(criterionCalculation.plannedQuestionCount)}
                                </td>
                                <td className={`text-end cell-competency-total ${criterionCalculation.hasMismatch ? "table-danger" : ""}`}>
                                    {formatNumber(criterionCalculation.competencyCount)}
                                </td>
                                <td colSpan={3}></td>
                            </tr>

                            {criterion.competencies.map((competency, competencyIndex) => (
                                <tr key={competency.standard.standardId}>
                                    <td>
                                        <WorkshopStandardDisplay standard={competency.standard} />
                                    </td>
                                    <td className="text-end align-middle cell-criterion"></td>
                                    <td className="text-end align-middle cell-competency">
                                        {formatNumber(criterionCalculation.competencies[competencyIndex].totalCount)}
                                    </td>
                                    <td className="text-end align-middle cell-t1">
                                        <ControlledIntegerTextBox
                                            control={control}
                                            name={`criteria.${criterionIndex}.competencies.${competencyIndex}.tax1Count`}
                                            readOnly={disabled}
                                            className="spec-workshop-input-sm text-end form-control-sm d-inline px-2"
                                            onChange={value => handleTax1CountChange(criterion.criterionId, competency.standard.standardId, value)}
                                        />
                                    </td>
                                    <td className="text-end align-middle cell-t2">
                                        <ControlledIntegerTextBox
                                            control={control}
                                            name={`criteria.${criterionIndex}.competencies.${competencyIndex}.tax2Count`}
                                            readOnly={disabled}
                                            className="spec-workshop-input-sm text-end form-control-sm d-inline px-2"
                                            onChange={value => handleTax2CountChange(criterion.criterionId, competency.standard.standardId, value)}
                                        />
                                    </td>
                                    <td className="text-end align-middle cell-t3">
                                        <ControlledIntegerTextBox
                                            control={control}
                                            name={`criteria.${criterionIndex}.competencies.${competencyIndex}.tax3Count`}
                                            readOnly={disabled}
                                            className="spec-workshop-input-sm text-end form-control-sm d-inline px-2"
                                            onChange={value => handleTax3CountChange(criterion.criterionId, competency.standard.standardId, value)}
                                        />
                                    </td>
                                </tr>
                            ))}
                        </Fragment>
                    );
                })}
            </tbody>
            <tfoot>
                <tr>
                    <th></th>
                    <th className="text-end cell-criterion">
                        {formatNumber(calculations.totalQuestionCount)}
                    </th>
                    <th className="text-end cell-competency">
                        {formatNumber(calculations.totalCompetencyCount)}
                    </th>
                    <th className="text-end cell-t1"></th>
                    <th className="text-end cell-t2"></th>
                    <th className="text-end cell-t3"></th>
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