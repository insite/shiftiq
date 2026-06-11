import { useSiteProvider } from "@/contexts/site/SiteProviderContext";
import { defaultNotificationCriteria, NotificationCriteria, toApiDashboardNotificationCriteria } from "./NotificationCriteria";
import { NotificationRow, toNotificationRow } from "./NotificationRow";
import Search from "@/components/search/Search";
import NotificationSearch_Result from "./NotificationSearch_Result";
import NotificationSearch_Criteria from "./NotificationSearch_Criteria";
import { TimeZoneId } from "@/helpers/date/timeZones";
import { shiftClient } from "@/api/shiftClient";
import { mapQueryResult } from "@/models/QueryResult";

export default function NotificationSearch() {
    const { siteSetting: { TimeZoneId: timeZoneId } } = useSiteProvider();

    return (
        <Search<NotificationCriteria, NotificationRow>
            cacheKey="search.notification"
            defaultCriteria={defaultNotificationCriteria()}
            resultElement={<NotificationSearch_Result />}
            criteriaElement={<NotificationSearch_Criteria />}
            downloadElement={null}
            onLoad={(pageIndex, criteria) => load(pageIndex, criteria, timeZoneId)}
        />
    );
}

async function load(pageIndex: number, criteria: NotificationCriteria, timeZoneId: TimeZoneId) {
    const apiCriteria = toApiDashboardNotificationCriteria(criteria);
    const result = await shiftClient.dashboard.searchNotifications(apiCriteria, pageIndex);

    return mapQueryResult(result?.queryResult ?? null, row => toNotificationRow(row, result!.users, result!.visibleOnDashboard, timeZoneId));
}