interface InputCompetency
{
    StandardId: string;
    Tax1Count: number | null;
    Tax2Count: number | null;
    Tax3Count: number | null;
}

interface InputCriterion
{
    CriterionId: string;
    // Scaled decimal transport value: original weight * 10000.
    Weight: number;
    Competencies: InputCompetency[];
}

export interface ApiSpecWorkshopInput {
    FormLimit: number;
    QuestionLimit: number;
    Criteria: InputCriterion[];
}
