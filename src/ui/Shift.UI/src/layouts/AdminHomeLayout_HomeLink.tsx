import ActionLink from "@/components/ActionLink";
import { useSiteProvider } from "@/contexts/site/SiteProviderContext";

export default function AdminHomeLayout_HomeLink() {
    const { siteSetting: { Home: home } } = useSiteProvider();

    return (
        <ActionLink href={home.Url} className='text-light' title={home.Text}>
            {home.Image ? (
                <img className="home-image me-2" src={home.Image} alt={home.Text} />
            ) : (
                <i className={`${home.Icon} me-1`}></i>
            )}
            <span className="hide-compact">{home.Text}</span>
        </ActionLink>
    );
}
