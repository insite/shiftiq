import { shiftClient } from "@/api/shiftClient";
import Button from "@/components/Button";
import FormCard from "@/components/form/FormCard";
import TextArea from "@/components/TextArea";
import { useStatusProvider } from "@/contexts/status/StatusProviderContext";
import { useState } from "react";

export default function TestPermissions() {
    const [isRequesting, setIsRequesting] = useState(false);
    const [permissions, setPermissions] = useState("");

    const { addError, removeError } = useStatusProvider();

    async function handleClick() {
        setIsRequesting(true);

        try {
            const result = await shiftClient.me.permissions();
            setPermissions(result ? JSON.stringify(result, null, 2) : "Not Found");

            removeError();
        } catch (err) {
            addError(err);
        } finally {
            setIsRequesting(false);
        }
    }

    return (
        <FormCard hasBottomMargin={false}>
            <div className="mb-3">
                <Button
                    type="button"
                    variant="request"
                    isLoading={isRequesting}
                    text="Request Permissions"
                    onClick={handleClick}
                />
            </div>
            <div>
                <TextArea
                    readOnly
                    rows={20}
                    value={permissions}
                />
            </div>
        </FormCard>
    );
}