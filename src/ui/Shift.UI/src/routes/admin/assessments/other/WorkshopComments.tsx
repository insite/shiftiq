import ActionLink from "@/components/ActionLink";
import Button from "@/components/Button";
import FormCard from "@/components/form/FormCard";
import Icon from "@/components/icon/Icon";
import { urlHelper } from "@/helpers/urlHelper";
import { Fragment } from "react";
import { useWorkshopOtherProvider } from "@/contexts/workshop/WorkshopOtherProviderContext";
import { flagEnumToTextClass } from "@/contexts/workshop/models/WorkshopEnums";

export default function WorkshopComments() {
    const { bankId, formId, specificationId, comments } = useWorkshopOtherProvider();
    const inSiteReturnUrl = urlHelper.getInSiteReturnUrl("tab=comments");

    const addUrl = formId
        ? `/ui/admin/assessments/comments/author?bank=${bankId}&form=${formId}&${inSiteReturnUrl}`
        : `/ui/admin/assessments/comments/author?bank=${bankId}&spec=${specificationId}&${inSiteReturnUrl}`;

    return (
        <FormCard>
            <div className="mb-3">
                <Button
                    variant="add"
                    text="New Comment"
                    href={addUrl}
                    disabled={!comments}
                />
            </div>

            {comments && comments.map((c, index) => (
                <Fragment key={c.commentId}>
                    {index > 0 && <hr/>}

                    <h5 className="mb-0">
                        {c.authorName}
                        <div className="d-inline-block ms-3 fs-6">
                            {c.flag !== "None" && (
                                <span className={`${flagEnumToTextClass(c.flag)} me-3`}>
                                    <Icon style="solid" name="flag" />
                                </span>
                            )}
                            {c.category && (
                                <span className="badge bg-custom-default me-3">
                                    {c.category}
                                </span>
                            )}
                            {c.eventFormat && (
                                <span className="badge bg-custom-default me-3">
                                    {c.eventFormat}
                                </span>
                            )}

                            <ActionLink
                                href={`/admin/assessments/comments/delete?bank=${bankId}&comment=${c.commentId}&${inSiteReturnUrl}`}
                                title="Delete Comment"
                                className="me-2"
                            >
                                <Icon style="solid" name="trash-alt" className="icon" />
                            </ActionLink>

                            <ActionLink
                                href={`/ui/admin/assessments/comments/revise?bank=${bankId}&comment=${c.commentId}&${inSiteReturnUrl}`}
                                title="Revise Comment"
                            >
                                <Icon style="solid" name="pencil" className="icon" />
                            </ActionLink>
                        </div>
                    </h5>

                    <div className="form-text">
                        posted {c.postedOn}
                    </div>

                    <div className="mt-1 fw-light">
                        Subject: {c.subject}
                    </div>

                    <div className="mt-2" dangerouslySetInnerHTML={{__html: c.text}} />
                </Fragment>
            ))}
        </FormCard>
    );
}