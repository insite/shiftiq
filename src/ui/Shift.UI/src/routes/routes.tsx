import AdminHomeLayout from "@/layouts/AdminHomeLayout";
import { Navigate, RouteObject } from "react-router";
import NotFound from "./NotFound";
import ErrorPage from "./ErrorPage";
import { formRouteHelper } from "./formRouteHelper";

const children = [
    { index: true, element: <Navigate replace to="/client/admin/home" /> },
    ...formRouteHelper.routeObjects
];

export const routes: RouteObject[] = [
    {
        errorElement: <ErrorPage />,
        children: [
            { index: true, element: <Navigate replace to="/client/test" /> },
            {
                element: <AdminHomeLayout />,
                path: "client",
                children
            },
        ]
    },
    { path: "*", element: <NotFound /> }
];