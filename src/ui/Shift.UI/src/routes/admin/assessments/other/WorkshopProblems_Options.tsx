import "./WorkshopProblems_Options.css";
import Icon from "@/components/icon/Icon";
import { WorkshopProblemQuestion } from "@/contexts/workshop/models/WorkshopProblemQuestion";
import { numberHelper } from "@/helpers/numberHelper";
import { textHelper } from "@/helpers/textHelper";

interface Props {
    q: WorkshopProblemQuestion;
}

export default function WorkshopProblems_Options({ q }: Props) {
    return (
        <table className="FormWorkshop_Problems_Options">
            <tbody>
                {q.options.map(o => (
                    <tr key={o.number}>
                        <td>
                            {o.points > 0 ? (
                                <Icon style="regular" name="check-circle" className="text-success" />
                            ) : (
                                <Icon style="regular" name="times-circle" className="text-danger" />
                            )}
                        </td>
                        <td>
                            {o.letter}.
                        </td>
                        <td>
                            {o.title ? (
                                <div dangerouslySetInnerHTML={{__html: o.title}} />    
                            ) : (
                                <i>{textHelper.none()}</i>
                            )}
                        </td>
                        <td className="form-text option-points">
                            &bull; {numberHelper.formatDecimal(o.points)}
                        </td>
                    </tr>
                ))}
            </tbody>
        </table>
    );
}