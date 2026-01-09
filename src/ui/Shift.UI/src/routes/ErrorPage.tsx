import Alert from "@/components/Alert";
import { useRouteError } from "react-router";

export default function ErrorPage() {
    const error = useRouteError();

    return (
        <Alert alertType="error" className="my-4 mx-4">
            {String(error)}
        </Alert>
    );
}