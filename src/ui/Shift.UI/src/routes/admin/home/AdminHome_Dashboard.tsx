import "./AdminHome_Dashboard.css";

import AdminHome_Dashboard_Welcome from "./AdminHome_Dashboard_Welcome";
import AdminHome_Dashboard_Notifications from "./AdminHome_Dashboard_Notifications";
import { Dashboard } from "./Dashboard";
import AdminHome_Dashboard_Tile from "./AdminHome_Dashboard_Tile";
import AdminHome_Dashboard_ReportTile from "./AdminHome_Dashboard_ReportTile";

interface Props {
    dashboard: Dashboard
}

export default function AdminHome_Dashboard({ dashboard }: Props) {
    return (
        <>
            <AdminHome_Dashboard_Welcome />

            <AdminHome_Dashboard_Notifications notifications={dashboard?.activeNotifications} />

            {dashboard?.hideMyDashboard !== true && (
                <>
                    <h3>My Dashboard</h3>

                    <div className="row g-3 mb-3 dashboard-cards">
                        <AdminHome_Dashboard_Tile
                            title="View Assessments"
                            summaryLabel="Assessments banks"
                            summaryCount={dashboard.counts.bankCount}
                            leftLabel="Active Assessments"
                            leftTotalCount={dashboard.counts.bankCount}
                            leftCount={dashboard.counts.activeBankCount}
                            rightLabel="Published Forms"
                            rightTotalCount={dashboard.counts.formCount}
                            rightCount={dashboard.counts.publishedFormCount}
                            buttonHref="/ui/admin/assessments/home"
                            buttonText="Go to Assessments"
                        />

                        <AdminHome_Dashboard_Tile
                            title="Access Contacts"
                            summaryLabel="Contacts in account"
                            summaryCount={dashboard.counts.personCount}
                            leftLabel="Active in last 3 months"
                            leftTotalCount={dashboard.counts.personCount}
                            leftCount={dashboard.counts.activePersonCount}
                            rightLabel="Users with access"
                            rightTotalCount={dashboard.counts.personCount}
                            rightCount={dashboard.counts.approvedPersonCount}
                            buttonHref="/ui/admin/contacts/home"
                            buttonText="Go to Contacts"
                        />

                        <AdminHome_Dashboard_Tile
                            title="Manage Courses"
                            summaryLabel="Courses available"
                            summaryCount={dashboard.counts.courseCount}
                            leftLabel="Published Courses"
                            leftTotalCount={dashboard.counts.courseCount}
                            leftCount={dashboard.counts.publishedCourseCount}
                            rightLabel="People Enrolled"
                            rightTotalCount={dashboard.counts.startedEnrollmentCount}
                            rightCount={dashboard.counts.completedEnrollmentCount}
                            buttonHref="/ui/admin/courses/home"
                            buttonText="Go to Courses"
                        />

                        <AdminHome_Dashboard_ReportTile />
                    </div>
                </>
            )}

            <h3>My Apps</h3>
        </>
    );
}