import { fetchHelper } from "@/api/fetchHelper";
import { ApiAIProctoringSession } from "./ApiAIProctoringSession";

export const _attemptController = {
    async retrieveProctoringStatus(attemptId: string): Promise<ApiAIProctoringSession | null> {
        return await fetchHelper.get<ApiAIProctoringSession>(`/api/assessments/attempts/${attemptId}/proctoring-status`);
    },
}