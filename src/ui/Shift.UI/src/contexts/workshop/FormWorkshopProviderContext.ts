import { createContext, useContext } from "react";
import { DateTimeParts } from "@/helpers/date/dateTimeTypes";
import { FormWorkshopVerifiedQuestion } from "./models/FormWorkshopVerifiedQuestion";
import { FormWorkshopState } from "./states/FormWorkshopState";

type ContextData = FormWorkshopState & {
    initState: (state: FormWorkshopState) => void;
    modifyThirdPartyAssessment: (enabled: boolean) => void;
    modifyVerifiedQuestions: (questionOrderMatch: boolean, questionOrderVerified: DateTimeParts | null, verifiedQuestions: FormWorkshopVerifiedQuestion[] | null) => void;
}

export const FormWorkshopProviderContext = createContext<ContextData>({
    bankId: "empty",
    formId: "empty",
    details: null,
    statistics: null,
    readOnly: true,
    initState() {},
    modifyThirdPartyAssessment() {},
    modifyVerifiedQuestions() {},
});

export function useFormWorkshopProvider() {
    return useContext(FormWorkshopProviderContext);
}