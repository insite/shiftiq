import "./WorkshopProblems.css";
import ActionLink from "@/components/ActionLink";
import Alert from "@/components/Alert";
import { urlHelper } from "@/helpers/urlHelper";
import WorkshopProblems_Options from "./WorkshopProblems_Options";
import { textHelper } from "@/helpers/textHelper";
import FormCard from "@/components/form/FormCard";
import { useWorkshopOtherProvider } from "@/contexts/workshop/WorkshopOtherProviderContext";

export default function WorkshopProblems() {
    const { bankId, formId, specificationId, problemQuestions } = useWorkshopOtherProvider();

    if (!problemQuestions) {
        return null;
    }

    if (problemQuestions.length === 0) {
        return (
            <FormCard>
                <Alert alertType="success">
                    Congrats! We couldn't find any problems with the questions in this Form.
                </Alert>
            </FormCard>
        );
    }

    console.log("formId", formId);//\\

    const returnUrl = urlHelper.getInSiteReturnUrl("tab=problems");

    return (
        <FormCard>
            <table className="table table-striped FormWorkshop_Problems">
                <thead>
                    <tr>
                        <th>Question</th>
                        <th>Description</th>
                        <th></th>
                    </tr>
                </thead>
                <tbody>
                    {problemQuestions.map(q => (
                        <tr key={q.questionId}>
                            <td className="px-4 pb-4">
                                <div>
                                    <i>{q.questionSetName}</i>
                                </div>
                                <div>
                                    <strong>Question #{q.questionBankIndex + 1} ({q.questionAssetNumber}.{q.questionAssetVersion})</strong>:
                                </div>
                                <div className="my-2 question-title">
                                    {q.questionTitle ? (
                                        <div dangerouslySetInnerHTML={{__html: q.questionTitle}} />
                                    ) : (
                                        <i>{textHelper.none()}</i>
                                    )}
                                </div>
                                <div>
                                    <WorkshopProblems_Options q={q} />
                                </div>
                            </td>
                            <td>
                                {q.problemDescription}
                            </td>
                            <td className="text-end text-nowrap">
                                <ActionLink
                                    title="Edit Question"
                                    href={`/ui/admin/assessments/questions/change?bank=${bankId}&question=${q.questionId}&${returnUrl}`}
                                    icon={{ style: "solid", name: "pencil" }}
                                />
                                {q.canDelete && (
                                    <ActionLink
                                        title="Delete Question"
                                        href={`/admin/assessments/questions/delete?bank=${bankId}&question=${q.questionId}&${returnUrl}`}
                                        icon={{ style: "solid", name: "trash-alt" }}
                                    />
                                )}
                                <ActionLink
                                    title="Jump to Question"
                                    href={
                                        formId
                                            ? `/client/admin/assessment/forms/workshop/${formId}?tab=questions&question=${q.questionId}`
                                            : `/client/admin/assessment/specs/workshop/${specificationId}?tab=questions&question=${q.questionId}`
                                    }
                                    icon={{ style: "regular", name: "reply", className: "fa-rotate-270" }}
                                    enforceHttpRedirect
                                />
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </FormCard>
    );
}