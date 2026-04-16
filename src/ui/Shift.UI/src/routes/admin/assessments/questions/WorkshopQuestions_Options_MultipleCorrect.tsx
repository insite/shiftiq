import { WorkshopQuestion } from "@/contexts/workshop/models/WorkshopQuestion";
import WorkshopQuestions_Options_TableHeader from "./WorkshopQuestions_Options_TableHeader";
import WorkshopQuestions_Options_TableCells from "./WorkshopQuestions_Options_TableCells";
import { numberHelper } from "@/helpers/numberHelper";
import Icon from "@/components/icon/Icon";

interface Props {
    question: WorkshopQuestion;
    isEditable: boolean;
    onSaveColumnHeader: (columnIndex: number, title: string) => Promise<void>;
    onSaveOptionTitle: (optionNumber: number, columnIndex: number | null, title: string) => Promise<void>;
}

export default function WorkshopQuestions_Options_MultipleCorrect({
    question,
    isEditable,
    onSaveColumnHeader,
    onSaveOptionTitle,
}: Props)
{
    const options = question.options ?? [];

    if (options.length === 0) {
        return null;
    }

    return (
        <div className="mb-3 text-dark">
            <table className="option-grid">
                <WorkshopQuestions_Options_TableHeader
                    question={question}
                    isEditable={isEditable}
                    onSaveColumnHeader={onSaveColumnHeader}
                />
                <tbody>
                    {options.map(option => (
                        <tr key={option.number}>
                            <td>{getMultipleCorrectIcon(option.isTrue)}</td>
                            <td>{option.letter}.</td>
                            <WorkshopQuestions_Options_TableCells
                                question={question}
                                option={option}
                                isEditable={isEditable}
                                onSaveOptionTitle={onSaveOptionTitle}
                            />
                            <td className="form-text option-points">&bull; {numberHelper.formatDecimal(option.points, 2)} points</td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
}

function getMultipleCorrectIcon(isTrue: boolean | null) {
    if (isTrue === null) {
        return <Icon style="regular" name="exclamation-triangle" className="text-warning" title="Not Configured" />;
    }

    if (isTrue) {
        return <Icon style="regular" name="check-square" />;
    }

    return <Icon style="regular" name="square" />;
}