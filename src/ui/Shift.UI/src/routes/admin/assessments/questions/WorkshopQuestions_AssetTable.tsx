import { shiftClient } from "@/api/shiftClient";
import ActionLink from "@/components/ActionLink";
import { WorkshopQuestion } from "@/contexts/workshop/models/WorkshopQuestion";
import { useWorkshopQuestionProvider } from "@/contexts/workshop/WorkshopQuestionProviderContext";
import { allQuestionConditionItems, questionTypeDescriptions } from "@/contexts/workshop/models/WorkshopEnums";
import InlineEditor from "@/components/inlineeditor/InlineEditor";
import { numberHelper } from "@/helpers/numberHelper";
import { textHelper } from "@/helpers/textHelper";

interface Props {
    question: WorkshopQuestion;
    returnUrl: string;
}

export default function WorkshopQuestions_AssetTable({
    question,
    returnUrl,
}: Props)
{
    const {
        bankId,
        formId,
        readOnly,
        taxonomyItems,
        modifyQuestionCondition,
        modifyQuestionTaxonomy,
        modifyQuestionLIG,
        modifyQuestionReference,
        modifyQuestionCode,
        modifyQuestionTag
    } = useWorkshopQuestionProvider();

    const taxonomyValue = question.questionTaxonomy != null ? String(question.questionTaxonomy) : "";
    const taxonomyName = taxonomyItems.find(x => x.value === taxonomyValue)?.text ?? "";

    async function handleQuestionFieldSave(field: "Condition" | "Taxonomy" | "LIG" | "Reference" | "Code" | "Tag", value: string) {
        await shiftClient.workshop.modifyQuestion(bankId, question.questionId, field, null, value);

         switch (field) {
            case "Condition":
                modifyQuestionCondition(question.questionId, value);
                break;
            case "Taxonomy":
                modifyQuestionTaxonomy(question.questionId, parseInt(value));
                break;
            case "LIG":
                modifyQuestionLIG(question.questionId, value);
                break;
            case "Reference":
                modifyQuestionReference(question.questionId, value);
                break;
            case "Code":
                modifyQuestionCode(question.questionId, value);
                break;
            case "Tag":
                modifyQuestionTag(question.questionId, value);
                break;
            default:
                throw new Error(`Unexpected field: ${field}`);
         }
    }

    return (
        <table className="property-grid asset-table">
            <tbody>
                <tr>
                    <td>Asset #</td>
                    <td>{question.questionAssetNumber}.{question.questionAssetVersion}</td>
                </tr>

                <tr>
                    <td>Publication</td>
                    <td>{question.questionPublicationStatusDescription}</td>
                </tr>

                <tr>
                    <td>Condition</td>
                    <td>
                        <InlineEditor
                            type="ComboBox"
                            value={question.questionCondition ?? "Unassigned"}
                            valueHtml={question.questionCondition}
                            items={allQuestionConditionItems}
                            disabled={readOnly}
                            onSave={value => handleQuestionFieldSave("Condition", value)}
                        />
                    </td>
                </tr>

                <tr>
                    <td>Taxonomy</td>
                    <td>
                        <InlineEditor
                            type="ComboBox"
                            value={taxonomyValue}
                            valueHtml={taxonomyName}
                            items={taxonomyItems}
                            disabled={readOnly}
                            onSave={value => handleQuestionFieldSave("Taxonomy", value)}
                        />
                    </td>
                </tr>

                <tr>
                    <td>LIG</td>
                    <td>
                        <InlineEditor
                            type="TextBox"
                            value={question.questionLikeItemGroup}
                            valueHtml={question.questionLikeItemGroup}
                            maxLength={64}
                            disabled={readOnly}
                            onSave={value => handleQuestionFieldSave("LIG", value)}
                        />
                    </td>
                </tr>

                <tr>
                    <td>Reference</td>
                    <td className="word-break">
                        <InlineEditor
                            type="TextBox"
                            value={question.questionReference}
                            valueHtml={question.questionReference}
                            maxLength={500}
                            disabled={readOnly}
                            onSave={value => handleQuestionFieldSave("Reference", value)}
                        />
                    </td>
                </tr>

                <tr>
                    <td>Code</td>
                    <td className="word-break">
                        <InlineEditor
                            type="TextBox"
                            value={question.questionCode}
                            valueHtml={question.questionCode}
                            maxLength={40}
                            disabled={readOnly}
                            onSave={value => handleQuestionFieldSave("Code", value)}
                        />
                    </td>
                </tr>

                <tr>
                    <td>Tag</td>
                    <td>
                        <InlineEditor
                            type="TextBox"
                            value={question.questionTag}
                            valueHtml={question.questionTag}
                            maxLength={100}
                            disabled={readOnly}
                            onSave={value => handleQuestionFieldSave("Tag", value)}
                        />
                    </td>
                </tr>

                {(question.forms ?? []).length > 0 && (
                    <tr>
                        <td>Form</td>
                        <td>
                            {(question.forms ?? []).map(f => (
                                <div key={f.formId} className="form-ref-row">
                                    <ActionLink
                                        href={`/client/admin/assessment/forms/workshop/${f.formId}?tab=questions&question=${question.questionId}`}
                                        target="_blank"
                                        className={f.formId === formId ? "fst-italic" : ""}
                                    >
                                        {f.formName} [{f.formAssetNumber}.{f.formAssetversion}]
                                    </ActionLink>
                                </div>
                            ))}
                        </td>
                    </tr>
                )}

                {question.questionLayoutType !== "None" && (
                    <tr>
                        <td>Layout</td>
                        <td>{question.questionLayoutType}</td>
                    </tr>
                )}

                <tr>
                    <td>Type</td>
                    <td>{questionTypeDescriptions[question.questionType]}</td>
                </tr>

                <tr>
                    <td>Points</td>
                    <td>{question.questionPoints !== null ? numberHelper.formatDecimal(question.questionPoints) : textHelper.none() }</td>
                </tr>

                {question.questionCutScore != null && (
                    <tr>
                        <td>Cut-Score</td>
                        <td>{question.questionCutScore}</td>
                    </tr>
                )}

                {question.questionCalculationMethodDescription && question.questionCalculationMethodDescription.toLowerCase() !== "default" && (
                    <tr>
                        <td>Calculation Method</td>
                        <td>{question.questionCalculationMethodDescription}</td>
                    </tr>
                )}

                {question.questionClassificationDifficulty != null && (
                    <tr>
                        <td>Difficulty</td>
                        <td>{question.questionClassificationDifficulty}</td>
                    </tr>
                )}

                {question.questionRandomizationEnabled && (
                    <tr>
                        <td>Randomize</td>
                        <td>Options</td>
                    </tr>
                )}

                {question.source && (
                    <tr>
                        <td>Source</td>
                        <td>
                            <ActionLink
                                href={`/ui/admin/assessments/questions/analysis?bank=${bankId}&question=${question.source.questionId}&${returnUrl}`}
                            >
                                {question.source.questionAssetNumber}
                            </ActionLink>
                        </td>
                    </tr>
                )}
            </tbody>
        </table>
    );
}