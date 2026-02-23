import ActionLink from "@/components/ActionLink";

export default function ReactHome() {
    return (
        <>
            <ul>
                <li className="mb-3"><ActionLink href="/client/test">/client/test</ActionLink></li>
                <li><ActionLink href="/client/react/signin">/client/react/signin</ActionLink></li>
                <li><ActionLink href="/client/admin/records/gradebooks/search">/client/admin/records/gradebooks/search</ActionLink></li>
                <li><ActionLink href="/client/admin/content/files/search">/client/admin/content/files/search</ActionLink></li>
                <li><ActionLink href="/client/admin/workflows/cases-statuses/search">/client/admin/workflows/cases-statuses/search</ActionLink></li>
                <li><ActionLink href="/client/admin/sites/pages/content/e2986fa8-5580-45ee-a196-b0110145373f">/client/admin/sites/pages/content/e2986fa8-5580-45ee-a196-b0110145373f</ActionLink></li>
            </ul>
        </>
    );
}