import ActionLink from "@/components/ActionLink";
import AdminHome_Dashboard_Radial from "./AdminHome_Dashboard_Radial";

interface Props {
    title: string;
    summaryLabel: string;
    summaryCount: number;
    leftLabel: string;
    leftTotalCount: number;
    leftCount: number;
    rightLabel: string;
    rightTotalCount: number;
    rightCount: number;
    buttonHref: string;
    buttonText: string;
}

export default function AdminHome_Dashboard_Tile({
    title,
    summaryLabel,
    summaryCount,
    leftLabel,
    leftTotalCount,
    leftCount,
    rightLabel,
    rightTotalCount,
    rightCount,
    buttonHref,
    buttonText,
}: Props) {
    return (
        <div className="col-6 col-xl-3 d-flex">
            <div className="card border-1 shadow w-100 h-100">
                <div className="card-body d-flex flex-column align-items-center">
                    <h3 className="text-center">
                        {title}
                    </h3>

                    <div className="card-description">
                        {summaryLabel}: <span>{summaryCount}</span>
                    </div>

                    <div className="card-radials">
                        <AdminHome_Dashboard_Radial
                            totalCount={leftTotalCount}
                            count={leftCount}
                            label={leftLabel}
                        />
                        <AdminHome_Dashboard_Radial
                            totalCount={rightTotalCount}
                            count={rightCount}
                            label={rightLabel}
                        />
                    </div>

                    <ActionLink href={buttonHref} className="btn btn-sm btn-primary">
                        {buttonText}
                    </ActionLink>
                </div>
            </div>
        </div>
    );
}