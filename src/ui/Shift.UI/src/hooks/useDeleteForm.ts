import { useEffect, useState } from "react";
import { useLocation, useNavigate, useParams } from "react-router";
import { formRouteHelper } from "@/routes/formRouteHelper";
import { useLoadAction } from "./useLoadAction";
import { useSaveAction } from "./useSaveAction";

export function useDeleteForm<Model extends object>(
    onLoad: (id: string) => Promise<Model>,
    onDelete: (id: string) => Promise<boolean>,
) {
    const params = useParams();
    const id = params["id"];
    if (!id) {
        throw new Error(`ID is not specified`);
    }

    const navigate = useNavigate();
    const location = useLocation();
    const outlineUrl = formRouteHelper.getParentUrl(location.pathname, params) ?? "/client/admin/home";
    const searchUrl = formRouteHelper.getGrandParentUrl(location.pathname, params) ?? "/client/admin/home";

    const [model, setModel] = useState<Model | null>(null);

    const { runLoad } = useLoadAction(() => onLoad(id!));
    const { isSaving, runSave } = useSaveAction();

    useEffect(() => {
        runLoad().then(setModel);
    }, [runLoad]);

    async function handleDelete() {
        if (await runSave(() => onDelete(id!))) {
            navigate(searchUrl);
        }
    }

    return {
        model,
        backUrl: outlineUrl,
        isSaving,
        handleDelete,
    };
}