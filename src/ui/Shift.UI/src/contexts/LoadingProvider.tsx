import { createContext, ReactNode, useContext, useEffect, useMemo, useReducer, useState } from "react";
import { Spinner } from "react-bootstrap";
import "./LoadingProvider.css";

interface ContextData {
    addLoading: (actionKey: string) => void;
    removeLoading: (actionKey: string) => void;
    clearLoading: (actionKey: string) => void;
}

const LoadingProviderContext = createContext<ContextData>({
    addLoading: () => {},
    removeLoading: () => {},
    clearLoading: () => {},
});

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

export default function LoadingContext({ className, children, disabled }: Props) {
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

let _counter = 1;

// eslint-disable-next-line react-refresh/only-export-components
export function useLoadingProvider() {
    const { addLoading, removeLoading, clearLoading } = useContext(LoadingProviderContext);
    const [actionKey] = useState(() => `sequential-action-key.${_counter++}`);

    const methods = useMemo(() => ({
        addLoading: () => addLoading(actionKey),
        removeLoading: () => removeLoading(actionKey),
    }), [addLoading, removeLoading, actionKey]);

    useEffect(() => () => clearLoading(actionKey), [clearLoading, actionKey]);

    return methods;
}