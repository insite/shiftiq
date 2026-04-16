import "./WorkshopQuestions.css";

import { shiftClient } from "@/api/shiftClient";
import Alert from "@/components/Alert";
import FormCard from "@/components/form/FormCard";
import { useLoadingProvider } from "@/contexts/loading/LoadingProviderContext";
import { useStatusProvider } from "@/contexts/status/StatusProviderContext";
import { useWorkshopQuestionProvider } from "@/contexts/workshop/WorkshopQuestionProviderContext";
import { WorkshopStandard } from "@/contexts/workshop/models/WorkshopStandard";
import { useEffect, useMemo, useRef, useState } from "react";
import { workshopQuestionAdapter } from "./workshopQuestionAdapter";
import WorkshopQuestions_Filter from "./WorkshopQuestions_Filter";
import WorkshopQuestions_Row from "./WorkshopQuestions_Row";
import { workshopQuestionFilter, WorkshopQuestionFilterState } from "./workshopQuestionFilter";
import { useSiteProvider } from "@/contexts/site/SiteProviderContext";
import { WorkshopQuestion } from "@/contexts/workshop/models/WorkshopQuestion";
import { WorkshopAreaCompetencies } from "@/contexts/workshop/models/WorkshopAreaCompetencies";

interface Props {
    selectedQuestionId: string | null;
    defaultFilter: WorkshopQuestionFilterState | null;
}

export default function WorkshopQuestions({ selectedQuestionId, defaultFilter }: Props) {
    const {
        bankId,
        formId,
        specificationId,
        areaCompetencies,
        sections,
        sectionId,
        sectionCompetencies,
        sectionQuestions,
        questionChangeDates,
        selectSection,
        setSectionData,
        setQuestionChangeDates,
    } = useWorkshopQuestionProvider();

    const { addLoading, removeLoading } = useLoadingProvider();
    const { addError, removeError } = useStatusProvider();
    const { siteSetting: { TimeZoneId: timeZoneId } } = useSiteProvider();

    const [draftFilter, setDraftFilter] = useState<WorkshopQuestionFilterState>(() => workshopQuestionFilter.createWorkshopQuestionsFilterState(null, null));
    const [appliedFilter, setAppliedFilter] = useState<WorkshopQuestionFilterState>(() => workshopQuestionFilter.createWorkshopQuestionsFilterState(null, null));
    const [isFilterLoading, setIsFilterLoading] = useState(false);

    const scrolledToQuestionRef = useRef<boolean>(false);

    const tableRef = useRef<HTMLTableElement>(null);

    const filteredQuestions = useMemo(() => {
        return workshopQuestionFilter.filterQuestions(sectionQuestions, appliedFilter, questionChangeDates, timeZoneId);
    }, [sectionQuestions, appliedFilter, questionChangeDates, timeZoneId]);

    const serializedFilter = useMemo(() => {
        return workshopQuestionFilter.serializeFilter(appliedFilter);
    }, [appliedFilter]);

    useEffect(() => {
        const defaultCompetencyId = getDefaultCompetencyId(sectionCompetencies);

        setAppliedFilter(prev => ({
            ...prev,
            sectionId,
            competencyId: prev.competencyId === "" || sectionCompetencies?.some(x => x.standardId === prev.competencyId) ? prev.competencyId: defaultCompetencyId,
        }));
        setDraftFilter(prev => ({
            ...prev,
            sectionId,
            competencyId: prev.competencyId === "" || sectionCompetencies?.some(x => x.standardId === prev.competencyId) ? prev.competencyId: defaultCompetencyId,
        }));
    }, [sectionId, sectionCompetencies]);

    useEffect(() => {
        if (defaultFilter) {
            setDraftFilter(defaultFilter);
            setAppliedFilter(defaultFilter);
        }
    }, [defaultFilter]);

    useEffect(() => {
        if (scrolledToQuestionRef.current || !selectedQuestionId || !filteredQuestions.some(question => question.questionId === selectedQuestionId)) {
            return;
        }

        moveToQuestion(selectedQuestionId);

        scrolledToQuestionRef.current = true;
    }, [selectedQuestionId, filteredQuestions]);

    async function handleSectionChange(nextSectionId: string | null) {
        if (!nextSectionId || nextSectionId === sectionId || !sections) {
            return;
        }

        const previousSectionId = sectionId;
        const nextSection = sections.find(section => section.sectionId === nextSectionId);
        if (!nextSection) {
            throw new Error(`Section ${sectionId} is not found`);
        }

        selectSection(nextSectionId);

        if (nextSection.competencies && nextSection.questions) {
            return;
        }

        addLoading();

        try {
            const { competencies, questions } = await retrieveSection(formId, specificationId, nextSectionId, areaCompetencies);
            setSectionData(competencies, questions);
            removeError();
        } catch (err) {
            addError(err, "Failed to load section questions");
            if (previousSectionId) {
                selectSection(previousSectionId);
            }
        } finally {
            removeLoading();
        }
    }

    function handleCompetencyChange(nextCompetencyId: string | null) {
        setAppliedFilter(prev => ({
            ...prev,
            competencyId: nextCompetencyId
        }));
        setDraftFilter(prev => ({
            ...prev,
            competencyId: nextCompetencyId
        }));
    }

    async function handleApplyFilter(): Promise<boolean> {
        if (draftFilter.changedOn
            && (draftFilter.changedOn !== "Custom" || draftFilter.changedOnBefore || draftFilter.changedOnSince)
            && !questionChangeDates
        ) {
            setIsFilterLoading(true);
            try {
                const newQuestionChangeDates = await shiftClient.workshop.collectQuestionChangeDates(bankId);
                setQuestionChangeDates(workshopQuestionAdapter.getQuestionChangeDates(newQuestionChangeDates));
                removeError();
            } catch (err) {
                addError(err, "Error while preparing filter data");
                return false;
            } finally {
                setIsFilterLoading(false);
            }
        }

        setAppliedFilter(cloneFilter(appliedFilter, draftFilter));

        return true;
    }

    function handleClearFilter() {
        const competencyId = getDefaultCompetencyId(sectionCompetencies);
        const emptyDraftFilter = workshopQuestionFilter.createWorkshopQuestionsFilterState(sectionId, competencyId);
        const emptyAppliedFilter = workshopQuestionFilter.createWorkshopQuestionsFilterState(sectionId, competencyId);

        setDraftFilter(emptyDraftFilter);
        setAppliedFilter(emptyAppliedFilter);
    }

    if (!sections?.length) {
        return (
            <FormCard>
                <Alert alertType="warning">There are no questions for this form.</Alert>
            </FormCard>
        );
    }

    return (
        <FormCard bodyClassName="WorkshopQuestions">
            <WorkshopQuestions_Filter
                filter={draftFilter}
                isLoading={isFilterLoading}
                onSectionChange={handleSectionChange}
                onCompetencyChange={handleCompetencyChange}
                onFilterChange={setDraftFilter}
                onApply={handleApplyFilter}
                onClear={handleClearFilter}
            />

            <hr className="mt-4 mb-3" />

            {sectionQuestions === null ? (
                <Alert alertType="information">Questions are loading...</Alert>
            ) : (
                <>
                    <h3 className="questions-header">
                        Questions
                        <span className="form-text">({filteredQuestions.length.toLocaleString()})</span>
                    </h3>

                    {sectionQuestions.length === 0 ? (
                        <Alert alertType="information">No questions found for the selected section.</Alert>
                    ) : filteredQuestions.length === 0 ? (
                        <Alert alertType="information">No questions match the current filter.</Alert>
                    ) : (
                        <table ref={tableRef} className="table question-grid">
                            <tbody>
                                {filteredQuestions.map((question, index) => (
                                    <WorkshopQuestions_Row
                                        key={question.questionId}
                                        id={getQuestionRowId(question.questionId)}
                                        question={question}
                                        hasPrev={index > 0}
                                        hasNext={index < filteredQuestions.length - 1}
                                        serializedFilter={serializedFilter}
                                        onMoveTop={() => handleMoveTop(tableRef)}
                                        onMovePrev={index > 0 ? () => moveToQuestion(filteredQuestions[index - 1].questionId) : undefined}
                                        onMoveNext={index < filteredQuestions.length - 1 ? () => moveToQuestion(filteredQuestions[index + 1].questionId) : undefined}
                                    />
                                ))}
                            </tbody>
                        </table>
                    )}
                </>
            )}
        </FormCard>
    );
}

