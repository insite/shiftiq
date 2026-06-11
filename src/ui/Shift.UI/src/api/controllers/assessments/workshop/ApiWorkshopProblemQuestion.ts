export interface ApiWorkshopProblemQuestion {
    QuestionId: number;
    QuestionBankIndex: number;
    QuestionAssetNumber: number;
    QuestionAssetVersion: number;
    QuestionSetName: string;
    QuestionTitle: string | null | undefined;
    CanDelete: boolean;
    ProblemDescription: string;
    Options: {
        Number: number;
        Title: string | null | undefined;
        Letter: string;
        Points: number;
    }[];
}