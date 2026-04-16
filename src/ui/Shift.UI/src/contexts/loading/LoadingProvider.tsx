import { ReactNode, useMemo, useReducer } from "react";
import { Spinner } from "react-bootstrap";
import "./LoadingProvider.css";
import { LoadingProviderContext } from "./LoadingProviderContext";

interface AddAction {
    type: "add";
    actionKey: string;
}

interface RemoveAction {
    type: "remove";
    actionKey: string;
}

interface ClearAction {
    type: "clear";
    actionKey: string;
}

type Action = AddAction | RemoveAction | ClearAction;

interface State {
    refs: {
        actionKey: string;
        count: number;
    }[];
}

const _initialState: State = {
    refs: []
}

function reducer(state: State, action: Action): State {
    const { type, actionKey } = action;
    switch (type) {
        case "add":
        {
            const refToAdd = state.refs.find(x => x.actionKey === actionKey);
            return {...state, refs: [
                ...state.refs.filter(x => x.actionKey !== actionKey),
                {
                    actionKey,
                    count: refToAdd ? refToAdd.count + 1 : 1
                }
            ]};
        }

        case "remove":
        {
            const refToRemove = state.refs.find(x => x.actionKey === actionKey);
            const newState = {...state, refs:
                state.refs.filter(x => x.actionKey !== actionKey)
            };
            if (refToRemove && refToRemove.count > 1) {
                newState.refs.push({
                    actionKey,
                    count: refToRemove.count - 1
                })
            }
            return newState;
        }

        case "clear":
                return {...state, refs:
                    state.refs.filter(x => x.actionKey !== actionKey)
                };

        default:
            throw new Error(`Unknown action: ${type}`);
    }
}

interface Props {
    className?: string;
    children?: ReactNode;
    disabled?: boolean;
}

export default function LoadingProvider({ className, children, disabled }: Props) {
    const [{ refs }, dispatch] = useReducer(reducer, _initialState);

    const providerValue = useMemo(() => ({
        addLoading: (actionKey: string) => dispatch({ type: "add", actionKey }),
        removeLoading: (actionKey: string) => dispatch({ type: "remove", actionKey }),
        clearLoading: (actionKey: string) => dispatch({ type: "clear", actionKey }),
    }), [dispatch]);

    return (
        <LoadingProviderContext.Provider value={providerValue}>
            <div className={"loading-provider " + (className ?? "")}>
                <div
                    data-visible={disabled !== true && refs.length ? true : undefined}
                    className="backdrop-panel"
                >
                    <div className="loading-panel shadow-lg">
                        <Spinner animation="border" role="status" className="me-3" />
                        Loading...
                    </div>
                </div>

                {children}
            </div>
        </LoadingProviderContext.Provider>
    );
}