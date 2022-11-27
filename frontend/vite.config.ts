import { resolve } from "path";
import { defineConfig, loadEnv } from "vite";
import vue from "@vitejs/plugin-vue";
import Components from "unplugin-vue-components/vite";
import VueJSX from "@vitejs/plugin-vue-jsx";
import AutoImport from "unplugin-auto-import/vite";
import vueSetupExtend from "vite-plugin-vue-setup-extend";
import { visualizer } from "rollup-plugin-visualizer";
import { antdDayjs } from "antd-dayjs-vite-plugin";
import Less2CssVariablePlugin from "antd-less-to-css-variable";

export const r = (...args) => resolve(__dirname, ".", ...args);

const removeModulePreloadPlugin = (keys) => {
	if (!keys || !keys.length) {
		return;
	}
	return {
		name: "remove-module-preload",
		transformIndexHtml: {
			enforce: "after",
			transform(html, ctx) {
				let result = html;
				keys.forEach((key) => {
					result = result.replace(
						new RegExp(`<link rel="modulepreload"?.*${key}?.*`),
						""
					);
				});
				return result;
			},
		},
	};
};

// https://vitejs.dev/config/
export default ({ mode }) => {
	// 加载env 配置文件
	const envConfig = loadEnv(mode, process.cwd(), "");

	// 启动端口
	const viteProt: number = parseInt(envConfig.VITE_PORT);

	const alias = {
		"~": `${resolve(__dirname, "./")}`,
		"@": `${resolve(__dirname, "src")}`,
		"@/": `${resolve(__dirname, "src")}/`,
	};

	return defineConfig({
		server: {
			host: "0.0.0.0",
			port: viteProt,
			proxy: {
				"/api": {
					target: envConfig.VITE_API_BASEURL,
					ws: false,
					changeOrigin: true,
					rewrite: (path) => path.replace(/^\/api/, ""),
				},
			},
		},
		resolve: {
			alias,
		},
		// 解决警告You are running the esm-bundler build of vue-i18n.
		define: {
			"process.env": envConfig,
			__VUE_I18N_FULL_INSTALL__: true,
			__VUE_I18N_LEGACY_API__: true,
			__VUE_I18N_PROD_DEVTOOLS__: true,
		},
		build: {
			// 生产环境取消 console
			minify: "terser",

			// sourcemap: true,
			manifest: true,
			// 启用/禁用 gzip 压缩
			reportCompressedSize: false,

			terserOptions: {
				compress: {
					drop_console: true,
					drop_debugger: true,
				},
				keep_classnames: true,
			},

			rollupOptions: {
				output: {
					manualChunks: {
						echarts: ["echarts"],
						"ant-design-vue": ["ant-design-vue"],
						vue: ["vue", "vue-router", "vuex", "vue-i18n"],
					},
				},
			},
			chunkSizeWarningLimit: 1000,
		},
		plugins: [
			vue({
				script: {
					// 开启ref转换
					// refTransform: true,
				},
			}),
			vueSetupExtend(),
			VueJSX(),
			AutoImport({
				imports: ["vue"],
				dirs: ["./src/utils/permission"],
				dts: r("src/auto-imports.d.ts"),
			}),
			// 组件按需引入
			Components({
				dirs: [r("src/components")],
				dts: false,
				resolvers: [],
			}),
			antdDayjs(),
			visualizer(),
		],
		css: {
			preprocessorOptions: {
				less: {
					// 必须开启，不然ant的样式库引入时会报错
					javascriptEnabled: true,
					plugins: [new Less2CssVariablePlugin()],
				},
			},
		},
		optimizeDeps: {},
	});
};
