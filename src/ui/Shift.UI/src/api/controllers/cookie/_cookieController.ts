import { fetchHelper } from "@/api/fetchHelper";

interface LoginResult {
    token: string;
}

export const _cookieController = {
    login(organizationCode: string, email: string): Promise<LoginResult | null> {
        return fetchHelper.post<LoginResult>(`/security/cookies/login`, null, [
            { name: "organizationCode", value: organizationCode },
            { name: "email", value: email },
        ]);
    },
    async logout(): Promise<void> {
        await fetchHelper.post(`/security/cookies/logout`, null);
    },
}