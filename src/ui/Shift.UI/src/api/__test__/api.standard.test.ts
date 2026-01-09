import { expect, test } from "vitest";
import { shiftClient } from "../shiftClient";
import { ApiError } from "../apiError";

test("/competency/standards: non-authenticated", async () => {
    await global.logout();

    await expect(shiftClient.standard.search({}, 0, 10)).rejects.toThrowError(new ApiError(401, ""));
    await expect(shiftClient.standard.retrieve("0c071b03-6fe1-400f-82f4-78ff6f751ae7")).rejects.toThrowError(new ApiError(401, ""));
});

test("/competency/standards/search: authenticated", async () => {
    await global.login();

    const searchResult = await shiftClient.standard.search({
        ContentTitle: "Standard for React Test (do not delete)",
        StandardType: "Area"
    }, 0, 10);

    expect(searchResult).not.toBe(null);
    expect(searchResult!.totalRowCount).toBe(1);
    expect(searchResult!.rowsPerPage).toBeGreaterThan(0);
    expect(searchResult!.rows.length).toBe(1);
    expect(searchResult!.rows[0].Id.toLowerCase()).toBe("995e94c2-21e9-40fb-9c5b-a6a99e7cbc20");
    expect(searchResult!.rows[0].Title).toBe("Standard for React Test (do not delete)");
    expect(searchResult!.rows[0].Type).toBe("Area");
});

test("/competency/standards/retrieve: authenticated", async () => {
    await global.login();

    const retrieveResult = await shiftClient.standard.retrieve("995e94c2-21e9-40fb-9c5b-a6a99e7cbc20");

    expect(retrieveResult).not.toBe(null);
    expect(retrieveResult!.StandardId.toLowerCase()).toBe("995e94c2-21e9-40fb-9c5b-a6a99e7cbc20");
    expect(retrieveResult!.ContentTitle).toBe("Standard for React Test (do not delete)");
});