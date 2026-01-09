import { defineConfig } from "vitest/config";
import path from "path";

export default defineConfig({
    test: {
        alias: {
            "@": path.resolve(process.cwd(), "./src"),
        },
        environment: "happy-dom",
        environmentOptions: {
            happyDOM: {
                url: "https://local-api.shiftiq.com",
                settings: {
                    fetch: {
                        disableStrictSSL: true
                    }
                }
            },
        },
        setupFiles: ["./src/test/setup.rt.ts"]
    },
})