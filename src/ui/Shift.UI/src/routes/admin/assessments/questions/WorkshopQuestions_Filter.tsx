import Button from "@/components/Button";
import ComboBox from "@/components/combobox/ComboBox";
import DatePicker from "@/components/date/DatePicker";
import MultiSelect from "@/components/multiselect/MultiSelect";
import { ListItem } from "@/models/listItem";
import { allFlagItems, allQuestionConditionItems, WorkshopFlag } from "@/contexts/workshop/models/WorkshopEnums";
import { useWorkshopQuestionProvider } from "@/contexts/workshop/WorkshopQuestionProviderContext";
import FormField from "@/components/form/FormField";
import { WorkshopQuestionFilterState, WorkshopQuestionRange } from "./workshopQuestionFilter";
import { useRef, useState } from "react";

interface Props {
    filter: WorkshopQuestionFilterState;
    isLoading: boolean;
    onSectionChange: (sectionId: string | null) => void;
    onCompetencyChange: (competencyId: string | null) => void;
    onFilterChange: (filter: WorkshopQuestionFilterState) => void;
    onApply: () => Promise<boolean>;
    onClear: () => void;
}

const ligItems: ListItem[] = [
    { value: "", text: "" },
    { value: "true", text: "LIG" },
    { value: "false", text: "No LIG" },
];

const referenceItems: ListItem[] = [
    { value: "", text: "" },
    { value: "true", text: "Reference" },
    { value: "false", text: "No Reference" },
];

const questionChangedOnItems: ListItem[] = [
    { value: "", text: "" },
    { value: "Today", text: "Today" },
    { value: "Yesterday", text: "Yesterday" },
    { value: "ThisWeek", text: "This Week" },
    { value: "LastWeek", text: "Last Week" },
    { value: "ThisMonth", text: "This Month" },
    { value: "LastMonth", text: "Last Month" },
    { value: "ThisYear", text: "This Year" },
    { value: "LastYear", text: "Last Year" },
    { value: "Custom", text: "Custom Dates" },
];

