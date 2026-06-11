import { ApiContentModel } from "@/api/models/ApiContentModel";

export interface ApiPageContentModifyModel {
    Content: ApiContentModel | null;
    Blocks: {
        BlockId: string | null;
        BlockIdNumber: number | null;
        BlockType: string;
        Title: string;
        Hook: string | null;
        Content: ApiContentModel | null;
    }[] | null;
    DeletedBlockIds: string[] | null;
}