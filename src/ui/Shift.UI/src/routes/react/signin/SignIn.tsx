import { shiftClient } from "@/api/shiftClient";
import { cache } from "@/cache/cache";
import Button from "@/components/Button";
import FormField from "@/components/form/FormField";
import FormSection from "@/components/form/FormSection";
import Icon from "@/components/icon/Icon";
import TextBox from "@/components/TextBox";
import { useSiteProvider } from "@/contexts/site/SiteProviderContext";
import { useStatusProvider } from "@/contexts/status/StatusProviderContext";
import { cookieHelper } from "@/helpers/cookieHelper";
import { cssHelper } from "@/helpers/cssHelper";
import { localStorageHelper } from "@/helpers/localStorageHelper";
import { shiftConfig } from "@/helpers/shiftConfig";
import { useState } from "react";
import { Spinner } from "react-bootstrap";
import { useForm } from "react-hook-form";
import { useNavigate, useSearchParams } from "react-router";

interface FormFields {
    email: string;
    organizationCode: string;
    impersonate: boolean;
    impersonatorUserEmail: string;
    impersonatorOrganizationCode: string;
}

export default function SignIn() {
    const [isLogginIn, setIsLogginIn] = useState(false);
    const [isChangingTheme, setIsChangingTheme] = useState(false);
    const [theme, setTheme] = useState(cookieHelper.getTheme);
    const [impersonate, setImpersonate] = useState(false);

    const { register, handleSubmit, getValues, formState: { errors } } = useForm<FormFields>({
        defaultValues: async () => {
            const account = localStorageHelper.getTempAccount();

            return Promise.resolve({
                email: account?.email ?? shiftConfig.localUser ?? "",
                organizationCode: account?.organization ?? shiftConfig.localOrganization ?? "",
                impersonate: account?.impersonate ?? false,
                impersonatorUserEmail: account?.impersonatorEmail ?? shiftConfig.localUser ?? "",
                impersonatorOrganizationCode: account?.impersonatorOrganization ?? shiftConfig.localOrganization ?? "",
            });
        }
    });

    const { siteSetting: { UserName }, refreshSiteSetting } = useSiteProvider();
    const { addError } = useStatusProvider();

    const navigate = useNavigate();
    const [searchParams] = useSearchParams();

    async function handleValidSubmit(fields: FormFields) {
        const impersonatorOrganizationCode = fields.impersonate ? fields.impersonatorOrganizationCode : null;
        const impersonatorUserEmail = fields.impersonate ? fields.impersonatorUserEmail : null;

        setIsLogginIn(true);

        try {
            await shiftClient.cookie.login(fields.organizationCode, fields.email, impersonatorOrganizationCode, impersonatorUserEmail);
            refreshSiteSetting();
            cache.clear();

            localStorageHelper.setTempAccount({
                email: fields.email,
                organization: fields.organizationCode,
                impersonate: fields.impersonate,
                impersonatorOrganization: fields.impersonatorOrganizationCode,
                impersonatorEmail: fields.impersonatorUserEmail,
            });

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

    async function handleThemeClick() {
        if (isChangingTheme) {
            return;
        }

        const newTheme = theme === "dark" ? "light" : "dark";

        setTheme(newTheme);
        setIsChangingTheme(true);

        try {
            await shiftClient.cookie.changeTheme(newTheme);
        } catch (error) {
            addError(error, "Failed to change theme");
        } finally {
            setIsChangingTheme(false);
        }

        const latestTheme = cookieHelper.getTheme();

        cssHelper.refreshTheme();

        setTheme(latestTheme);
    }

    function handleImpersonateChange() {
        setImpersonate(Boolean(getValues("impersonate")));
    }

    return (
        <>
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
                    <FormField>
                        <label>
                            <input
                                type="checkbox"
                                {...register("impersonate", {
                                    onChange: handleImpersonateChange,
                                })}
                                disabled={isLogginIn}
                            /> Impersonate
                        </label>
                    </FormField>
                    {impersonate && (
                        <>
                            <FormField label="Impersonator Organization" required>
                                <TextBox
                                    {...register("impersonatorOrganizationCode", {
                                        required: true
                                    })}
                                    error={errors.impersonatorOrganizationCode}
                                    disabled={isLogginIn}
                                />
                            </FormField>
                            <FormField label="Impersonator Email" required>
                                <TextBox
                                    {...register("impersonatorUserEmail", {
                                        validate: value => {
                                            if (!value) {
                                                return "The field is required";
                                            }
                                            if (value.toLowerCase() === getValues("email").toLowerCase()) {
                                                return "Impersonator Email and Email cannot be the same"
                                            }
                                            return undefined;
                                        }
                                    })}
                                    error={errors.impersonatorUserEmail}
                                    disabled={isLogginIn}
                                />
                            </FormField>
                        </>
                    )}
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

            <FormSection className="w-50">
                <FormField label="Light/dark mode switch">
                    <div className="d-flex align-items-center mt-2">
                        <div className="form-check form-switch mode-switch" data-bs-toggle="mode" onClick={handleThemeClick}>
                            <input
                                type="checkbox"
                                className="form-check-input"
                                defaultChecked={theme === "dark"}
                            />
                            <label className="form-check-label">
                                <Icon style="light" name="sun-bright" className="fs-lg" />
                            </label>
                            <label className="form-check-label">
                                <Icon style="light" name="moon" className="fs-lg" />
                            </label>
                        </div>

                        {isChangingTheme && (
                            <>
                                <Spinner animation="border" role="status" size="sm" className="ms-2 me-2" />
                                Saving ...
                            </>
                        )}
                    </div>
                </FormField>
            </FormSection>
        </>
    );
}