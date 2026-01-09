import { translate } from "@/helpers/translate";
import SearchResult from "@/components/search/SearchResult";
import { FileRow } from "./FileRow";
import DateTimeField from "@/routes/_shared/fields/DateTimeField";
import { numberHelper } from "@/helpers/numberHelper";
import { urlHelper } from "@/helpers/urlHelper";

export default function FileSearch_Result() {
    return (
        <SearchResult<FileRow> columns={[
            {
                key: "organizationCode",
                title: translate("Organization Code"),
                item: row => row.organizationCode
            },
            {
                key: "object",
                title: translate("Object"),
                item: row => (
                    <>
                        {row.objectType}
                        <div className="form-text text-body-secondary">{row.objectId}</div>
                    </>
                )
            },
            {
                key: "file",
                title: translate("File"),
                item: row => (
                    <>
                        <i className='far fa-download me-1'></i>
                        <a target="_blank" href={urlHelper.getFileUrl(row.fileId, row.fileName)}>{row.documentName}</a>
                        <span className="ms-2 badge bg-info float-end">{row.fileLocation}</span>
                        <div className="form-text text-body-secondary">
                            {row.fileName}
                            <span className="ms-2 float-end">{numberHelper.formatBytes(row.fileSize)}</span>
                        </div>
                    </>
                )
            },
            {
                key: "uploaded",
                title: translate("Uploaded"),
                item: row => (
                    <>
                        <DateTimeField dateTime={row.fileUploaded} />
                        <div className="form-text text-body-secondary">
                            {row.userName}
                        </div>
                    </>
                )
            },
            {
                key: "visibility",
                title: translate("Visibility"),
                item: row => row.visibility
            }
        ]} />
    );
}