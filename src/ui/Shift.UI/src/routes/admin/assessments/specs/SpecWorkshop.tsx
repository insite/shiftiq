import SpecWorkshopProvider from "@/contexts/workshop/SpecWorkshopProvider";
import WorkshopOtherProvider from "@/contexts/workshop/WorkshopOtherProvider";
import WorkshopQuestionProvider from "@/contexts/workshop/WorkshopQuestionProvider";
import { useEffect, useRef, useState } from "react";
import { useParams, useSearchParams } from "react-router";
import { workshopQuestionFilter, WorkshopQuestionFilterState } from "../questions/workshopQuestionFilter";
import { useSiteProvider } from "@/contexts/site/SiteProviderContext";
import { usePageProvider } from "@/contexts/page/PageProviderContext";
import { useWorkshopOtherProvider } from "@/contexts/workshop/WorkshopOtherProviderContext";
import { useWorkshopQuestionProvider } from "@/contexts/workshop/WorkshopQuestionProviderContext";
import { useSpecWorkshopProvider } from "@/contexts/workshop/SpecWorkshopProviderContext";
import { useLoadAction } from "@/hooks/useLoadAction";
import { shiftClient } from "@/api/shiftClient";
import { workshopOtherAdapter } from "../other/workshopOtherAdapter";
import { workshopQuestionAdapter } from "../questions/workshopQuestionAdapter";
import { specWorkshopAdapter } from "./specWorkshopAdapter";
import { WorkshopAreaCompetencies } from "@/contexts/workshop/models/WorkshopAreaCompetencies";
import { WorkshopStandard } from "@/contexts/workshop/models/WorkshopStandard";
import FormTabs from "@/components/form/FormTabs";
import FormTab from "@/components/form/FormTab";
import { translate } from "@/helpers/translate";
import WorkshopQuestions from "../questions/WorkshopQuestions";
import WorkshopComments from "../other/WorkshopComments";
import WorkshopAttachments from "../other/WorkshopAttachments";
import WorkshopProblems from "../other/WorkshopProblems";
import Button from "@/components/Button";
import SpecWorkshop_Details from "./SpecWorkshop_Details";
import ValidationSummary from "@/components/ValidationSummary";
import { useForm } from "react-hook-form";
import { useSaveAction } from "@/hooks/useSaveAction";
import { SpecWorkshopDetailsValues, toApiSpecWorkshopInput, toSpecWorkshopDetailsValues } from "./SpecWorkshopDetailsValues";
import SpecWorkshop_BankView from "./SpecWorkshop_BankView";

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

export default function SpecWorkshop() {
    return (
        <WorkshopOtherProvider>
            <WorkshopQuestionProvider>
                <SpecWorkshopProvider>
                    <SpecWorkshopInternal />
                </SpecWorkshopProvider>
            </WorkshopQuestionProvider>
        </WorkshopOtherProvider>
    );
}

