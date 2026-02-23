import { expect, test } from "vitest";
import { shiftClient } from "../shiftClient";
import { ApiError } from "../apiError";

test("/api/content/translations/translate: non-authenticated", async () => {
    await global.logout();

    await expect(shiftClient.translation.translate([])).rejects.toThrowError(new ApiError(401, ""));
});