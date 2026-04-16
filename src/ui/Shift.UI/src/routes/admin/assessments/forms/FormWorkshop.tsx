import { shiftClient } from "@/api/shiftClient";
import { usePageProvider } from "@/contexts/page/PageProviderContext";
import { useLoadAction } from "@/hooks/useLoadAction";
import { useEffect, useState } from "react";
import { useParams, useSearchParams } from "react-router";
import { formWorkshopAdapter } from "./formWorkshopAdapter";
import { useSiteProvider } from "@/contexts/site/SiteProviderContext";
import Button from "@/components/Button";
import { translate } from "@/helpers/translate";
import FormWorkshop_FormDetails from "./FormWorkshop_FormDetails";
import FormTab from "@/components/form/FormTab";
import FormTabs from "@/components/form/FormTabs";
import { useFormWorkshopProvider } from "@/contexts/workshop/FormWorkshopProviderContext";
import FormWorkshopProvider from "@/contexts/workshop/FormWorkshopProvider";
import { workshopOtherAdapter } from "../other/workshopOtherAdapter";
import WorkshopOtherProvider from "@/contexts/workshop/WorkshopOtherProvider";
import { useWorkshopOtherProvider } from "@/contexts/workshop/WorkshopOtherProviderContext";
import WorkshopQuestionProvider from "@/contexts/workshop/WorkshopQuestionProvider";
import { useWorkshopQuestionProvider } from "@/contexts/workshop/WorkshopQuestionProviderContext";
import WorkshopQuestions from "../questions/WorkshopQuestions";
import { workshopQuestionFilter, WorkshopQuestionFilterState } from "../questions/workshopQuestionFilter";
import { workshopQuestionAdapter } from "../questions/workshopQuestionAdapter";
import WorkshopComments from "../other/WorkshopComments";
import WorkshopAttachments from "../other/WorkshopAttachments";
import WorkshopProblems from "../other/WorkshopProblems";

const outlinePageUrl = "/ui/admin/assessments/banks/outline";

interface Counts {
    questions: number;
    comments: number;
    attachments: number;
    problems: number;
}

const defaultCounts: Counts = {
    questions: 0,
    comments: 0,
    attachments: 0,
    problems: 0,
};

function getSubtitle(count: number) {
    return count ? `(${count})` : undefined;
}

export default function FormWorkshop() {
    return (
        <WorkshopOtherProvider>
            <WorkshopQuestionProvider>
                <FormWorkshopProvider>
                    <FormWorkshopInternal />
                </FormWorkshopProvider>
            </WorkshopQuestionProvider>
        </WorkshopOtherProvider>
    );
}

function FormWorkshopInternal() {
    const params = useParams();
    const id = params["id"];
    const [searchParams] = useSearchParams();
    const tab = searchParams.get("tab");
    const selectedQuestionId = searchParams.get("question") ?? null;
    const serializedFilter = searchParams.get("filter");

    const [backUrl, setBackUrl] = useState(outlinePageUrl);
    const [counts, setCounts] = useState(defaultCounts);
    const [defaultFilter, setDefaultFilter] = useState<WorkshopQuestionFilterState | null>(null);

    const { siteSetting: { TimeZoneId: timeZoneId } } = useSiteProvider();
    const { setActionSubtitle, setBreadcrumbItemPath } = usePageProvider();
    const { initOtherState } = useWorkshopOtherProvider();
    const { initQuestionState } = useWorkshopQuestionProvider();
    const { initState } = useFormWorkshopProvider();

    const { runLoad } = useLoadAction(async () => {
        if (!id) {
            throw new Error("ID is not provided");
        }

        const newDefaultFilter = serializedFilter ? workshopQuestionFilter.deserializeFilter(serializedFilter) : null;

        const model = await shiftClient.workshop.retrieveForm(id, newDefaultFilter?.sectionId ?? null, selectedQuestionId);
        if (!model) {
            return;
        }

        const questionChangeDates = newDefaultFilter && workshopQuestionFilter.hasDateFilter(newDefaultFilter)
            ? await shiftClient.workshop.collectQuestionChangeDates(model.BankId)
            : null;

        const otherState = workshopOtherAdapter.getState(model.BankId, id, null, model.Comments, model.Attachments, model.ProblemQuestions, timeZoneId);
        const questionState = workshopQuestionAdapter.getState(model.BankId, id, null, model.QuestionData, questionChangeDates, null);
        const formState = formWorkshopAdapter.getState(id, model, timeZoneId);

        initOtherState(otherState);
        initQuestionState(questionState);
        initState(formState);

        if (newDefaultFilter) {
            setDefaultFilter(newDefaultFilter);
        } else if (selectedQuestionId) {
            const selectedQuestion = questionState.sectionQuestions?.find(x => x.questionId === selectedQuestionId);
            if (selectedQuestion) {
                setDefaultFilter(workshopQuestionFilter.createWorkshopQuestionsFilterState(questionState.sectionId, selectedQuestion.standardId));
            }
        }

        setCounts({
            questions: questionState.totalQuestionCount,
            comments: otherState.comments!.length,
            attachments: otherState.attachments!.length,
            problems: otherState.problemQuestions!.length,
        });

        const newBackUrl = `${outlinePageUrl}?bank=${formState.bankId}&form=${id}`;

        setBackUrl(newBackUrl);
        setActionSubtitle(formState.details!.formName);
        setBreadcrumbItemPath(outlinePageUrl, newBackUrl);
    });

    useEffect(() => { runLoad(); }, [runLoad]);

    return (
        <>
            <FormTabs defaultTab={tab === "questions" || tab === "comments" || tab === "attachments" || tab === "problems" ? tab : "form"}>
                <FormTab tab="form" title={translate("Form")} icon={{ style: "regular", name: "window" }}>
                    <FormWorkshop_FormDetails />
                </FormTab>
                <FormTab tab="questions" title={translate("Questions")} subtitle={getSubtitle(counts.questions)} icon={{ style: "regular", name: "question" }}>
                    <WorkshopQuestions
                        selectedQuestionId={selectedQuestionId}
                        defaultFilter={defaultFilter}
                    />
                </FormTab>
                <FormTab tab="comments" title={translate("Comments")} subtitle={getSubtitle(counts.comments)} icon={{ style: "regular", name: "comments" }}>
                    <WorkshopComments />
                </FormTab>
                <FormTab tab="attachments" title={translate("Attachments")} subtitle={getSubtitle(counts.attachments)} icon={{ style: "regular", name: "paperclip" }}>
                    <WorkshopAttachments />
                </FormTab>
                <FormTab tab="problems" title={translate("Problems")} subtitle={getSubtitle(counts.problems)} icon={{ style: "regular", name: "exclamation-triangle" }}>
                    <WorkshopProblems />
                </FormTab>
            </FormTabs>

            <Button
                variant="close"
                href={backUrl}
            />
        </>
    );
}