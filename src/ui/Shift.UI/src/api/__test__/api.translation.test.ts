import { expect, test } from "vitest";
import { shiftClient } from "../shiftClient";
import { ApiError } from "../apiError";

test("/content/translations/translate: non-authenticated", async () => {
    await global.logout();

    await expect(shiftClient.translation.translate([])).rejects.toThrowError(new ApiError(401, ""));
});

test("/content/translations/translate: authenticated", async () => {
    await global.login();

    const searchResult = await shiftClient.translation.translate(["French"]);

    expect(searchResult).not.toBe(null);
    expect(searchResult![0].en).toBe("French");
    expect(searchResult![0].fr).toBe("Français");
});