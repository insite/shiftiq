import ActionLink from "@/components/ActionLink";

interface Props {
    url: string;
    title: string;
    icon: string;
    isShortcut: boolean;
}

export default function  AdminHome_PanelTile({ url, title, icon, isShortcut }: Props) {
    return (
        <div className="col">
            <ActionLink className="card card-hover card-tile border-1 shadow" href={url}>
                <div className={`card-body text-center ${isShortcut ? "text-danger" : ""}`}>
                    <i className={`${icon} fa-3x mb-3`}></i>
                    <h3 className={`h5 ${isShortcut ? "" : "text-nowrap"} nav-heading mb-2`}>{title}</h3>
                </div>
            </ActionLink>
        </div>
    );
}