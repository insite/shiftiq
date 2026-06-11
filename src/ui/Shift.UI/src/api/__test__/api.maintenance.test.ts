import { expect, test } from "vitest";
import { shiftClient } from "../shiftClient";
import { ApiError } from "../apiError";

test("/api/platform/maintenance-lockout: non-authenticated", async () => {
    await global.logout();

    await expect(shiftClient.maintenance.maintenanceLockout()).rejects.toThrowError(new ApiError(401, ""));
});

test("/api/platform/maintenance-lockout: authenticated", async () => {
    await global.login();

    const result = await shiftClient.maintenance.maintenanceLockout();

    expect(result).not.toBe(null);
    expect(result!.IsClosed).toBeTypeOf("boolean");
});