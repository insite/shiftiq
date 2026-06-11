import { useState } from "react";
import { useStatusProvider } from "@/contexts/status/StatusProviderContext";

export function useSaveWithResultAction<T>(): {
    isSaving: boolean;
    runSave: (action: () => Promise<T>) => Promise<T | undefined>;
} {
    const [isSaving, setIsSaving] = useState(false);

    const { addError, removeError } = useStatusProvider();

    const [runSave] = useState(() => 
        async function run(action: () => Promise<T>): Promise<T | undefined> {
            if (isSaving) {
                throw new Error("Cannot use runSave while it is saving");
            }

            setIsSaving(true);

            let result: T;

            try {
                result = await action();
                removeError();
            } catch (err) {
                addError(err, "Failed to save");
                return undefined;
            } finally {
                setIsSaving(false);
            }

            return result;
        }
    );

    return {
        isSaving,
        runSave
    }
}