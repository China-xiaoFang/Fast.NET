const generatePrimaryColors = () => {
	const result = {
		primary: `var(--primary-color)`,
	};
	for (let i = 0; i < 10; i++) {
		result[`primary-${i}`] = `var(--primary-${i})`;
	}
	return result;
};

const generateFontSize = () => {
	const result = {};
	for (let i = 10; i < 32; i++) {
		result[i] = `${i}px`;
	}
	return result;
};

const colors = require("tailwindcss/colors");

module.exports = {
	content: ["./src/**/*.vue", "./src/**/*.js"],
	darkMode: "class", // or 'media' or 'class'
	corePlugins: {
		preflight: false,
	},
	theme: {
		extend: {},
		colors: {
			transparent: "transparent",
			current: "currentColor",
			black: colors.black,
			white: colors.white,
			gray: colors.neutral,
			indigo: colors.indigo,
			red: colors.rose,
			yellow: colors.amber,
			...generatePrimaryColors(),
		},
		fontWeight: {
			1: 100,
			2: 200,
			3: 300,
			4: 400,
			5: 500,
			6: 600,
			7: 700,
			8: 800,
			9: 900,
		},
		fontSize: {
			...generateFontSize(),
		},
	},
	variants: {},
	plugins: [],
};
