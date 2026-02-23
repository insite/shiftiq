import { fetchHelper } from "@/api/fetchHelper";
import { MultiLanguageText } from "@/helpers/language";

export const _translationController = {
    async translate(englishTexts: string[]): Promise<MultiLanguageText[] | null> {
        return fetchHelper.post<MultiLanguageText[]>("/api/content/translations/translate", englishTexts);
    }
}