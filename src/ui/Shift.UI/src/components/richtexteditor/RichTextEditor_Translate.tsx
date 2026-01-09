import { useSiteProvider } from "@/contexts/SiteProvider";
import { Language, languageNames } from "../../helpers/language";
import { Spinner } from "react-bootstrap";

interface Props {
    isTranslating: boolean;
    language: Language;
    onTranslate: () => void;
}

export default function RichTextEditor_Translate({
    isTranslating,
    language,
    onTranslate
}: Props) {
    const { siteSetting } = useSiteProvider();

    if (siteSetting.SupportedLanguages.length < 2) {
        return null;
    }

    function handleLanguageClick() {
        const languages = siteSetting.SupportedLanguages.map(x => languageNames[x]).join(", ");
        const message = `Supported Languages: ${languages}`;

        window.alert(message);
    }

    function handleTranslateClick() {
        if (isTranslating) {
            return;
        }

        const languages = siteSetting.SupportedLanguages.filter(x => x !== "en").map(x => languageNames[x]).join(", ");
        const message = `Are you sure you want to translate this content from ${languageNames.en} to ${languages}`;

        if (!window.confirm(message)) {
            return;
        }

        onTranslate();
    }

    return (
        <div className="language">
            <span
                className="btn btn-default btn-xs lang-out"
                onClick={handleLanguageClick}
            >
                {languageNames[language]}
            </span>
            <span
                className="btn btn-xs btn-default ms-1"
                onClick={handleTranslateClick}
            >
                {isTranslating ? (
                    <>
                        <Spinner animation="border" role="status" size="sm" className="me-2" />
                        Translating...
                    </>

                ) : (
                    <>
                        <i className="fas fa-globe me-2"></i>
                        Translate
                    </>
                )}
            </span>
        </div>
    )
}