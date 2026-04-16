import { expect, test } from "vitest";
import { WorkshopStandard } from "@/contexts/workshop/models/WorkshopStandard";
import { SpecWorkshopDetails } from "@/contexts/workshop/models/SpecWorkshopDetails";
import { specWorkshopAdapter } from "../specWorkshopAdapter";
import { getSpecWorkshopDetailsCalculations } from "../SpecWorkshopDetailsCalculations";
import { toApiSpecWorkshopInput, toSpecWorkshopDetailsValues } from "../SpecWorkshopDetailsValues";

test("specWorkshopAdapter converts API weight into legacy percent values", () => {
    const area = createStandard("area-1", "A1", "Area 1");
    const competency = createStandard("competency-1", "C1", "Competency 1", area);

    const result = specWorkshopAdapter.getState(
        "spec-1",
        {
            BankId: "bank-1",
            Standards: [],
            QuestionData: {
                Sections: [],
                Taxonomies: [],
                TotalQuestionCount: 0,
                FirstSectionId: "",
                FirstSectionAreaId: null,
                FirstSectionStandards: null,
                FirstSectionQuestions: [],
            },
            Comments: [],
            Attachments: [],
            ProblemQuestions: [],
            Details: {
                SpecName: "Workshop Spec",
                AssetNumber: 10,
                FrameworkId: null,
                FormLimit: 2,
                QuestionLimit: 25,
                Criteria: [{
                    CriterionId: "criterion-1",
                    Title: "Criterion 1",
                    Weight: 4000,
                    StandardIds: [area.standardId],
                    Competencies: [{
                        StandardId: competency.standardId,
                        Tax1Count: 1,
                        Tax2Count: 2,
                        Tax3Count: 3,
                        QuestionCount: 6,
                        Tax1CountActual: 0,
                        Tax2CountActual: 0,
                        Tax3CountActual: 0,
                        UnassignedCount: null,
                    }],
                }],
            },
        },
        null,
        [area],
        [competency],
    );

    expect(result.details!.criteria[0].weight).toEqual(4000);
});

test("buildSpecWorkshopInput converts legacy percent values back to API weight scale", () => {
    const details = createDetails();
    const values = toSpecWorkshopDetailsValues(details);

    values.formLimit = 3;
    values.questionLimit = 20;
    values.criteria[0].weightPercent = 40;
    values.criteria[0].competencies[0].tax1Count = 5;
    values.criteria[0].competencies[0].tax2Count = null;
    values.criteria[0].competencies[0].tax3Count = 1;

    const result = toApiSpecWorkshopInput(details, values);

    expect(result.FormLimit).toEqual(3);
    expect(result.QuestionLimit).toEqual(20);
    expect(result.Criteria[0].Weight).toEqual(4000);
    expect(result.Criteria[0].Competencies[0]).toEqual({
        StandardId: "competency-1",
        Tax1Count: 5,
        Tax2Count: null,
        Tax3Count: 1,
    });
});

test("getSpecWorkshopCalculations computes legacy table totals and mismatch state", () => {
    const details = createDetails();

    const result = getSpecWorkshopDetailsCalculations(details);

    expect(result.totalWeight).toEqual(100);
    expect(result.totalQuestionCount).toEqual(25);
    expect(result.totalCompetencyCount).toEqual(24);
    expect(result.criteria[0].plannedQuestionCount).toEqual(10);
    expect(result.criteria[0].competencyCount).toEqual(10);
    expect(result.criteria[0].hasMismatch).toEqual(false);
    expect(result.criteria[1].plannedQuestionCount).toEqual(15);
    expect(result.criteria[1].competencyCount).toEqual(14);
    expect(result.criteria[1].hasMismatch).toEqual(true);
});

function createDetails(): SpecWorkshopDetails {
    const area = createStandard("area-1", "A1", "Area 1");
    const competency = createStandard("competency-1", "C1", "Competency 1", area);
    const competency2 = createStandard("competency-2", "C2", "Competency 2", area);

    return {
        specName: "Workshop Spec",
        assetNumber: 10,
        frameworkStandard: null,
        formLimit: 2,
        questionLimit: 25,
        criteria: [{
            criterionId: "criterion-1",
            title: "Criterion 1",
            weight: 4000,
            areaStandards: [area],
            competencies: [{
                standard: competency,
                tax1Count: 4,
                tax2Count: 3,
                tax3Count: 3,
                questionCount: 10,
                tax1CountActual: 0,
                tax2CountActual: 0,
                tax3CountActual: 0,
                unassignedCount: null,
            }],
        }, {
            criterionId: "criterion-2",
            title: "Criterion 2",
            weight: 6000,
            areaStandards: [area],
            competencies: [{
                standard: competency2,
                tax1Count: 5,
                tax2Count: 5,
                tax3Count: 4,
                questionCount: 14,
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
