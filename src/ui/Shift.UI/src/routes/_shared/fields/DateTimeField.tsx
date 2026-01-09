import { dateTimeHelper } from "@/helpers/date/dateTimeHelper";
import { DateTimeParts, defaultFormatType } from "@/helpers/date/dateTimeTypes";

interface Props {
    dateTime: DateTimeParts | null | undefined;
}

export default function DateTimeField({ dateTime }: Props) {
    if (!dateTime) {
        return null;
    }
    return (
        <>
            {dateTimeHelper.formatDate(dateTime.date, defaultFormatType)}
            {" "}
            <span className="form-text text-body-secondary">
                - {dateTimeHelper.formatTime(dateTime)}
            </span>
        </>
    )
}