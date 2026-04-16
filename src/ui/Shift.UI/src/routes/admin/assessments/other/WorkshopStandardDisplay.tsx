import { WorkshopStandard } from "@/contexts/workshop/models/WorkshopStandard";
import { textHelper } from "@/helpers/textHelper";

interface StandardProps {
    standard: WorkshopStandard | null | undefined;
}

export default function WorkshopStandardDisplay({ standard }: StandardProps) {
    if (!standard) {
        return textHelper.none();
    }

    return (
        <>
            {`${standard.parent ? standard.parent.code + " " : ""}${standard.code}. ${standard.title}`}
            <div className="fs-xs text-body-secondary">
                <strong className="me-1">{standard.label}</strong>
                {`Asset #${standard.assetNumber}`}
            </div>
        </>
    )
}