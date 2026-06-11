import { MenuItem } from "@/routes/_formroutes/formRoutes";
import { createContext, useContext } from "react";

export interface PageProviderContextBreadcrumbItem {
    originalPath: string;
    title: string;
    path: string;
    category: string | null;
}

interface ContextData {
    actionTitle: string | null;
    actionSubtitle: string | null;
    description: string | null;
    breadcrumbs: PageProviderContextBreadcrumbItem[];
    menu: MenuItem[] | null;
    isTitleVisible: boolean;
    setActionSubtitle: (actionSubtitle: string | null, description?: string | null) => void;
    setBreadcrumbItemPath: (originalPath: string, path: string) => void;
    hideTitle: () => void;
}

export const PageProviderContext = createContext<ContextData>({
    actionTitle: null,
    actionSubtitle: null,
    description: null,
    breadcrumbs: [],
    menu: null,
    isTitleVisible: false,
    setActionSubtitle() {},
    setBreadcrumbItemPath() {},
    hideTitle() {},
});

export function usePageProvider() {
    return useContext(PageProviderContext);
}