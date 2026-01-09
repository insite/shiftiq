import { expect, test } from "vitest";
import { shiftClient } from "../shiftClient";
import { ApiError } from "../apiError";

test("/workflow/case-statuses/*: non-authenticated", async () => {
    await global.logout();

    await expect(shiftClient.caseStatus.search({}, 0, null)).rejects.toThrowError(new ApiError(401, ""));
    await expect(shiftClient.caseStatus.download({}, "csv", [])).rejects.toThrowError(new ApiError(401, ""));
    await expect(shiftClient.caseStatus.retrieve("f3ad8690-76e0-4ede-afb0-a0c2a21ed947")).rejects.toThrowError(new ApiError(401, ""));
    await expect(shiftClient.caseStatus.create({
        CaseType: "",
        StatusName: "",
        StatusSequence: 0,
        StatusCategory: "",
        ReportCategory: null,
        StatusDescription: null,
    })).rejects.toThrowError(new ApiError(401, ""));
    await expect(shiftClient.caseStatus.update("f3ad8690-76e0-4ede-afb0-a0c2a21ed947", {
        StatusName: "",
        StatusSequence: 0,
        StatusCategory: "",
        ReportCategory: null,
        StatusDescription: null,
    })).rejects.toThrowError(new ApiError(401, ""));
    await expect(shiftClient.caseStatus.delete("f3ad8690-76e0-4ede-afb0-a0c2a21ed947")).rejects.toThrowError(new ApiError(401, ""));
});

test("/workflow/case-statuses/search, create, delete: authenticated", async () => {
    await global.login();

    const createResult = await shiftClient.caseStatus.create({
        CaseType: "PLA",
        StatusName: "Appeal",
        StatusSequence: 11,
        StatusCategory: "Open",
        ReportCategory: "",
        StatusDescription: "",
    });

    const searchResult = await shiftClient.caseStatus.search({
        CaseTypeExact: "PLA"
    }, 0, null);

    await shiftClient.caseStatus.delete(createResult.StatusIdentifier);

    expect(createResult.CaseType).toBe("PLA");
    expect(createResult.StatusName).toBe("Appeal");
    expect(createResult.StatusSequence).toBe(11);
    expect(createResult.StatusCategory).toBe("Open");

    expect(searchResult).not.toBe(null);
    expect(searchResult!.totalRowCount).toBeGreaterThan(0);
    expect(searchResult!.rowsPerPage).toBeGreaterThan(0);
    expect(searchResult!.rows.length).toBeGreaterThan(0);
    expect(searchResult!.rows[0].CaseType).toBe("PLA");
});