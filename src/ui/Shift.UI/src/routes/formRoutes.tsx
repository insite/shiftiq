import { ReactNode } from "react";
import TestIndex from "./test/TestIndex";
import AdminHome from "./admin/home/AdminHome";
import GradebookSearch from "./admin/records/gradebooks/search/GradebookSearch";
import GradebookOutline from "./admin/records/gradebooks/outline/GradebookOutline";
import GradebookRename from "./admin/records/gradebooks/rename/GradebookRename";
import GradebookDelete from "./admin/records/gradebooks/delete/GradebookDelete";
import GradebookOpen from "./admin/records/gradebooks/open/GradebookOpen";
import FileSearch from "./admin/content/files/search/FileSearch";
import SignIn from "./react/signin/SignIn";
import ReactHome from "./react/home/ReactHome";
import CaseStatusSearch from "./admin/workflows/case-statuses/search/CaseStatusSearch";
import CaseStatusCreate from "./admin/workflows/case-statuses/create/CaseStatusCreate";
import CaseStatusEdit from "./admin/workflows/case-statuses/edit/CaseStatusEdit";
import CaseStatusDelete from "./admin/workflows/case-statuses/delete/CaseStatusDelete";

export interface MenuItem {
    href: string;
    icon: string;
    title: string;
}

export interface CustomBreadcrumbItem {
    title: string;
    path: string;
    category?: string;
}

export interface FormRoute {
    parent?: FormRoute;
    title: string;
    menuTitle?: string;
    path: string;
    category?: string;
    element: ReactNode;
    menu?: MenuItem[];
    children?: FormRoute[] | null;
    customBreadcrumbs?: CustomBreadcrumbItem[];
}

export const formRoutes: FormRoute[] = [
    {
        title: "Toolkits",
        menuTitle: "Admin",
        path: "/client/admin/home",
        element: <AdminHome />,
        children: [
            {
                title: "React Home",
                menuTitle: "React",
                path: "/client/react/home",
                element: <ReactHome />,
                children: [
                    {
                        title: "Sign In",
                        path: "/client/react/signin",
                        element: <SignIn />,
                    },
                    {
                        title: "React Test",
                        path: "/client/test",
                        element: <TestIndex />,
                    },
                ]
            },
            {
                title: "Gradebooks",
                category: "Gradebooks",
                menuTitle: "Search",
                path: "/client/admin/records/gradebooks/search",
                element: <GradebookSearch />,
                menu: [
                    {
                        href: "/client/admin/records/gradebooks/open",
                        icon: "fas fa-plus-circle",
                        title: "Add New Gradebook"
                    },
                ],
                children: [
                    {
                        title: "Gradebook Outline",
                        menuTitle: "Outline",
                        path: "/client/admin/records/gradebooks/outline/:id",
                        element: <GradebookOutline />,
                        children: [
                            {
                                title: "Rename Gradebook",
                                menuTitle: "Rename",
                                path: "/client/admin/records/gradebooks/rename/:id",
                                element: <GradebookRename />,
                            },
                            {
                                title: "Delete Gradebook",
                                menuTitle: "Delete",
                                path: "/client/admin/records/gradebooks/delete/:id",
                                element: <GradebookDelete />,
                            }
                        ]
                    },
                    {
                        title: "Open Gradebook",
                        menuTitle: "Open",
                        path: "/client/admin/records/gradebooks/open",
                        element: <GradebookOpen />,
                    },
                ]
            },
            {
                title: "Files",
                category: "Files",  
                menuTitle: "Search",
                path: "/client/admin/content/files/search",
                element: <FileSearch />
            },
            {
                title: "Case Statuses",
                category: "Case Statuses",  
                menuTitle: "Search",
                path: "/client/admin/workflows/case-statuses/search",
                element: <CaseStatusSearch />,
                menu: [
                    {
                        href: "/client/admin/workflows/case-statuses/create",
                        icon: "fas fa-plus-circle",
                        title: "Add New Case Status"
                    },
                ],
                children: [
                    {
                        title: "New Case Status",
                        menuTitle: "New",
                        path: "/client/admin/workflows/case-statuses/create",
                        element: <CaseStatusCreate />,
                    },
                    {
                        title: "Edit Case Status",
                        menuTitle: "Edit",
                        path: "/client/admin/workflows/case-statuses/edit/:id",
                        element: <CaseStatusEdit />,
                        children:[
                            {
                                title: "Delete Case Status",
                                menuTitle: "Delete",
                                path: "/client/admin/workflows/case-statuses/delete/:id",
                                element: <CaseStatusDelete />,
                            },
                        ]
                    },
                ]
            }
        ]
    },
];