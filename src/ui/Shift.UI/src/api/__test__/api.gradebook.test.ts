import { expect, test } from "vitest";
import { shiftClient } from "../shiftClient";
import { ApiError } from "../apiError";

test("/progress/gradebooks: non-authenticated", async () => {
    await global.logout();

    await expect(shiftClient.gradebook.search({}, 0, null)).rejects.toThrowError(new ApiError(401, ""));
    await expect(shiftClient.gradebook.download({}, "csv", [])).rejects.toThrowError(new ApiError(401, ""));
    await expect(shiftClient.gradebook.retrieve("ed420b32-5535-4c4c-8ecc-2e995e40b046")).rejects.toThrowError(new ApiError(401, ""));
});

test("/progress/gradebooks/search: authenticated", async () => {
    await global.login();

    const searchResult = await shiftClient.gradebook.search({
        GradebookTitle: "Test Gradebook",
    }, 0, null);

    expect(searchResult).not.toBe(null);
    expect(searchResult!.totalRowCount).toBeGreaterThan(0);
    expect(searchResult!.rowsPerPage).toBeGreaterThan(0);
    expect(searchResult!.rows.length).toBeGreaterThan(0);
    expect(searchResult!.rows[0].GradebookIdentifier).toBeTypeOf("string");
    expect(searchResult!.rows[0].GradebookTitle).toBe("Test Gradebook");
    expect(searchResult!.rows[0].GradebookCreated).toBeTypeOf("string");
    expect(searchResult!.rows[0].GradebookEnrollmentCount).toBeTypeOf("number");
    expect(searchResult!.rows[0].AchievementCountGranted).toBeTypeOf("number");
    expect(searchResult!.rows[0].IsLocked).toBeTypeOf("boolean");
});

test("/progress/gradebooks/download: authenticated", async () => {
    await global.login();

    const searchResult = await shiftClient.gradebook.download({}, "csv", []);

    expect(searchResult).not.toBe(null);
    expect(searchResult!.filename.toLowerCase()).toBeTypeOf("string");
    expect(searchResult!.data.type).toBe("text/csv");
});

test("/progress/gradebooks/retrieve: authenticated", async () => {
    await global.login();

    const retrieveResult = await shiftClient.gradebook.retrieve("c18689ca-3578-4011-aca2-bfe2d7426da1");

    expect(retrieveResult).not.toBe(null);
    expect(retrieveResult!.GradebookIdentifier.toLowerCase()).toBe("c18689ca-3578-4011-aca2-bfe2d7426da1");
    expect(retrieveResult!.IsLocked).toBe(false);
    expect(retrieveResult!.GradebookTitle).toBe("Test Gradebook");
    expect(retrieveResult!.GradebookType).toBe("Scores");
    expect(retrieveResult!.LastChangeType).toBeTypeOf("string");
    expect(retrieveResult!.LastChangeUser).toBeTypeOf("string");
    expect(retrieveResult!.GradebookCreated).toBeTypeOf("string");
    expect(retrieveResult!.LastChangeTime).toBeTypeOf("string");
});