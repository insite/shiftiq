import { translate } from "@/helpers/translate";
import Icon from "../icon/Icon";

export default function Search_CriteriaTitle() {
    return (
        <>
            <Icon style="Solid" name="filter" className="me-2" />
            {translate("Criteria")}
        </>
    );
}