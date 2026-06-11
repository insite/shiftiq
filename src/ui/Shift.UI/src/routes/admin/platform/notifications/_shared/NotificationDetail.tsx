import { Control, FieldErrors, UseFormRegister } from "react-hook-form";
import { NotificationFormValues } from "./NotificationFormValues";
import FormField from "@/components/form/FormField";
import { translate } from "@/helpers/translate";
import TextBox from "@/components/TextBox";
import ControlledDatePicker from "@/components/date/ControlledDatePicker";
import { useSiteProvider } from "@/contexts/site/SiteProviderContext";
import IconButton from "@/components/iconbutton/IconButton";
import ControlledComboBox from "@/components/combobox/ControlledComboBox";

interface Props {
    notificationId: string | null;
    disabled: boolean;
    register: UseFormRegister<NotificationFormValues>;
    control: Control<NotificationFormValues>;
    errors: FieldErrors<NotificationFormValues>;
    onDelete?(): void;
}

export default function NotificationDetail({
    notificationId,
    disabled,
    register,
    control,
    errors,
    onDelete,
}: Props)
{
    const { siteSetting: { TimeZoneId: timeZoneId } } = useSiteProvider();

    return (
        <div className="w-50">
            {notificationId && (
                <FormField label={translate("Notification Identifier")}>
                    {notificationId}
                    <IconButton
                        iconName="trash-alt"
                        iconStyle="solid"
                        className="ms-2"
                        disabled={disabled}
                        title="Delete the notification"
                        onClick={onDelete}
                    />
                </FormField>
            )}

            <FormField label={translate("Type")} required>
                <ControlledComboBox
                    control={control}
                    name="type"
                    disabled={disabled}
                    items={[
                        { value: "PlatformUpdate", text: "Platform Update" },
                        { value: "ReleaseNotes", text: "Release Notes" },
                        { value: "Other", text: "Other" },
                    ]}
                />
            </FormField>

            <FormField label={translate("Title")} required>
                <TextBox
                    {...register("title", {
                        required: true
                    })}
                    autoFocus
                    readOnly={disabled}
                    maxLength={200}
                    error={errors.title}
                />
            </FormField>

            <FormField label={translate("Details")}>
                <TextBox
                    {...register("details")}
                    readOnly={disabled}
                    maxLength={200}
                    error={errors.details}
                />
            </FormField>

            <FormField label={translate("Link Text")}>
                <TextBox
                    {...register("linkText")}
                    readOnly={disabled}
                    maxLength={200}
                    error={errors.linkText}
                />
            </FormField>

            <FormField label={translate("Link Url")}>
                <TextBox
                    {...register("linkUrl")}
                    readOnly={disabled}
                    maxLength={200}
                    error={errors.linkUrl}
                />
            </FormField>

            <FormField label={translate("Date Range")} bodyClassName="d-flex align-items-center justify-content-stretch gap-2">
                <ControlledDatePicker
                    control={control}
                    name="startDate"
                    showTime={true}
                    defaultTimeZoneId={timeZoneId}
                    readOnly={disabled}
                    className="flex-fill"
                />
                -
                <ControlledDatePicker
                    control={control}
                    name="endDate"
                    showTime={true}
                    defaultTimeZoneId={timeZoneId}
                    readOnly={disabled}
                    className="flex-fill"
                />
            </FormField>

            <FormField label={translate("Active?")} required>
                <label>
                    <input
                        type="radio"
                        {...register("isActive")}
                        radioGroup="isActive"
                        value="yes"
                        disabled={disabled}
                    />
                    Yes
                </label>
                <label>
                    <input
                        type="radio"
                        {...register("isActive")}
                        radioGroup="isActive"
                        value="no"
                        disabled={disabled}
                    />
                    No
                </label>
            </FormField>

        </div>
    );
}