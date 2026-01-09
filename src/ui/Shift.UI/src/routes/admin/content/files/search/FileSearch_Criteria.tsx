import TextBox from "@/components/TextBox";
import { translate } from "@/helpers/translate";
import { useForm } from "react-hook-form";
import { useSearch } from "@/components/search/Search";
import SearchCriteria from "@/components/search/SearchCriteria";
import { FileCriteria } from "./FileCriteria";
import ControlledDatePicker from "@/components/date/ControlledDatePicker";
import { useSiteProvider } from "@/contexts/SiteProvider";
import OrganizationFinder from "@/routes/_shared/finders/OrganizationFinder";
import FileObjectTypeComboBox from "@/routes/_shared/comboboxes/FileObjectTypeComboBox";
import FileVisibilityComboBox from "@/routes/_shared/comboboxes/FileVisibilityComboBox";
import UserFinder from "@/routes/_shared/finders/UserFinder";

export default function FileSearch_Criteria() {
    const { criteria, isLoading, setCriteria } = useSearch<FileCriteria, object>();
    const { siteSetting } = useSiteProvider();

    const { register, control, handleSubmit, getValues, reset } = useForm<FileCriteria>({
        defaultValues: criteria
    });

    return (
        <SearchCriteria<FileCriteria>
            contentSize="size-3"
            control={control}
            onGetCriteria={getValues}
            onSubmit={handleSubmit(setCriteria)}
            onReset={reset}
        >
            <div className="row">
                <div className="col-4">
                    <OrganizationFinder
                        control={control}
                        name="organizationIdentifier"
                        placeholder={translate("Organization")}
                        className="mb-2"
                        disabled={isLoading}
                    />
                    <FileObjectTypeComboBox
                        control={control}
                        name="objectType"
                        placeholder={translate("Object Type")}
                        className="mb-2"
                        disabled={isLoading}
                    />
                    <TextBox
                        {...register("objectIdentifier")}
                        placeholder={translate("Object Identifier")}
                        className="mb-2"
                        readOnly={isLoading}
                        maxLength={36}
                    />
                </div>
                <div className="col-4">
                    <TextBox
                        {...register("fileName")}
                        placeholder={translate("File Name")}
                        className="mb-2"
                        readOnly={isLoading}
                        maxLength={100}
                    />
                    <TextBox
                        {...register("documentName")}
                        placeholder={translate("Document Name")}
                        className="mb-2"
                        readOnly={isLoading}
                        maxLength={100}
                    />
                    <FileVisibilityComboBox
                        control={control}
                        name="visibility"
                        placeholder={translate("Visibility")}
                        className="mb-2"
                        disabled={isLoading}
                    />
                </div>
                <div className="col-4">
                    <ControlledDatePicker
                        control={control}
                        name="fileUploadedSince"
                        showTime={true}
                        defaultTimeZoneId={siteSetting.TimeZoneId}
                        className="mb-2"
                        readOnly={isLoading}
                        placeholder={translate("Uploaded Since")}
                    />
                    <ControlledDatePicker
                        control={control}
                        name="fileUploadedBefore"
                        showTime={true}
                        defaultTimeZoneId={siteSetting.TimeZoneId}
                        className="mb-2"
                        readOnly={isLoading}
                        placeholder={translate("Uploaded Before")}
                    />
                    <UserFinder
                        control={control}
                        name="fileUploadedBy"
                        placeholder={translate("Uploaded By")}
                        className="mb-2"
                        disabled={isLoading}
                    />
                </div>
            </div>
        </SearchCriteria>
    );
}