export default function WorkshopQuestions_Filter({
    filter,
    isLoading,
    onSectionChange,
    onCompetencyChange,
    onFilterChange,
    onApply,
    onClear,
}: Props)
{
    const {
        sectionItems,
        sectionCompetencyItems,
        taxonomyItems,
        sectionId,
    } = useWorkshopQuestionProvider();

    const [status, setStatus] = useState<"filterApplied" | "filterCleared" | "error" | "none">("none");

    const timeoutRef = useRef<number>(null);

    function updateFilter<Key extends keyof WorkshopQuestionFilterState>(
        key: Key,
        value: WorkshopQuestionFilterState[Key]
    ) {
        onFilterChange({
            ...filter,
            [key]: value,
        });
    }

    async function handleApply(command: "apply" | "clear") {
        let success = true;
        
        if (command === "apply") {
            success = await onApply();
        } else {
            onClear();
        }

        if (timeoutRef.current) {
            clearTimeout(timeoutRef.current);
        }

        setStatus(success ? command === "apply" ? "filterApplied" : "filterCleared" : "error");

        timeoutRef.current = window.setTimeout(() => {
            timeoutRef.current = null;
            setStatus("none");
        }, 5000);
    }

    return (
        <div className="WorkshopQuestions_Filter">
            <div className="row">
                <div className="col-md-6">
                    <FormField label="Section" className="question-filter-section">
                        <ComboBox
                            value={sectionId}
                            items={sectionItems}
                            disabled={isLoading || sectionItems.length === 0}
                            onChange={value => onSectionChange(value)}
                        />
                    </FormField>
                </div>

                <div className="col-md-6">
                    <FormField label="Competency" className="question-filter-competency">
                        <ComboBox
                            value={filter.competencyId ?? ""}
                            items={sectionCompetencyItems}
                            disabled={isLoading || sectionCompetencyItems.length === 0}
                            onChange={value => onCompetencyChange(value)}
                        />
                    </FormField>
                </div>
            </div>

            <div className="row">
                <div className="col-sm-6">
                    <div className="row">
                        <div className="col-md-6">
                            <FormField label="Question Flag" className="question-filter-flag">
                                <MultiSelect
                                    values={filter.flags}
                                    items={allFlagItems}
                                    showButtons
                                    disabled={isLoading}
                                    placeholder="Any"
                                    onChange={values => updateFilter("flags", values as WorkshopFlag[])}
                                />
                            </FormField>
                        </div>

                        <div className="col-md-6">
                            <FormField label="Condition" className="question-filter-condition">
                                <MultiSelect
                                    values={filter.conditions}
                                    items={allQuestionConditionItems}
                                    showButtons
                                    disabled={isLoading}
                                    placeholder="Any"
                                    onChange={values => updateFilter("conditions", values)}
                                />
                            </FormField>
                        </div>
                    </div>
                </div>

                <div className="col-sm-6">
                    <div className="row">
                        <div className="col-lg-4">
                            <FormField label="Taxonomy" className="question-filter-taxonomy">
                                <ComboBox
                                    value={filter.taxonomy != null ? String(filter.taxonomy) : ""}
                                    items={taxonomyItems}
                                    placeholder="Any"
                                    disabled={isLoading}
                                    onChange={value => updateFilter("taxonomy", value ? parseInt(value, 10) : null)}
                                />
                            </FormField>
                        </div>

                        <div className="col-lg-4">
                            <FormField label="LIG" className="question-filter-lig">
                                <ComboBox
                                    value={filter.hasLig == null ? "" : String(filter.hasLig)}
                                    items={ligItems}
                                    placeholder="Any"
                                    disabled={isLoading}
                                    onChange={value => updateFilter("hasLig", value ? value === "true" : null)}
                                />
                            </FormField>
                        </div>

                        <div className="col-lg-4">
                            <FormField label="Reference" className="question-filter-reference">
                                <ComboBox
                                    value={filter.hasReference == null ? "" : String(filter.hasReference)}
                                    items={referenceItems}
                                    placeholder="Any"
                                    disabled={isLoading}
                                    onChange={value => updateFilter("hasReference", value ? value === "true" : null)}
                                />
                            </FormField>
                        </div>
                    </div>
                </div>
            </div>

            <div className="row">
                <div className="col-lg-3">
                    <FormField label="Question Changed On" className="question-filter-changed-shortcut">
                        <ComboBox
                            value={filter.changedOn}
                            items={questionChangedOnItems}
                            disabled={isLoading}
                            onChange={value => updateFilter("changedOn", value as WorkshopQuestionRange)}
                        />
                    </FormField>
                </div>

                {filter.changedOn === "Custom" && (
                    <div className="col-lg-5">
                        <FormField label="&nbsp;" className="question-filter-changed-dates">
                            <div className="d-flex align-items-center gap-3">
                                <DatePicker
                                    value={filter.changedOnSince}
                                    placeholder=">="
                                    disabled={isLoading}
                                    onChange={value => updateFilter("changedOnSince", value)}
                                />
                                to
                                <DatePicker
                                    value={filter.changedOnBefore}
                                    placeholder="<="
                                    disabled={isLoading}
                                    onChange={value => updateFilter("changedOnBefore", value)}
                                />
                            </div>
                        </FormField>
                    </div>                    
                )}

            </div>

            <div className="question-filter-actions">
                <Button
                    type="button"
                    variant="apply-filter"
                    className="me-2"
                    isLoading={isLoading}
                    onClick={() => handleApply("apply")}
                />
                <Button
                    type="button"
                    variant="clear"
                    className="btn-default"
                    disabled={isLoading}
                    onClick={() => handleApply("clear")}
                />

                {status === "filterApplied" && (
                    <span className="ms-2 p-1 badge text-bg-dark">Filter is applied</span>
                )}
                {status === "filterCleared" && (
                    <span className="ms-2 p-1 badge text-bg-dark">Filter is cleared</span>
                )}
                {status === "error" && (
                    <span className="ms-2 p-1 badge text-bg-danger">Error while trying to apply filter</span>
                )}
            </div>
        </div>
    );
}