async function retrieveSection(
    formId: string | null,
    specificationId: string | null,
    nextSectionId: string,
    areaCompetencies: WorkshopAreaCompetencies | null,
): Promise<{ competencies: WorkshopStandard[], questions: WorkshopQuestion[] }>
{
    if (formId) {
        const section = await shiftClient.workshop.retrieveSection(formId, nextSectionId);
        if (!section) {
            throw new Error("The data on the page is outdated, please refresh the page to continue.");
        }
        return workshopQuestionAdapter.getSectionData(section);
    }

    if (!specificationId) {
        throw new Error("Both formId and specificationId are empty");
    }
    if (!areaCompetencies) {
        throw new Error("areaCompetencies is null");
    }

    const set = await shiftClient.workshop.retrieveSpecSet(specificationId, nextSectionId);
    if (!set) {
        throw new Error("The data on the page is outdated, please refresh the page to continue.");
    }
    return workshopQuestionAdapter.getSetData(set, areaCompetencies);
}

function cloneFilter(currentFilter: WorkshopQuestionFilterState, nextFilter?: WorkshopQuestionFilterState): WorkshopQuestionFilterState {
    const filter = nextFilter ?? currentFilter;

    return {
        ...filter,
        flags: [...filter.flags],
        conditions: [...filter.conditions],
    };
}

function getDefaultCompetencyId(competencies: WorkshopStandard[] | null): string | null {
    return competencies?.[0]?.standardId ?? null;
}

function handleMoveTop(tableRef: React.RefObject<HTMLTableElement | null>) {
    const table = tableRef.current;
    if (!table) {
        return;
    }

    scrollToY(table.getBoundingClientRect().top + window.scrollY);
}

function moveToQuestion(questionId: string) {
    const row = document.getElementById(getQuestionRowId(questionId));
    if (!row) {
        return;
    }

    scrollToY(row.getBoundingClientRect().top + window.scrollY);
}

function getQuestionRowId(questionId: string) {
    return `questions_row_${questionId}`;
}

function scrollToY(top: number) {
    const header = document.querySelector("header.navbar:first-of-type");
    const headerHeight = header instanceof HTMLElement ? header.offsetHeight : 0;

    let scrollTo = top - headerHeight;
    if (scrollTo < 0) {
        scrollTo = 0;
    }

    window.scrollTo({
        top: scrollTo,
        behavior: "smooth",
    });
}
