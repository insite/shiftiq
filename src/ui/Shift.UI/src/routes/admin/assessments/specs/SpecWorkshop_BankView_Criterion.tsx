import WorkshopStandardDisplay from "../other/WorkshopStandardDisplay";
import { formatBankViewNumber, getSpecWorkshopBankViewCalculations } from "./SpecWorkshopBankViewCalculations";

interface Props {
    criterion: ReturnType<typeof getSpecWorkshopBankViewCalculations>["criteria"][number];
}

export default function SpecWorkshop_BankView_Criterion({ criterion }: Props) {
    return (
        <>
            <tr className="fw-bold">
                <td>{criterion.title}</td>
                <td className="text-center cell-planned-criterion">{formatBankViewNumber(criterion.plannedCriterion)}</td>
                <td className="text-center cell-planned-competency"></td>
                <td className="text-center cell-total-actual"></td>
                <td className="text-center cell-variance"></td>
                <td className="text-center cell-t1-planned"></td>
                <td className="text-center cell-t1-actual"></td>
                <td className="text-center cell-t2-planned"></td>
                <td className="text-center cell-t2-actual"></td>
                <td className="text-center cell-t3-planned"></td>
                <td className="text-center cell-t3-actual"></td>
                <td className="text-center cell-unassigned"></td>
            </tr>

            {criterion.competencies.map(competency => (
                <tr key={competency.standard.standardId}>
                    <td>
                        <WorkshopStandardDisplay standard={competency.standard} />
                    </td>
                    <td className="text-center cell-planned-criterion"></td>
                    <td className="text-center cell-planned-competency">{formatBankViewNumber(competency.plannedCompetency)}</td>
                    <td className="text-center cell-total-actual">{formatBankViewNumber(competency.totalActual)}</td>
                    <td className={getVarianceClassName("text-center cell-variance", competency.varianceState)}>
                        {formatBankViewNumber(competency.variance)}
                    </td>
                    <td className="text-center cell-t1-planned">{formatBankViewNumber(competency.t1Planned)}</td>
                    <td className={getActualTaxonomyClassName("text-center cell-t1-actual", competency.isT1Shortfall)}>
                        {formatBankViewNumber(competency.t1Actual)}
                    </td>
                    <td className="text-center cell-t2-planned">{formatBankViewNumber(competency.t2Planned)}</td>
                    <td className={getActualTaxonomyClassName("text-center cell-t2-actual", competency.isT2Shortfall)}>
                        {formatBankViewNumber(competency.t2Actual)}
                    </td>
                    <td className="text-center cell-t3-planned">{formatBankViewNumber(competency.t3Planned)}</td>
                    <td className={getActualTaxonomyClassName("text-center cell-t3-actual", competency.isT3Shortfall)}>
                        {formatBankViewNumber(competency.t3Actual)}
                    </td>
                    <td className="text-center cell-unassigned">{formatBankViewNumber(competency.unassigned)}</td>
                </tr>
            ))}
        </>
    );
}

function getActualTaxonomyClassName(baseClassName: string, hasShortfall: boolean): string {
    return hasShortfall ? `${baseClassName} text-danger` : baseClassName;
}

function getVarianceClassName(baseClassName: string, state: "negative" | "positive" | "none"): string {
    if (state === "negative") {
        return `${baseClassName} table-danger`;
    }
    if (state === "positive") {
        return `${baseClassName} table-success`;
    }
    return baseClassName;
}