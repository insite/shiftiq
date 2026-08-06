import FinderSearchWindow from "@/components/finder/FinderSearchWindow";
import { ImportAction } from "./ImportAction";
import { translate } from "@/helpers/translate";
import { useCallback } from "react";
import { shiftClient } from "@/api/shiftClient";
import { shiftConfig } from "@/helpers/shiftConfig";
import { toFinderSearchWindowData } from "@/components/finder/FinderSearchWindowData";

interface Props {
    pendingPerson: {
        userId: string | null;
        firstName: string;
        lastName: string;
    } | null;
    onSelect: (match: ImportAction["match"]) => void;
}

export default function PendingPeopleGrid_SelectMatch({
    pendingPerson,
    onSelect,
}: Props) {
    const handleLoad = useCallback(async (pageIndex: number, keyword: string) => {
        const result = pendingPerson
            ? await shiftClient.people.search({
                    FirstNameExact: pendingPerson.firstName,
                    LastNameExact: pendingPerson.lastName,
                    EmailLike: keyword
                }, pageIndex, shiftConfig.finderPageSize, null)
            : null;

        return toFinderSearchWindowData(result, row => ({
            value: row.UserId,
            text: `${row.UserEmail} (${row.PersonCode})`
        }));
    }, [pendingPerson]);

    return (
        <FinderSearchWindow
            value={pendingPerson?.userId ?? null}
            show={!!pendingPerson}
            windowTitle={translate("Matched People")}
            columnHeaderTitle={translate("Email and Employee ID")}
            hideClearButton
            reloadOnShow
            onLoad={handleLoad}
            onChange={item => onSelect({ userId: item.value, userFullName: item.text, })}
            onClose={() => onSelect(null)}
        />
    );
}