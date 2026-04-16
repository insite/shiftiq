import ActionLink from "@/components/ActionLink";
import { WorkshopComment } from "@/contexts/workshop/models/WorkshopComment";
import { useEffect, useState } from "react";
import { useWorkshopQuestionProvider } from "@/contexts/workshop/WorkshopQuestionProviderContext";
import IconButton from "@/components/iconbutton/IconButton";
import { shiftClient } from "@/api/shiftClient";

interface Props {
    questionId: string;
    comments: WorkshopComment[];
    candidateCommentCount: number;
    returnUrl: string;
}

export default function WorkshopQuestions_Comments({
    questionId,
    comments,
    candidateCommentCount,
    returnUrl,
}: Props) {
    const { bankId, formId, readOnly, showHideComment } = useWorkshopQuestionProvider();

    const [showHidden, setShowHidden] = useState(false);
    const [localComments, setLocalComments] = useState(comments);
    const [isSaving, setIsSaving] = useState(false);

    useEffect(() => {
        setLocalComments(comments);
    }, [comments]);

    if (comments.length === 0 && candidateCommentCount === 0) {
        return null;
    }

    const candidateCommentUrl = formId
        ? `/ui/admin/assessments/bankscomments/search?bank=${bankId}&form=${formId}&question=${questionId}&role=Candidate&showAuthor=0&panel=results`
        : `/ui/admin/assessments/bankscomments/search?bank=${bankId}&question=${questionId}&role=Candidate&showAuthor=0&panel=results`;

    async function handleShowHideComment(commentId: string, hidden: boolean) {
        const confirmText = hidden ? "Are you sure you want to hide this comment?" : "Are you sure you want to make this comment visible?";
        if (!window.confirm(confirmText)) {
            return;
        }

        setIsSaving(true);

        try {
            await shiftClient.workshop.showHideComment(bankId, commentId, hidden);
            showHideComment(questionId, commentId, hidden);
        } finally {
            setIsSaving(false);
        }
    }

    return (
        <div className="WorkshopQuestions_Comments col-md-12">
            <h3 className="p-1 bg-secondary">
                Administrator Comments
                <span className="float-end d-block me-1 fs-6 comments-actions">
                    {candidateCommentCount > 0 && (
                        <ActionLink
                            href={candidateCommentUrl}
                            target="_blank"
                            className="candidate-comments-link"
                        >
                            {candidateCommentCount} Candidate Comment{candidateCommentCount === 1 ? "" : "s"}
                        </ActionLink>
                    )}
                    <IconButton
                        iconStyle="solid"
                        iconName={showHidden ? "eye-slash" : "eye"}
                        className="ms-2"
                        onClick={() => setShowHidden(prev => !prev)}
                    />
                </span>
            </h3>

            <div className={`posted-comments ${showHidden ? "show-hidden" : ""}`}>
                {localComments.length === 0 && <div className="form-text">No administrator comments.</div>}

                {localComments.map(comment => {
                    const isHidden = !!comment.isHidden;
                    const isVisible = !isHidden || showHidden;

                    return (
                        <div
                            key={comment.commentId}
                            className={`question-comment border-bottom mb-3 me-2 ${isHidden ? "comment-hidden" : ""} ${isVisible ? "" : "d-none"}`}
                        >
                            <div className="row">
                                <div className="col-md-11">
                                    <div className="fs-5">{comment.authorName}</div>
                                    <div className="form-text mb-1">posted {comment.postedOn}</div>
                                    {comment.subject && <div className="fw-light">Subject: {comment.subject}</div>}
                                    <div className="mt-2" dangerouslySetInnerHTML={{ __html: comment.text }} />
                                </div>
                                <div className="col-md-1 d-flex align-items-end justify-content-start gap-0 flex-column">
                                    <ActionLink
                                        title="Revise Comment"
                                        icon={{ style: "solid", name: "pencil", className: "icon" }}
                                        href={`/ui/admin/assessments/comments/revise?bank=${bankId}&comment=${comment.commentId}&${returnUrl}`}
                                    />
                                    <IconButton
                                        iconStyle="solid"
                                        iconName={isHidden ? "eye" : "eye-slash"}
                                        iconClassName="icon"
                                        className="d-inline"
                                        title={isHidden ? "Unhide Comment" : "Hide Comment"}
                                        disabled={readOnly || isSaving}
                                        onClick={() => handleShowHideComment(comment.commentId, !isHidden)}
                                    />
                                    <ActionLink
                                        title="Delete Comment"
                                        icon={{ style: "solid", name: "trash-alt", className: "icon" }}
                                        href={`/admin/assessments/comments/delete?bank=${bankId}&comment=${comment.commentId}&${returnUrl}`}
                                    />
                                </div>
                            </div>
                        </div>
                    );
                })}
            </div>
        </div>
    );
}
