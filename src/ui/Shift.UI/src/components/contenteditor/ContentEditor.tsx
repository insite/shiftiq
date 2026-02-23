import "./ContentEditor.css";

import { Tab, Tabs } from "react-bootstrap";
import ContentEdior_Editor from "./ContentEditor_Editor";
import { FieldErrors, useFieldArray, useForm } from "react-hook-form";
import ValidationSummary from "../ValidationSummary";
import { ContentEditorValues } from "./ContentEditorValues";
import FormCard from "../form/FormCard";
import Button from "../Button";
import { useSaveAction } from "@/hooks/useSaveAction";
import { useLocation, useNavigate, useParams } from "react-router";
import { formRouteHelper } from "@/routes/formRouteHelper";
import { useState } from "react";
import { useSiteProvider } from "@/contexts/SiteProvider";
import { useLoadAction } from "@/hooks/useLoadAction";
import { ContentEditorResult } from "./ContentEditorResult";

interface Props {
    defaultValues: ContentEditorValues | ((id: string) => Promise<ContentEditorValues>);
    onSave: (id: string, result: ContentEditorResult) => Promise<boolean>;
}

export default function ContentEdior({
    defaultValues,
    onSave
}: Props
) {
    const params = useParams();
    const id = params["id"];
    const navigate = useNavigate();
    const location = useLocation();
    const backUrl = formRouteHelper.getParentUrl(location.pathname, params)!;

    const [activeKey, setActiveKey] = useState("0");
    const [saveStatusVisible, setSaveStatusVisible] = useState(false);
    const [noChangesVisible, setNoChangesVisible] = useState(false);

    const { siteSetting: { CurrentLanguage: language } } = useSiteProvider();

    const { isLoaded, runLoad } = useLoadAction(async () => {
        if (typeof defaultValues !== "function") {
            return defaultValues;
        }
        if (!id) {
            throw new Error("ID is not specified");
        }
        return await defaultValues(id);
    });

    const { isSaving, runSave } = useSaveAction();
    const [clickedButton, setClickedButton] = useState<"save" | "saveAndClose" | null>(null);

    const { handleSubmit, getFieldState, formState: { errors }, control } = useForm<ContentEditorValues>({
        defaultValues: runLoad,
    });

    const { fields } = useFieldArray({
        control,
        name: "editors",
    });

    async function handleValidSubmit(values: ContentEditorValues) {
        setClickedButton("save");

        let hasChanges = false;

        if (await runSave(async () => hasChanges = await onSave(id!, getResult(values))) ) {
            if (hasChanges) {
                setSaveStatusVisible(true);
                setTimeout(() => setSaveStatusVisible(false), 5000);
            } else {
                setNoChangesVisible(true);
                setTimeout(() => setNoChangesVisible(false), 5000);
            }
        }
    }

    async function handleValidSubmitWithClose(values: ContentEditorValues) {
        setClickedButton("saveAndClose");

        if (await runSave(() => onSave(id!, getResult(values)))) {
            navigate(backUrl);
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
        
        let hasChanges = false;
        for (let i = 0; i < values.editors.length; i++) {
            if (values.editors[i].control) {
                continue;
            }
            const { isDirty } = getFieldState(`editors.${i}.value`);
            if (isDirty) {
                hasChanges = true;
                break;
            }
        }

        if (hasChanges) {
            for (let i = 0; i < values.editors.length; i++) {
                if (values.editors[i].control) {
                    continue;
                }
                result.fields.push({
                    fieldName: values.editors[i].options.fieldName,
                    value: values.editors[i].value,
                });
            }
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
                                        disabled={isSaving}
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
                disabled={!isLoaded || isSaving && clickedButton !== "saveAndClose"}
                isLoading={isSaving && clickedButton === "saveAndClose"}
                onClick={handleSubmit(handleValidSubmitWithClose, handleErrorSubmit)}
            />
            <Button
                type="button"
                variant="save"
                className="btn-default me-2"
                disabled={!isLoaded || isSaving && clickedButton !== "save"}
                isLoading={isSaving && clickedButton === "save"}
                onClick={handleSubmit(handleValidSubmit, handleErrorSubmit)}
            />
            <Button
                variant="close"
                href={backUrl}
                disabled={isSaving}
            />

            {saveStatusVisible && (
                <span className="ms-2 p-1 badge text-bg-dark">Changes have been saved</span>
            )}

            {noChangesVisible && (
                <span className="ms-2 p-1 badge text-bg-dark">No changes to save</span>
            )}
        </form>
    )
}