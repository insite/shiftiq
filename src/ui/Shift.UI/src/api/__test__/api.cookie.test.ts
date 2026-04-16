import { expect, test } from "vitest";
import { shiftClient } from "../shiftClient";
import { timerHelper } from "@/helpers/timerHelper";

test("POST /api/security/cookies/refresh-session: non-authenticated", async () => {
    await global.logout();

    await shiftClient.cookie.refreshSession();
});

test("POST /api/security/cookies/refresh-session: authenticated", async () => {
    await global.login();

    const afterLoginStartTime = timerHelper.getStartTimeInMs();

    expect(afterLoginStartTime).not.toBe(null);

    await new Promise(resolve => setTimeout(resolve, 1050));

    expect(timerHelper.getStartTimeInMs()).toBe(afterLoginStartTime!);

    await shiftClient.cookie.refreshSession();

    expect(timerHelper.getStartTimeInMs()).toBeGreaterThan(afterLoginStartTime!);
});
