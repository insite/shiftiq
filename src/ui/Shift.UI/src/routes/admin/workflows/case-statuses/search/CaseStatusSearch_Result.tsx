import ActionLink from "@/components/ActionLink";
import { translate } from "@/helpers/translate";
import SearchResult from "@/components/search/SearchResult";
import { CaseStatusRow } from "./CaseStatusRow";

export default function CaseStatusSearch_Result() {
    return (
        <SearchResult<CaseStatusRow> columns={[
            {
                key: "statusName",
                title: translate("Status Name"),
                itemClassName: "text-nowrap",
                item: row => (
                    <ActionLink href={`/client/admin/workflows/case-statuses/edit/${row.statusIdentifier}`}>
                        {row.statusName}
                    </ActionLink>
                )
            },
            {
                key: "caseType",
                title: translate("Case Type"),
                itemClassName: "text-nowrap",
                item: row => row.caseType
            },
            {
                key: "statusCategory",
                title: translate("Status Category"),
                itemClassName: "text-nowrap",
                item: row => row.statusCategory
            },
            {
                key: "statusSequence",
                title: translate("Status Sequence"),
                titleClassName: "text-center",
                itemClassName: "text-nowrap text-center",
                item: row => row.statusSequence.toString()
            },
        ]} />
    );
}