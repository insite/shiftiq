import { translate } from "@/helpers/translate";
import Icon from "../icon/Icon";

export default function Search_DownloadTitle() {
    return (
        <>
            <Icon style="solid" name="download" className="me-2" />
            {translate("Downloads")}
        </>
    );
}