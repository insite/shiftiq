import "./ContentEditor.css";

import { Tab, Tabs } from "react-bootstrap";
import ContentEdior_Editor from "./ContentEditor_Editor";
import { FieldErrors, useFieldArray, useForm } from "react-hook-form";
import ValidationSummary from "../ValidationSummary";
import { ContentEditorValues } from "./ContentEditorValues";
import FormCard from "../form/FormCard";
import Button from "../Button";
import { useSaveAction } from "@/hooks/useSaveAction";
import { useLocation, useNavigate, useParams, useSearchParams } from "react-router";
import { formRouteHelper } from "@/routes/formRouteHelper";
import { useEffect, useRef, useState } from "react";
import { useSiteProvider } from "@/contexts/site/SiteProviderContext";
import { useLoadAction } from "@/hooks/useLoadAction";
import { ContentEditorResult } from "./ContentEditorResult";
import { urlHelper } from "@/helpers/urlHelper";
import { useNavigationPreventer } from "@/hooks/useNavigationPreventer";

interface Props {
    isDirty?: boolean;
    defaultValues: ContentEditorValues | ((id: string) => Promise<ContentEditorValues>);
    onSave: (id: string, result: ContentEditorResult) => Promise<boolean>;
    onGetBackUrl?: (id: string, selectedFieldName: string) => string;
    onTabSelected?: (id: string, selectedFieldName: string) => void;
}

