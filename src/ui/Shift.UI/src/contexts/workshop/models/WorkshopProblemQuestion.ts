export interface WorkshopProblemQuestion {
    questionId: number;
    questionBankIndex: number;
    questionAssetNumber: number;
    questionAssetVersion: number;
    questionSetName: string;
    questionTitle: string | null;
    canDelete: boolean;
    problemDescription: string;
    options: {
        number: number,
        title: string | null;
        letter: string;
        points: number;
    }[];
}