import { WorkshopQuestion } from "@/contexts/workshop/models/WorkshopQuestion";
import WorkshopQuestions_Options_BooleanTable from "./WorkshopQuestions_Options_BooleanTable";
import WorkshopQuestions_Options_Matching from "./WorkshopQuestions_Options_Matching";
import WorkshopQuestions_Options_MultipleCorrect from "./WorkshopQuestions_Options_MultipleCorrect";
import WorkshopQuestions_Options_SingleCorrect from "./WorkshopQuestions_Options_SingleCorrect";

interface Props {
    question: WorkshopQuestion;
    isEditable: boolean;
    onSaveColumnHeader: (columnIndex: number, title: string) => Promise<void>;
    onSaveOptionTitle: (optionNumber: number, columnIndex: number | null, title: string) => Promise<void>;
    onSaveOptionPoints: (optionNumber: number, points: string) => Promise<void>;
}

export default function WorkshopQuestions_Options({
    question,
    isEditable,
    onSaveColumnHeader,
    onSaveOptionTitle,
    onSaveOptionPoints,
}: Props)
{
    switch (question.questionType) {
        case "Matching":
            return (
                <WorkshopQuestions_Options_Matching
                    question={question}
                />
            );

        case "SingleCorrect":
        case "TrueOrFalse":
            return (
                <WorkshopQuestions_Options_SingleCorrect
                    question={question}
                    isEditable={isEditable}
                    onSaveColumnHeader={onSaveColumnHeader}
                    onSaveOptionTitle={onSaveOptionTitle}
                    onSaveOptionPoints={onSaveOptionPoints}
                />
            );

        case "MultipleCorrect":
            return (
                <WorkshopQuestions_Options_MultipleCorrect
                    question={question}
                    isEditable={isEditable}
                    onSaveColumnHeader={onSaveColumnHeader}
                    onSaveOptionTitle={onSaveOptionTitle}
                />
            );

        case "BooleanTable":
            return (
                <WorkshopQuestions_Options_BooleanTable
                    question={question}
                    isEditable={isEditable}
                    onSaveColumnHeader={onSaveColumnHeader}
                    onSaveOptionTitle={onSaveOptionTitle}
                />
            );
            
        default:
            return null;
    }
}
