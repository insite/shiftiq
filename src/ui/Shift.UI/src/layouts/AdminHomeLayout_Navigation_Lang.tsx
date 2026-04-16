import { shiftClient } from "@/api/shiftClient";
import Icon from "@/components/icon/Icon";
import { useSiteProvider } from "@/contexts/site/SiteProviderContext";
import { Language, localizedLanguageNames } from "@/helpers/language";
import { useState } from "react";
import { Spinner } from "react-bootstrap";

export default function AdminHomeLayout_Navigation_Lang() {
    const [isLoading, setIsLoading] = useState(false);
    const { siteSetting } = useSiteProvider();

    if (siteSetting.SupportedLanguages.length === 1) {
        return null;
    }

    async function changeLanguage(language: string) {
        setIsLoading(true);

        try {
            await shiftClient.cookie.changeLanguage(language as Language);
        } finally {
            window.location.reload();
        }
    }

    return (
        <li className="nav-item fs-sm dropdown">

            <a href="#" className="nav-link dropdown-toggle text-uppercase" data-bs-toggle="dropdown" data-bs-auto-close="outside" aria-expanded="false">
                <Icon style="regular" name="globe" className="me-2 fa-width-auto" />
                {siteSetting.CurrentLanguage}
                {isLoading && <Spinner animation="border" role="status" size="sm" className="ms-2" />}
            </a>

            {!isLoading && (
                <ul className="dropdown-menu dropdown-menu-end">
                    {siteSetting.SupportedLanguages.filter(x => x !== siteSetting.CurrentLanguage).map(lang => (
                        <li key={lang}>
                            <button
                                type="button"
                                className="dropdown-item"
                                disabled={isLoading}
                                onClick={() => changeLanguage(lang)}
                            >
                                {localizedLanguageNames[lang]}
                            </button>
                        </li>
                    ))}
                </ul>
            )}
        </li>
    );
}