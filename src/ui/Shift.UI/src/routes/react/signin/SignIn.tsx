import { shiftClient } from "@/api/shiftClient";
import { cache } from "@/cache/cache";
import Button from "@/components/Button";
import FormField from "@/components/form/FormField";
import FormSection from "@/components/form/FormSection";
import TextBox from "@/components/TextBox";
import { useSiteProvider } from "@/contexts/SiteProvider";
import { useStatusProvider } from "@/contexts/StatusProvider";
import { shiftConfig } from "@/helpers/shiftConfig";
import { useState } from "react";
import { useForm } from "react-hook-form";
import { useNavigate, useSearchParams } from "react-router";

interface FormFields {
    email: string;
    organizationCode: string;
}

export default function SignIn() {
    const [isLogginIn, setIsLogginIn] = useState(false);

    const { register, handleSubmit, formState: { errors } } = useForm<FormFields>({
        defaultValues: {
            email: shiftConfig.localUser ?? "",
            organizationCode: shiftConfig.localOrganization ?? "",
        }
    });

    const { siteSetting: { UserName }, refreshSiteSetting } = useSiteProvider();
    const { addError } = useStatusProvider();

    const navigate = useNavigate();
    const [searchParams] = useSearchParams();

    async function handleValidSubmit(fields: FormFields) {
        setIsLogginIn(true);

        try {
            await shiftClient.cookie.login(fields.organizationCode, fields.email);
            refreshSiteSetting();
            cache.clear();

            const url = searchParams.get("returnUrl") ?? "/client/react/home";
            navigate(url);
        } catch (error) {
            addError(error, "Failed to login");
        } finally {
            setIsLogginIn(false);
        }
    }

    async function handleLogout() {
        setIsLogginIn(true);

        try {
            await shiftClient.cookie.logout();
            refreshSiteSetting();
            cache.clear();
        } catch (error) {
            addError(error, "Failed to logout");
        } finally {
            setIsLogginIn(false);
        }
    }

    return (
        <form autoComplete="off" onSubmit={handleSubmit(handleValidSubmit)}>
            <FormSection className="w-50">
                <FormField label="Organization" required>
                    <TextBox
                        autoFocus
                        {...register("organizationCode", {
                            required: true
                        })}
                        error={errors.organizationCode}
                        disabled={isLogginIn}
                    />
                </FormField>
                <FormField label="Email" required>
                    <TextBox
                        {...register("email", {
                            required: true
                        })}
                        error={errors.email}
                        disabled={isLogginIn}
                    />
                </FormField>
                <FormField hasBottomMargin={false}>
                    <Button
                        variant="save"
                        text="Login"
                        loadingMessage="Logging In..."
                        isLoading={isLogginIn}
                    />
                    {UserName && (
                        <Button
                            type="button"
                            variant="delete"
                            text="Logout"
                            className="ms-2"
                            onClick={handleLogout}
                        />
                    )}
                </FormField>
            </FormSection>
        </form>
    );
}