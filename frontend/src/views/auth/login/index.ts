// 这里可以导入其他文件（比如：组件，工具js，第三方插件js，json文件，图片文件等等）
// 例如：import 《组件名称》 from '《组件路径》';
import loginApi from "@/api/auth/loginApi";
import { required } from "@/utils/formRules";

export default {
	name: "login",
	// import引入的组件需要注入到对象中才能使用
	components: {},
	data() {
		// 这里存放数据
		return {
			activeKey: "userAccount",
			webSiteInfo: this.$tool.cache.get(this.$cacheKey.WEB_SITE_INFO),
			isLoading: false,
			config: {
				lang:
					this.$tool.cache.get(this.$cacheKey.APP_LANG) ||
					this.$sysConfig.LANG,
				theme:
					this.$tool.cache.get(this.$cacheKey.APP_THEME) || "default",
			},
			lang: [
				{
					name: "简体中文",
					value: "zh-CN",
				},
				{
					name: "English",
					value: "en-US",
				},
			],
			rules: {
				account: [required(this.$t("login.accountError"), ["blur"])],
				password: [required(this.$t("login.PWError"), ["blur"])],
			},
			ruleForm: {
				account: "superAdmin",
				password: "123456",
				validCode: "",
				validCodeReqNo: "",
				autoLogin: false,
			},
		};
	},
	// 监听属性 类似于data概念
	computed: {},
	// 监控data中的数据变化
	watch: {
		"config.theme": function (val) {
			document.body.setAttribute("data-theme", val);
		},
		"config.lang": function (val) {
			this.$i18n.locale = val;
			this.$tool.cache.set(this.$cacheKey.APP_LANG, val);
		},
	},
	// 方法集合
	methods: {
		/**
		 * 登录
		 */
		async login() {
			this.$refs.loginForm.validate().then(async () => {
				this.isLoading = true;
				// 登录
				await loginApi
					.webLogin({
						account: this.ruleForm.account,
						password: this.ruleForm.password,
						loginMethod: 1,
					})
					.finally(() => {
						this.isLoading = false;
					});
				this.$message.success(this.$t("login.loginSuccess"));
				this.$router.push({ path: "/" });
			});
		},
		/**
		 * 配置语言
		 */
		configLang(key) {
			this.config.lang = key;
		},
	},
	// 生命周期 - 创建完成（可以访问当前this实例）
	created() {
		this.$store.commit("clearViewTags");
		this.$store.commit("clearKeepLive");
		this.$store.commit("clearIframeList");
	},
	// 生命周期 - 挂载完成（可以访问DOM元素）
	mounted() {},
	beforeCreate() {}, // 生命周期 - 创建之前
	beforeMount() {}, // 生命周期 - 挂载之前
	beforeUpdate() {}, // 生命周期 - 更新之前
	updated() {}, // 生命周期 - 更新之后
	beforeDestroy() {}, // 生命周期 - 销毁之前
	destroyed() {}, // 生命周期 - 销毁完成
	activated() {}, // 如果页面有keep-alive缓存功能，这个函数会触发
};
