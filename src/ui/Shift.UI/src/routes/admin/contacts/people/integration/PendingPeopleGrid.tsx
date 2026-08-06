import { shiftClient } from "@/api/shiftClient";
import SearchGrid, { SearchGridFunctions } from "@/components/search/SearchGrid";
import { translate } from "@/helpers/translate";
import { mapQueryResult, QueryResult } from "@/models/QueryResult";
import { useMemo, useRef, useState } from "react";
import PendingPeopleGrid_ActionField from "./PendingPeopleGrid_ActionField";
import { ImportAction } from "./ImportAction";
import PendingPeopleGrid_SelectMatch from "./PendingPeopleGrid_SelectMatch";
import Button from "@/components/Button";
import { useStatusProvider } from "@/contexts/status/StatusProviderContext";
import { ApiImportPendingPerson } from "@/api/controllers/pending-people/ApiImportPendingPerson";
import { AlertType } from "@/components/Alert";
import { ApiImportResult } from "@/api/controllers/pending-people/ApiImportResult";

interface Row {
    pendingPersonId: string;
    personCode: string;
    email: string;
    firstName: string;
    lastName: string;
    employeeStatus: string;
    matchCount: number;
}

interface ActionList {
    [key: string]: ImportAction;
}

interface Selected {
    pendingPersonId: string;
    userId: string | null;
    firstName: string;
    lastName: string;
}

interface Props {
    onCountUpdated(count: number): void;
    onImported(): void;
}

export default function PendingPeopleGrid({ onCountUpdated, onImported }: Props) {
    const [actions, setActions] = useState<ActionList>({});
    const [pendingPersonIds, setPendingPesonIds] = useState<string[]>([]);
    const [selected, setSelected] = useState<Selected | null>(null);
    const [isImporting, setIsImporting] = useState(false);

    const { addError, removeError, addStatus, removeStatus } = useStatusProvider();

    const stats = useMemo(() => {
        const newStats = {
            ignore: 0,
            create: 0,
            match: 0,
        };
        for (const action of Object.values(actions)) {
            switch(action.name) {
                case "create":
                    newStats.create++;
                    break;
                case "match":
                    newStats.match++;
                    break;
            }
        }
        newStats.ignore = pendingPersonIds.length - newStats.create - newStats.match;
        return newStats;
    }, [actions, pendingPersonIds]);

    const gridRef = useRef<SearchGridFunctions>(null);

    async function handleLoad(pageIndex: number) {
        const queryResult = await load(pageIndex);
        setPendingPesonIds(queryResult.rows.map(x => x.pendingPersonId));
        onCountUpdated(queryResult.totalRowCount);
        return queryResult;
    }

    function handleActionChange(row: Row, actionName: ImportAction["name"], usePreSelectedMatch: boolean) {
        if (actionName === "match" && (!usePreSelectedMatch || !actions[row.pendingPersonId]?.match)) {
            setSelected({
                pendingPersonId: row.pendingPersonId,
                userId: actions[row.pendingPersonId]?.match?.userId ?? null,
                firstName: row.firstName,
                lastName: row.lastName,
            });
        } else {
            setActions(prev => {
                const newActions = {...prev};
                newActions[row.pendingPersonId] = { name: actionName, match: newActions[row.pendingPersonId]?.match ?? null };
                return newActions;
            });
        }
    }

    function handleSelectMatch(match: ImportAction["match"]) {
        if (!selected) {
            return;
        }

        if (match) {
            setActions(prev => {
                const newActions = {...prev};
                newActions[selected.pendingPersonId] = { name: "match", match };
                return newActions;
            });
        }

        setSelected(null);
    }

    function handleClickMatch(e: React.MouseEvent<HTMLAnchorElement>, row: Row) {
        e.preventDefault();

        if (!isImporting) {
            handleActionChange(row, "match", false);
        }
    }

    async function handleImport()
    {
        const count = stats.create + stats.match;
        if (count === 0) {
            window.alert("Nothing to import");
            return;
        }

        const peopleText = createPeopleText(count);
        const question = `Are you sure to import ${peopleText}?`;

        if (!window.confirm(question)) {
            return;
        }

        const imports = createImports(actions, pendingPersonIds);

        removeStatus();

        setIsImporting(true);

        try {
            const result = await shiftClient.pendingPeople.import(imports);
            if (!result) {
                return;
            }

            const { type, message } = createResultMessage(result);

            setActions({});

            await gridRef.current?.refresh();

            removeError();
            addStatus(type, message);

            onImported();
        } catch (err) {
            addError(err, "Error while importing");
        } finally {
            setIsImporting(false);
        }
    }

    return (
        <>
            <SearchGrid<Row>
                ref={gridRef}
                columns={[
                    {
                        key: "action",
                        title: translate("Action"),
                        item: row => (
                            <PendingPeopleGrid_ActionField
                                pendingPersonId={row.pendingPersonId}
                                action={actions[row.pendingPersonId] ?? { name: "ignore", match: null }}
                                disabled={isImporting}
                                onChange={actionName => handleActionChange(row, actionName, true)}
                                onSelectMatch={() => handleActionChange(row, "match", false)}
                            />
                        )
                    },
                    {
                        key: "name",
                        title: translate("Name"),
                        item: row => `${row.firstName} ${row.lastName}`
                    },
                    {
                        key: "personCode",
                        title: translate("Employee ID"),
                        item: row => row.personCode
                    },
                    {
                        key: "email",
                        title: translate("Email"),
                        item: row => row.email
                    },
                    {
                        key: "matches",
                        titleClassName: "text-center",
                        itemClassName: "text-center",
                        title: translate("Matches"),
                        item: row => (
                            <a href="#" onClick={e => handleClickMatch(e, row)}>
                                {row.matchCount}
                            </a>
                        )
                    },
                    {
                        key: "status",
                        titleClassName: "text-center",
                        itemClassName: "text-center",
                        title: translate("Status"),
                        item: row => row.employeeStatus,
                    },
                ]}
                onLoad={handleLoad}
            />

            <PendingPeopleGrid_SelectMatch
                pendingPerson={selected}
                onSelect={handleSelectMatch}
            />

            <Button
                variant="save"
                type="button"
                disabled={stats.create + stats.match === 0}
                text={translate("Import")}
                isLoading={isImporting}
                loadingMessage={translate("Importing...")}
                onClick={handleImport}
            />

            <span className="ms-3 form-text">
                {`Ignore = ${stats.ignore}, Create = ${stats.create}, Match = ${stats.match}`}
            </span>
        </>
    );
}

