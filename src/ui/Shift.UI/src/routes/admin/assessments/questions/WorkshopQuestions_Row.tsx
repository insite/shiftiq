import { shiftClient } from "@/api/shiftClient";
import { WorkshopQuestion } from "@/contexts/workshop/models/WorkshopQuestion";
import { WorkshopStandard } from "@/contexts/workshop/models/WorkshopStandard";
import { MultiLanguageText } from "@/helpers/language";
import { useMemo, useState } from "react";
import WorkshopQuestions_Actions from "./WorkshopQuestions_Actions";
import WorkshopQuestions_AssetTable from "./WorkshopQuestions_AssetTable";
import WorkshopQuestions_Comments from "./WorkshopQuestions_Comments";
import WorkshopQuestions_Options from "./WorkshopQuestions_Options";
import WorkshopQuestions_Rationale from "./WorkshopQuestions_Rationale";
import InlineEditor from "@/components/inlineeditor/InlineEditor";
import WorkshopQuestions_NavButtons from "./WorkshopQuestions_NavButtons";
import Icon from "@/components/icon/Icon";
import { useWorkshopQuestionProvider } from "@/contexts/workshop/WorkshopQuestionProviderContext";
import { urlHelper } from "@/helpers/urlHelper";
import { numberHelper } from "@/helpers/numberHelper";
import WorkshopQuestions_SelectFile from "./WorkshopQuestions_SelectFile";

interface Props {
    id: string;
    question: WorkshopQuestion;
    hasPrev: boolean;
    hasNext: boolean;
    serializedFilter: string;
    onMoveTop: () => void;
    onMovePrev: (() => void) | undefined;
    onMoveNext: (() => void) | undefined;
}

