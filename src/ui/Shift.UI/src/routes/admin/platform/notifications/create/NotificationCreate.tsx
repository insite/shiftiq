import { useNewForm } from "@/hooks/useNewForm";
import { NotificationFormValues, toApiNotificationFormValues } from "../_shared/NotificationFormValues";
import { shiftClient } from "@/api/shiftClient";
import ValidationSummary from "@/components/ValidationSummary";
import FormCard from "@/components/form/FormCard";
import FormField from "@/components/form/FormField";
import Button from "@/components/Button";
import NotificationDetail from "../_shared/NotificationDetail";
import { cache } from "@/cache/cache";

const defaultFormValues: NotificationFormValues = {
    type: "PlatformUpdate",
    title: "",
    details: "",
    linkText: "",
    linkUrl: "",
    startDate: null,
    endDate: null,
    isActive: "yes",
};

export default function NotificationCreate() {
    const {
        backUrl,
        isSaving,
        register,
        control,
        errors,
        handleSubmit
    } = useNewForm<NotificationFormValues>(defaultFormValues, null, save);

    return (
        <form autoComplete="off" onSubmit={handleSubmit}>
            <ValidationSummary errors={errors} />

            <FormCard>
                <NotificationDetail
                    notificationId={null}
                    disabled={isSaving}
                    register={register}
                    control={control}
                    errors={errors}
                />

                <FormField hasBottomMargin={false}>
                    <Button
                        variant="save"
                        className="me-2"
                        isLoading={isSaving}
                    />
                    <Button variant="cancel" href={backUrl} />
                </FormField>
            </FormCard>
        </form>
    );
}

async function save (values: NotificationFormValues) {
    const notification = toApiNotificationFormValues(null, values);
    const result = await shiftClient.dashboard.modifyNotification(notification);
    cache.search.clearRows("search.notification");
    return result?.NotificationId ?? null;
}