import { shiftClient } from "@/api/shiftClient";
import { useSearch } from "@/components/search/Search";
import SearchDownload from "@/components/search/SearchDownload";
import { FileCriteria, toApiSearchFiles } from "./FileCriteria";

export default function FileSearch_Download() {
    const { criteria } = useSearch<FileCriteria, object>();

    return (
        <SearchDownload
            columns={[
                { value: "OrganizationId", text: "Organization Id" },
                { value: "OrganizationCode", text: "Organization Code" },
                { value: "ObjectType", text: "Object Type" },
                { value: "ObjectId", text: "Object Id" },
                { value: "FileId", text: "File Id" },
                { value: "FileLocation", text: "File Location" },
                { value: "FileName", text: "File Name" },
                { value: "DocumentName", text: "Document Name" },
                { value: "FileSize", text: "File Size" },
                { value: "FileUploaded", text: "File Uploaded" },
                { value: "UserId", text: "User Id" },
                { value: "Visibility", text: "Visibility" }
            ]}
            onDownload={(format, visibleColumns) => shiftClient.file.download(toApiSearchFiles(criteria), format, visibleColumns)}
        >
        </SearchDownload>
    );
}