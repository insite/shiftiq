import { ReactNode, useMemo, useReducer } from "react";
import { FormWorkshopProviderContext } from "./FormWorkshopProviderContext";
import { DateTimeParts } from "@/helpers/date/dateTimeTypes";
import { FormWorkshopVerifiedQuestion } from "./models/FormWorkshopVerifiedQuestion";
import { FormWorkshopState } from "./states/FormWorkshopState";

interface InitStateAction {
    type: "initState";
    state: FormWorkshopState;
}

interface ModifyThirdPartyAssessmentAction {
    type: "modifyThirdPartyAssessment";
    enabled: boolean;
}

interface ModifyVerifiedQuestionsAction {
    type: "modifyVerifiedQuestions",
    questionOrderMatch: boolean;
    questionOrderVerified: DateTimeParts | null;
    verifiedQuestions: FormWorkshopVerifiedQuestion[] | null;
}

type Action = InitStateAction | ModifyThirdPartyAssessmentAction | ModifyVerifiedQuestionsAction;

const _initialState: FormWorkshopState = {
    bankId: "empty",
    formId: "empty",
    details: null,
    statistics: null,
    readOnly: true,
}

function reducer(state: FormWorkshopState, action: Action): FormWorkshopState {
    const { type } = action;
    switch (type) {
        case "initState":
        {
            return {
                ...action.state,
            };
        }

        case "modifyThirdPartyAssessment":
        {
            return {
                ...state,
                details: {
                    ...state.details!,
                    thirdPartyAssessmentEnabled: action.enabled,
                }
            };
        }

        case "modifyVerifiedQuestions":
        {
            return {
                ...state,
                details: {
                    ...state.details!,
                    questionOrderMatch: action.questionOrderMatch,
                    questionOrderVerified: action.questionOrderVerified,
                    verifiedQuestions: action.verifiedQuestions,
                }
            };
        }

        default:
            throw new Error(`Unknown action: ${type}`);
    }
}

interface Props {
    children?: ReactNode;
}

export default function FormWorkshopProvider({ children }: Props) {
    const [state, dispatch] = useReducer(reducer, _initialState);

    const methods = useMemo(() => ({
        initState(state: FormWorkshopState): void {
            dispatch({ type: "initState", state });
        },
        modifyThirdPartyAssessment(enabled: boolean): void {
            dispatch({ type: "modifyThirdPartyAssessment", enabled });
        },
        modifyVerifiedQuestions(questionOrderMatch: boolean, questionOrderVerified: DateTimeParts | null, verifiedQuestions: FormWorkshopVerifiedQuestion[] | null): void {
            dispatch({ type: "modifyVerifiedQuestions", questionOrderMatch, questionOrderVerified, verifiedQuestions });
        },
    }), [dispatch]);

    const providerValue = useMemo(() => ({
        ...state,
        ...methods,
    }), [methods, state]);

    return (
        <FormWorkshopProviderContext.Provider value={providerValue}>
            {children}
        </FormWorkshopProviderContext.Provider>
    );
}