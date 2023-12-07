import { loadEnv } from "vite";
import type { UserConfig, ConfigEnv, ProxyOptions } from "vite";
import { resolve } from "path";
import vue from "@vitejs/plugin-vue";
import vueJsx from "@vitejs/plugin-vue-jsx";
import { isProd } from "./src/utils/vite";
// 解决 setup 语法糖导致不能给页面设置 name，导致 keep-alive 失效的问题。https://zhuanlan.zhihu.com/p/481640259
import VueSetupExtend from "vite-plugin-vue-setup-extend";

// 打包优化插件
import viteCompression from "vite-plugin-compression";
import legacyPlugin from "@vitejs/plugin-legacy";
// import viteImagemin from "vite-plugin-imagemin";

const pathResolve = (dir: string): any => {
    return resolve(__dirname, ".", dir);
};

/** 配置项文档：https://cn.vitejs.dev/config */
const ViteConfig = ({ mode }: ConfigEnv): UserConfig => {
    const viteEnv = loadEnv(mode, process.cwd()) as ImportMetaEnv;
    const { VITE_PORT, VITE_OPEN, VITE_BASE_PATH, VITE_OUT_DIR, VITE_AXIOS_PROXY_URL } = viteEnv;

    // 配置别名
    const alias: Record<string, string> = {
        "@": pathResolve("./src"),
        assets: pathResolve("./src/assets"),
        "vue-i18n": isProd(mode) ? "vue-i18n/dist/vue-i18n.cjs.prod.js" : "vue-i18n/dist/vue-i18n.cjs.js",
    };

    // 配置代理
    let proxy: Record<string, string | ProxyOptions> = {};
    if (VITE_AXIOS_PROXY_URL && VITE_AXIOS_PROXY_URL !== "getCurrentDomain") {
        proxy = {
            "/api": {
                target: VITE_AXIOS_PROXY_URL,
                ws: true,
                /** 是否允许跨域 */
                changeOrigin: true,
                rewrite: (path: string) => path.replace(/^\/api/, ""),
            },
        };
    }

    return {
        root: process.cwd(),
        resolve: { alias },
        /** 打包时根据实际情况修改 base */
        base: VITE_BASE_PATH,
        server: {
            /** 是否开启 HTTPS */
            // https: false,
            /** 设置 host: true 才可以使用 Network 的形式，以 IP 访问项目 */
            host: true, // host: "0.0.0.0"
            /** 端口号 */
            port: VITE_PORT,
            /** 是否自动打开浏览器 */
            open: VITE_OPEN,
            /** 跨域设置允许 */
            cors: true,
            /** 端口被占用时，是否直接退出 */
            strictPort: false,
            /** 接口代理 */
            proxy: proxy,
        },
        build: {
            /** 消除打包大小超过 500kb 警告，不建议使用 */
            chunkSizeWarningLimit: 2000,
            /** Vite 2.6.x 以上需要配置 minify: "terser", terserOptions 才能生效 */
            minify: "terser",
            // esbuild 打包更快，但是不能去除 console.log，terser打包慢，但能去除 console.log
            // minify: "terser",
            /** 在打包代码时移除 console.log、debugger 和 注释 */
            terserOptions: {
                compress: {
                    drop_console: true,
                    drop_debugger: true,
                    pure_funcs: ["console.log"],
                },
                format: {
                    /** 删除注释 */
                    comments: true,
                },
            },
            /** 关闭文件计算，禁用 gzip 压缩大小报告，可略微减少打包时间 */
            reportCompressedSize: false,
            /** 启用/禁用 CSS 代码拆分 */
            cssCodeSplit: false,
            /** 关闭生成 map 文件，可以达到缩小打包体积 */
            sourcemap: false,
            /** 打包输出路径 */
            outDir: VITE_OUT_DIR,
            /** 打包情况目录 */
            emptyOutDir: true,
            /** 打包后静态资源目录 */
            assetsDir: "static",
            /** 10kb，小于此阈值的导入或引用资源将内联为 base64 编码 */
            assetsInlineLimit: 1024 * 10,
            /** 静态资源打包处理 */
            rollupOptions: {
                output: {
                    // 在 UMD 构建模式下为这些外部化的依赖提供一个全局变量
                    globals: {},
                    chunkFileNames: "static/js/[name]-[hash].js",
                    entryFileNames: "static/js/[name]-[hash].js",
                    assetFileNames: "static/[ext]/[name]-[hash].[ext]",
                    manualChunks: {
                        // 分包配置，配置完成自动按需加载
                        vue: ["vue", "vue-router", "pinia", "vue-i18n", "element-plus"],
                        echarts: ["echarts"],
                    },
                    // manualChunks(id) {
                    //     /** 解决 vendor-xxxxxx.js 文件越来越大的问题 https://www.cnblogs.com/jyk/p/16029074.html */
                    //     if (id.includes("node_modules/")) {
                    //         const arr = id.toString().split("node_modules/")[1].split("/");
                    //         switch (arr[0]) {
                    //             case "@vue":
                    //             case "element-plus": // UI 库
                    //             case "@element-plus": // 图标
                    //             case ".pnpm": // pnpm，这里不知道为啥 pnpm 会打包进来
                    //                 return "_" + arr[0];
                    //             default:
                    //                 return "__vendor_";
                    //         }
                    //     }
                    // },
                },
            },
        },
        /** Vite 插件 */
        plugins: [
            vue(),
            // vue 可以使用 jsx/tsx 语法
            vueJsx(),
            // name 可以写在 script 标签上
            VueSetupExtend(),
            /** 兼容旧版 Chrome 和 IE浏览器 */
            legacyPlugin({
                /** 需要兼容的目标列表，可以设置多个 */
                targets: ["defaults", "ie >= 11", "chrome 52"],
                /** 面向 IE11 时需要此插件 */
                additionalLegacyPolyfills: ["regenerator-runtime/runtime"],
            }),
            /** gzip静态资源压缩，但是这个好像没用 */
            viteCompression({
                // 记录压缩文件及其压缩率。默认true
                verbose: true,
                // 是否禁用压缩，默认false
                disable: false,
                // 删除源文件
                deleteOriginFile: true,
                // 需要使用压缩前的最小文件大小，单位字节（byte） b，1b(字节)=8bit(比特), 1KB=1024B
                threshold: 1024 * 100,
                // 压缩算法 可选 'gzip' | 'brotliCompress' | 'deflate' | 'deflateRaw'
                algorithm: "gzip",
                // 压缩后的文件格式
                ext: ".gz", // 文件类型
            }),
            /** 图片压缩 */
            // viteImagemin({
            //     gifsicle: {
            //         optimizationLevel: 7,
            //         interlaced: false,
            //     },
            //     optipng: {
            //         optimizationLevel: 7,
            //     },
            //     mozjpeg: {
            //         quality: 20,
            //     },
            //     pngquant: {
            //         quality: [0.8, 0.9],
            //         speed: 4,
            //     },
            //     svgo: {
            //         plugins: [
            //             {
            //                 name: "removeViewBox",
            //             },
            //             {
            //                 name: "removeEmptyAttrs",
            //                 active: false,
            //             },
            //         ],
            //     },
            // }),
        ],
    };
};

export default ViteConfig;
