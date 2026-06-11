import { useEditForm } from "@/hooks/useEditForm";
import { fromApiNotification, NotificationFormValues, toApiNotificationFormValues } from "../_shared/NotificationFormValues";
import { shiftClient } from "@/api/shiftClient";
import ValidationSummary from "@/components/ValidationSummary";
import FormCard from "@/components/form/FormCard";
import NotificationDetail from "../_shared/NotificationDetail";
import FormField from "@/components/form/FormField";
import Button from "@/components/Button";
import { useSiteProvider } from "@/contexts/site/SiteProviderContext";
import { useNavigate } from "react-router";
import { cache } from "@/cache/cache";

export default function NotificationEdit() {
    const {
        id,
        backUrl,
        isLoaded,
        isSaving,
        isDisabled,
        register,
        control,
        errors,
        handleSubmit,
    } = useEditForm(load, save);

    const navigate = useNavigate();

    const { siteSetting: { TimeZoneId: timeZoneId } } = useSiteProvider();

    async function load (id: string) {
        const model = await shiftClient.dashboard.retrieveNotification(id);
        return model ? fromApiNotification(model, timeZoneId) : null as unknown as NotificationFormValues;
    }

    async function onDelete() {
        if (!window.confirm("Are you sure to delete this notification?")) {
            return;
        }

        await shiftClient.dashboard.deleteNotification(id);
        cache.search.clearRows("search.notification");
        navigate(backUrl);
    }

    return (
        <form autoComplete="off" onSubmit={handleSubmit}>
            <ValidationSummary errors={errors} />

            <FormCard>
                <NotificationDetail
                    notificationId={id}
                    disabled={isDisabled}
                    register={register}
                    control={control}
                    errors={errors}
                    onDelete={onDelete}
                />

                <FormField hasBottomMargin={false}>
                    <Button
                        variant="save"
                        className="me-2"
                        disabled={!isLoaded}
                        isLoading={isSaving}
                    />
                    <Button variant="cancel" href={backUrl} />
                </FormField>
            </FormCard>
        </form>
    );
}

async function save (id: string, values: NotificationFormValues) {
    const notification = toApiNotificationFormValues(id, values);
    await shiftClient.dashboard.modifyNotification(notification);
    cache.search.clearRows("search.notification");
}