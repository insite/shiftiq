import SearchCriteria from "@/components/search/SearchCriteria";
import { NotificationCriteria } from "./NotificationCriteria";
import { useForm } from "react-hook-form";
import { useSearch } from "@/components/search/Search";
import TextBox from "@/components/TextBox";
import { translate } from "@/helpers/translate";
import ControlledComboBox from "@/components/combobox/ControlledComboBox";

export default function NotificationSearch_Criteria() {
    const { criteria, isLoading, setCriteria } = useSearch<NotificationCriteria, object>();

    const { register, control, handleSubmit, getValues, reset } = useForm<NotificationCriteria>({
        defaultValues: criteria
    });

    return (
        <SearchCriteria<NotificationCriteria>
            contentSize="size-3"
            control={control}
            onGetCriteria={getValues}
            onSubmit={handleSubmit(setCriteria)}
            onReset={reset}
        >
            <div className="row">
                <div className="col-4">
                    <TextBox
                        {...register("title")}
                        placeholder={translate("Title")}
                        className="mb-2"
                        readOnly={isLoading}
                        maxLength={200}
                    />
                    <ControlledComboBox
                        className="mb-2"
                        control={control}
                        name="onlyVisibleOnDashboard"
                        items={[
                            { value: "no", text: "All Notifications" },
                            { value: "yes", text: "Only Visible on Dashboard" },
                        ]}
                        disabled={isLoading}
                    />
                </div>
            </div>
        </SearchCriteria>
    );
}