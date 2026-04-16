import { MenuItem } from "@/routes/_formroutes/formRoutes";
import { formRouteHelper } from "@/routes/formRouteHelper";
import { ReactNode, useEffect, useMemo, useReducer } from "react";
import { useLocation } from "react-router";
import { PageProviderContext, PageProviderContextBreadcrumbItem } from "./PageProviderContext";

interface InitAction {
    type: "init";
    actionTitle: string | null;
    breadcrumbs: PageProviderContextBreadcrumbItem[];
    menu: MenuItem[] | null;
}

interface SetActionSubtitleAction {
    type: "setActionSubtitle";
    actionSubtitle: string | null;
    description: string | null;
}

interface SetBreadcrumbItemPathAction {
    type: "setBreadcrumbItemPath";
    originalPath: string;
    path: string;
}

type Action = InitAction | SetActionSubtitleAction | SetBreadcrumbItemPathAction;

interface State {
    actionTitle: string | null;
    actionSubtitle: string | null;
    description: string | null;
    breadcrumbs: PageProviderContextBreadcrumbItem[];
    menu: MenuItem[] | null;
}

const _initialState: State = {
    actionTitle: null,
    actionSubtitle: null,
    description: null,
    breadcrumbs: [],
    menu: null,
}

function reducer(state: State, action: Action): State {
    const { type } = action;
    switch (type) {
        case "init":
        {
            return {
                actionTitle: action.actionTitle,
                actionSubtitle: null,
                description: null,
                breadcrumbs: action.breadcrumbs,
                menu: action.menu,
            };
        }

        case "setActionSubtitle":
        {
            if (action.actionSubtitle === state.actionSubtitle) {
                return state;
            }
            return {
                ...state,
                actionSubtitle: action.actionSubtitle,
                description: action.description,
            };
        }

        case "setBreadcrumbItemPath":
        {
            const item = state.breadcrumbs.find(x => x.originalPath === action.originalPath);
            if (!item || item.path === action.path) {
                return state;
            }

            return {
                ...state,
                breadcrumbs: state.breadcrumbs.map(b => (
                    b !== item ? {...b} : {
                        ...b,
                        path: action.path
                    }
                )),
            };
        }

        default:
            throw new Error(`Unknown action: ${type}`);
    }
}

interface Props {
    children?: ReactNode;
}

export default function PageProvider({ children }: Props) {
    const location = useLocation();
    const [state, dispatch] = useReducer(reducer, _initialState);

    const methods = useMemo(() => ({
        setActionSubtitle(actionSubtitle: string | null, description?: string | null): void {
            dispatch({ type: "setActionSubtitle", actionSubtitle, description: description ?? null });
        },

        setBreadcrumbItemPath(originalPath: string, path: string): void {
            dispatch({ type: "setBreadcrumbItemPath", originalPath, path });
        },
    }), [dispatch]);

    const providerValue = useMemo(() => ({
        ...state,
        ...methods,
    }), [state, methods]);

    useEffect(() => {
        const formBreadcrumbs = formRouteHelper.getBreadcrumbs(location.pathname);

        const curentForm = formBreadcrumbs && formBreadcrumbs.length > 0 ? formBreadcrumbs[formBreadcrumbs.length - 1] : null;
        const actionTitle = curentForm?.title ?? null;
        const menu = curentForm && "menu" in curentForm ? curentForm.menu as MenuItem[] : null;

        const breadcrumbs: PageProviderContextBreadcrumbItem[] = formBreadcrumbs?.map(b => ({
            originalPath: b.path,
            title: "menuTitle" in b && b.menuTitle ? b.menuTitle : b.title,
            path: b.path,
            category: b.category ?? null,
        })) ?? [];

        dispatch({ type: "init", actionTitle, breadcrumbs, menu });
    }, [location.pathname]);

    useEffect(() => {
        document.title = `${state.actionTitle ?? "Unknown Form"} | Shift iQ`;
    }, [state.actionTitle]);

    return (
        <PageProviderContext.Provider value={providerValue}>
            {children}
        </PageProviderContext.Provider>
    );
}