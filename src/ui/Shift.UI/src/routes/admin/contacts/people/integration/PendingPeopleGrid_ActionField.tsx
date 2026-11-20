import IconButton from "@/components/iconbutton/IconButton";
import { ImportAction } from "./ImportAction";
import { urlHelper } from "@/helpers/urlHelper";

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
    function handleSelectMatch(e: React.MouseEvent) {
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
                            <a
                                target="_blank"
                                href={urlHelper.getActionUrl(`/ui/admin/contacts/people/edit?contact=${action.match.userId}`)}
                            >
                                {action.match.userFullName}
                            </a>
                            <IconButton
                                iconStyle="solid"
                                iconName="pencil"
                                className="ms-1 fs-xs"
                                title="Change Match"
                                onClick={handleSelectMatch}
                            />
                        </>
                    )}
                </label>
            </div>
        </>
    );
}