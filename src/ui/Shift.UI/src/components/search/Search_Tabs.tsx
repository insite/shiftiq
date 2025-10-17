import { ReactNode } from "react"
import { Tab, Tabs } from "react-bootstrap";
import Search_DownloadTitle from "./Search_DownloadTitle";
import LoadingContext from "@/contexts/LoadingProvider";
import Search_ResultTitle from "./Search_ResultTitle";
import Search_CriteriaTitle from "./Search_CriteriaTitle";
import { SelectedTab } from "@/models/enums";

interface Props {
    selectedTab: SelectedTab;
    resultElement: ReactNode;
    criteriaElement: ReactNode;
    downloadElement: ReactNode;
    totalRowCount: number;
    onSelectTab: (selectedTab: SelectedTab) => void;
}

export default function Search_Tabs({
    selectedTab,
    resultElement,
    criteriaElement,
    downloadElement,
    totalRowCount,
    onSelectTab
}: Props) {
    return (
        <Tabs activeKey={selectedTab} transition={false} onSelect={tab => onSelectTab(tab as SelectedTab)}>
            <Tab eventKey="result" title={<Search_ResultTitle count={totalRowCount} />}>
                <LoadingContext>
                    {resultElement}
                </LoadingContext>
            </Tab>
            {criteriaElement && (
                <Tab eventKey="criteria" title={<Search_CriteriaTitle />}>
                    <LoadingContext>
                        {criteriaElement}
                    </LoadingContext>
                </Tab>
            )}
            {downloadElement && (
                <Tab eventKey="download" title={<Search_DownloadTitle />}>
                    <LoadingContext>
                        {downloadElement}
                    </LoadingContext>
                </Tab>
            )}
        </Tabs>
    );
}