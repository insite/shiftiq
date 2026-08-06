import FormTab from "@/components/form/FormTab";
import FormTabs from "@/components/form/FormTabs";
import ImportReportGrid, { ImportReportGridFunctions } from "./ImportReportGrid";
import { translate } from "@/helpers/translate";
import { useRef, useState } from "react";
import PendingPeopleGrid from "./PendingPeopleGrid";

export default function PersonIntegration() {
    const [pendingPersonCount, setPendingPersonCount] = useState<number | undefined>(undefined);
    const [importReportCount, setImportReportCount] = useState<number | undefined>(undefined);

    const reportGridRef = useRef<ImportReportGridFunctions>(null);

    function handleImported() {
        reportGridRef.current?.refresh();
    }

    return (
        <FormTabs defaultTab="people">
            <FormTab
                tab="people"
                title={translate("Pending People")}
                icon={{ style: "regular", name: "user" }}
                count={pendingPersonCount}
            >
                <PendingPeopleGrid
                    onCountUpdated={setPendingPersonCount}
                    onImported={handleImported}
                />
            </FormTab>
            <FormTab
                tab="reports"
                title={translate("Import Reports")}
                icon={{ style: "regular", name: "download" }}
                count={importReportCount}
            >
                <ImportReportGrid
                    ref={reportGridRef}
                    onCountUpdated={setImportReportCount}
                />
            </FormTab>
        </FormTabs>
    );
}