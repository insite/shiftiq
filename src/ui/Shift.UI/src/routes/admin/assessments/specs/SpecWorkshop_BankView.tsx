import "./SpecWorkshop_BankView.css";

import { useEffect, useMemo, useRef } from "react";
import { useSpecWorkshopProvider } from "@/contexts/workshop/SpecWorkshopProviderContext";
import { formatBankViewNumber, getSpecWorkshopBankViewCalculations } from "./SpecWorkshopBankViewCalculations";
import FormCard from "@/components/form/FormCard";
import SpecWorkshop_BankView_Criterion from "./SpecWorkshop_BankView_Criterion";

export default function SpecWorkshop_BankView() {
    const { details } = useSpecWorkshopProvider();
    const tableRef = useRef<HTMLTableElement | null>(null);

    const calculations = useMemo(() => {
        return details ? getSpecWorkshopBankViewCalculations(details) : null;
    }, [details]);

    useEffect(() => {
        const table = tableRef.current;

        if (!table) {
            return;
        }

        const currentTable = table;

        window.addEventListener("scroll", updateHeaders);
        window.addEventListener("resize", updateHeaders);
        window.addEventListener("click", updateHeaders);

        updateHeaders();

        return () => {
            window.removeEventListener("scroll", updateHeaders);
            window.removeEventListener("resize", updateHeaders);
            window.removeEventListener("click", updateHeaders);
            resetHeaders();
        };

        function resetHeaders() {
            const headers = currentTable.querySelectorAll("thead > tr > th");
            headers.forEach(header => {
                (header as HTMLTableCellElement).style.transform = "";
            });
        }

        function updateHeaders() {
            if (!isVisible(currentTable)) {
                resetHeaders();
                return;
            }

            const headers = Array.from(currentTable.querySelectorAll("thead > tr > th")) as HTMLTableCellElement[];
            const lastRow = currentTable.querySelector("tbody > tr:last-child") as HTMLTableRowElement | null;

            if (headers.length === 0 || !lastRow) {
                resetHeaders();
                return;
            }

            const scrollTop = window.scrollY;
            const navbar = document.querySelector("header.navbar") as HTMLElement | null;
            const headerHeight = navbar ? navbar.offsetHeight - 1 : 0;

            let top = scrollTop - getOffsetTop(currentTable) + headerHeight;

            if (top > 0) {
                const bottom = scrollTop - getOffsetTop(lastRow) + headerHeight + headers[0].offsetHeight;
                if (bottom > 0) {
                    top -= bottom;
                }
            }

            if (top <= 0) {
                resetHeaders();
                return;
            }

            const transform = `translateY(${String(top)}px)`;
            headers.forEach(header => {
                header.style.transform = transform;
            });
        }
    }, [details]);

    if (!details || !calculations) {
        return null;
    }

    return (
        <FormCard>
            <table ref={tableRef} className="table table-sm table-bankview">
                <thead>
                    <tr>
                        <th className="align-bottom">Competency</th>
                        <th className="text-center text-nowrap align-bottom">Planned<br />GAC</th>
                        <th className="text-center text-nowrap align-bottom">Planned<br />Competency</th>
                        <th className="text-center text-nowrap align-bottom">Total<br />Actual</th>
                        <th className="text-center text-nowrap align-bottom">Variance</th>
                        <th className="text-center text-nowrap align-bottom">T1<br />Planned</th>
                        <th className="text-center text-nowrap align-bottom">T1<br />Actual</th>
                        <th className="text-center text-nowrap align-bottom">T2<br />Planned</th>
                        <th className="text-center text-nowrap align-bottom">T2<br />Actual</th>
                        <th className="text-center text-nowrap align-bottom">T3<br />Planned</th>
                        <th className="text-center text-nowrap align-bottom">T3<br />Actual</th>
                        <th className="text-center text-nowrap align-bottom">Unassigned</th>
                    </tr>
                </thead>
                <tbody>
                    {calculations.criteria.map(criterion => (
                        <SpecWorkshop_BankView_Criterion
                            key={criterion.criterionId}
                            criterion={criterion}
                        />
                    ))}
                </tbody>
                <tfoot>
                    <tr>
                        <th></th>
                        <th className="text-center cell-planned-criterion">{formatBankViewNumber(calculations.totals.plannedCriterion)}</th>
                        <th className="text-center cell-planned-competency">{formatBankViewNumber(calculations.totals.plannedCompetency)}</th>
                        <th className="text-center cell-total-actual">{formatBankViewNumber(calculations.totals.totalActual)}</th>
                        <th className={getVarianceClassName("text-center cell-variance", calculations.totals.varianceState)}>
                            {formatBankViewNumber(calculations.totals.variance)}
                        </th>
                        <th className="text-center cell-t1-planned">{formatBankViewNumber(calculations.totals.t1Planned)}</th>
                        <th className="text-center cell-t1-actual">{formatBankViewNumber(calculations.totals.t1Actual)}</th>
                        <th className="text-center cell-t2-planned">{formatBankViewNumber(calculations.totals.t2Planned)}</th>
                        <th className="text-center cell-t2-actual">{formatBankViewNumber(calculations.totals.t2Actual)}</th>
                        <th className="text-center cell-t3-planned">{formatBankViewNumber(calculations.totals.t3Planned)}</th>
                        <th className="text-center cell-t3-actual">{formatBankViewNumber(calculations.totals.t3Actual)}</th>
                        <th className="text-center cell-unassigned">{formatBankViewNumber(calculations.totals.unassigned)}</th>
                    </tr>
                    <tr>
                        <th colSpan={3}></th>
                        <th className="text-center cell-completed">{formatBankViewNumber(calculations.totals.completedPercent, "%")}</th>
                        <th colSpan={8}></th>
                    </tr>
                </tfoot>
            </table>
        </FormCard>
    );
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

function getOffsetTop(element: Element): number {
    return element.getBoundingClientRect().top + window.scrollY;
}

function isVisible(element: HTMLElement): boolean {
    return element.offsetWidth > 0 || element.offsetHeight > 0 || element.getClientRects().length > 0;
}