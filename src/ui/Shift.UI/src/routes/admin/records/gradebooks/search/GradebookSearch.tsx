import Search from "@/components/search/Search";
import { GradebookRow, toGradebookRow } from "./GradebookRow";
import { useSiteProvider } from "@/contexts/SiteProvider";
import { TimeZoneId } from "@/helpers/date/timeZones";
import { shiftClient } from "@/api/shiftClient";
import { mapQueryResult } from "@/models/QueryResult";
import GradebookSearch_Result from "./GradebookSearch_Result";
import GradebookSearch_Criteria from "./GradebookSearch_Criteria";
import GradebookSearch_Download from "./GradebookSearch_Download";
import { defaultGradebookCriteria, GradebookCriteria, toApiSearchGradebooks } from "./GradebookCriteria";

export default function GradebookSearch() {
    const { siteSetting } = useSiteProvider();

    return (
        <Search<GradebookCriteria, GradebookRow>
            cacheKey="search.gradebook"
            defaultCriteria={defaultGradebookCriteria()}
            resultElement={<GradebookSearch_Result />}
            criteriaElement={<GradebookSearch_Criteria />}
            downloadElement={<GradebookSearch_Download />}
            onLoad={(pageIndex, criteria) => load(pageIndex, criteria, siteSetting.TimeZoneId)}
        />
    );
}

async function load(pageIndex: number, criteria: GradebookCriteria, timeZoneId: TimeZoneId) {
    const filter = toApiSearchGradebooks(criteria);
    const result = await shiftClient.gradebook.search(filter, pageIndex, criteria.sortByColumn);

    return mapQueryResult(result, row => toGradebookRow(row, timeZoneId));
}