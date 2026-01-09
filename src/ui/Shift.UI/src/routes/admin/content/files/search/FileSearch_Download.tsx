import { shiftClient } from "@/api/shiftClient";
import { useSearch } from "@/components/search/Search";
import SearchDownload from "@/components/search/SearchDownload";
import { FileCriteria, toApiSearchFiles } from "./FileCriteria";

export default function FileSearch_Download() {
    const { criteria } = useSearch<FileCriteria, object>();

    return (
        <SearchDownload
            columns={[
                { value: "OrganizationIdentifier", text: "Organization Identifier" },
                { value: "OrganizationCode", text: "Organization Code" },
                { value: "ObjectType", text: "Object Type" },
                { value: "ObjectIdentifier", text: "Object Identifier" },
                { value: "FileIdentifier", text: "File Identifier" },
                { value: "FileLocation", text: "File Location" },
                { value: "FileName", text: "File Name" },
                { value: "DocumentName", text: "Document Name" },
                { value: "FileSize", text: "File Size" },
                { value: "FileUploaded", text: "File Uploaded" },
                { value: "UserIdentifier", text: "User Identifier" },
                { value: "Visibility", text: "Visibility" }
            ]}
            onDownload={(format, visibleColumns) => shiftClient.file.download(toApiSearchFiles(criteria), format, visibleColumns)}
        >
        </SearchDownload>
    );
}