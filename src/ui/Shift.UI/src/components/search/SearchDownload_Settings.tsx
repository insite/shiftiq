import { translate } from "@/helpers/translate";
import { ChangeFileFormatHandler, ChangeHiddenColumnsHandler, ChangeRemoveSpacesHandler } from "./useSearchDownload";
import { useSiteProvider } from "@/contexts/SiteProvider";
import { numberHelper } from "@/helpers/numberHelper";

interface Props {
    fileFormat: "csv" | "json";
    showHiddenColumns: boolean;
    removeSpaces: boolean;
    handleChangeFileFormat: ChangeFileFormatHandler;
    handleChangeHiddenColumns: ChangeHiddenColumnsHandler;
    handleChangeRemoveSpaces: ChangeRemoveSpacesHandler;
}

export default function SearchDownload_Settings({
    fileFormat,
    showHiddenColumns,
    removeSpaces,
    handleChangeFileFormat,
    handleChangeHiddenColumns,
    handleChangeRemoveSpaces
}: Props) {
    const { siteSetting: {
        PlatformSearchDownloadMaximumRows: platformSearchDownloadMaximumRows,
        PartitionEmail: partitionEmail,
    } } = useSiteProvider();

    return (
        <>
            <div className="form-group mb-3">
                <label className="form-label">
                    {translate("File Format")}
                </label>
                <div>
                    <label>
                        <input
                            type="radio"
                            name="outputFileFormat"
                            value="csv"
                            checked={fileFormat === "csv"}
                            onChange={() => handleChangeFileFormat("csv")}
                        /> Data Export (.csv)
                    </label>
                    <br/>
                    <label>
                        <input
                            type="radio"
                            name="outputFileFormat"
                            value="json"
                            checked={fileFormat === "json"}
                            onChange={() => handleChangeFileFormat("json")}
                        /> Data Export (.json)
                    </label>
                </div>
            </div>

            <div className="form-group mb-3">
                <label className="form-label">
                    {translate("Hidden Columns")}
                </label>
                <div>
                    <label>
                        <input
                            type="radio"
                            name="hiddenColumns"
                            value="show"
                            checked={showHiddenColumns}
                            onChange={() => handleChangeHiddenColumns(true)}
                        /> Show
                    </label>
                    <label>
                        <input
                            type="radio"
                            name="hiddenColumns"
                            value="hide"
                            checked={!showHiddenColumns}
                            onChange={() => handleChangeHiddenColumns(false)}
                        /> Hide
                    </label>
                </div>
            </div>

            <div className="form-group mb-3">
                <label className="form-label">
                    {translate("Column Headings")}
                </label>
                <div>
                    <label>
                        <input
                            type="checkbox"
                            checked={removeSpaces}
                            onChange={e => handleChangeRemoveSpaces(e.target.checked)}
                        /> Remove Spaces
                    </label>
                </div>
            </div>

            <div className="alert alert-info fs-xs mt-3">
                <strong>Please Note:</strong>
                {" "}
                You can download a maximum of {numberHelper.formatInt(platformSearchDownloadMaximumRows)} rows. 
                If you need to download more than this, please request an export from
                {" "}
                <a target="_blank" href={`mailto:${partitionEmail}`}>
                    {partitionEmail}
                </a>
                .
            </div>
        </>
    );
}