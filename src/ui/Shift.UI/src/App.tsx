import "@/styles/ComboBox.css";
import { createBrowserRouter, RouterProvider } from "react-router";
import { ErrorBoundary } from "./components/ErrorBoundary";
import { routes } from "./routes/routes";

const router = createBrowserRouter(routes);

export default function App() {
    return (
        <ErrorBoundary>
            <RouterProvider router={router} />
        </ErrorBoundary>
    );
}
