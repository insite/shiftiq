import Alert from "@/components/Alert";
import { useLocation } from "react-router";

export default function NotFound() {
    const location = useLocation();
    return (
        <Alert alertType="error" className="mt-4">
            The page &nbsp;<b>{location.pathname}</b>&nbsp; is not found
        </Alert>
    );
}