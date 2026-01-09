import { fetchHelper } from "../../fetchHelper";
import { ApiSearchAchievements } from "./ApiSearchAchievements";
import { ApiAchievementMatch } from "./ApiAchievementMatch";
import { ApiAchievementModel } from "./ApiAchievementModel";

const _baseUrl = "/progress/achievements";

export const _achievementController = {
    async retrieve(achievementId: string) {
        return await fetchHelper.get<ApiAchievementModel | null>(`${_baseUrl}/${achievementId}`, null, true);
    },

    async search(query: ApiSearchAchievements, pageIndex: number, pageSize: number) {
        return await fetchHelper.getPagedRows<ApiAchievementMatch>(`${_baseUrl}/search`, query, pageIndex, pageSize, null, null);
    },
}