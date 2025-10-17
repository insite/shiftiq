import React, { ReactNode } from "react";
import Alert from "./Alert";

interface Props {
    children?: ReactNode;
}

interface State {
    error?: Error;
}

export class ErrorBoundary extends React.Component<Props, State> {
    constructor(props: Props) {
        super(props);
        this.state = {};
    }

    static getDerivedStateFromError(error: Error): State {
        return { error };
    }

    render() {
        if (this.state.error) {
            return (
                <Alert alertType="error" className="my-4 mx-4">
                    {this.state.error.message ?? "Unknown error"}
                </Alert>
            );
        }
        return this.props.children;
    }
}