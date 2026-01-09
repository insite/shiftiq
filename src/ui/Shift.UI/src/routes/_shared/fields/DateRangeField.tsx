import { dateTimeHelper } from "@/helpers/date/dateTimeHelper";
import { DateTimeParts, defaultFormatType } from "@/helpers/date/dateTimeTypes";

interface Props {
    date1: DateTimeParts | null | undefined;
    date2: DateTimeParts | null | undefined;
}

export default function DateRangeField({ date1, date2 }: Props) {
    if (!date1 && !date2) {
        return null;
    }
    if (date1 && !date2) {
        return dateTimeHelper.formatDate(date1.date, defaultFormatType);
    }
    if (!date1 && date2) {
        return dateTimeHelper.formatDate(date2.date, defaultFormatType);
    }

    return (
        <>
            {dateTimeHelper.formatDate(date1!.date, defaultFormatType)} - {dateTimeHelper.formatDate(date2!.date, defaultFormatType)}
        </>
    )
}