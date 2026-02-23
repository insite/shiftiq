import { expect, test } from "vitest";
import { fetchHelper } from "../fetchHelper";
import { ApiSiteSetting } from "../models/ApiSiteSetting";
import { shiftClient } from "../shiftClient";
import { ApiError } from "../apiError";

test("/api/me/context for regular user1", async () => {
    await global.loginUser1();

    const result = await fetchHelper.get<ApiSiteSetting>("/api/me/context", [{ name: "refresh", value: "true" }]);

    expect(result.OrganizationCode?.toLowerCase()).toBe("e01");
    expect(result.UserName?.toLowerCase()).toBe("react");
    expect(result.IsAdministrator).toBe(false);
    expect(result.IsOperator).toBe(false);
    expect(result.IsMultiOrganization).toBe(false);
    expect(result.Permissions.Accounts).toBe(false);
    expect(result.Permissions.Integrations).toBe(false);
    expect(result.Permissions.Settings).toBe(false);
    expect(result.SupportedLanguages.length).toBeGreaterThanOrEqual(1);
    expect(result.NavigationGroups.length).toBe(0);
    expect(result.StylePath).toBeTypeOf("string");
});

test("/api/content/files/search for regular user1", async () => {
    await global.loginUser1();

    await expect(shiftClient.file.search({}, 0, null)).rejects.toThrowError(new ApiError(403, ""));
});

// test("/api/progress/gradebooks/search for regular user2", async () => {
//     await global.loginUser2();

//     const result = await shiftClient.gradebook.search({}, 0, null);

//     console.log(result);
// });