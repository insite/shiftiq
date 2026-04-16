import FormCard from "@/components/form/FormCard";
import FormTab from "@/components/form/FormTab";
import FormTabs from "@/components/form/FormTabs";
import { AttachmentType, WorkshopAttachment } from "@/contexts/workshop/models/WorkshopAttachment";
import { useMemo, useRef, useState } from "react";
import WorkshopAttachments_Group from "./WorkshopAttachments_Group";
import TextBox from "@/components/TextBox";
import { useSearchParams } from "react-router";
import { useWorkshopOtherProvider } from "@/contexts/workshop/WorkshopOtherProviderContext";

export interface Group {
    attachmentType: AttachmentType;
    groupTitle: string;
    groupAttachments: WorkshopAttachment[];
    selectedAttachmentId: string | null;
}

function groupAttachments(attachments: WorkshopAttachment[], selectedAttachmentId: string | null): {
    groups: Group[],
    selectedAttachmentType: AttachmentType,
} {
    let selectedAttachmentType: AttachmentType | null = null;

    const groups = attachments.reduce((result, a) => {
        let group = result.find(x => x.attachmentType === a.attachmentType);
        if (!group) {
            result.push(group = {
                attachmentType: a.attachmentType,
                groupTitle: a.attachmentType === "Image" ? "Images" : a.attachmentType === "Document" ? "Documents" : "Others",
                groupAttachments: [],
                selectedAttachmentId: null,
            });
        }

        group.groupAttachments.push(a);

        if (selectedAttachmentId && selectedAttachmentId.toLowerCase() === a.attachmentId.toLowerCase()) {
            group.selectedAttachmentId = selectedAttachmentId;
            selectedAttachmentType = a.attachmentType;
        }

        return result;
    }, [] as Group[]);

    groups.sort((a, b) => {
        const sequenceA = a.attachmentType === "Image" ? 0 : a.attachmentType === "Document" ? 1 : 2;
        const sequenceB = b.attachmentType === "Image" ? 0 : b.attachmentType === "Document" ? 1 : 2;
        return sequenceA < sequenceB ? -1 : sequenceA > sequenceB ? 1 : 0;
    });

    if (!selectedAttachmentType) {
        selectedAttachmentType = groups.length > 0 ? groups[0].attachmentType : "Image";
    }

    return {
        groups,
        selectedAttachmentType
    };
}

export default function WorkshopAttachments() {
    const [filterText, setFilterText] = useState("");
    const filterTimeoutRef = useRef<number>(null);

    const [searchParams] = useSearchParams();
    const selectedAttachmentId = searchParams.get("attachment");

    const { bankId, attachments } = useWorkshopOtherProvider();

    const { groups, selectedAttachmentType } = useMemo(() => {
        return attachments
            ? groupAttachments(attachments, selectedAttachmentId)
            : {
                groups: null,
                selectedAttachmentType: "Image"
            };
    }, [attachments, selectedAttachmentId]);

    function handleFilterTextChange(e: React.ChangeEvent<HTMLInputElement>) {
        if (filterTimeoutRef.current) {
            clearTimeout(filterTimeoutRef.current);
        }

        const newFilterText = e.target.value;

        filterTimeoutRef.current = window.setTimeout(() => {
            filterTimeoutRef.current = null;
            setFilterText(newFilterText);
        }, 500);
    }

    return (
        <FormCard>
            <div className="mb-3">
                <TextBox
                    placeholder="Filter Attachments"
                    className="w-25"
                    onChange={handleFilterTextChange}
                />
            </div>

            {groups && groups.length > 0 && (
                <FormTabs defaultTab={selectedAttachmentType}>
                    {groups && groups.map(g => (
                        <FormTab key={g.attachmentType} tab={g.attachmentType} title={g.groupTitle} subtitle={`(${g.groupAttachments.length})`}>
                            <WorkshopAttachments_Group
                                bankId={bankId}
                                attachments={g.groupAttachments}
                                selectedAttachmentId={g.selectedAttachmentId}
                                filterText={filterText}
                            />
                        </FormTab>
                    ))}
                </FormTabs>
            )}
        </FormCard>
    );
}