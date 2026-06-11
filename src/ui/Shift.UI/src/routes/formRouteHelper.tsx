import { Params, RouteObject } from "react-router";
import { CustomBreadcrumbItem, FormRoute, formRoutes } from "./_formroutes/formRoutes";

const _list: FormRoute[] = [];

const _routeObjects: RouteObject[] = [];

function arePathsEqual(templatePath: string, path: string) {
    const templateParts = templatePath.split("/");
    const parts = path.split("/");

    if (templateParts.length !== parts.length) {
        return false;
    }

    for (let i = 0; i < templateParts.length; i++) {
        if (templateParts[i][0] !== ":" && templateParts[i].toLowerCase() !== parts[i]) {
            return false;
        }
    }

    return true;
}

function buildList(parentFormRoutes: FormRoute[]) {
    for (const parent of parentFormRoutes) {
        _routeObjects.push({
            path: parent.path,
            element: parent.element,
        });

        _list.push(parent);

        if (parent.children) {
            for (const child of parent.children) {
                child.parent = parent;
            }
            buildList(parent.children);
        }
    }
}

function getFormRoute(path: string) {
    if (path.endsWith("/")) {
        path = path.substring(0, path.length - 1);
    }

    path = path.toLowerCase();

    const formWithoutParams = _list.find(x => x.path.toLowerCase() === path);

    return formWithoutParams ?? _list.find(x => arePathsEqual(x.path, path));
}

function setUrlParams(url: string, params: Readonly<Params<string>>) {
    const parts = url.split("/");
    let result = "";

    for (const part of parts) {
        if (!part) {
            continue;
        }
        
        result += "/";

        if (part[0] === ":") {
            const p = params[part.substring(1)];
            result += p;
        } else {
            result += part;
        }
    }

    return result;
}

function getBreadcrumbs(path: string, includeHomeIfNoParent: boolean = true): (FormRoute | CustomBreadcrumbItem)[] | null {
    const formRoute = getFormRoute(path);
    if (!formRoute) {
        return null;
    }

    if (formRoute.customBreadcrumbs) {
        return [
            ...formRoute.customBreadcrumbs,
            formRoute
        ];
    }

    const breadcrumbs: (FormRoute | CustomBreadcrumbItem)[] = formRoute.parent
        ? [...getBreadcrumbs(formRoute.parent.path, false)!, formRoute]
        : includeHomeIfNoParent
            ? [getFormRoute("/client/admin/home")!, formRoute]
            : [formRoute];

    return breadcrumbs;
}

buildList(formRoutes);

export const formRouteHelper = {
    routeObjects: _routeObjects,

    getBreadcrumbs,

    setUrlParams,

    getParentUrl(path: string, params: Readonly<Params<string>>) {
        const formRoute = getFormRoute(path);
        const parentPath = formRoute?.parent?.path;

        return parentPath ? setUrlParams(parentPath, params) : undefined;
    },

    getGrandParentUrl(path: string, params: Readonly<Params<string>>) {
        const formRoute = getFormRoute(path);
        if (!formRoute?.parent) {
            return undefined;
        }

        const parentFormRoute = getFormRoute(formRoute.parent.path);
        const grandParentPath = parentFormRoute?.parent?.path;

        return grandParentPath ? setUrlParams(grandParentPath, params) : undefined;
    },
}