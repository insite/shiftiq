import ActionLink from "@/components/ActionLink";

export default function ReactHome() {
    return (
        <>
            <ul>
                <li className="mb-3"><ActionLink href="/client/test">/client/test</ActionLink></li>
                <li><ActionLink href="/client/react/signin">/client/react/signin</ActionLink></li>
                <li><ActionLink href="/client/admin/records/gradebooks/search">/client/admin/records/gradebooks/search</ActionLink></li>
                <li><ActionLink href="/client/admin/content/files/search">/client/admin/content/files/search</ActionLink></li>
                <li><ActionLink href="/client/admin/workflows/case-statuses/search">/client/admin/workflows/case-statuses/search</ActionLink></li>
            </ul>
        </>
    );
}