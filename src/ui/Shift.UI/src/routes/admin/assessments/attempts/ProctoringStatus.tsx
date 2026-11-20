import { shiftClient } from "@/api/shiftClient";
import FormSection from "@/components/form/FormSection";
import { usePageProvider } from "@/contexts/page/PageProviderContext";
import { useOutlineForm } from "@/hooks/useOutlineForm";

const attemptViewUrl = "/ui/admin/assessments/attempts/view";

interface Status {
    title: string;
    viewUrl: string;
}

export default function ProctoringStatus() {
    const { setActionSubtitle, setBreadcrumbItemPath } = usePageProvider();
    
    const { model } = useOutlineForm(async (id) => {
        const model = await load(id);
        const url = `/ui/admin/assessments/attempts/view?attempt=${id}`;

        setActionSubtitle(model.title);
        setBreadcrumbItemPath(attemptViewUrl, url);

        return model;
    });

    return (
        <FormSection>
            {model ? (
                <iframe
                    src={model.viewUrl}
                    style={{
                        width: "100%",
                        height: "600px",
                        border: 0,
                    }}
                     allowFullScreen
                />
            ) : (
                <>Loading...</>
            )}
        </FormSection>
    );
}

async function load(attemptId: string): Promise<Status> {
    const result = await shiftClient.attempt.retrieveProctoringStatus(attemptId);
    return {
        title: result?.Title ?? "Undefined",
        viewUrl: result?.ViewUrl ?? "Undefined",
    };
}