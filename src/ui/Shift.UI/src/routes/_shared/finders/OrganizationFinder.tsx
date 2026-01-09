import ControlledFinder, { ControlledFinderProps } from "@/components/finder/ControlledFinder";
import { shiftClient } from "@/api/shiftClient";
import { shiftConfig } from "@/helpers/shiftConfig";
import { toFinderSearchWindowData } from "@/components/finder/FinderSearchWindowData";

type Props<Criteria extends object> = Omit<ControlledFinderProps<Criteria>, "windowTitle" | "columnHeaderTitle" | "onLoadText" | "onLoadItems">;

export default function OrganizationFinder<Criteria extends object>(props: Props<Criteria>) {
    return (
        <ControlledFinder
            {...props}
            windowTitle="Organizations"
            columnHeaderTitle="Title"
            onLoadText={loadText}
            onLoadItems={loadItems}
        />
    )
}

async function loadText(id: string) {
    return (await shiftClient.organization.retrieve(id))?.CompanyName ?? "";
}

async function loadItems(pageIndex: number, keyword: string) {
    const result = await shiftClient.organization.search({
        CompanyNameContains: keyword
    }, pageIndex, shiftConfig.finderPageSize, null);

    return toFinderSearchWindowData(result, row => ({
        value: row.OrganizationIdentifier,
        text: row.CompanyName || "Untitled Organization"
    }));
}