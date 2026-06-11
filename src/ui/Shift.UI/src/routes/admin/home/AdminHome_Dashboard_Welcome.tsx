import { useSiteProvider } from "@/contexts/site/SiteProviderContext";
import { dateTimeHelper } from "@/helpers/date/dateTimeHelper";
import { TimeParts } from "@/helpers/date/dateTimeTypes";

export default function AdminHome_Dashboard_Welcome() {
    const { siteSetting: { TimeZoneId: timeZoneId, UserName: userName } } = useSiteProvider();
    const now = dateTimeHelper.now(timeZoneId);

    return (
        <>
            <div className="mb-1" style={{ fontSize: "smaller" }}>
                {dateTimeHelper.formatDate(now.date, "dddd, mmmm d, yyyy")}
            </div>

            <h1>
                {`${getGreeting(now.time)}, ${userName}`}
            </h1>
        </>
    )
}

function getGreeting(time: TimeParts): string {
    if (time.hour! > 3 && time.hour! < 12) {
        return "Good Morning";
    }

    if (time.hour! < 18) {
        return "Good Afternoon";
    }

    if (time.hour! < 23) {
        return "Good Evening";
    }

    return "Hello";
}