export default function WorkshopQuestions_Row({
    id,
    question,
    hasPrev,
    hasNext,
    serializedFilter,
    onMoveTop,
    onMovePrev,
    onMoveNext,
}: Props) {
    const {
        bankId,
        formId,
        sectionCompetencies,
        sectionCompetencyItems,
        readOnly,
        modifyQuestionStandard,
        modifyQuestionTitle,
        modifyQuestionColumnHeader,
        modifyQuestionOptionTitle,
        modifyQuestionOptionPoints,
        modifyQuestionOptionColumnTitle
    } = useWorkshopQuestionProvider();

    const [insertFile, setInsertFile] = useState<{ callback: (fileUrl: string, documentName: string, isImage: boolean) => void } | null>(null);

    const returnUrl = urlHelper.getInSiteReturnUrl(`tab=questions&question=${question.questionId}&filter=${serializedFilter}`);
    const isEditable = !readOnly && question.canEdit;

    const competencyText = useMemo(() => {
        return getCompetencyText(sectionCompetencies, question.standardId);
    }, [sectionCompetencies, question.standardId]);

    async function handleSelectFile(insertFile: (fileUrl: string, documentName: string, isImage: boolean) => void) {
        setInsertFile({ callback: insertFile });
    }

    function handleSelected(fileUrl: string, documentName: string, isImage: boolean) {
        insertFile?.callback(fileUrl, documentName, isImage);
        setInsertFile(null);
    }

    async function handleSaveTitle(value: MultiLanguageText) {
        const valueToSave = JSON.stringify(value);
        const html = await shiftClient.workshop.modifyQuestion(bankId, question.questionId, "Title", null, valueToSave);
        modifyQuestionTitle(question.questionId, value, html);
    }

    async function handleSaveColumnHeader(columnIndex: number, value: string) {
        const html = await shiftClient.workshop.modifyQuestion(bankId, question.questionId, "ColumnHeader", columnIndex, value);
        modifyQuestionColumnHeader(question.questionId, columnIndex, value, html);
    }

    async function handleSaveCompetency(value: string) {
        await shiftClient.workshop.modifyQuestion(bankId, question.questionId, "Standard", null, value);
        modifyQuestionStandard(question.questionId, value);
    }

    async function handleSaveOptionTitle(optionNumber: number, columnIndex: number | null, title: string) {
        const field = columnIndex === null ? "Title" : "ColumnTitle";
        const html = await shiftClient.workshop.modifyOption(bankId, question.questionId, optionNumber, field, columnIndex, title);

        if (columnIndex === null) {
            modifyQuestionOptionTitle(question.questionId, optionNumber, title, html);
        } else {
            modifyQuestionOptionColumnTitle(question.questionId, optionNumber, columnIndex, title, html);
        }
    }

    async function handleSaveOptionPoints(optionNumber: number, points: string) {
        const pointsDecimal = numberHelper.parseDecimal(points);
        if (pointsDecimal === null) {
            return;
        }
        await shiftClient.workshop.modifyOption(bankId, question.questionId, optionNumber, "Points", null, String(pointsDecimal));
        modifyQuestionOptionPoints(question.questionId, optionNumber, pointsDecimal);
    }

    return (
        <tr id={id}>
            {insertFile && (
                <WorkshopQuestions_SelectFile
                    onSelect={handleSelected}
                    onClose={() => setInsertFile(null)}
                />
            )}

            <td>
                <WorkshopQuestions_Actions
                    bankId={bankId}
                    formId={formId}
                    question={question}
                    isEditable={isEditable}
                    returnUrl={returnUrl}
                />
            </td>

            <td>
                <div className="position-relative text-dark mb-3">
                    <InlineEditor
                        className="question"
                        type="MarkdownEditor"
                        value={question.questionTitle}
                        valueHtml={question.questionTitleHtml}
                        disabled={!isEditable}
                        enableSelectFile
                        onSelectFile={handleSelectFile}
                        onSave={handleSaveTitle}
                    />
                </div>

                <WorkshopQuestions_Options
                    question={question}
                    isEditable={isEditable}
                    onSaveColumnHeader={handleSaveColumnHeader}
                    onSaveOptionTitle={handleSaveOptionTitle}
                    onSaveOptionPoints={handleSaveOptionPoints}
                />

                <WorkshopQuestions_NavButtons
                    question={question}
                    hasPrev={hasPrev}
                    hasNext={hasNext}
                    onMoveTop={onMoveTop}
                    onMovePrev={onMovePrev}
                    onMoveNext={onMoveNext}
                />

                <div className="alert alert-warning mt-3">
                    <Icon style="solid" name="clipboard-list" className="mt-1 position-absolute" />
                    <div className="ms-4">
                        <InlineEditor
                            type="ComboBox"
                            className="standard-selector"
                            value={question.standardId}
                            valueHtml={competencyText}
                            items={sectionCompetencyItems}
                            disabled={!isEditable || sectionCompetencyItems.length === 0}
                            onSave={handleSaveCompetency}
                        />
                    </div>
                </div>

                <WorkshopQuestions_Rationale question={question} />

                <div className="row posted-comments-section">
                    <WorkshopQuestions_Comments
                        questionId={question.questionId}
                        comments={question.comments}
                        candidateCommentCount={question.candidateCommentCount}
                        returnUrl={returnUrl}
                    />
                </div>

                <div className="col-sm-6 col-xs-6 d-lg-none">
                    <WorkshopQuestions_AssetTable
                        question={question}
                        returnUrl={returnUrl}
                    />
                </div>
            </td>

            <td className="d-none d-lg-table-cell text-dark question-right-cell">
                <WorkshopQuestions_AssetTable
                    question={question}
                    returnUrl={returnUrl}
                />
            </td>
        </tr>
    );
}

function getCompetencyText(competencies: WorkshopStandard[] | null, standardId: string | null): string {
    const competency = competencies?.find(x => x.standardId === standardId);
    if (!competency) {
        return "";
    }
    return `
        ${competency.parent ? competency.parent.code + " " : ""}${competency.code}. ${competency.title}
        <div class="fs-xs text-body-secondary">
            <strong class="me-1">${competency.label}</strong>
            Asset #${competency.assetNumber}
        </div>
    `;
}