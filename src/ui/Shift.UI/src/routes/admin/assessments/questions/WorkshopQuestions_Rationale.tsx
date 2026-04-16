import Alert from "@/components/Alert";
import Icon from "@/components/icon/Icon";
import { WorkshopQuestion } from "@/contexts/workshop/models/WorkshopQuestion";

interface Props {
    question: WorkshopQuestion;
}

export default function WorkshopQuestions_Rationale({ question }: Props) {
    const hasRationale = !!question.rationale || !!question.rationaleOnCorrectAnswer || !!question.rationaleOnIncorrectAnswer;

    if (!hasRationale) {
        return null;
    }

    return (
        <div className="WorkshopQuestions_Rationale">
            {question.rationale && (
                <Alert alertType="information" hideIcon className="mb-2">
                    <Icon style="solid" name="info-square" className="me-2" />
                    <strong>Feedback to all candidates:</strong>
                    <span className="ms-1" dangerouslySetInnerHTML={{ __html: question.rationale }} />
                </Alert>
            )}
            {question.rationaleOnCorrectAnswer && (
                <Alert alertType="success" hideIcon className="mb-2">
                    <Icon style="solid" name="check-square" className="me-2" />
                    <strong>Feedback on correct answers:</strong>
                    <span className="ms-1" dangerouslySetInnerHTML={{ __html: question.rationaleOnCorrectAnswer }} />
                </Alert>
            )}
            {question.rationaleOnIncorrectAnswer && (
                <Alert alertType="error" hideIcon className="mb-2">
                    <Icon style="solid" name="times-circle" className="me-2" />
                    <strong>Feedback on incorrect answers:</strong>
                    <span className="ms-1" dangerouslySetInnerHTML={{ __html: question.rationaleOnIncorrectAnswer }} />
                </Alert>
            )}
        </div>
    );
}
