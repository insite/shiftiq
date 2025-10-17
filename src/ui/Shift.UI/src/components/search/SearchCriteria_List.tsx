import { BaseCriteria } from "./BaseCriteria";
import SearchListManager from "./SearchListManager";
import { translate } from "@/helpers/translate";

interface Props<Criteria extends BaseCriteria> {
    onGetDataForSave: () => object;
    onChange: (data: Criteria | null) => void;
}

export default function SearchCriteria_List<Criteria extends BaseCriteria>({
    onGetDataForSave,
    onChange
}: Props<Criteria>) {
    return (
        <SearchListManager<Criteria>
            listTypeKey="criteria"
            dataForSave={onGetDataForSave}
            titlePlaceholder={translate("New Saved Filter")}
            addTooltip={translate("Add a saved filter")}
            confirmDeleteText={translate("Are you sure you want to delete this saved filter?")}
            saveTooltip={translate("Save the selected saved filter")}
            deleteTooltip={translate("Delete the selected saved filter")}
            onChange={onChange}
        />
    );
}