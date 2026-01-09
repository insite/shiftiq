import { expect, test } from "vitest";
import { shiftClient } from "../shiftClient";
import { ApiError } from "../apiError";

test("/booking/events/<eventId>: non-authenticated", async () => {
    await global.logout();
    
    await expect(shiftClient.event.retrieve("ed420b32-5535-4c4c-8ecc-2e995e40b046")).rejects.toThrowError(new ApiError(401, ""));
});

test("/booking/events/<eventId>: authenticated", async () => {
    await global.login();

    const result = await shiftClient.event.retrieve("ed420b32-5535-4c4c-8ecc-2e995e40b046");

    expect(result).not.toBe(null);
    expect(result!.EventIdentifier.toLowerCase()).toBe("ed420b32-5535-4c4c-8ecc-2e995e40b046");
    expect(result!.EventTitle).toBe("Class for React Test (do not delete)");
});