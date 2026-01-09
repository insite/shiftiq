import { useEffect, useState } from "react";
import { useParams } from "react-router";
import { useLoadAction } from "./useLoadAction";

export function useOutlineForm<Model extends object>(
    onLoad: (id: string) => Promise<Model>,
) {
    const params = useParams();
    const id = params["id"];
    if (!id) {
        throw new Error(`ID is not specified`);
    }

    const [model, setModel] = useState<Model | null>(null);

    const { runLoad } = useLoadAction(() => onLoad(id!));

    const [reloadModel] = useState(() => async () => {
        const newModel = await onLoad(id);
        setModel(newModel);
    });

    useEffect(() => {
        runLoad().then(setModel);
    }, [runLoad]);

    return {
        id,
        model,
        reloadModel,
    };
}