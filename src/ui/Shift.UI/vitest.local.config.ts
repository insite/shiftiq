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
                url: "http://localhost:3000",
                settings: {
                    fetch: {
                        disableStrictSSL: true
                    }
                }
            },
        },
        setupFiles: ["./src/test/setup.local.ts"]
    },
})