import { shiftClient } from "@/api/shiftClient";
import Button from "@/components/Button";
import ComboBox from "@/components/combobox/ComboBox";
import FormField from "@/components/form/FormField";
import Icon from "@/components/icon/Icon";
import TextArea from "@/components/TextArea";
import { allFlagItems, WorkshopFlag } from "@/contexts/workshop/models/WorkshopEnums";
import { useWorkshopQuestionProvider } from "@/contexts/workshop/WorkshopQuestionProviderContext";
import { useSaveAction } from "@/hooks/useSaveAction";
import { useState } from "react";
import { Modal } from "react-bootstrap";
import { workshopQuestionAdapter } from "./workshopQuestionAdapter";

const authorTypes = [
    { value: "Administrator", text: "Administrator" },
    { value: "Candidate", text: "Candidate" },
];

interface Props {
    bankId: string;
    questionId: string;
    fieldId: string | null;
}

export default function WorkshopQuestions_NewComment({
    bankId,
    questionId,
    fieldId,
}: Props) {
    const [show, setShow] = useState(false);
    const [authorType, setAuthorType] = useState(authorTypes[0].value);
    const [flag, setFlag] = useState(allFlagItems[allFlagItems.length - 1].value);
    const [text, setText] = useState("");

    const { modifyQuestionComments } = useWorkshopQuestionProvider();

    const { isSaving, runSave } = useSaveAction();

    function handleNewComment(e: React.MouseEvent<HTMLAnchorElement, MouseEvent>) {
        e.preventDefault();
        setAuthorType(authorTypes[0].value);
        setFlag(allFlagItems[allFlagItems.length - 1].value);
        setText("");
        setShow(true);
    }

    async function handlePost() {
        if (!text) {
            window.alert("Text is a required field");
            return;
        }

        await runSave(async () => {
            const result = await shiftClient.workshop.postFieldComment(bankId, fieldId!, authorType, flag as WorkshopFlag, text);
            if (result) {
                const comments = workshopQuestionAdapter.getComments(result.Comments);
                modifyQuestionComments(questionId, comments, result.CandidateCommentCount);
            }
        });

        setShow(false);
    }

    return (
        <>
            <a
                href="#new-comment"
                title="New Comment"
                onClick={handleNewComment}
            >
                <Icon style="solid" name="comment" />
            </a>

            <Modal
                show={show}
                className="insite-modal"
                dialogClassName="modal-dialog modal-lg"
                onHide={() => setShow(false)}
            >
                <Modal.Header closeButton>
                    <Modal.Title as="h5">New Comment</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <div className="px-2">
                        <FormField label="Author Type">
                            <ComboBox
                                value={authorType}
                                items={authorTypes}
                                disabled={isSaving}
                                onChange={value => setAuthorType(value ?? "")}
                            />
                        </FormField>
                        <FormField label="Flag">
                            <ComboBox
                                value={flag}
                                items={allFlagItems}
                                disabled={isSaving}
                                onChange={value => setFlag(value ?? "")}
                            />
                        </FormField>
                        <FormField label="Text" hasBottomMargin={false}>
                            <TextArea
                                autoFocus
                                rows={4}
                                value={text}
                                disabled={isSaving}
                                onChange={e => setText(e.target.value)}
                            />
                        </FormField>
                    </div>
                </Modal.Body>
                <Modal.Footer>
                    <Button
                        variant="save"
                        text="Post"
                        className="me-1"
                        isLoading={isSaving}
                        loadingMessage="Posting..."
                        onClick={handlePost}
                    />
                    <Button variant="close" onClick={() => setShow(false)} />
                </Modal.Footer>
            </Modal>
        </>
    );
}