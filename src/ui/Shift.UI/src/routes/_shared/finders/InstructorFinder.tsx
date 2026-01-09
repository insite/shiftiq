import ControlledFinder, { ControlledFinderProps } from "@/components/finder/ControlledFinder";
import { shiftClient } from "@/api/shiftClient";
import { shiftConfig } from "@/helpers/shiftConfig";
import { toFinderSearchWindowData } from "@/components/finder/FinderSearchWindowData";

type Props<Criteria extends object> = Omit<ControlledFinderProps<Criteria>, "windowTitle" | "columnHeaderTitle" | "onLoadText" | "onLoadItems">;

export default function InstructorFinder<Criteria extends object>(props: Props<Criteria>) {
    return (
        <ControlledFinder
            {...props}
            windowTitle="Instructors"
            columnHeaderTitle="Title"
            onLoadText={loadText}
            onLoadItems={loadItems}
        />
    )
}

async function loadText(id: string) {
    const result = await shiftClient.people.search({ UserIdentifier: id }, 0, 1, ["UserName"]);
    return result?.rows?.length ? result.rows[0].UserName : "";
}

async function loadItems(pageIndex: number, keyword: string) {
    const result = await shiftClient.people.search({
        EventRole: "Instructor",
        FullName: keyword,
    }, pageIndex, shiftConfig.finderPageSize, ["UserId", "UserName"]);

    return toFinderSearchWindowData(result, row => ({
        value: row.UserId,
        text: row.UserName
    }));
}