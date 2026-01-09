import { expect, test } from "vitest";
import { shiftClient } from "../shiftClient";
import { ApiError } from "../apiError";

test("/directory/people/search: non-authenticated", async () => {
    await global.logout();

    await expect(shiftClient.people.search({}, 0, 10, [])).rejects.toThrowError(new ApiError(401, ""));
});

test("/directory/people/search: authenticated", async () => {
    await global.login();

    const searchResult = await shiftClient.people.search({
        FullName: "Aleksey Terzi"
    }, 0, 10, []);

    expect(searchResult).not.toBe(null);
    expect(searchResult!.totalRowCount).toBe(1);
    expect(searchResult!.rowsPerPage).toBeGreaterThan(0);
    expect(searchResult!.rows.length).toBe(1);
    expect(searchResult!.rows[0].UserName).toBe("Aleksey Terzi");
});