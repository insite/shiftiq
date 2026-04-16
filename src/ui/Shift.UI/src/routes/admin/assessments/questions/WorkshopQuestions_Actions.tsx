import { shiftClient } from "@/api/shiftClient";
import ActionLink from "@/components/ActionLink";
import Icon from "@/components/icon/Icon";
import { allFlagItems, flagEnumToTextClass } from "@/contexts/workshop/models/WorkshopEnums";
import { WorkshopQuestion } from "@/contexts/workshop/models/WorkshopQuestion";
import InlineEditor from "@/components/inlineeditor/InlineEditor";
import WorkshopQuestions_NewComment from "./WorkshopQuestions_NewComment";
import { workshopValidation } from "@/contexts/workshop/models/workshopValidation";
import { useWorkshopQuestionProvider } from "@/contexts/workshop/WorkshopQuestionProviderContext";

interface Props {
    bankId: string;
    formId: string | null;
    question: WorkshopQuestion;
    isEditable: boolean;
    returnUrl: string;
}

export default function WorkshopQuestions_Actions({
    bankId,
    formId,
    question,
    isEditable,
    returnUrl
}: Props) {
    const formParam = formId ? `&form=${formId}` : "";
    const changeUrl = `/ui/admin/assessments/questions/change?bank=${bankId}${formParam}&question=${question.questionId}&${returnUrl}`;
    const analysisUrl = `/ui/admin/assessments/questions/analysis?bank=${bankId}&question=${question.questionId}&${returnUrl}`;

    const candidateCommentUrl = formId
        ? `/ui/admin/assessments/bankscomments/search?bank=${bankId}&form=${formId}&question=${question.questionId}&role=Candidate&showAuthor=0&panel=results`
        : `/ui/admin/assessments/bankscomments/search?bank=${bankId}&question=${question.questionId}&role=Candidate&showAuthor=0&panel=results`;

    const { modifyQuestionFlag } = useWorkshopQuestionProvider();

    async function handleSaveFlag(value: string) {
        const flag = workshopValidation.validateFlag(value);
        await shiftClient.workshop.modifyQuestion(bankId, question.questionId, "Flag", null, flag);
        modifyQuestionFlag(question.questionId, flag);
    }

    return (
        <>
            <div className="sequence" title="Bank Question Number">{question.questionBankIndex + 1}</div>

            {question.canNavigateToChangePage && (
                <div className="mb-1">
                    <ActionLink
                        title="Edit Question"
                        href={changeUrl}
                        icon={{ style: "solid", name: "pencil" }}
                    />
                </div>
            )}

            <div className="mb-1">
                <WorkshopQuestions_NewComment
                    bankId={bankId}
                    questionId={question.questionId}
                    fieldId={question.fieldId}
                />
            </div>

            {question.canCopyField && (
                <div className="mb-1">
                    <a
                        href="#duplicate"
                        title="Duplicate"
                    >
                        <Icon style="solid" name="file" />
                    </a>
                </div>
            )}

            <div className="mb-1">
                <ActionLink
                    title="Question Analysis"
                    href={analysisUrl}
                    icon={{ style: "solid", name: "chart-bar" }}
                />
            </div>

            {question.candidateCommentCount > 0 && (
                <div className="mb-1">
                    <ActionLink title="Candidate Comments" href={candidateCommentUrl} target="_blank">
                        <p className="bubble speech">{question.candidateCommentCount}</p>
                    </ActionLink>
                </div>
            )}

            <div className="mb-1" title="Form Question Number">
                <span className="badge rounded-pill bg-custom-default fs-5">{question.questionFormSequence ?? "?"}</span>
            </div>

            <div className="d-flex flex-column align-items-center">
                {question.questionFlag !== "None" && (
                    <span className={flagEnumToTextClass(question.questionFlag)}>
                        <Icon style="solid" name="flag" className="me-1" />
                    </span>
                )}
                <InlineEditor
                    type="ComboBox"
                    className="mt-0"
                    textClassName="form-text text-dark fw-bold"
                    value={question.questionFlag}
                    valueHtml={question.questionFlag}
                    items={allFlagItems}
                    disabled={!isEditable}
                    onSave={handleSaveFlag}
                />
            </div>
        </>
    );
}