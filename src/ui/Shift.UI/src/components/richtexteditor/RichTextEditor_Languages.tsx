import { useMemo } from "react";
import showdown from "showdown";
import { useSiteProvider } from "@/contexts/SiteProvider";
import { Language, MultiLanguageText } from "../../helpers/language";
import { RichTextEditorMode } from "./RichTextEditorMode";

interface Props {
    text?: MultiLanguageText | null;
    mode: RichTextEditorMode;
    excludeLanguage: Language;
    onSelect: (language: Language) => void;
}

interface LanguageItem {
    language: Language;
    text: string;
}

const _converter = new showdown.Converter({ simpleLineBreaks: true });

export default function RichTextEditor_Languages({
    text,
    mode,
    excludeLanguage,
    onSelect
}: Props) {
    const { siteSetting } = useSiteProvider();

    const items = useMemo(() => {
        if (!text || Object.keys(text).length === 1 && text[excludeLanguage]) {
            return null;
        }

        const items: LanguageItem[] = [];

        for (const language of siteSetting.SupportedLanguages) {
            if (language === excludeLanguage) {
                continue;
            }

            const langText = text[language];
            if (langText) {
                items.push({
                    language,
                    text: mode === "markdown" ? _converter.makeHtml(langText) : langText,
                });
            }
        }

        return items;
    }, [text, mode, excludeLanguage, siteSetting]);

    if (!items) {
        return null;
    }

    return (
        <table className="translation-list">
            <tbody>
                {items.map(({ language, text }) => (
                    <tr key={language}>
                        <td className="lang-name">
                            <a
                                href="#"
                                onClick={e => {
                                    e.preventDefault();
                                    onSelect(language);
                                }}
                            >
                                {language}
                            </a>
                        </td>
                        <td className="lang-value">
                            <div dangerouslySetInnerHTML={{__html: text}} />
                        </td>
                    </tr>
                ))}
            </tbody>
        </table>
    );
}