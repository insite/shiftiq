import "./CEPerformanceReport.css";

import ComboBox from "@/components/combobox/ComboBox";
import Icon from "@/components/icon/Icon";
import TextBox from "@/components/TextBox";

const comboItems = {
    regions: [
        { value: "", text: "All Regions" },
        { value: "north", text: "North" },
        { value: "south", text: "South" },
        { value: "west", text: "West" },
    ],
    genders: [
        { value: "", text: "All Genders" },
        { value: "female", text: "Female" },
        { value: "male", text: "Male" },
        { value: "unknown", text: "Not Specified" },
    ],
    categories: [
        { value: "", text: "Category" },
        { value: "compliance", text: "Compliance" },
        { value: "leadership", text: "Leadership" },
        { value: "conduct", text: "Conduct" },
    ],
};

const metrics = [
    { value: "116k", subValue: "of 120k", label: "Active\nLearners", percent: 97 },
    { value: "1.8M", subValue: "of 2.4M", label: "CEC Issued\nThis Year", percent: 75 },
    { value: "85%", subValue: "", label: "Compliance\nRate", percent: 85 },
];

const categorySegments = [
    { label: "HR Compliance", percent: 45, color: "#62d6d8" },
    { label: "Leadership", percent: 25, color: "#43b5cf" },
    { label: "Workplace Conduct", percent: 20, color: "#2f92be" },
    { label: "DEI", percent: 10, color: "#395f8f" },
];

const courses = [
    { title: "Advanced HR Law", issuances: "2,000 issuances", completion: "92% completion" },
    { title: "Conduct Standards", issuances: "2,000 issuances", completion: "92% completion" },
    { title: "DEI for HR Leaders", issuances: "2,000 issuances", completion: "92% completion" },
];

const topCategories = [
    "40% Compliance",
    "30% Leadership",
    "12% Conduct",
    "9% DEI",
];

const trend = [
    { label: "Q2 2025", value: 400000 },
    { label: "Q3 2024", value: 350000 },
    { label: "Q4 2024", value: 600000 },
    { label: "Q1 2025", value: 520000 },
    { label: "Q2 2025", value: 600000 },
    { label: "Q3 2025", value: 450000 },
    { label: "Q4 2025", value: 400000 },
    { label: "Q1 2026", value: 750000 },
];

const learners = [
    { name: "Contact Name1", userId: "5892-4587", issued: 62, category: "Compliance" },
    { name: "Contact Name2", userId: "3684-6587", issued: 58, category: "Compliance" },
    { name: "Contact Name3", userId: "2687-8963", issued: 30, category: "Compliance" },
    { name: "Contact Name4", userId: "7785-5420", issued: 30, category: "Leadership" },
    { name: "Contact Name5", userId: "6532-1542", issued: 25, category: "Conduct" },
    { name: "Contact Name6", userId: "5423-5987", issued: 25, category: "Leadership" },
    { name: "Contact Name7", userId: "2453-0785", issued: 25, category: "DEI" },
];

const chartWidth = 680;
const chartHeight = 300;
const chartPadding = { top: 22, right: 16, bottom: 42, left: 72 };
const maxTrendValue = 800000;
const yAxisValues = [800000, 600000, 400000, 200000, 0];

