import { expect, test } from "vitest";
import { shiftClient } from "../shiftClient";
import { ApiError } from "../apiError";

test("/security/users: non-authenticated", async () => {
    await global.logout();

    await expect(shiftClient.user.search({}, 0, 10, null)).rejects.toThrowError(new ApiError(401, ""));
    await expect(shiftClient.user.retrieve("0c071b03-6fe1-400f-82f4-78ff6f751ae7")).rejects.toThrowError(new ApiError(401, ""));
});

test("/security/users/search: authenticated", async () => {
    await global.login();

    const searchResult = await shiftClient.user.search({
        UserFullNameContains: "Aleksey Terzi"
    }, 0, 10, null);

    expect(searchResult).not.toBe(null);
    expect(searchResult!.totalRowCount).toBeGreaterThan(0);
    expect(searchResult!.rowsPerPage).toBeGreaterThan(0);
    expect(searchResult!.rows.length).toBeGreaterThan(0);
    expect(searchResult!.rows[0].UserIdentifier).toBeTypeOf("string");
    expect(searchResult!.rows[0].FullName).toBe("Aleksey Terzi");
});

test("/security/users/retrieve: authenticated", async () => {
    await global.login();

    const retrieveResult = await shiftClient.user.retrieve("3ab3b7bc-f17d-4d38-a555-0b58db206669");

    expect(retrieveResult).not.toBe(null);
    expect(retrieveResult!.UserIdentifier.toLowerCase()).toBe("3ab3b7bc-f17d-4d38-a555-0b58db206669");
    expect(retrieveResult!.FullName).toBe("Aleksey Terzi");
});