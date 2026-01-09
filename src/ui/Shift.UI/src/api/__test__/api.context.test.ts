import { expect, test } from "vitest";
import { fetchHelper } from "../fetchHelper";
import { ApiSiteSetting } from "../models/ApiSiteSetting";

test("/me/context: non-authenticated", async () => {
    await global.logout();

    const result = await fetchHelper.get<ApiSiteSetting>("/me/context", [{ name: "refresh", value: "false" }]);

    expect(result.TimeZoneId.toLowerCase()).toBe("utc");
    expect(result.UserName ?? null).toBe(null);
    expect(result.IsAdministrator).toBe(false);
    expect(result.IsOperator).toBe(false);
    expect(result.Permissions.Accounts).toBe(false);
    expect(result.Permissions.Integrations).toBe(false);
    expect(result.Permissions.Settings).toBe(false);
    expect(result.SupportedLanguages.length).toBeGreaterThanOrEqual(1);
});

test("/me/context: authenticated operator", async () => {
    await global.login();

    const result = await fetchHelper.get<ApiSiteSetting>("/me/context", [{ name: "refresh", value: "true" }]);

    expect(result.OrganizationCode?.toLowerCase()).toBe("e01");
    expect(result.UserName?.toLowerCase()).toBe("aleksey terzi");
    expect(result.IsAdministrator).toBe(true);
    expect(result.IsOperator).toBe(true);
    expect(result.Permissions.Accounts).toBe(true);
    expect(result.Permissions.Integrations).toBe(true);
    expect(result.Permissions.Settings).toBe(true);
    expect(result.SupportedLanguages.length).toBeGreaterThanOrEqual(1);
    expect(result.NavigationGroups.length).toBe(3);
    expect(result.NavigationGroups[0].MenuItems.length).toBeGreaterThanOrEqual(1);
    expect(result.NavigationGroups[1].MenuItems.length).toBeGreaterThanOrEqual(1);
    expect(result.NavigationGroups[2].MenuItems.length).toBeGreaterThanOrEqual(1);
});