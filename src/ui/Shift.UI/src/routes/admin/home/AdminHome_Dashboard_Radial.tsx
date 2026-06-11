interface Props {
    totalCount: number;
    count: number;
    label: string;
}

export default function AdminHome_Dashboard_Radial({ totalCount, count, label }: Props) {
    return (
        <div className="d-flex flex-column align-items-center">
            <div className="mb-2">
                <div className="circular-progress skills-progress" style={{ "--ar-progress-value": Math.round(100 * count / totalCount) } as React.CSSProperties}>
                    <span>{count}</span>
                </div>
            </div>
            <div className="chart-status">
                {label}
            </div>
        </div>
    );
}