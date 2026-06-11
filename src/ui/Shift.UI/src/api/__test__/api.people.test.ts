import { expect, test } from "vitest";
import { shiftClient } from "../shiftClient";
import { ApiError } from "../apiError";

test("/api/contacts/people/search: non-authenticated", async () => {
    await global.logout();

    await expect(shiftClient.people.search({}, 0, 10, [])).rejects.toThrowError(new ApiError(401, ""));
});

test("/api/contacts/people/search: authenticated", async () => {
    await global.login();

    const searchResult = await shiftClient.people.search({
        FullName: "Aleksey Terzi"
    }, 0, 10, []);

    expect(searchResult).not.toBe(null);
    expect(searchResult!.totalRowCount).toBeGreaterThan(0);
    expect(searchResult!.rowsPerPage).toBeGreaterThan(0);
    expect(searchResult!.rows.length).toBeGreaterThan(0);
    expect(searchResult!.rows[0].UserName).toBe("Aleksey Terzi");
});
