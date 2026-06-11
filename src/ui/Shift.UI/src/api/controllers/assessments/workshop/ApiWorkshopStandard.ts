export interface ApiWorkshopStandard {
    StandardId: string;
    ParentId: string | null | undefined;
    AssetNumber: number;
    Sequence: number;
    Code: string;
    Label: string;
    Title: string;

    Parent: ApiWorkshopStandard | null | undefined;
}