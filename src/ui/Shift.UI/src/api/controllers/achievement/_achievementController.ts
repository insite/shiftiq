import { fetchHelper } from "../../fetchHelper";
import { ApiSearchAchievements } from "./ApiSearchAchievements";
import { ApiAchievementMatch } from "./ApiAchievementMatch";
import { ApiAchievementModel } from "./ApiAchievementModel";

export const _achievementController = {
    async retrieve(achievementId: string) {
        return await fetchHelper.get<ApiAchievementModel | null>(`/api/progress/achievements/${achievementId}`, null, true);
    },

    async search(query: ApiSearchAchievements, pageIndex: number, pageSize: number) {
        return await fetchHelper.getPagedRows<ApiAchievementMatch>(`/api/progress/achievements/search`, query, pageIndex, pageSize, null, null);
    },
}