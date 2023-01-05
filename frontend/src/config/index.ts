const DEFAULT_CONFIG = {
	// 首页地址
	DASHBOARD_URL: "/index",

	// 接口地址
	API_URL: import.meta.env.VITE_API_BASEURL,

	// 请求超时
	TIMEOUT: 10000,

	// OriginName
	ORIGIN_NAME: "Fast-Net-Origin",

	// TokenName // Authorization
	TOKEN_NAME: "Authorization",

	// Token前缀，注意最后有个空格，如不需要需设置空字符串 // Bearer
	TOKEN_PREFIX: "Bearer ",

	// 追加其他头
	HEADERS: {},

	// 请求是否开启缓存
	REQUEST_CACHE: false,

	// 语言
	LANG: "zh-cn",
};

export default DEFAULT_CONFIG;
