import { WorkshopReplaceQuestionCommand } from "@/api/controllers/workshop/_workshopController";
import { shiftClient } from "@/api/shiftClient";
import Button from "@/components/Button";
import Icon from "@/components/icon/Icon";
import { WorkshopQuestion } from "@/contexts/workshop/models/WorkshopQuestion";
import { useWorkshopQuestionProvider } from "@/contexts/workshop/WorkshopQuestionProviderContext";
import { Dropdown } from "react-bootstrap";
import { workshopQuestionAdapter } from "./workshopQuestionAdapter";
import { useLoadingProvider } from "@/contexts/loading/LoadingProviderContext";
import { useStatusProvider } from "@/contexts/status/StatusProviderContext";

interface Props {
    question: WorkshopQuestion;
    hasPrev: boolean;
    hasNext: boolean;
    onMoveTop: () => void;
    onMovePrev: (() => void) | undefined;
    onMoveNext: (() => void) | undefined;
}

export default function WorkshopQuestions_NavButtons({
    question,
    hasPrev,
    hasNext,
    onMoveTop,
    onMovePrev,
    onMoveNext,
}: Props) {
    const { bankId, setSectionData } = useWorkshopQuestionProvider();
    const { addLoading, removeLoading } = useLoadingProvider();
    const { addError, removeError } = useStatusProvider();

    async function handelReplaceQuestion(command: WorkshopReplaceQuestionCommand) {
        if (!question.fieldId) {
            return;
        }

        addLoading();
        
        try {
            const section = await shiftClient.workshop.replaceFieldQuestion(bankId, question.fieldId, command);
            if (section) {
                const { competencies, questions } = workshopQuestionAdapter.getSectionData(section);
                setSectionData(competencies, questions);
            } else {
                window.alert("The data on the page is outdated, please refresh the page to complete the operation.");
            }
            removeError();
        } catch (err) {
            addError(err);
        } finally {
            removeLoading();
        }
    }

    return (
        <>
            {(question.replaceButtons.newVersion
                || question.replaceButtons.newQuestionAndSurplus
                || question.replaceButtons.newQuestionAndPurge
                || question.replaceButtons.rollbackQuestion) && (
                    <Dropdown className="d-inline-block me-2">
                        <Dropdown.Toggle variant="default" size="sm">
                            <Icon style="solid" name="cog" className="me-2" />
                            Replace
                        </Dropdown.Toggle>
                        <Dropdown.Menu>
                            {question.replaceButtons.newVersion && (
                                <Dropdown.Item onClick={() => handelReplaceQuestion("NewVersion")}>
                                    <Icon style="regular" name="arrow-alt-up" className="me-1" />
                                    New version
                                </Dropdown.Item>
                            )}
                            {question.replaceButtons.newQuestionAndSurplus && (
                                <Dropdown.Item onClick={() => handelReplaceQuestion("NewQuestionAndSurplus")}>
                                    <Icon style="regular" name="plus-circle" className="me-1" />
                                    New Question and Surplus
                                </Dropdown.Item>
                            )}
                            {question.replaceButtons.newQuestionAndPurge && (
                                <Dropdown.Item onClick={() => handelReplaceQuestion("NewQuestionAndPurge")}>
                                    <Icon style="regular" name="plus-hexagon" className="me-1" />
                                    New Question and Purge
                                </Dropdown.Item>
                            )}
                            {question.replaceButtons.rollbackQuestion && (
                                <Dropdown.Item onClick={() => handelReplaceQuestion("RollbackQuestion")}>
                                    <Icon style="regular" name="undo-alt" className="me-1" />
                                    Revert/Rollback
                                </Dropdown.Item>
                            )}
                        </Dropdown.Menu>
                    </Dropdown>
                )
            }

            <Button type="button" variant="top" className="me-2" onClick={onMoveTop} />

            {hasPrev && (
                <Button type="button" variant="previous" className="me-2" onClick={onMovePrev} />
            )}

            {hasNext && (
                <Button type="button" variant="next" onClick={onMoveNext} />
            )}
        </>
    );
}