export default function ContentEdior({
    isDirty: isCustomDirty = false,
    defaultValues,
    onSave,
    onGetBackUrl,
    onTabSelected
}: Props
) {
    const params = useParams();
    const id = params["id"];
    const [searchParams] = useSearchParams();
    const tab = searchParams.get("tab");
    const navigate = useNavigate();
    const location = useLocation();

    const [originalValues, setOriginalValue] = useState<ContentEditorValues | null>(null);
    const [activeKey, setActiveKey] = useState("0");
    const [status, setStatus] = useState<"none" | "saved" | "noChanges" | "error">("none");
    const [redirectAfterSave, setRedirectAfterSave] = useState<string | null>(null);

    const { siteSetting: { CurrentLanguage: language } } = useSiteProvider();

    const { isLoaded, runLoad } = useLoadAction(async () => {
        let newDefaultValues: ContentEditorValues;
        if (typeof defaultValues !== "function") {
            newDefaultValues = defaultValues;
        } else if (!id) {
            throw new Error("ID is not specified");
        } else {
            newDefaultValues = await defaultValues(id);
        }

        if (tab) {
            const tabIndex = newDefaultValues.editors.findIndex(x => x.options.fieldName.toLowerCase() === tab.toLowerCase());
            if (tabIndex >= 0) {
                setActiveKey(String(tabIndex));
            }
        }

        setOriginalValue(newDefaultValues);

        return newDefaultValues;
    });

    const { isSaving, runSave } = useSaveAction();
    const [clickedButton, setClickedButton] = useState<"save" | "saveAndClose" | null>(null);

    const { handleSubmit, reset, formState: { errors, isDirty }, control } = useForm<ContentEditorValues>({
        defaultValues: runLoad,
    });

    const { fields } = useFieldArray({
        control,
        name: "editors",
    });

    const timeoutRef = useRef<number>(null);

    useNavigationPreventer(isLoaded && (isDirty || isCustomDirty) && !redirectAfterSave);

    const isSavingOrRedirecting = isSaving || !!redirectAfterSave;

    useEffect(() => {
        if (!id || !originalValues || !onTabSelected) {
            return;
        }

        const selectedFieldName = originalValues.editors[Number(activeKey)].options.fieldName;

        onTabSelected(id, selectedFieldName);
    }, [activeKey, originalValues, id, onTabSelected]);

    useEffect(() => {
        if (!redirectAfterSave) {
            return;
        }

        if (redirectAfterSave.toLowerCase().startsWith("/client/")) {
            navigate(redirectAfterSave);
        } else {
            window.location.href = urlHelper.getActionUrl(redirectAfterSave);
        }
    }, [redirectAfterSave, navigate])

    function getBackUrl() {
        if (!id || !onGetBackUrl || !originalValues) {
            return formRouteHelper.getParentUrl(location.pathname, params)!;
        }

        const selectedFieldName = originalValues.editors[Number(activeKey)].options.fieldName;

        return onGetBackUrl(id, selectedFieldName);
    }

    async function handleValidSubmit(values: ContentEditorValues) {
        setClickedButton("save");

        let hasChanges: boolean = false;
        const success = await runSave(async () => hasChanges = await onSave(id!, getResult(values)));

        if (timeoutRef.current) {
            clearTimeout(timeoutRef.current);
        }

        if (!success) {
            setStatus("error");
        }
        else if (hasChanges) {
            reset(values);
            setStatus("saved");
        } else {
            setStatus("noChanges");
        }

        timeoutRef.current = window.setTimeout(() => {
            timeoutRef.current = null;
            setStatus("none");
        }, 5000);
    }

    async function handleValidSubmitWithClose(values: ContentEditorValues) {
        setClickedButton("saveAndClose");

        if (await runSave(() => onSave(id!, getResult(values)))) {
            setRedirectAfterSave(getBackUrl());
        }
    }

    function handleErrorSubmit(errors: FieldErrors<ContentEditorValues>) {
        const index = errors.editors?.findIndex?.(x => x?.value?.message);
        if (index !== undefined && index >= 0) {
            setActiveKey(String(index));
        }
    }

    function getResult(values: ContentEditorValues): ContentEditorResult {
        const result: ContentEditorResult = {
            fields: []
        };

        if (!isDirty) {
            return result;
        }
        
        for (let i = 0; i < values.editors.length; i++) {
            if (values.editors[i].control) {
                continue;
            }
            result.fields.push({
                fieldName: values.editors[i].options.fieldName,
                value: values.editors[i].value,
            });
        }

        return result;
    }

    return (
        <form
            autoComplete="off"
            onSubmit={handleSubmit(handleValidSubmit, handleErrorSubmit)}
            className="content-editor"
        >
            <ValidationSummary errors={errors} />

            {isLoaded && (
                <FormCard>
                    <Tabs
                        activeKey={activeKey}
                        transition={false}
                        onSelect={key => setActiveKey(key ?? "0")}
                    >
                        {fields.map((v, index) => (
                            <Tab
                                key={index}
                                eventKey={index}
                                title={v.options.title}
                                className={v.options.type === "markdownAndHtml" ? "markdown-and-html" : ""} 
                            >
                                {v.control ?? (
                                    <ContentEdior_Editor
                                        control={control}
                                        name={`editors.${index}.value`}
                                        options={v.options}
                                        defaultLanguage={language}
                                        disabled={isSavingOrRedirecting}
                                    />
                                )}
                            </Tab>
                        ))}
                    </Tabs>
                </FormCard>
            )}

            <Button
                type="button"
                variant="save"
                text="Save &amp; Close"
                className="me-2"
                disabled={!isLoaded || isSavingOrRedirecting && clickedButton !== "saveAndClose"}
                isLoading={isSavingOrRedirecting && clickedButton === "saveAndClose"}
                onClick={handleSubmit(handleValidSubmitWithClose, handleErrorSubmit)}
            />
            <Button
                type="button"
                variant="save"
                className="btn-default me-2"
                disabled={!isLoaded || isSavingOrRedirecting && clickedButton !== "save"}
                isLoading={isSavingOrRedirecting && clickedButton === "save"}
                onClick={handleSubmit(handleValidSubmit, handleErrorSubmit)}
            />
            <Button
                variant="close"
                href={getBackUrl()}
                disabled={isSavingOrRedirecting}
            />

            {status === "saved" && (
                <span className="ms-2 p-1 badge text-bg-dark">Changes have been saved</span>
            )}

            {status === "noChanges" && (
                <span className="ms-2 p-1 badge text-bg-dark">No changes to save</span>
            )}

            {status === "error" && (
                <span className="ms-2 p-1 badge text-bg-danger">Error while trying to save data</span>
            )}
        </form>
    )
}