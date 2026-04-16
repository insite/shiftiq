import { Fragment, useEffect } from "react";
import { useParams } from "react-router";
import ActionLink from "@/components/ActionLink";
import { formRouteHelper } from "@/routes/formRouteHelper";
import AdminHomeLayout_FormHeader_Env from "./AdminHomeLayout_FormHeader_Env";
import Icon from "@/components/icon/Icon";
import { usePageProvider } from "@/contexts/page/PageProviderContext";

export default function AdminHomeLayout_FormHeader() {
    const params = useParams();

    const { actionTitle, actionSubtitle, description, breadcrumbs, menu } = usePageProvider();

    useEffect(() => {
        document.title = `${actionTitle ?? "Unknown Form"} | Shift iQ`;
    }, [actionTitle]);

    return (
        <div className="form-header border-bottom">
            <AdminHomeLayout_FormHeader_Env />

            <nav aria-label="breadcrumb">
                <ol className="breadcrumb">
                    {breadcrumbs?.map(({ originalPath, title, path, category }, index) => (
                        <Fragment key={originalPath}>
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
                                <Icon style="solid" name={icon} className="ms-2 me-1" />{title}
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
                                {description && (
                                    <span className="form-text ms-2">{description}</span>
                                )}
                            </>
                        )}
                    </h1>
                </div>
            </div>
        </div>
    );
}