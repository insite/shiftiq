import { translate } from "./translate"

export const textHelper = {
    none(s?: string | null): string {
        return s ? s : translate("None");
    },

    in(s: string | null | undefined, list: string[]) {
        return !!s && !!list.find(x => x.toLowerCase() === s.toLowerCase());
    }
}