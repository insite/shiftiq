import { expect, test } from "vitest";
import { shiftClient } from "../shiftClient";
import { ApiError } from "../apiError";

test("/security/organizations: non-authenticated", async () => {
    await global.logout();

    await expect(shiftClient.organization.search({}, 0, 10, null)).rejects.toThrowError(new ApiError(401, ""));
    await expect(shiftClient.organization.retrieve("ed420b32-5535-4c4c-8ecc-2e995e40b046")).rejects.toThrowError(new ApiError(401, ""));
});

test("/security/organizations/search: authenticated", async () => {
    await global.login();

    const searchResult = await shiftClient.organization.search({}, 0, 10, null);

    expect(searchResult).not.toBe(null);
    expect(searchResult!.totalRowCount).toBeGreaterThan(0);
    expect(searchResult!.rowsPerPage).toBeGreaterThan(0);
    expect(searchResult!.rows.length).toBeGreaterThan(0);
    expect(searchResult!.rows[0].OrganizationIdentifier).toBeTypeOf("string");
    expect(searchResult!.rows[0].CompanyName).toBeTypeOf("string");
});

test("/security/organizations/retrieve: authenticated", async () => {
    await global.login();

    const retrieveResult = await shiftClient.organization.retrieve("0c071b03-6fe1-400f-82f4-78ff6f751ae7");

    expect(retrieveResult).not.toBe(null);
    expect(retrieveResult!.OrganizationIdentifier.toLowerCase()).toBe("0c071b03-6fe1-400f-82f4-78ff6f751ae7");
});