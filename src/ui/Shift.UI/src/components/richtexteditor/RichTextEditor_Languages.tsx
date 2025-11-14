import showdown from "showdown";
import { useSiteProvider } from "@/contexts/SiteProvider";
import { Language, MultiLanguageText } from "./language";
import { useMemo } from "react";

interface Props {
    markdown?: MultiLanguageText | null;
    excludeLanguage: Language;
    onSelect: (language: Language) => void;
}

interface LanguageItem {
    language: Language;
    text: string;
}

const _converter = new showdown.Converter({ simpleLineBreaks: true });

export default function RichTextEditor_Languages({
    markdown,
    excludeLanguage,
    onSelect
}: Props) {
    const { siteSetting } = useSiteProvider();

    const items = useMemo(() => {
        if (!markdown || Object.keys(markdown).length === 1 && markdown[excludeLanguage]) {
            return null;
        }

        const items: LanguageItem[] = [];

        for (const language of siteSetting.SupportedLanguages) {
            if (language === excludeLanguage) {
                continue;
            }

            const text = markdown[language];
            if (text) {
                items.push({
                    language,
                    text: _converter.makeHtml(text),
                });
            }
        }

        return items;
    }, [markdown, excludeLanguage, siteSetting]);

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