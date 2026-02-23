import { fetchHelper } from "@/api/fetchHelper";
import { ApiPageContentModel } from "./ApiPageContentModel";
import { ApiPageContentModifyModel } from "./ApiPageContentModifyModel";

export const _pageContentController = {
    async retrieve(pageId: string): Promise<ApiPageContentModel | null> {
        return await fetchHelper.get<ApiPageContentModel | null>(`/api/workspace/pages-contents/${pageId}`, null, true);
    },

    async modify(pageId: string, model: ApiPageContentModifyModel): Promise<void> {
        await fetchHelper.post(`/api/workspace/pages-contents/${pageId}`, model);
    },
}