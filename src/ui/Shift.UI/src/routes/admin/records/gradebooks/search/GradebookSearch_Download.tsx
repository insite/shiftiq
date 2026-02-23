import { shiftClient } from "@/api/shiftClient";
import Button from "@/components/Button";
import { useSearch } from "@/components/search/Search";
import SearchDownload from "@/components/search/SearchDownload";
import { translate } from "@/helpers/translate";
import { GradebookCriteria, toApiSearchGradebooks } from "./GradebookCriteria";

export default function GradebookSearch_Download() {
    const { criteria } = useSearch<GradebookCriteria, object>();

    return (
        <SearchDownload
            columns={[
                { value: "AchievementId", text: "Achievement Id" },
                { value: "AchievementTitle", text: "Achievement Title" },
                { value: "ClassInstructors", text: "Class Instructors" },
                { value: "ClassScheduledEndDate", text: "Class Scheduled End Date" },
                { value: "ClassScheduledStartDate", text: "Class Scheduled Start Date" },
                { value: "ClassTitle", text: "Class Title" },
                { value: "EventId", text: "Event Id" },
                { value: "FrameworkId", text: "Framework Id" },
                { value: "GradebookCreated", text: "Gradebook Created" },
                { value: "GradebookId", text: "Gradebook Id" },
                { value: "GradebookTitle", text: "Gradebook Title" },
                { value: "GradebookType", text: "Gradebook Type" },
                { value: "IsLocked", text: "Is Locked" },
                { value: "LastChangeTime", text: "Last Change Time" },
                { value: "LastChangeType", text: "Last Change Type" },
                { value: "LastChangeUser", text: "Last Change User" },
                { value: "PeriodId", text: "Period Id" },
                { value: "Reference", text: "Reference" },
            ]}
            onDownload={(format, visibleColumns) => shiftClient.gradebook.download(toApiSearchGradebooks(criteria), format, visibleColumns)}
        >
            <h4 className="mt-3">
                {translate("Other")}
            </h4>

            <div>
                <Button
                    text="Outcome Summary (*.xlsx)"
                    variant="download-excel"
                    type="button"
                    className="w-100"
                    onClick={() => window.alert("Not Implemented")}
                />
            </div>            
        </SearchDownload>
    );
}