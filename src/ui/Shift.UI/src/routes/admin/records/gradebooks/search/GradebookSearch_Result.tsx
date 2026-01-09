import ActionLink from "@/components/ActionLink";
import { translate } from "@/helpers/translate";
import SearchResult from "@/components/search/SearchResult";
import { GradebookRow } from "./GradebookRow";
import DateRangeField from "@/routes/_shared/fields/DateRangeField";
import DateTimeField from "@/routes/_shared/fields/DateTimeField";

export default function GradebookSearch_Result() {
    return (
        <SearchResult<GradebookRow> columns={[
            {
                key: "title",
                title: translate("Title"),
                itemClassName: "text-nowrap",
                item: row => (
                    <>
                        <ActionLink href={`/client/admin/records/gradebooks/outline/${row.gradebookId}`}>
                            {row.gradebookTitle}
                        </ActionLink>
                        {row.isLocked ? <i className="fas fa-lock text-danger ms-2" title="Locked"></i> : null}
                    </>
                )
            },
            {
                key: "class",
                title: translate("Class"),
                item: row => row.classId && (
                    <ActionLink href={`/ui/admin/events/classes/outline?event=${row.classId}`}>
                        {row.classTitle}
                    </ActionLink>
                )
            },
            {
                key: "numberOfLearners",
                className: "text-center",
                title: translate("# of Learners"),
                itemClassName: "text-nowrap",
                item: row => row.gradebookEnrollmentCount
            },
            {
                key: "achievementsGranted",
                className: "text-center",
                title: translate("Achievements Granted"),
                itemClassName: "text-nowrap",
                item: row => row.achievementCountGranted
            },
            {
                key: "scheduled",
                title: translate("Scheduled"),
                item: row => <DateRangeField date1={row.classStarted} date2={row.classEnded} />
            },
            {
                key: "achievement",
                title: translate("Achievement"),
                item: row => row.achievementId && (
                    <ActionLink href={`/ui/admin/records/achievements/outline?id=${row.achievementId}`}>
                        {row.achievementTitle}
                    </ActionLink>
                )
            },
            {
                key: "created",
                title: translate("Created"),
                itemClassName: "text-nowrap",
                item: row => <DateTimeField dateTime={row.gradebookCreated} />
            },
        ]} />
    );
}