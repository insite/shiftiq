import Icon from "@/components/icon/Icon";
import InlineEditor from "@/components/inlineeditor/InlineEditor";
import { WorkshopQuestion } from "@/contexts/workshop/models/WorkshopQuestion";

interface Props {
    question: WorkshopQuestion;
    isEditable: boolean;
    isBooleanTable?: boolean;
    onSaveColumnHeader: (columnIndex: number, value: string) => Promise<void>;
}

export default function WorkshopQuestions_Options_TableHeader({
    question,
    isEditable,
    isBooleanTable = false,
    onSaveColumnHeader
}: Props)
{
    if (question.layoutColumns && question.layoutColumns.length > 0) {
        return  (
            <thead>
                <tr>
                    <th></th>
                    {!isBooleanTable && <th></th>}
                    {question.layoutColumns.map((x, index) => (
                        <th key={index}>
                            <InlineEditor
                                type="TextBox"
                                className="w-100"
                                textClassName={`${x.alignment === "Left" ? "" : x.alignment === "Right" ? "text-end" : "text-center"} ${x.cssClass ?? ""}`}
                                value={x.textMarkdown}
                                valueHtml={x.textHtml}
                                disabled={!isEditable}
                                onSave={value => onSaveColumnHeader(index, value)}
                            />
                        </th>
                    ))}
                    {isBooleanTable && (
                        <>
                            <th style={{ width: "30px" }} title="True"><Icon style="regular" name="check" /></th>
                            <th style={{ width: "30px" }} title="False"><Icon style="regular" name="times" /></th>
                        </>
                    )}
                    <th></th>
                </tr>
            </thead>
        );
    }

    return isBooleanTable ? (
        <thead>
            <tr>
                <th></th>
                <th></th>
                <th style={{ width: "30px" }} title="True"><Icon style="regular" name="check" /></th>
                <th style={{ width: "30px" }} title="False"><Icon style="regular" name="times" /></th>
                <th></th>
            </tr>
        </thead>
    ) : null;
}