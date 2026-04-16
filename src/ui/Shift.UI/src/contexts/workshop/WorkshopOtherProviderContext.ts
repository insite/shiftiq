import { createContext, useContext } from "react";
import { WorkshopOtherState } from "./states/WorkshopOtherState";

type ContextData = WorkshopOtherState & {
    initOtherState: (state: WorkshopOtherState) => void;
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
});

export function useWorkshopOtherProvider() {
    return useContext(WorkshopOtherProviderContext);
}