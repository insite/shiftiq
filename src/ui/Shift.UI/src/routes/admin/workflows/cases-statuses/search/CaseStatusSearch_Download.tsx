import { shiftClient } from "@/api/shiftClient";
import { useSearch } from "@/components/search/Search";
import SearchDownload from "@/components/search/SearchDownload";
import { CaseStatusCriteria, toApiSearchCaseStatuses } from "./CaseStatusCriteria";

export default function CaseStatusSearch_Download() {
    const { criteria } = useSearch<CaseStatusCriteria, object>();

    return (
        <SearchDownload
            columns={[
                { value: "OrganizationId", text: "Organization Id" },
                { value: "CaseType", text: "Case Type" },
                { value: "StatusId", text: "Status Id" },
                { value: "StatusName", text: "Status Name" },
                { value: "StatusSequence", text: "Status Sequence" },
                { value: "StatusCategory", text: "Status Category" },
                { value: "ReportCategory", text: "Report Category" },
                { value: "StatusDescription", text: "Status Description" },
            ]}
            onDownload={(format, visibleColumns) => shiftClient.caseStatus.download(toApiSearchCaseStatuses(criteria), format, visibleColumns)}
        />
    );
}