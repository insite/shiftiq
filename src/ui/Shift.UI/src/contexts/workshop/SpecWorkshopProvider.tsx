import { ReactNode, useMemo, useReducer } from "react";
import { SpecWorkshopCriterion } from "./models/SpecWorkshopCriterion";
import { SpecWorkshopCriterionCompetency } from "./models/SpecWorkshopCriterionCompetency";
import { SpecWorkshopState } from "./states/SpecWorkshopState";
import { SpecWorkshopProviderContext } from "./SpecWorkshopProviderContext";

interface InitStateAction {
    type: "initState";
    state: SpecWorkshopState;
}

interface ModifyFormLimitAction {
    type: "modifyFormLimit";
    formLimit: number;
}

interface ModifyQuestionLimitAction {
    type: "modifyQuestionLimit";
    questionLimit: number;
}

interface ModifyCriterionWeightAction {
    type: "modifyCriterionWeight";
    criterionId: string;
    weight: number;
}

interface ModifyTax1CountAction {
    type: "modifyTax1Count";
    criterionId: string;
    standardId: string;
    tax1Count: number | null;
}

interface ModifyTax2CountAction {
    type: "modifyTax2Count";
    criterionId: string;
    standardId: string;
    tax2Count: number | null;
}

interface ModifyTax3CountAction {
    type: "modifyTax3Count";
    criterionId: string;
    standardId: string;
    tax3Count: number | null;
}

type Action = InitStateAction | ModifyFormLimitAction | ModifyQuestionLimitAction | ModifyCriterionWeightAction | ModifyTax1CountAction | ModifyTax2CountAction | ModifyTax3CountAction;

const _initialState: SpecWorkshopState = {
    bankId: "empty",
    specificationId: "empty",
    details: null,
    readOnly: true,
}

function modifyCriterion(
    state: SpecWorkshopState,
    criterionId: string,
    action: (criterion: SpecWorkshopCriterion) => SpecWorkshopCriterion
): SpecWorkshopState {
    const criterion = state.details?.criteria.find(x => x.criterionId === criterionId);
    if (!criterion) {
        throw new Error(`Criterion ${criterionId} is not found`);
    }
    return {
        ...state,
        details: {
            ...state.details!,
            criteria: state.details!.criteria.map(x => x.criterionId === criterionId ? action(x) : x)
        }
    };
}

function modifyCompetency(
    state: SpecWorkshopState,
    criterionId: string,
    standardId: string,
    action: (criterion: SpecWorkshopCriterionCompetency) => SpecWorkshopCriterionCompetency
): SpecWorkshopState {
    return modifyCriterion(state, criterionId, criterion => {
        const competency = criterion.competencies.find(x => x.standard.standardId === standardId);
        if (!competency) {
            throw new Error(`Competency ${standardId} is not found`);
        }
        return {
            ...criterion,
            competencies: criterion.competencies.map(x => x.standard.standardId === standardId ? action(x) : x)
        };
    });
}

function reducer(state: SpecWorkshopState, action: Action): SpecWorkshopState {
    const { type } = action;
    switch (type) {
        case "initState":
        {
            return {
                ...action.state,
            };
        }

        case "modifyFormLimit":
        {
            return {
                ...state,
                details: {
                    ...state.details!,
                    formLimit: action.formLimit,
                }
            };
        }

        case "modifyQuestionLimit":
        {
            return {
                ...state,
                details: {
                    ...state.details!,
                    questionLimit: action.questionLimit,
                }
            };
        }

        case "modifyCriterionWeight":
            return modifyCriterion(state, action.criterionId, criterion => ({...criterion, weight: action.weight}));

        case "modifyTax1Count":
            return modifyCompetency(state, action.criterionId, action.standardId, competency => ({...competency, tax1Count: action.tax1Count}));

        case "modifyTax2Count":
            return modifyCompetency(state, action.criterionId, action.standardId, competency => ({...competency, tax2Count: action.tax2Count}));

        case "modifyTax3Count":
            return modifyCompetency(state, action.criterionId, action.standardId, competency => ({...competency, tax3Count: action.tax3Count}));

        default:
            throw new Error(`Unknown action: ${type}`);
    }
}

interface Props {
    children?: ReactNode;
}

export default function SpecWorkshopProvider({ children }: Props) {
    const [state, dispatch] = useReducer(reducer, _initialState);

    const methods = useMemo(() => ({
        initState(state: SpecWorkshopState): void {
            dispatch({ type: "initState", state });
        },
        modifyFormLimit(formLimit: number): void {
            dispatch({ type: "modifyFormLimit", formLimit });
        },
        modifyQuestionLimit(questionLimit: number): void {
            dispatch({ type: "modifyQuestionLimit", questionLimit });
        },
        modifyCriterionWeight(criterionId: string, weight: number): void {
            dispatch({ type: "modifyCriterionWeight", criterionId, weight });
        },
        modifyTax1Count(criterionId: string, standardId: string, tax1Count: number | null): void {
            dispatch({ type: "modifyTax1Count", criterionId, standardId, tax1Count });
        },
        modifyTax2Count(criterionId: string, standardId: string, tax2Count: number | null): void {
            dispatch({ type: "modifyTax2Count", criterionId, standardId, tax2Count });
        },
        modifyTax3Count(criterionId: string, standardId: string, tax3Count: number | null): void {
            dispatch({ type: "modifyTax3Count", criterionId, standardId, tax3Count });
        },
    }), [dispatch]);

    const providerValue = useMemo(() => ({
        ...state,
        ...methods,
    }), [methods, state]);

    return (
        <SpecWorkshopProviderContext.Provider value={providerValue}>
            {children}
        </SpecWorkshopProviderContext.Provider>
    );
}