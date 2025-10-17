import { shiftClient } from "@/api/shiftClient";
import ControlledFinder, { ControlledFinderProps } from "@/components/finder/ControlledFinder";
import { toFinderSearchWindowData } from "@/components/finder/FinderSearchWindowData";
import { FINDER_PAGE_SIZE } from "@/helpers/constants";

type Props<Criteria extends object> = Omit<ControlledFinderProps<Criteria>, "windowTitle" | "columnHeaderTitle" | "onLoadText" | "onLoadItems">;

export default function FrameworkFinder<Criteria extends object>(props: Props<Criteria>) {
    return (
        <ControlledFinder
            {...props}
            windowTitle="Frameworks"
            columnHeaderTitle="Framework Name"
            onLoadText={loadText}
            onLoadItems={loadItems}
        />
    )
}

async function loadText(id: string) {
    return (await shiftClient.standard.retrieve(id))?.ContentTitle ?? "";
}

async function loadItems(pageIndex: number, keyword: string) {
    const result = await shiftClient.standard.search({
        StandardType: "Framework",
        ContentTitle: keyword
    }, pageIndex, FINDER_PAGE_SIZE);

    return toFinderSearchWindowData(result, row => ({
        value: row.Id,
        text: row.Name
    }));
}