export default function CEPerformanceReport() {
    const chartPoints = trend.map((point, index) => getTrendPoint(point.value, index)).join(" ");

    return (
        <div className="CEPerformanceReport">
            <div className="ce-report-header">
                <h2>Top Level Summary</h2>
                <h2>Categories</h2>
                <div className="ce-report-toggle" aria-label="Report metric selection">
                    <span className="active">Course Registrations</span>
                    <button type="button" aria-label="Toggle course registrations and course completions">
                        <span></span>
                    </button>
                    <span>Courses Completions</span>
                </div>
            </div>

            <div className="ce-report-top-grid">
                <section className="ce-report-panel ce-report-summary" aria-labelledby="ce-summary-title">
                    <h3 id="ce-summary-title" className="visually-hidden">Top Level Summary</h3>
                    {metrics.map(metric => (
                        <MetricRing key={metric.label} {...metric} />
                    ))}
                </section>

                <section className="ce-report-panel ce-report-categories" aria-labelledby="ce-categories-title">
                    <h3 id="ce-categories-title" className="visually-hidden">Categories</h3>
                    <div className="ce-report-specialities">
                        <h4>Specialities/Demographics</h4>
                        <div className="ce-report-pie-wrap">
                            <div className="ce-report-pie" role="img" aria-label="Category distribution chart">
                                <span className="pie-label pie-label-45">45%</span>
                                <span className="pie-label pie-label-25">25%</span>
                                <span className="pie-label pie-label-20">20%</span>
                                <span className="pie-label pie-label-10">10%</span>
                            </div>
                            <ul className="ce-report-legend">
                                {categorySegments.map(segment => (
                                    <li key={segment.label}>
                                        <span style={{ backgroundColor: segment.color }}></span>
                                        {segment.label}
                                    </li>
                                ))}
                            </ul>
                        </div>
                        <div className="ce-report-filter-row">
                            <ComboBox items={comboItems.regions} defaultValue="" />
                            <ComboBox items={comboItems.genders} defaultValue="" />
                        </div>
                    </div>

                    <div className="ce-report-ce-type">
                        <h4>CE Type</h4>
                        <div className="ce-type-bars">
                            <div className="ce-type-bar">
                                <div className="ce-type-top">30%<br />free</div>
                                <div className="ce-type-bottom">70%<br />paid</div>
                            </div>
                            <div className="ce-type-bar">
                                <div className="ce-type-top">40%<br />general</div>
                                <div className="ce-type-bottom">60%<br />leader-<br />ship</div>
                            </div>
                        </div>
                    </div>

                    <div className="ce-report-top-courses">
                        <h4>Top Courses/Programs</h4>
                        {courses.map(course => (
                            <div key={course.title} className="ce-report-course">
                                <div>
                                    <strong>{course.title}</strong>
                                    <span>{course.issuances}</span>
                                    <span>{course.completion}</span>
                                </div>
                                <button type="button" className="btn btn-sm btn-default">View</button>
                            </div>
                        ))}
                    </div>

                    <div className="ce-report-top-categories">
                        <h4>Top Categories</h4>
                        <ul>
                            {topCategories.map(category => (
                                <li key={category}>{category}</li>
                            ))}
                        </ul>
                    </div>
                </section>
            </div>

            <div className="ce-report-section-header">
                <h2>Completion Trends</h2>
                <h2>Learners/Portfolio Drill-Down</h2>
            </div>

            <div className="ce-report-bottom-grid">
                <section className="ce-report-panel ce-report-trends" aria-labelledby="ce-trends-title">
                    <h3 id="ce-trends-title" className="visually-hidden">Completion Trends</h3>
                    <div className="ce-report-chart">
                        <h4>CE Issuance Over Time</h4>
                        <svg viewBox={`0 0 ${chartWidth} ${chartHeight}`} role="img" aria-label="CE issuance over time line chart">
                            {yAxisValues.map(value => {
                                const y = getTrendY(value);
                                return (
                                    <g key={value}>
                                        <line className="ce-chart-grid-line" x1={chartPadding.left} x2={chartWidth - chartPadding.right} y1={y} y2={y} />
                                        <text className="ce-chart-axis-label" x={chartPadding.left - 12} y={y + 5} textAnchor="end">{value}</text>
                                    </g>
                                );
                            })}
                            <polyline className="ce-chart-line" points={chartPoints} />
                            {trend.map((point, index) => {
                                const [cx, cy] = getTrendPoint(point.value, index).split(",").map(Number);
                                return (
                                    <g key={`${point.label}-${index}`}>
                                        <circle className="ce-chart-point" cx={cx} cy={cy} r="6" />
                                        <text className="ce-chart-x-label" x={cx} y={chartHeight - 12} textAnchor="middle">{point.label}</text>
                                    </g>
                                );
                            })}
                        </svg>
                    </div>
                    <div className="ce-report-side-actions">
                        <div>
                            <strong>Performance<br />Audit Logs</strong>
                            <button type="button" className="btn btn-sm btn-default">View</button>
                        </div>
                        <div>
                            <strong>Additional<br />Reports</strong>
                            <button type="button" className="btn btn-sm btn-default">View</button>
                        </div>
                    </div>
                </section>

                <section className="ce-report-panel ce-report-drilldown" aria-labelledby="ce-drilldown-title">
                    <h3 id="ce-drilldown-title" className="visually-hidden">Learners Portfolio Drill-Down</h3>
                    <div className="ce-report-search">
                        <TextBox placeholder="Enter learner or course name" />
                        <Icon style="solid" name="search" />
                    </div>
                    <div className="ce-report-drilldown-filters">
                        <ComboBox items={comboItems.categories} defaultValue="" />
                        <ComboBox items={comboItems.regions} defaultValue="" />
                        <button type="button" className="btn btn-sm btn-secondary">Export Selection</button>
                    </div>
                    <table className="table table-sm ce-report-table">
                        <thead>
                            <tr>
                                <th></th>
                                <th>Learner Name</th>
                                <th>User ID</th>
                                <th>CEC Issued<br />This Year</th>
                                <th>Top Category</th>
                                <th>Certificates/<br />Transcripts</th>
                            </tr>
                        </thead>
                        <tbody>
                            {learners.map(learner => (
                                <tr key={learner.userId}>
                                    <td><Icon style="regular" name="circle" /></td>
                                    <td><strong>{learner.name}</strong></td>
                                    <td>{learner.userId}</td>
                                    <td className="text-center">{learner.issued}</td>
                                    <td>{learner.category}</td>
                                    <td className="ce-report-table-actions">
                                        <Icon style="solid" name="file-lines" title="Certificate" />
                                        <Icon style="solid" name="download" title="Download transcript" />
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </section>
            </div>
        </div>
    );
}

function MetricRing({ value, subValue, label, percent }: {
    value: string;
    subValue: string;
    label: string;
    percent: number;
}) {
    return (
        <div className="ce-report-metric">
            <div
                className="ce-report-ring"
                style={{ background: `conic-gradient(#5ab0cf ${percent * 3.6}deg, #edf0f2 0deg)` }}
            >
                <div>
                    <strong>{value}</strong>
                    {subValue && <span>{subValue}</span>}
                </div>
            </div>
            <span>{label.split("\n").map(line => <span key={line}>{line}</span>)}</span>
        </div>
    );
}

function getTrendPoint(value: number, index: number) {
    const chartLeft = chartPadding.left;
    const chartRight = chartWidth - chartPadding.right;
    const x = chartLeft + ((chartRight - chartLeft) / (trend.length - 1)) * index;

    return `${x},${getTrendY(value)}`;
}

function getTrendY(value: number) {
    const chartTop = chartPadding.top;
    const chartBottom = chartHeight - chartPadding.bottom;

    return chartBottom - (value / maxTrendValue) * (chartBottom - chartTop);
}
