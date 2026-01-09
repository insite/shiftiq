import { formRouteHelper } from "@/routes/formRouteHelper";
import { useLocation, useNavigate, useParams } from "react-router";
import { useLoadAction } from "./useLoadAction";
import { useSaveAction } from "./useSaveAction";
import { useForm } from "react-hook-form";

export function useEditForm<FormValues extends object>(
    onLoad: (id: string) => Promise<FormValues>,
    onSave: (id: string, values: FormValues) => Promise<unknown>,
) {
    const params = useParams();
    const id = params["id"];
    if (!id) {
        throw new Error(`ID is not specified`);
    }

    const navigate = useNavigate();
    const location = useLocation();
    const backUrl = formRouteHelper.getParentUrl(location.pathname, params)!;

    const { isLoaded, runLoad } = useLoadAction(() => onLoad(id));
    const { isSaving, runSave } = useSaveAction();

    const { register, control, handleSubmit, formState: { errors } } = useForm({
        defaultValues: runLoad
    });

    async function handleSave(values: FormValues) {
        if (await runSave(() => onSave(id!, values))) {
            navigate(backUrl);
        }
    }

    return {
        backUrl,
        isLoaded,
        isSaving,
        isDisabled: !isLoaded || isSaving,
        register,
        control,
        errors,
        handleSubmit: handleSubmit(handleSave),
    }
}