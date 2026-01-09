import { expect, test } from "vitest";
import { shiftClient } from "../shiftClient";
import { ApiError } from "../apiError";

test("/progress/achievements/*: non-authenticated", async () => {
    await global.logout();

    await expect(shiftClient.achievement.search({}, 0, 0)).rejects.toThrowError(new ApiError(401, ""));
    await expect(shiftClient.achievement.retrieve("f3ad8690-76e0-4ede-afb0-a0c2a21ed947")).rejects.toThrowError(new ApiError(401, ""));
});

test("/progress/achievements/search: authenticated", async () => {
    await global.login();

    const result = await shiftClient.achievement.search({}, 0, 10);

    expect(result).not.toBe(null);
    expect(result!.pageIndex).toBe(0);
    expect(result!.totalRowCount).toBeGreaterThan(0);
    expect(result!.rowsPerPage).toBe(10);
    expect(result!.rows.length).toBeGreaterThan(0);
    expect(result!.rows[0].AchievementId).toBeTypeOf("string");
    expect(result!.rows[0].AchievementTitle).toBeTypeOf("string");
});

test("/progress/achievements/<achievementId>: authenticated", async () => {
    await global.login();

    const searchResult = await shiftClient.achievement.search({}, 0, 10);
    const achievementId = searchResult!.rows[0].AchievementId;

    const result = await shiftClient.achievement.retrieve(achievementId);

    expect(result!.AchievementIdentifier.toLowerCase()).toBe(achievementId.toLowerCase());
    expect(result!.AchievementTitle).toBeTypeOf("string");
});