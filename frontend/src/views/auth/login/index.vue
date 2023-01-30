<template>
	<div>
		{{ test }}
		<br />
		<a-button type="primary" @click="login('SuperAdmin', 1)"
			>测试账号登录</a-button
		>
		<br />
		<br />
		<a-button type="primary" @click="login('superAdmin@18kboy.icu', 2)"
			>测试邮箱登录</a-button
		>
		<br />
		<br />
		<a-button type="primary" @click="login('18888888888', 3)"
			>测试手机登录</a-button
		>
		<br />
	</div>
</template>

<script src="./index.ts" lang="ts"></script>

<style src="./index.less" lang="less"></style>
