import { fetchHelper } from "@/api/fetchHelper";
import { ApiEventModel } from "./ApiEventModel";

const _baseUrl = "/booking/events";

export const _eventController = {
    async retrieve(eventId: string) {
        return await fetchHelper.get<ApiEventModel | null>(`${_baseUrl}/${eventId}`, null, true);
    },
}