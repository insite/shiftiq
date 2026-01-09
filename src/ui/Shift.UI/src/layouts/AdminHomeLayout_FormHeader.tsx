import { Fragment, useEffect, useMemo } from "react";
import { useLocation, useParams } from "react-router";
import ActionLink from "@/components/ActionLink";
import { useSiteProvider } from "@/contexts/SiteProvider";
import { formRouteHelper } from "@/routes/formRouteHelper";
import { MenuItem } from "@/routes/formRoutes";
import { environmentHelper } from "@/helpers/environmentHelper";

export default function AdminHomeLayout_FormHeader() {
    const location = useLocation();
    const params = useParams();
    const { siteSetting } = useSiteProvider();

    const { indicator } = environmentHelper.getIndicator(siteSetting.Environment.Name);

    const { breadcrumbs, menu, actionTitle } = useMemo(() => {
        const breadcrumbs = formRouteHelper.getBreadcrumbs(location.pathname);

        const curentForm = breadcrumbs && breadcrumbs.length > 0 ? breadcrumbs[breadcrumbs.length - 1] : null;
        const menu = curentForm && "menu" in curentForm ? curentForm.menu as MenuItem[] : null;

        return {
            breadcrumbs: breadcrumbs?.map(b => ({
                title: "menuTitle" in b && b.menuTitle ? b.menuTitle : b.title,
                path: b.path,
                category: b.category,
            })),
            menu,
            actionTitle: curentForm?.title,
        };
    }, [location.pathname]);

    useEffect(() => {
        document.title = `${actionTitle ?? "Unknown Form"} | Shift iQ`;
    }, [actionTitle]);

    return (
        <div className="form-header border-bottom">
            {siteSetting.Environment.Name.toLowerCase() !== "production" && (
                <div className={`float-end text-${indicator}`}>
                    <i className="fa-solid fa-thumbtack me-2"></i>Please remember you are <strong>not</strong> working in a live version.
                </div>
            )}
            <nav aria-label="breadcrumb">
                <ol className="breadcrumb">
                    {breadcrumbs?.map(({ title, path, category }, index) => (
                        <Fragment key={path}>
                            {category && (
                                <li
                                    className="breadcrumb-item"
                                    aria-current="page"
                                >
                                    {category}
                                </li>
                            )}
                            <li
                                className={`breadcrumb-item ${index === breadcrumbs.length - 1 ? "active": ""}`}
                                aria-current="page"
                            >
                                {path && index !== breadcrumbs.length - 1 ? (
                                    <ActionLink href={formRouteHelper.setUrlParams(path, params)}>
                                        {title}
                                    </ActionLink>
                                ) : <>{title}</>}
                            </li>
                        </Fragment>
                    ))}
                    {menu && menu.map(({ href, icon, title }) => (
                        <li key={href} className="ms-5">
                            <ActionLink href={href}>
                                <i className={`${icon} ms-2 me-1`}></i>{title}
                            </ActionLink>
                        </li>
                    ))}
                </ol>
            </nav>
            <div className="row pb-2">
                <div className="col-lg-12">
                    <h1 className="mb-1">
                        {actionTitle}
                    </h1>
                </div>
            </div>
        </div>
    );
}