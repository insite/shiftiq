import Search from "@/components/search/Search";
import { CaseStatusRow, toCaseStatusRow } from "./CaseStatusRow";
import { shiftClient } from "@/api/shiftClient";
import { mapQueryResult } from "@/models/QueryResult";
import CaseStatusSearch_Result from "./CaseStatusSearch_Result";
import CaseStatusSearch_Criteria from "./CaseStatusSearch_Criteria";
import CaseStatusSearch_Download from "./CaseStatusSearch_Download";
import { defaultCaseStatusCriteria, CaseStatusCriteria, toApiSearchCaseStatuses } from "./CaseStatusCriteria";

export default function CaseStatusSearch() {
    return (
        <Search<CaseStatusCriteria, CaseStatusRow>
            cacheKey="search.caseStatus"
            defaultCriteria={defaultCaseStatusCriteria()}
            resultElement={<CaseStatusSearch_Result />}
            criteriaElement={<CaseStatusSearch_Criteria />}
            downloadElement={<CaseStatusSearch_Download />}
            onLoad={(pageIndex, criteria) => load(pageIndex, criteria)}
        />
    );
}

async function load(pageIndex: number, criteria: CaseStatusCriteria) {
    const filter = toApiSearchCaseStatuses(criteria);
    const result = await shiftClient.caseStatus.search(filter, pageIndex, criteria.sortByColumn);

    return mapQueryResult(result, row => toCaseStatusRow(row));
}