import TextBox from "@/components/TextBox";
import { translate } from "@/helpers/translate";
import { useForm } from "react-hook-form";
import { useSearch } from "@/components/search/Search";
import SearchCriteria from "@/components/search/SearchCriteria";
import { CaseStatusCriteria } from "./CaseStatusCriteria";
import CaseStatusCategoryComboBox from "@/routes/_shared/comboboxes/CaseStatusCategoryComboBox";

export default function CaseStatusSearch_Criteria() {
    const { criteria, isLoading, setCriteria } = useSearch<CaseStatusCriteria, object>();

    const { register, control, handleSubmit, getValues, reset } = useForm<CaseStatusCriteria>({
        defaultValues: criteria
    });

    return (
        <SearchCriteria<CaseStatusCriteria>
            contentSize="size-3"
            control={control}
            onGetCriteria={getValues}
            onSubmit={handleSubmit(setCriteria)}
            onReset={reset}
        >
            <div className="row">
                <div className="col-4">
                    <TextBox
                        {...register("statusNameContains")}
                        placeholder={translate("Status Name")}
                        className="mb-2"
                        readOnly={isLoading}
                        maxLength={50}
                    />
                    <TextBox
                        {...register("caseTypeContains")}
                        placeholder={translate("Case Type")}
                        className="mb-2"
                        readOnly={isLoading}
                        maxLength={50}
                    />
                </div>
                <div className="col-4">
                    <CaseStatusCategoryComboBox
                        name="statusCategoryExact"
                        control={control}
                        disabled={isLoading}
                        placeholder={translate("Status Category")}
                        className="mb-2"
                    />
                    <TextBox
                        {...register("reportCategoryContains")}
                        placeholder={translate("Report Category")}
                        className="mb-2"
                        readOnly={isLoading}
                        maxLength={10}
                    />
                </div>
            </div>
        </SearchCriteria>
    );
}