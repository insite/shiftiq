import { ImportAction } from "./ImportAction";

interface Props {
    pendingPersonId: string;
    action: ImportAction;
    disabled: boolean;
    onChange: (actionName: ImportAction["name"]) => void;
    onSelectMatch: () => void;
}

export default function PendingPeopleGrid_ActionField({
    pendingPersonId,
    action,
    disabled,
    onChange,
    onSelectMatch,
}: Props) {
    function handleSelectMatch(e: React.MouseEvent<HTMLAnchorElement>) {
        e.preventDefault();
        if (!disabled) {
            onSelectMatch();
        }
    }

    const groupName = `action_${pendingPersonId}`;

    return (
        <>
            <div>
                <label>
                    <input
                        type="radio"
                        name={groupName}
                        value="ignore"
                        checked={action.name === "ignore"}
                        disabled={disabled}
                        onChange={() => onChange("ignore")}
                    />
                    Ignore
                </label>
            </div>
            <div>
                <label>
                    <input
                        type="radio"
                        name={groupName}
                        value="create"
                        checked={action.name === "create"}
                        disabled={disabled}
                        onChange={() => onChange("create")}
                    />
                    Create
                </label>
            </div>
            <div>
                <label>
                    <input
                        type="radio"
                        name={groupName}
                        value="match"
                        checked={action.name === "match"}
                        disabled={disabled}
                        onChange={() => onChange("match")}
                    />
                    Match
                </label>
                <label>
                    {action.name === "match" && action.match && (
                        <>
                            :{" "}
                            <a href="#" onClick={handleSelectMatch}>
                                {action.match.userFullName}
                            </a>
                        </>
                    )}
                </label>
            </div>
        </>
    );
}