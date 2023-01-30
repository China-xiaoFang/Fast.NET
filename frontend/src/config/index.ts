const DEFAULT_CONFIG = {
	// 首页地址
	DASHBOARD_URL: "/index",

	// 请求超时，30秒
	TIMEOUT: 1000 * 10,

	// TenantId
	TENANT_ID_NAME: "Fast-NET-TenantId",

	// OriginName
	ORIGIN_NAME: "Fast-Net-Origin",

	// TokenName // Authorization
	TOKEN_NAME: "Authorization",

	// Token前缀，注意最后有个空格，如不需要需设置空字符串 // Bearer
	TOKEN_PREFIX: "Bearer ",

	// UUID
	UUID: "Fast-Net-UUID",

	// 追加其他头
	HEADERS: {},

	// 请求是否开启缓存
	REQUEST_CACHE: false,

	// 语言
	LANG: "zh-cn",

	// HTML页面
	HTML_PAGE: {
		// 404页面
		404: "/html/40x.html",
		// 500页面
		500: "/html/50x.html",
	},
};

export default DEFAULT_CONFIG;
