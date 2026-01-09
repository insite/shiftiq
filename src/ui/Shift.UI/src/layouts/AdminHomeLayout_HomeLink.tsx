import ActionLink from "@/components/ActionLink";

export default function AdminHomeLayout_HomeLink() {
    return (
        <ActionLink href="/client/admin/home" className='text-light'>
            <i className='fas fa-home me-1'></i>Home
        </ActionLink>
    );
}
