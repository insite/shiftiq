import { DownloadConfig } from "@/models/DownloadConfig";
import SearchListManager from "./SearchListManager";
import { translate } from "@/helpers/translate";

interface Props {
    download: DownloadConfig;
    onChange: (data: DownloadConfig | null) => void;
}

export default function SearchDownload_List({
    download,
    onChange
}: Props) {
    return (
        <SearchListManager<DownloadConfig>
            listTypeKey="download"
            dataForSave={download}
            titlePlaceholder={translate("New Download templates")}
            addTooltip={translate("Add a saved settings")}
            confirmDeleteText={translate("Are you sure you want to delete this saved settings?")}
            saveTooltip={translate("Save the selected saved settings")}
            deleteTooltip={translate("Delete the selected saved settings")}
            onChange={onChange}
        />
    );
}