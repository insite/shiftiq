import { WorkshopNewQuestionCommand } from "@/api/controllers/assessments/workshop/_workshopController";
import Button from "@/components/Button";
import Icon from "@/components/icon/Icon";
import { IconName } from "@/components/icon/IconName";
import { ButtonGroup, Dropdown } from "react-bootstrap";

interface Question {
    command: WorkshopNewQuestionCommand;
    text: string;
    title: string;
    iconName: IconName;
}

const questions: Question[] = [
    {
        command: "QuickMultipleChoice",
        text: "Multiple Choice",
        title: "Add a new multiple choice question to selected set",
        iconName: "check-circle",
    },
    {
        command: "QuickMultipleCorrect",
        text: "Multiple Correct",
        title: "Add a new multiple correct question to selected set",
        iconName: "check-square",
    },
    {
        command: "QuickComposedEssay",
        text: "Composed Essay",
        title: "Add a new composed essay response question to selected set",
        iconName: "file-lines",
    },
    {
        command: "QuickComposedVoice",
        text: "Composed Voice",
        title: "Add a new composed voice response question to selected set",
        iconName: "microphone",
    },
    {
        command: "QuickBooleanTable",
        text: "Multiple True/False List",
        title: "Add a new multiple true/false list question to selected set",
        iconName: "th",
    },
    {
        command: "QuickMatching",
        text: "Matching",
        title: "Add a new matching question to selected set",
        iconName: "exchange",
    },
];

interface Props {
    bankId: string;
    setId: string;
    competencyId: string | null;
    returnUrl: string;
    isAddingNewQuestion: boolean;
    onClick(command: WorkshopNewQuestionCommand): void;
}

export default function WorkshopQuestions_AddButton({
    bankId,
    setId,
    competencyId,
    returnUrl,
    isAddingNewQuestion,
    onClick
}: Props) {
    let newQuestionUrl = `/ui/admin/assessments/questions/add?bank=${bankId}&set=${setId}`;
    if (competencyId) {
        newQuestionUrl += `&competency=${competencyId}`
    };
    newQuestionUrl += `&${returnUrl}`;

    return (
        <Dropdown as={ButtonGroup}>
            <Button
                variant="add"
                iconStyle="regular"
                title="Add a new multiple choice question to selected set"
                isLoading={isAddingNewQuestion}
                loadingMessage="Adding new question..."
                onClick={() => onClick("QuickMultipleChoice")}
            />
            <Dropdown.Toggle split variant="default" size="sm" disabled={isAddingNewQuestion} />

            <Dropdown.Menu>
                {questions.map(x => (
                    <Dropdown.Item key={x.command} onClick={() => onClick(x.command)} title={x.title}>
                        <Icon style="regular" name={x.iconName} className="me-1" />
                        {x.text}
                    </Dropdown.Item>                    
                ))}

                <Dropdown.Divider />

                <Dropdown.Item href={newQuestionUrl} title="Add a new question to selected set using Question Creator">
                    <Icon style="solid" name="plus-circle" className="me-1" />
                    Go To Creator
                </Dropdown.Item>
            </Dropdown.Menu>
        </Dropdown>
    );
}