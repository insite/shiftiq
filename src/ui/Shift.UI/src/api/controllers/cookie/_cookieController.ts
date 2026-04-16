import { ApiError } from "@/api/apiError";
import { fetchHelper } from "@/api/fetchHelper";
import { Param } from "@/api/requestHelper";
import { Language } from "@/helpers/language";

interface LoginResult {
    token: string;
}

export const _cookieController = {
    login(organizationCode: string, email: string, impersonatorOrganizationCode: string | null, impersonatorUserEmail: string | null): Promise<LoginResult | null> {
        const params: Param[] = [
            { name: "organizationCode", value: organizationCode },
            { name: "email", value: email },
        ];

        if (impersonatorOrganizationCode && impersonatorUserEmail) {
            params.push({ name: "impersonatorOrganizationCode", value: impersonatorOrganizationCode });
            params.push({ name: "impersonatorUserEmail", value: impersonatorUserEmail });
        }

        return fetchHelper.post<LoginResult>(`/api/security/cookies/login`, null, params);
    },

    async logout(): Promise<void> {
        await fetchHelper.post(`/api/security/cookies/logout`, null);
    },

    async changeLanguage(language: Language): Promise<void> {
        await fetchHelper.post(`/api/security/cookies/change-language/${language}`, null);
    },

    async changeTheme(theme: "dark" | "light"): Promise<void> {
        await fetchHelper.post(`/api/security/cookies/change-theme/${theme}`, null);
    },

    async refreshSession(): Promise<void> {
        try {
            await fetchHelper.post("/api/security/cookies/refresh-session", null, null, false, true);
        } catch (err) {
            if (!(err instanceof ApiError) || err.status !== 401) {
                throw err;
            }
        }
    },
}