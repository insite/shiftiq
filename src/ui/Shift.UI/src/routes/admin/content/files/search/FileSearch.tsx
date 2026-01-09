import FileSearch_Result from "./FileSearch_Result";
import FileSearch_Criteria from "./FileSearch_Criteria";
import Search from "@/components/search/Search";
import { FileCriteria, toApiSearchFiles } from "./FileCriteria";
import { FileRow } from "./FileRow";
import { dateTimeHelper } from "@/helpers/date/dateTimeHelper";
import { useSiteProvider } from "@/contexts/SiteProvider";
import { TimeZoneId } from "@/helpers/date/timeZones";
import FileSearch_Download from "./FileSearch_Download";
import { shiftClient } from "@/api/shiftClient";
import { mapQueryResult } from "@/models/QueryResult";

export default function FileSearch() {
    const { siteSetting } = useSiteProvider();

    return (
        <Search<FileCriteria, FileRow>
            cacheKey="search.file"
            defaultCriteria={{
                organizationIdentifier: "",
                objectType: "",
                objectIdentifier: "",
                fileName: "",
                documentName: "",
                fileUploadedSince: null,
                fileUploadedBefore: null,
                fileUploadedBy: "",
                visibility: "",
                visibleColumns: [],
                sortByColumn: ""
            }}
            resultElement={<FileSearch_Result />}
            criteriaElement={<FileSearch_Criteria />}
            downloadElement={<FileSearch_Download />}
            onLoad={(pageIndex, criteria) => load(pageIndex, criteria, siteSetting.TimeZoneId)}
        />
    );
}

async function load(pageIndex: number, criteria: FileCriteria, timeZoneId: TimeZoneId) {
    const filter = toApiSearchFiles(criteria);
    const result = await shiftClient.file.search(filter, pageIndex, criteria.sortByColumn);

    return mapQueryResult(result, row => ({
        organizationId: row.OrganizationIdentifier,
        organizationCode: row.OrganizationCode,
        objectType: row.ObjectType,
        objectId: row.ObjectIdentifier,
        fileId: row.FileIdentifier,
        fileLocation: row.FileLocation,
        fileName: row.FileName,
        documentName: row.DocumentName,
        fileSize: row.FileSize,
        fileUploaded: dateTimeHelper.parseServerDateTime(row.FileUploaded, timeZoneId)!,
        userId: row.UserIdentifier,
        userName: row.UserFullName || "Someone Someone",
        visibility: row.HasClaims ? "Private" : "Public",
    }));
}