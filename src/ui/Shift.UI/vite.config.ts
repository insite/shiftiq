import { defineConfig, loadEnv, normalizePath } from "vite"
import { viteStaticCopy } from "vite-plugin-static-copy"
import react from "@vitejs/plugin-react"
import path from "path";

// https://vitejs.dev/config/
export default defineConfig(({ mode }) => {
    const env = loadEnv(mode, process.cwd(), "");
    const isLocal = !!env.VITE_IS_LOCAL && env.VITE_IS_LOCAL.toLowerCase() === "true";
    return {
        plugins: [
            react(),
            viteStaticCopy({
                targets: [
                    {
                        src: normalizePath(path.resolve(process.cwd(), "setupApiUrl.js")),
                        dest: "./"
                    },
                ],
            }),
        ],
        resolve: {
            preserveSymlinks: true,
            alias: {
                "@": path.resolve(process.cwd(), "./src"),
            },
        },
        server: {
            port: 3000,
            strictPort: true
        },
        build: {
            outDir: "../../../source/InSite.UI/React",
            chunkSizeWarningLimit: 5000,
        },
        base: isLocal ? "" : "/react",
    };
})
