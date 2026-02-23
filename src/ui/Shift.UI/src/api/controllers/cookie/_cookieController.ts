import { fetchHelper } from "@/api/fetchHelper";
import { Language } from "@/helpers/language";

interface LoginResult {
    token: string;
}

export const _cookieController = {
    login(organizationCode: string, email: string): Promise<LoginResult | null> {
        return fetchHelper.post<LoginResult>(`/api/security/cookies/login`, null, [
            { name: "organizationCode", value: organizationCode },
            { name: "email", value: email },
        ]);
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
}