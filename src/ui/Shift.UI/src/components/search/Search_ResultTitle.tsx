import { numberHelper } from "@/helpers/numberHelper";
import { translate } from "@/helpers/translate";

export default function Search_ResultTitle({ count }: { count: number | null }) {
    return (
        <>
            <i className="fas fa-database me-2"></i>
            {translate("Results")}
            {count != null && (
                <span className="badge rounded-pill bg-info ms-1">
                    {numberHelper.formatNumber(count)}
                </span>
            )}
        </>
    );
}