async function load(pageIndex: number): Promise<QueryResult<Row>> {
    const result = await shiftClient.pendingPeople.search(pageIndex);

    return mapQueryResult(result, row => ({
        pendingPersonId: row.PendingPersonId,
        personCode: row.PersonCode,
        email: row.UserEmail,
        firstName: row.UserFirstName,
        lastName: row.UserLastName,
        employeeStatus: row.EmployeeStatus,
        matchCount: row.MatchCount,
    }));
}

function createImports(actions: ActionList, pendingPersonIds: string[]): ApiImportPendingPerson[] {
    const result: ApiImportPendingPerson[] = [];

    for (const pendingPersonId of pendingPersonIds) {
        const action = actions[pendingPersonId];
        const matchUserId = action?.name === "match" ? action.match!.userId : null;

        result.push({ PendingPersonId: pendingPersonId, MatchUserId: matchUserId, Action: action?.name ?? "ignore" });
    }

    return result;
}

function createResultMessage(result: ApiImportResult): { type: AlertType, message: string } {
    if (result.ImportedPeople.length === 0) {
        return { type: "error", message: "Unexpected error" };
    }

    let importedCount = 0;
    let notChangedCount = 0;
    let ignoredCount = 0;
    const failedCodes: string[] = [];

    for (const person of result.ImportedPeople) {
        if (person.Failure) {
            failedCodes.push(person.PersonCode);
        } else if (person.Status === "Created" || person.Status === "Modified") {
            importedCount++;
        } else if (person.Status === "NotChanged") {
            notChangedCount++;
        } else {
            ignoredCount++;
        }
    }

    const type: AlertType = failedCodes.length > 0 ? "warning" : "success";
    const messages: string[] = [];

    if (importedCount > 0) {
        messages.push(`Imported ${createPeopleText(importedCount)}.`);
    }

    if (notChangedCount > 0) {
        messages.push(`No changes for ${createPeopleText(notChangedCount)}.`);
    }

    if (ignoredCount > 0) {
        messages.push(`Ignored ${createPeopleText(ignoredCount)}.`);
    }

    if (failedCodes.length > 0) {
        messages.push(`These people failed import: ${failedCodes.join(", ")}`);
    }

    return { type, message: messages.join(" ") };
}

function createPeopleText(count: number): string {
    return count === 1 ? "one person" : `${count} people`;
}