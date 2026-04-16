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
                <li><ActionLink href="/client/admin/assessment/forms/workshop/7f959c01-ef9d-0a71-8241-238c97448d67">/client/admin/assessment/forms/workshop/7f959c01-ef9d-0a71-8241-238c97448d67</ActionLink></li>
                <li><ActionLink href="/client/admin/assessment/specs/workshop/7f959c01-3d1e-7b76-be57-2fb8c82d5bfc">/client/admin/assessment/specs/workshop/7f959c01-3d1e-7b76-be57-2fb8c82d5bfc</ActionLink></li>
                <li><ActionLink href="/client/admin/assessment/specs/workshop/6c160462-3041-4f65-b302-3a61e0ae977d">/client/admin/assessment/specs/workshop/6c160462-3041-4f65-b302-3a61e0ae977d</ActionLink></li>
            </ul>
        </>
    );
}