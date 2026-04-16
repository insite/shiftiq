import { shiftClient } from "@/api/shiftClient";
import Button from "@/components/Button";
import DateTimeField from "@/components/DateTimeField";
import FormField from "@/components/form/FormField";
import Icon from "@/components/icon/Icon";
import { useSiteProvider } from "@/contexts/site/SiteProviderContext";
import { useFormWorkshopProvider } from "@/contexts/workshop/FormWorkshopProviderContext";
import { translate } from "@/helpers/translate";
import { useSaveAction } from "@/hooks/useSaveAction";
import { useState } from "react";
import { Modal, Spinner } from "react-bootstrap";
import { formWorkshopAdapter } from "./formWorkshopAdapter";

export default function FormWorkshop_FormDetails_Questions() {
    const [show, setShow] = useState(false);

    const { siteSetting: { TimeZoneId: timeZoneId } } = useSiteProvider();

    const { formId, details, modifyVerifiedQuestions } = useFormWorkshopProvider();

    const { isSaving, runSave } = useSaveAction();

    if (details?.specificationType !== "Static") {
        return null;
    }

    async function handleVerifyQuestionOrderClick() {
        if (!details || !window.confirm(translate("Are you sure you want to update the verified question order?"))) {
            return;
        }

        await runSave(async () => {
            const result = await shiftClient.workshop.verifyStaticQuestionOrder(formId);
            if (!result){
                return;
            }
            const { questionOrderMatch, questionOrderVerified, verifiedQuestions } = formWorkshopAdapter.getVerifiedQuestions(result, timeZoneId);
            modifyVerifiedQuestions(questionOrderMatch, questionOrderVerified, verifiedQuestions);
        });
    }

    return (
        <>
            <FormField
                label={translate("Static Question Order")}
                description={translate("Indicates when the sequence of questions on the form was last verified. This is useful for static forms that are printed and stored outside the system for future use.")}
                editIconStyle="solid"
                editIcon="badge-check"
                editTitle={translate("Verify Question Order")}
                editDisabled={isSaving}
                onEditClick={handleVerifyQuestionOrderClick}
            >
                {isSaving && (
                    <>
                        <Spinner animation="border" role="status" size="sm" className="me-2" /> {translate("Verifying static question order...")}
                    </>
                )}
                {!isSaving && details.questionOrderVerified && (
                    <>
                        {translate("Verified")}
                        &nbsp;
                        <DateTimeField dateTime={details.questionOrderVerified} />
                        <button
                            type="button"
                            title={translate("Show Questions")}
                            className="btn btn-link m-0 p-0 ms-1"
                            onClick={() => setShow(true)}
                        >
                            <Icon style="solid" name="search" />
                        </button>

                        {details.questionOrderMatch ? (
                            <div className="fs-xs text-success mt-1">
                                <Icon style="solid" name="check" className="me-1" />
                                {translate("The current question order matches the expected, verified order.")}
                            </div>
                        ) : (
                            <div className="fs-xs text-danger mt-1">
                                <Icon style="solid" name="times" className="me-1" />
                                {translate("The current question order does not match the expected, verified order.")}
                            </div>
                        )}
                    </>
                )}
                {!isSaving && !details.questionOrderVerified && translate("Not Verified")}
            </FormField>

            <Modal
                show={show}
                className="insite-modal"
                dialogClassName="modal-dialog modal-lg modal-dialog-scrollable"
                onHide={() => setShow(false)}
            >
                <Modal.Header closeButton>
                    <Modal.Title as="h5">{translate("Verified Question Order")}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <table className="table table-striped">
                        <tbody>
                            {details.verifiedQuestions && (
                                details.verifiedQuestions.map(q => (
                                    <tr key={q.sequence}>
                                        <td style={{ width: "30px" }}>
                                            <span className="badge bg-primary">{q.sequence}</span>
                                            <span className="badge bg-info">{q.tag}</span>
                                        </td>
                                        <td className="mw-0">
                                            <p className="text-truncate">{q.text}</p>
                                            <div className="fs-sm text-body-secondary">{q.code}</div>
                                        </td>
                                    </tr>
                                ))
                            )}
                        </tbody>
                    </table>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="close" onClick={() => setShow(false)} />
                </Modal.Footer>
            </Modal>
        </>
    );
}