import Icon from "@/components/icon/Icon";
import WorkshopQuestions_Options_TableCells from "./WorkshopQuestions_Options_TableCells";
import WorkshopQuestions_Options_TableHeader from "./WorkshopQuestions_Options_TableHeader";
import { numberHelper } from "@/helpers/numberHelper";
import { WorkshopQuestion } from "@/contexts/workshop/models/WorkshopQuestion";

interface Props {
    question: WorkshopQuestion;
    isEditable: boolean;
    onSaveColumnHeader: (columnIndex: number, title: string) => Promise<void>;
    onSaveOptionTitle: (optionNumber: number, columnIndex: number | null, title: string) => Promise<void>;
}

export default function WorkshopQuestions_Options_BooleanTable({
    question,
    isEditable,
    onSaveColumnHeader,
    onSaveOptionTitle,
}: Props) {
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
                    isBooleanTable
                    onSaveColumnHeader={onSaveColumnHeader}
                />
                <tbody>
                    {options.map(option => (
                        <tr key={option.number}>
                            <td>{option.letter}.</td>
                            <WorkshopQuestions_Options_TableCells
                                question={question}
                                option={option}
                                isEditable={isEditable}
                                onSaveOptionTitle={onSaveOptionTitle}
                            />
                            <td title="True">{getBooleanTableIcon(option.isTrue, true)}</td>
                            <td title="False">{getBooleanTableIcon(option.isTrue, false)}</td>
                            <td className="form-text option-points">&bull; {numberHelper.formatDecimal(option.points, 2)} points</td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
}

function getBooleanTableIcon(isTrue: boolean | null, answer: boolean) {
    if (isTrue === null) {
        return <Icon style="regular" name="exclamation-triangle" className="text-warning" title="Not Configured" />;
    }

    return isTrue === answer
        ? <Icon style="regular" name="dot-circle" />
        : <Icon style="regular" name="circle" />;
}