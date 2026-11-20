import { expect, test } from "vitest";
import { shiftClient } from "../shiftClient";

// test("/api/assets/search Refresh", async () => {
//     await global.login();

//     await shiftClient.search.refresh();
// });

test("/api/assets/search Search", async () => {
    await global.login();

    const result = await shiftClient.search.search("ste co", 0);

    expect(result).toBeTypeOf("object");
});
