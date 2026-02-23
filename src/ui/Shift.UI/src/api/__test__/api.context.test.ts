import { expect, test } from "vitest";
import { fetchHelper } from "../fetchHelper";
import { ApiSiteSetting } from "../models/ApiSiteSetting";

test("/api/me/context: non-authenticated", async () => {
    await global.logout();

    const result = await fetchHelper.get<ApiSiteSetting>("/api/me/context", [{ name: "refresh", value: "false" }]);

    expect(result.Home.Text.length).toBeGreaterThanOrEqual(1);
    expect(result.Home.Icon.length).toBeGreaterThanOrEqual(1);
    expect(result.Home.Url.length).toBeGreaterThanOrEqual(1);
    expect(result.TimeZoneId.toLowerCase()).toBe("utc");
    expect(result.UserName ?? null).toBe(null);
    expect(result.IsAdministrator).toBe(false);
    expect(result.IsOperator).toBe(false);
    expect(result.IsMultiOrganization).toBe(false);
    expect(result.Permissions.Accounts).toBe(false);
    expect(result.Permissions.Integrations).toBe(false);
    expect(result.Permissions.Settings).toBe(false);
    expect(result.CurrentLanguage).toBeTypeOf("string");
    expect(result.CurrentLanguage.length).toBe(2);
    expect(result.CurrentLanguage).toBe(result.CurrentLanguage.toLowerCase());
    expect(result.SupportedLanguages.length).toBeGreaterThanOrEqual(1);
    expect(result.StylePath).toBeTypeOf("string");
    expect(result.StylePath.substring(result.StylePath.lastIndexOf("/"))).toBe(result.StylePath.substring(result.StylePath.lastIndexOf("/")).toLowerCase());
});

test("/api/me/context: authenticated operator", async () => {
    await global.login();

    const result = await fetchHelper.get<ApiSiteSetting>("/api/me/context", [{ name: "refresh", value: "true" }]);

    expect(result.Home.Text.length).toBeGreaterThanOrEqual(1);
    expect(result.Home.Icon.length).toBeGreaterThanOrEqual(1);
    expect(result.Home.Url.length).toBeGreaterThanOrEqual(1);
    expect(result.OrganizationCode?.toLowerCase()).toBe("e01");
    expect(result.UserName?.toLowerCase()).toBe("aleksey");
    expect(result.IsAdministrator).toBe(true);
    expect(result.IsOperator).toBe(true);
    expect(result.IsMultiOrganization).toBe(true);
    expect(result.Permissions.Accounts).toBe(true);
    expect(result.Permissions.Integrations).toBe(true);
    expect(result.Permissions.Settings).toBe(true);
    expect(result.CurrentLanguage).toBeTypeOf("string");
    expect(result.CurrentLanguage.length).toBe(2);
    expect(result.CurrentLanguage).toBe(result.CurrentLanguage.toLowerCase());
    expect(result.SupportedLanguages.length).toBeGreaterThanOrEqual(1);
    expect(result.NavigationGroups.length).toBe(3);
    expect(result.NavigationGroups[0].MenuItems.length).toBeGreaterThanOrEqual(1);
    expect(result.NavigationGroups[1].MenuItems.length).toBeGreaterThanOrEqual(1);
    expect(result.NavigationGroups[2].MenuItems.length).toBeGreaterThanOrEqual(1);
    expect(result.AdminNavigationGroups.length).toBeGreaterThanOrEqual(1);
    expect(result.StylePath).toBeTypeOf("string");
    expect(result.StylePath.substring(result.StylePath.lastIndexOf("/"))).toBe(result.StylePath.substring(result.StylePath.lastIndexOf("/")).toLowerCase());
});