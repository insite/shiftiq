import ControlledFinder, { ControlledFinderProps } from "@/components/finder/ControlledFinder";
import { shiftClient } from "@/api/shiftClient";
import { FINDER_PAGE_SIZE } from "@/helpers/constants";
import { toFinderSearchWindowData } from "@/components/finder/FinderSearchWindowData";

type Props<Criteria extends object> = Omit<ControlledFinderProps<Criteria>, "windowTitle" | "columnHeaderTitle" | "onLoadText" | "onLoadItems">;

export default function AchievementFinder<Criteria extends object>(props: Props<Criteria>) {
    return (
        <ControlledFinder
            {...props}
            windowTitle="Achievements"
            columnHeaderTitle="Title"
            onLoadText={loadText}
            onLoadItems={loadItems}
        />
    )
}

async function loadText(id: string) {
    return (await shiftClient.achievement.retrieve(id))?.AchievementTitle ?? "";
}

async function loadItems(pageIndex: number, keyword: string) {
    const result = await shiftClient.achievement.search({
        AchievementTitle: keyword
    }, pageIndex, FINDER_PAGE_SIZE);

    return toFinderSearchWindowData(result, row => ({
        value: row.AchievementIdentifier,
        text: row.AchievementTitle
    }));
}