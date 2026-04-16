import { WorkshopQuestion } from "@/contexts/workshop/models/WorkshopQuestion";
import { numberHelper } from "@/helpers/numberHelper";

interface Props {
    question: WorkshopQuestion;
}

export default function WorkshopQuestions_Options_Matching({
    question,
}: Props)
{
    return (
        <div className="mb-3 text-dark">
            {(question.matches ?? []).length > 0 && (
                <>
                    <h5 className="p-1 bg-secondary">Matching Pairs</h5>
                    <table className="table table-condensed matching-pairs">
                        <tbody>
                            {(question.matches ?? []).map((pair, index) => (
                                <tr key={`${pair.left}-${pair.right}-${index}`}>
                                    <td className="w-50">{pair.left}</td>
                                    <td className="w-50">{pair.right}</td>
                                    <td className="form-text pair-points">&bull; {numberHelper.formatDecimal(pair.points, 2)} points</td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </>
            )}

            {(question.distractors ?? []).length > 0 && (
                <>
                    <h5 className="p-1 bg-secondary">Matching Distractors</h5>
                    <table className="table table-condensed">
                        <tbody>
                            {(question.distractors ?? []).map((distractor, index) => (
                                <tr key={`${distractor}-${index}`}>
                                    <td>{distractor}</td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </>
            )}
        </div>
    );
}
