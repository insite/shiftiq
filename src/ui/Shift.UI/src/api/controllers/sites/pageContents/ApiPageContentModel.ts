import { ApiContentModel } from "@/api/models/ApiContentModel";

export interface ApiPageContentModel {
    Title: string;
    ContentFields: string[];
    Content: ApiContentModel;
    Blocks: {
        BlockId: string;
        Title: string;
        Hook: string | null | undefined;
        BlockType: string;
        Content: ApiContentModel;
    }[];
}