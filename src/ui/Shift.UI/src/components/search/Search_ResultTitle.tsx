import { numberHelper } from "@/helpers/numberHelper";
import { translate } from "@/helpers/translate";
import Icon from "../icon/Icon";

export default function Search_ResultTitle({ count }: { count: number | null }) {
    return (
        <>
            <Icon style="Solid" name="database" className="me-2" />
            {translate("Results")}
            {count != null && (
                <span className="badge rounded-pill bg-info ms-1">
                    {numberHelper.formatNumber(count)}
                </span>
            )}
        </>
    );
}