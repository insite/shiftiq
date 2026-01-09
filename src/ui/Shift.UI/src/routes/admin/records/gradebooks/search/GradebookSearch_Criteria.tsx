import TextBox from "@/components/TextBox";
import { translate } from "@/helpers/translate";
import { useForm } from "react-hook-form";
import { useSearch } from "@/components/search/Search";
import SearchCriteria from "@/components/search/SearchCriteria";
import { GradebookCriteria } from "./GradebookCriteria";
import ControlledDatePicker from "@/components/date/ControlledDatePicker";
import { useSiteProvider } from "@/contexts/SiteProvider";
import PeriodFinder from "@/routes/_shared/finders/PeriodFinder";
import GradebookStatusComboBox from "@/routes/_shared/comboboxes/GradebookStatusComboBox";
import FrameworkFinder from "@/routes/_shared/finders/FrameworkFinder";
import AchievementFinder from "@/routes/_shared/finders/AchievementFinder";
import InstructorFinder from "@/routes/_shared/finders/InstructorFinder";

export default function GradebookSearch_Criteria() {
    const { criteria, isLoading, setCriteria } = useSearch<GradebookCriteria, object>();
    const { siteSetting } = useSiteProvider();

    const { register, control, handleSubmit, getValues, reset } = useForm<GradebookCriteria>({
        defaultValues: criteria
    });

    return (
        <SearchCriteria<GradebookCriteria>
            contentSize="size-3"
            control={control}
            onGetCriteria={getValues}
            onSubmit={handleSubmit(setCriteria)}
            onReset={reset}
        >
            <div className="row">
                <div className="col-4">
                    <TextBox
                        {...register("gradebookTitle")}
                        placeholder={translate("Title")}
                        className="mb-2"
                        readOnly={isLoading}
                        maxLength={100}
                    />
                    <ControlledDatePicker
                        control={control}
                        name="gradebookCreatedSince"
                        showTime={true}
                        defaultTimeZoneId={siteSetting.TimeZoneId}
                        className="mb-2"
                        readOnly={isLoading}
                        placeholder="Gradebook Created Since"
                    />
                    <ControlledDatePicker
                        control={control}
                        name="gradebookCreatedBefore"
                        showTime={true}
                        defaultTimeZoneId={siteSetting.TimeZoneId}
                        className="mb-2"
                        readOnly={isLoading}
                        placeholder="Gradebook Created Before"
                    />
                    <PeriodFinder
                        control={control}
                        name="periodIdentifier"
                        placeholder="Gradebook Period"
                        className="mb-2"
                        disabled={isLoading}
                    />
                </div>
                <div className="col-4">
                    <AchievementFinder
                        name="achievementIdentifier"
                        control={control}
                        disabled={isLoading}
                        placeholder={translate("Achievement")}
                        className="mb-2"
                    />
                    <FrameworkFinder
                        control={control}
                        name="frameworkIdentifier"
                        placeholder="Framework"
                        className="mb-2"
                        disabled={isLoading}
                    />
                    <GradebookStatusComboBox
                        name="gradebookStatus"
                        control={control}
                        disabled={isLoading}
                        placeholder={translate("Gradebook Status")}
                        className="mb-2"
                    />
                </div>
                <div className="col-4">
                    <TextBox
                        {...register("classTitle")}
                        placeholder={translate("Class Title")}
                        className="mb-2"
                        readOnly={isLoading}
                        maxLength={100}
                    />
                    <ControlledDatePicker
                        control={control}
                        name="classStartedSince"
                        showTime={true}
                        defaultTimeZoneId={siteSetting.TimeZoneId}
                        className="mb-2"
                        readOnly={isLoading}
                        placeholder="Class Started Since"
                    />
                    <ControlledDatePicker
                        control={control}
                        name="classStartedBefore"
                        showTime={true}
                        defaultTimeZoneId={siteSetting.TimeZoneId}
                        className="mb-2"
                        readOnly={isLoading}
                        placeholder="Class Started Before"
                    />
                    <InstructorFinder
                        control={control}
                        name="classInstructorIdentifier"
                        disabled={isLoading}
                        placeholder={translate("Class Instructor")}
                        className="mb-2"
                    />
                </div>
            </div>
        </SearchCriteria>
    );
}