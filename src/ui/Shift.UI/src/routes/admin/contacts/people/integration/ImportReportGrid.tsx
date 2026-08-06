import { shiftClient } from "@/api/shiftClient";
import ActionLink from "@/components/ActionLink";
import DateTimeField from "@/components/DateTimeField";
import Icon from "@/components/icon/Icon";
import SearchGrid, { SearchGridFunctions } from "@/components/search/SearchGrid";
import { useSiteProvider } from "@/contexts/site/SiteProviderContext";
import { dateTimeHelper } from "@/helpers/date/dateTimeHelper";
import { DateTimeParts } from "@/helpers/date/dateTimeTypes";
import { TimeZoneId } from "@/helpers/date/timeZones";
import { numberHelper } from "@/helpers/numberHelper";
import { translate } from "@/helpers/translate";
import { urlHelper } from "@/helpers/urlHelper";
import { mapQueryResult, QueryResult } from "@/models/QueryResult";
import { ForwardedRef, useImperativeHandle, useRef } from "react";

export interface ImportReportGridFunctions {
    refresh(): Promise<void>;
}

interface Row {
    fileId: string;
    fileName: string;
    documentName: string;
    fileUploaded: DateTimeParts;
    fileSize: number;
    userId: string;
    userFullName: string;
}

interface Props {
    ref?: ForwardedRef<ImportReportGridFunctions>,
    onCountUpdated: (count: number) => void;
}

export default function ImportReportGrid({ ref, onCountUpdated }: Props) {
    const { siteSetting: { TimeZoneId: timeZoneId } } = useSiteProvider();

    const gridRef = useRef<SearchGridFunctions>(null);

    useImperativeHandle(ref, () => {
        return {
            async refresh() {
                await gridRef.current?.refresh();
            }
        }
    }, []);

    async function handleLoad(pageIndex: number) {
        const queryResult = await load(pageIndex, timeZoneId);
        onCountUpdated(queryResult.totalRowCount);
        return queryResult;
    };

    return (
        <SearchGrid<Row>
            ref={gridRef}
            columns={[
                {
                    key: "file",
                    title: translate("Report"),
                    item: row => (
                        <>
                            <Icon style="regular" name="download" className="me-1" />
                            <a target="_blank" href={urlHelper.getFileUrl(row.fileId, row.fileName)}>{row.documentName}</a>
                            <span className="form-text ms-2">
                                ({numberHelper.formatBytes(row.fileSize)})
                            </span>
                        </>
                    )
                },
                {
                    key: "uploaded",
                    title: translate("Created"),
                    item: row => <DateTimeField dateTime={row.fileUploaded} />
                },
                {
                    key: "uploadedBy",
                    title: translate("Created By"),
                    item: row => (
                        <ActionLink href={`/ui/admin/contacts/people/edit?contact=${row.userId}`}>
                            {row.userFullName}
                        </ActionLink>
                    )
                },
            ]}
            onLoad={handleLoad}
        />
    );
}

async function load(pageIndex: number, timeZoneId: TimeZoneId): Promise<QueryResult<Row>> {
    const result = await shiftClient.people.searchImportReport(pageIndex);

    return mapQueryResult(result, row => ({
        fileId: row.FileId,
        fileName: row.FileName,
        documentName: row.DocumentName,
        fileUploaded: dateTimeHelper.parseServerDateTime(row.FileUploaded, timeZoneId)!,
        fileSize: row.FileSize,
        userId: row.UserId,
        userFullName: row.UserFullName,
    }));
}