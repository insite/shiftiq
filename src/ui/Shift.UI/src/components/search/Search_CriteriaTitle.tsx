import { translate } from "@/helpers/translate";
import Icon from "../icon/Icon";

export default function Search_CriteriaTitle() {
    return (
        <>
            <Icon style="solid" name="filter" className="me-2" />
            {translate("Criteria")}
        </>
    );
}