import { shiftClient } from "@/api/shiftClient";
import Icon from "@/components/icon/Icon";
import { useStatusProvider } from "@/contexts/status/StatusProviderContext";
import { useEffect, useState } from "react";
import { Toast } from "react-bootstrap"

export default function MaintenanceToast() {
    const [isClosed, setIsClosed] = useState(false);
    const [description, setDescription] = useState("");

    const { addError, removeError } = useStatusProvider();

    useEffect(() => {
        run();

        async function run() {
            try {
                const result = await shiftClient.maintenance.maintenanceLockout();
                if (result && !result.IsClosed && result.Description) {
                    setDescription(result.Description);
                }
                removeError();
            } catch (error) {
                addError(error, "Error while loading maintenance status");
            }
        }
    }, [addError, removeError]);

    if (isClosed || !description) {
        return null;
    }

    return (
        <div className="position-fixed p-3" style={{ zIndex: "11" }}>
            <Toast>
                <Toast.Header className="bg-danger text-white" closeButton={false}>
                    <Icon style="solid" name="bell-on" className="me-2" />
                    <span className="me-auto">Important Notice</span>
                    <button
                        type="button"
                        className="btn-close btn-close-white ms-2 mb-1"
                        data-bs-dismiss="toast"
                        aria-label="Close"
                        onClick={() => setIsClosed(true)}
                    ></button>
                </Toast.Header>
                <Toast.Body className="text-danger">
                    <div 
                        dangerouslySetInnerHTML={{
                            __html: description
                        }}
                    />
                </Toast.Body>
            </Toast>
        </div>
    );
}