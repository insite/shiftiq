import InlineEditor from "@/components/inlineeditor/InlineEditor";
import { WorkshopQuestion } from "@/contexts/workshop/models/WorkshopQuestion";
import { WorkshopQuestionOption } from "@/contexts/workshop/models/WorkshopQuestionOption";

interface Props {
    question: WorkshopQuestion;
    option: WorkshopQuestionOption;
    isEditable: boolean;
    onSaveOptionTitle: (optionNumber: number, columnIndex: number | null, title: string) => Promise<void>;
}

export default function WorkshopQuestions_Options_TableCells({
    question,
    option,
    isEditable,
    onSaveOptionTitle,
}: Props)
{
    return question.layoutColumns && question.layoutColumns.length > 0 && option.columns ?
        option.columns.map((x, index) => {
            const { alignment, cssClass } = question.layoutColumns![index];

            return (
                <td key={index} className="option-title">
                    <InlineEditor
                        type="TextBox"
                        className="w-100"
                        textClassName={`${alignment === "Left" ? "" : alignment === "Right" ? "text-end" : "text-center"} ${cssClass ?? ""}`}
                        value={x.textMarkdown}
                        valueHtml={x.textHtml}
                        disabled={!isEditable}
                        onSave={value => onSaveOptionTitle(option.number, index, value)}
                    />
                </td>
            );
        }) : (
            <td className="option-title">
                <InlineEditor
                    type="TextArea"
                    value={option.titleMarkdown}
                    valueHtml={option.titleHtml}
                    disabled={!isEditable}
                    onSave={value => onSaveOptionTitle(option.number, null, value)}
                />
            </td>
        );
}