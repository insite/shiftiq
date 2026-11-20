import { createContext, useContext } from "react";
import { WorkshopOtherState } from "./states/WorkshopOtherState";
import { WorkshopProblemQuestion } from "./models/WorkshopProblemQuestion";

type ContextData = WorkshopOtherState & {
    initOtherState: (state: WorkshopOtherState) => void;
    modifyProblemQuestions: (problemQuestions: WorkshopProblemQuestion[]) => void;
}

export const WorkshopOtherProviderContext = createContext<ContextData>({
    bankId: "empty",
    formId: null,
    specificationId: null,
    comments: null,
    attachments: null,
    problemQuestions: null,
    readOnly: true,
    initOtherState() {},
    modifyProblemQuestions() {},
});

export function useWorkshopOtherProvider() {
    return useContext(WorkshopOtherProviderContext);
}