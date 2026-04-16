import "./WorkshopQuestions_Options_SingleCorrect.css";
import Icon from "@/components/icon/Icon";
import { numberHelper } from "@/helpers/numberHelper";
import InlineEditor from "@/components/inlineeditor/InlineEditor";
import WorkshopQuestions_Options_TableHeader from "./WorkshopQuestions_Options_TableHeader";
import WorkshopQuestions_Options_TableCells from "./WorkshopQuestions_Options_TableCells";
import { WorkshopQuestion } from "@/contexts/workshop/models/WorkshopQuestion";

interface Props {
    question: WorkshopQuestion;
    isEditable: boolean;
    onSaveColumnHeader: (columnIndex: number, title: string) => Promise<void>;
    onSaveOptionTitle: (optionNumber: number, columnIndex: number | null, title: string) => Promise<void>;
    onSaveOptionPoints: (optionNumber: number, points: string) => Promise<void>;
}

export default function WorkshopQuestions_Options_SingleCorrect({
    question,
    isEditable,
    onSaveColumnHeader,
    onSaveOptionTitle,
    onSaveOptionPoints,
}: Props)
{
    const options = question.options ?? [];

    if (options.length === 0) {
        return null;
    }

    return (
        <div className="mb-3 text-dark WorkshopQuestions_Options_SingleCorrect">
            <table className="option-grid">
                <WorkshopQuestions_Options_TableHeader
                    question={question}
                    isEditable={isEditable}
                    onSaveColumnHeader={onSaveColumnHeader}
                />
                <tbody>
                    {options.map(option => (
                        <tr key={option.number}>
                            <td>
                                {option.points > 0
                                    ? <Icon style="regular" name="check-circle" className="text-success" />
                                    : <Icon style="regular" name="times-circle" className="text-danger" />
                                }
                            </td>
                            <td>{option.letter}.</td>
                            <WorkshopQuestions_Options_TableCells
                                question={question}
                                option={option}
                                isEditable={isEditable}
                                onSaveOptionTitle={onSaveOptionTitle}
                            />
                            <td className="form-text option-points">
                                &bull;{" "}
                                <InlineEditor
                                    type="TextBox"
                                    textClassName="d-inline fw-bold"
                                    editorClassName="d-inline"
                                    value={numberHelper.formatDecimal(option.points, 2)}
                                    valueHtml={numberHelper.formatDecimal(option.points, 2)}
                                    disabled={!isEditable}
                                    onSave={value => onSaveOptionPoints(option.number, value)}
                                />
                                {" "}points
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
}
