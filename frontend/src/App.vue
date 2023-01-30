<template>
	<a-config-provider :locale="locale">
		<router-view></router-view>
	</a-config-provider>
</template>

<script setup name="App">
import i18n from "@/locales";

const locale = i18n.global.messages[i18n.global.locale].lang;
</script>

<style scoped>
.logo {
	height: 6em;
	padding: 1.5em;
	will-change: filter;
}
.logo:hover {
	filter: drop-shadow(0 0 2em #646cffaa);
}
.logo.vue:hover {
	filter: drop-shadow(0 0 2em #42b883aa);
}
</style>
