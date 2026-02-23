import { Fragment, useEffect, useMemo } from "react";
import { useLocation, useParams } from "react-router";
import ActionLink from "@/components/ActionLink";
import { formRouteHelper } from "@/routes/formRouteHelper";
import { MenuItem } from "@/routes/formRoutes";
import AdminHomeLayout_FormHeader_Env from "./AdminHomeLayout_FormHeader_Env";
import Icon from "@/components/icon/Icon";
import { useSiteProvider } from "@/contexts/SiteProvider";

export default function AdminHomeLayout_FormHeader() {
    const location = useLocation();
    const params = useParams();

    const { actionSubtitle } = useSiteProvider();

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
            <AdminHomeLayout_FormHeader_Env />

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
                                <Icon style="Solid" name={icon} className="ms-2 me-1" />{title}
                            </ActionLink>
                        </li>
                    ))}
                </ol>
            </nav>
            <div className="row pb-2">
                <div className="col-lg-12">
                    <h1 className="mb-1">
                        {actionTitle}
                        {actionSubtitle && (
                            <>
                                &nbsp;-&nbsp;
                                <span className="text-info">{actionSubtitle}</span>
                            </>
                        )}
                    </h1>
                </div>
            </div>
        </div>
    );
}