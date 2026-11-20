import { fetchHelper } from "@/api/fetchHelper";
import { ApiProviewSession } from "./ApiProviewSession";

export const _proviewController = {
    async retrieveSession(form: string | null, resource: string | null, attempt: string): Promise<ApiProviewSession | null> {
        return await fetchHelper.get<ApiProviewSession>("/api/integrations/proctoring/proview/session", [
            { name: "form", value: form },
            { name: "resource", value: resource },
            { name: "attempt", value: attempt },
        ], false, true);
    },

    async modifyProviewSessionId(shiftSessionId: string, proviewSessionId: string): Promise<void> {
        return await fetchHelper.put(`/api/integrations/proctoring/proview/session/${shiftSessionId}`, {
            ProviewSessionId: proviewSessionId
        });
    },
}