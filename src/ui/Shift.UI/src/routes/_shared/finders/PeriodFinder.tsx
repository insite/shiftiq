import { shiftClient } from "@/api/shiftClient";
import ControlledFinder, { ControlledFinderProps } from "@/components/finder/ControlledFinder";
import { toFinderSearchWindowData } from "@/components/finder/FinderSearchWindowData";
import { shiftConfig } from "@/helpers/shiftConfig";

type Props<Criteria extends object> = Omit<ControlledFinderProps<Criteria>, "windowTitle" | "columnHeaderTitle" | "onLoadText" | "onLoadItems">;

export default function PeriodFinder<Criteria extends object>(props: Props<Criteria>) {
    return (
        <ControlledFinder
            {...props}
            windowTitle="Periods"
            columnHeaderTitle="Name"
            onLoadText={loadText}
            onLoadItems={loadItems}
        />
    )
}

async function loadText(id: string) {
    return (await shiftClient.period.retrieve(id))?.PeriodName ?? "";
}

async function loadItems(pageIndex: number, keyword: string) {
    const result = await shiftClient.period.search({
        Name: keyword
    }, pageIndex, shiftConfig.finderPageSize);

    return toFinderSearchWindowData(result, row => ({
        value: row.Id,
        text: row.Name
    }));
}