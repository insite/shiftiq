import { expect, test } from "vitest";
import { SpecWorkshopDetails } from "@/contexts/workshop/models/SpecWorkshopDetails";
import { WorkshopStandard } from "@/contexts/workshop/models/WorkshopStandard";
import { formatBankViewNumber, getSpecWorkshopBankViewCalculations } from "../SpecWorkshopBankViewCalculations";

test("getSpecWorkshopBankViewCalculations computes planned counts, row states, and totals", () => {
    const result = getSpecWorkshopBankViewCalculations(createDetails());

    expect(result.criteria[0].plannedCriterion).toEqual(10);
    expect(result.criteria[0].competencies[0].plannedCompetency).toEqual(6);
    expect(result.criteria[0].competencies[0].variance).toEqual(-6);
    expect(result.criteria[0].competencies[0].varianceState).toEqual("negative");
    expect(result.criteria[0].competencies[1].variance).toEqual(3);
    expect(result.criteria[0].competencies[1].varianceState).toEqual("positive");
    expect(result.criteria[1].competencies[0].variance).toEqual(0);
    expect(result.criteria[1].competencies[0].varianceState).toEqual("none");

    expect(result.totals.plannedCriterion).toEqual(20);
    expect(result.totals.plannedCompetency).toEqual(18);
    expect(result.totals.totalActual).toEqual(15);
    expect(result.totals.variance).toEqual(-3);
    expect(result.totals.completedPercent).toEqual(83);
    expect(result.totals.t1Planned).toEqual(8);
    expect(result.totals.t1Actual).toEqual(2);
    expect(result.totals.t2Planned).toEqual(6);
    expect(result.totals.t2Actual).toEqual(2);
    expect(result.totals.t3Planned).toEqual(4);
    expect(result.totals.t3Actual).toEqual(11);
    expect(result.totals.unassigned).toEqual(2);
});

test("getSpecWorkshopBankViewCalculations treats null actual totals and unassigned counts as zero in totals", () => {
    const result = getSpecWorkshopBankViewCalculations(createDetails());

    expect(result.criteria[0].competencies[0].totalActual).toBeNull();
    expect(result.criteria[0].competencies[0].unassigned).toBeNull();
    expect(result.totals.totalActual).toEqual(15);
    expect(result.totals.unassigned).toEqual(2);
});

test("formatBankViewNumber shows N/A for non-finite completed values", () => {
    const result = getSpecWorkshopBankViewCalculations(createZeroPlannedDetails());

    expect(result.totals.plannedCompetency).toEqual(0);
    expect(formatBankViewNumber(result.totals.completedPercent, "%")).toEqual("None");
});

function createDetails(): SpecWorkshopDetails {
    const area = createStandard("area-1", "A1", "Area 1");
    const competency1 = createStandard("competency-1", "C1", "Competency 1", area);
    const competency2 = createStandard("competency-2", "C2", "Competency 2", area);
    const competency3 = createStandard("competency-3", "C3", "Competency 3", area);

    return {
        specName: "Workshop Spec",
        assetNumber: 10,
        frameworkStandard: null,
        formLimit: 2,
        questionLimit: 10,
        criteria: [{
            criterionId: "criterion-1",
            title: "Criterion 1",
            weight: 5000,
            areaStandards: [area],
            competencies: [{
                standard: competency1,
                tax1Count: 1,
                tax2Count: 2,
                tax3Count: 0,
                questionCount: null,
                tax1CountActual: 0,
                tax2CountActual: 0,
                tax3CountActual: 0,
                unassignedCount: null,
            }, {
                standard: competency2,
                tax1Count: 1,
                tax2Count: 0,
                tax3Count: 1,
                questionCount: 7,
                tax1CountActual: 1,
                tax2CountActual: 0,
                tax3CountActual: 6,
                unassignedCount: null,
            }],
        }, {
            criterionId: "criterion-2",
            title: "Criterion 2",
            weight: 5000,
            areaStandards: [area],
            competencies: [{
                standard: competency3,
                tax1Count: 2,
                tax2Count: 1,
                tax3Count: 1,
                questionCount: 8,
                tax1CountActual: 1,
                tax2CountActual: 2,
                tax3CountActual: 5,
                unassignedCount: 2,
            }],
        }],
    };
}

function createZeroPlannedDetails(): SpecWorkshopDetails {
    const area = createStandard("area-1", "A1", "Area 1");
    const competency = createStandard("competency-1", "C1", "Competency 1", area);

    return {
        specName: "Workshop Spec",
        assetNumber: 10,
        frameworkStandard: null,
        formLimit: 0,
        questionLimit: 0,
        criteria: [{
            criterionId: "criterion-1",
            title: "Criterion 1",
            weight: 0,
            areaStandards: [area],
            competencies: [{
                standard: competency,
                tax1Count: null,
                tax2Count: null,
                tax3Count: null,
                questionCount: null,
                tax1CountActual: 0,
                tax2CountActual: 0,
                tax3CountActual: 0,
                unassignedCount: null,
            }],
        }],
    };
}

function createStandard(
    standardId: string,
    code: string,
    title: string,
    parent: WorkshopStandard | null = null,
): WorkshopStandard {
    return {
        standardId,
        assetNumber: 1,
        sequence: 1,
        code,
        label: "Standard",
        title,
        parent,
    };
}
