export interface WorkshopStandard {
    standardId: string;
    assetNumber: number;
    sequence: number;
    code: string;
    label: string;
    title: string;

    parent: WorkshopStandard | null;
}