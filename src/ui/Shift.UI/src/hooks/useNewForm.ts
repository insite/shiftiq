import { formRouteHelper } from "@/routes/formRouteHelper";
import { useLocation, useNavigate, useParams } from "react-router";
import { useSaveAction } from "./useSaveAction";
import { DefaultValues, useForm } from "react-hook-form";

export function useNewForm<FormValues extends object>(
    defaultValues: FormValues,
    outlineUrl: string,
    onSave: (values: FormValues) => Promise<string>,
) {
    const params = useParams();
    const navigate = useNavigate();
    const location = useLocation();
    const backUrl = formRouteHelper.getParentUrl(location.pathname, params)!;

    const { isSaving, runSave } = useSaveAction();

    const { register, control, handleSubmit, formState: { errors } } = useForm({
        defaultValues: defaultValues as DefaultValues<FormValues>
    });

    async function handleSave(values: FormValues) {
        let id: string;
        if (await runSave(async () => id = await onSave(values))) {
            if (outlineUrl) {
                navigate(`${outlineUrl}${id!}`);
            } else {
                navigate(backUrl);
            }
        }
    }

    return {
        backUrl,
        isSaving,
        register,
        control,
        errors,
        handleSubmit: handleSubmit(handleSave),
    }
}