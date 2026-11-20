import { ReactNode, useMemo, useReducer } from "react";
import { WorkshopOtherProviderContext } from "./WorkshopOtherProviderContext";
import { WorkshopOtherState } from "./states/WorkshopOtherState";
import { WorkshopProblemQuestion } from "./models/WorkshopProblemQuestion";

interface InitStateAction {
    type: "initState";
    state: WorkshopOtherState;
}

interface ModifyProblemQuestionsAction {
    type: "modifyProblemQuestions";
    problemQuestions: WorkshopProblemQuestion[];
}

type Action = InitStateAction | ModifyProblemQuestionsAction;

const _initialState: WorkshopOtherState = {
    bankId: "empty",
    formId: null,
    specificationId: null,
    comments: null,
    attachments: null,
    problemQuestions: null,
    readOnly: true,
}

function reducer(state: WorkshopOtherState, action: Action): WorkshopOtherState {
    const { type } = action;
    switch (type) {
        case "initState":
        {
            return {
                ...action.state,
            };
        }

        case "modifyProblemQuestions":
        {
            return {
                ...state,
                problemQuestions: action.problemQuestions,
            }
        }

        default:
            throw new Error(`Unknown action: ${type}`);
    }
}

interface Props {
    children?: ReactNode;
}

export default function WorkshopOtherProvider({ children }: Props) {
    const [state, dispatch] = useReducer(reducer, _initialState);

    const methods = useMemo(() => ({
        initOtherState(state: WorkshopOtherState): void {
            dispatch({ type: "initState", state });
        },
        modifyProblemQuestions(problemQuestions: WorkshopProblemQuestion[]): void {
            dispatch({ type: "modifyProblemQuestions", problemQuestions });
        },
    }), [dispatch]);

    const providerValue = useMemo(() => ({
        ...state,
        ...methods,
    }), [methods, state]);

    return (
        <WorkshopOtherProviderContext.Provider value={providerValue}>
            {children}
        </WorkshopOtherProviderContext.Provider>
    );
}