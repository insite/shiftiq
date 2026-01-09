import { useState } from "react";
import { useStatusProvider } from "@/contexts/StatusProvider";

export function useSaveAction(): {
    isSaving: boolean;
    runSave: (action: () => Promise<unknown>) => Promise<boolean>;
} {
    const [isSaving, setIsSaving] = useState(false);

    const { addError, removeError } = useStatusProvider();

    const [runSave] = useState(() => 
        async function run(action: () => Promise<unknown>): Promise<boolean> {
            if (isSaving) {
                throw new Error("Cannot use runSave while it is saving");
            }

            setIsSaving(true);

            try {
                await action();
                removeError();
            } catch (err) {
                addError(err, "Failed to save");
                return false;
            } finally {
                setIsSaving(false);
            }

            return true;
        }
    );

    return {
        isSaving,
        runSave
    }
}