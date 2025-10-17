import { ReactNode } from "react";
import { Spinner, Stack } from "react-bootstrap";

interface Props {
    text?: string;
    children?: ReactNode;
}

export default function LoadingPanel({ text, children }: Props) {
    if (!text && !children) {
        text = "Loading...";
    }

    return (
        <Stack direction="horizontal" className="d-flex justify-content-center" style={{ height: "200px" }}>
            <Spinner animation="border" role="status" className="me-3" />
            {text ? <>{text}</> : <>{children}</>}
        </Stack>
    );
}