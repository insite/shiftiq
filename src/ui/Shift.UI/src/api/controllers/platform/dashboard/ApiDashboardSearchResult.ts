import { ApiDashboardNotification } from "./ApiDashboardNotification";

export interface ApiDashboardSearchResult {
    Notifications: ApiDashboardNotification[];
    Users: Record<string, string>;
    VisibleOnDashboard: Record<string, boolean>;
}