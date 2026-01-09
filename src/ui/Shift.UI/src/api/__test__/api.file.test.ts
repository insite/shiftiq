import { expect, test } from "vitest";
import { shiftClient } from "../shiftClient";
import { ApiError } from "../apiError";
import { fetchHelper } from "../fetchHelper";
import { ApiUploadFileInfo } from "../controllers/file/ApiUploadFileInfo";

const fileText = "My test text";
const fileName = "test-text.txt";

test("/content/files: non-authenticated", async () => {
    await global.logout();

    await expect(shiftClient.file.count({})).rejects.toThrowError(new ApiError(401, ""));
    await expect(shiftClient.file.search({}, 0, null)).rejects.toThrowError(new ApiError(401, ""));
    await expect(shiftClient.file.download({}, "csv", [])).rejects.toThrowError(new ApiError(401, ""));
    await expect(shiftClient.file.retrieve("ed420b32-5535-4c4c-8ecc-2e995e40b046")).rejects.toThrowError(new ApiError(401, ""));
    await expect(shiftClient.file.uploadTempFile(createFile())).rejects.toThrowError(new ApiError(401, ""));
});

test("/content/files: authenticated", async () => {
    await global.login();

    // Happy Dom, XMLHttpRequest, and cookies seems are not friends, so am using workaround
    const file = createFile();
    const formData = new FormData();
    formData.append(file.name, file);

    const uploadResult = (await fetchHelper.postForm("/content/files/temp", formData)) as ApiUploadFileInfo[];

    expect(uploadResult).not.toBe(null);
    expect(uploadResult!.length).toBe(1);
    expect(uploadResult![0].FileName).toBe(fileName);

    const countResult = await shiftClient.file.count({
        ObjectTypeExact: "Temporary"
    });

    expect(countResult).toBeGreaterThan(0);

    const searchResult = await shiftClient.file.search({}, 0, null);

    expect(searchResult).not.toBe(null);
    expect(searchResult!.totalRowCount).toBeGreaterThan(0);
    expect(searchResult!.rowsPerPage).toBeGreaterThan(0);
    expect(searchResult!.rows.length).toBeGreaterThan(0);
    expect(searchResult!.rows[0].ObjectType).toBe("Temporary");

    const downloadResult = await shiftClient.file.download({
        ObjectTypeExact: "Temporary"
    }, "csv", []);

    expect(downloadResult).not.toBe(null);
    expect(downloadResult!.filename.toLowerCase()).toBeTypeOf("string");
    expect(downloadResult!.data.type).toBe("text/csv");

    const retrieveResult = await shiftClient.file.retrieve(uploadResult![0].FileIdentifier);

    expect(retrieveResult).not.toBe(null);
    expect(retrieveResult!.ObjectType).toBe("Temporary");
});

function createFile(): File {
    return new File([fileText], fileName, { type: "text/plain" });
}