function SpecWorkshopInternal() {
    const params = useParams();
    const id = params["id"];
    const [searchParams] = useSearchParams();
    const tab = searchParams.get("tab");
    const selectedQuestionId = searchParams.get("question") ?? null;
    const serializedFilter = searchParams.get("filter");

    const [backUrl, setBackUrl] = useState(outlinePageUrl);
    const [counts, setCounts] = useState(defaultCounts);
    const [defaultFilter, setDefaultFilter] = useState<WorkshopQuestionFilterState | null>(null);
    const [status, setStatus] = useState<"saved" | "error" | "none">("none");

    const timeoutRef = useRef<number>(null);

    const { siteSetting: { TimeZoneId: timeZoneId } } = useSiteProvider();
    const { setActionSubtitle, setBreadcrumbItemPath } = usePageProvider();
    const { initOtherState } = useWorkshopOtherProvider();
    const { initQuestionState } = useWorkshopQuestionProvider();
    const { initState, details, readOnly, specificationId } = useSpecWorkshopProvider();
    const { isSaving, runSave } = useSaveAction();
    const {
        control,
        handleSubmit,
        reset,
        formState: { errors },
    } = useForm<SpecWorkshopDetailsValues>({
        defaultValues: {
            formLimit: null,
            questionLimit: null,
            criteria: [],
        }
    });
    const initializedSpecificationIdRef = useRef<string | null>(null);

    const { runLoad } = useLoadAction(async () => {
        if (!id) {
            throw new Error("ID is not provided");
        }

        const newDefaultFilter = serializedFilter ? workshopQuestionFilter.deserializeFilter(serializedFilter) : null;

        const model = await shiftClient.workshop.retrieveSpec(id, newDefaultFilter?.sectionId ?? null, selectedQuestionId);
        if (!model) {
            return;
        }

        const questionChangeDates = newDefaultFilter && workshopQuestionFilter.hasDateFilter(newDefaultFilter)
            ? await shiftClient.workshop.collectQuestionChangeDates(model.BankId)
            : null;
            
        const { framework, areas, competencies } = workshopQuestionAdapter.getCompetencies(model.Standards, true);
        const areaCompetencies = createAreaCompetencies(competencies);

        const otherState = workshopOtherAdapter.getState(model.BankId, null, id, model.Comments, model.Attachments, model.ProblemQuestions, timeZoneId);
        const questionState = workshopQuestionAdapter.getState(model.BankId, null, id, model.QuestionData, questionChangeDates, areaCompetencies);
        const specState = specWorkshopAdapter.getState(id, model, framework, areas, competencies);

        initOtherState(otherState);
        initQuestionState(questionState);
        initState(specState);

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

        const newBackUrl = `${outlinePageUrl}?bank=${specState.bankId}&spec=${id}`;

        setBackUrl(newBackUrl);
        setActionSubtitle(specState.details!.specName, `Specification Asset #${specState.details!.assetNumber}`);
        setBreadcrumbItemPath(outlinePageUrl, newBackUrl);
    });

    useEffect(() => { runLoad(); }, [runLoad]);

    useEffect(() => {
        if (!details) {
            return;
        }

        if (initializedSpecificationIdRef.current === specificationId) {
            return;
        }

        reset(toSpecWorkshopDetailsValues(details));
        initializedSpecificationIdRef.current = specificationId;
    }, [details, reset, specificationId]);

    async function handleValidSubmit(values: SpecWorkshopDetailsValues) {
        if (!details) {
            return;
        }

        if (timeoutRef.current) {
            clearTimeout(timeoutRef.current);
        }

        setStatus("none");

        const input = toApiSpecWorkshopInput(details, values);

        if (await runSave(async () => {
            await shiftClient.workshop.modifySpec(specificationId, input);
        })) {
            setStatus("saved");
        } else {
            setStatus("error");
        }

        timeoutRef.current = window.setTimeout(() => {
            timeoutRef.current = null;
            setStatus("none");
        }, 5000);
    }

    return (
        <form autoComplete="off" onSubmit={handleSubmit(handleValidSubmit)}>
            <ValidationSummary errors={errors} />

            <FormTabs defaultTab={tab == "bankview" || tab === "questions" || tab === "comments" || tab === "attachments" || tab === "problems" ? tab : "spec"}>
                <FormTab tab="spec" title={translate("Specification")} icon={{ style: "regular", name: "clipboard-list" }}>
                    <SpecWorkshop_Details
                        control={control}
                        isSaving={isSaving}
                    />
                </FormTab>
                <FormTab tab="bankview" title={translate("Bank View")} icon={{ style: "regular", name: "balance-scale" }}>
                    <SpecWorkshop_BankView />
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

            <div className="mt-3">
                {!readOnly && (
                    <Button
                        variant="save"
                        className="me-2"
                        isLoading={isSaving}
                    />
                )}
                <Button
                    variant="close"
                    href={backUrl}
                    type="button"
                    disabled={isSaving}
                />

                {status === "saved" && (
                    <span className="ms-2 p-1 badge text-bg-dark">Changes are saved</span>
                )}
                {status === "error" && (
                    <span className="ms-2 p-1 badge text-bg-danger">Error while trying saving changes</span>
                )}
            </div>
        </form>
    );
}

function createAreaCompetencies(competencies: WorkshopStandard[]): WorkshopAreaCompetencies {
    const result: WorkshopAreaCompetencies = competencies.reduce((prev, cur) => {
        const list = prev[cur.parent!.standardId] ?? (prev[cur.parent!.standardId] = []);
        list.push(cur);
        return prev;
    }, {} as WorkshopAreaCompetencies);
    return result;
}
