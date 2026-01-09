import { expect, test } from "vitest";
import { shiftClient } from "../shiftClient";
import { ApiError } from "../apiError";

test("/progress/periods: non-authenticated", async () => {
    await global.logout();

    await expect(shiftClient.period.search({}, 0, 10)).rejects.toThrowError(new ApiError(401, ""));
    await expect(shiftClient.period.retrieve("0c071b03-6fe1-400f-82f4-78ff6f751ae7")).rejects.toThrowError(new ApiError(401, ""));
});

test("/progress/periods/search: authenticated", async () => {
    await global.login();

    const searchResult = await shiftClient.period.search({}, 0, 10);

    expect(searchResult).not.toBe(null);
    expect(searchResult!.totalRowCount).toBeGreaterThan(0);
    expect(searchResult!.rowsPerPage).toBeGreaterThan(0);
    expect(searchResult!.rows.length).toBeGreaterThan(0);
    expect(searchResult!.rows[0].Id).toBeTypeOf("string");
    expect(searchResult!.rows[0].Name).toBeTypeOf("string");
});

test("/progress/periods/retrieve: authenticated", async () => {
    await global.login();

    const retrieveResult = await shiftClient.period.retrieve("3d0827c1-d933-4482-ac8a-ac42a104ca7b");

    expect(retrieveResult).not.toBe(null);
    expect(retrieveResult!.PeriodId.toLowerCase()).toBe("3d0827c1-d933-4482-ac8a-ac42a104ca7b");
    expect(retrieveResult!.PeriodName).toBe("Period for React Test (do not delete)");
    expect(retrieveResult!.PeriodStart).toBeTypeOf("string");
    expect(retrieveResult!.PeriodEnd).toBeTypeOf("string");
});