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
		<a-button type="primary" @click="login('18888888888', 1)"
			>测试手机登录</a-button
		>
		<br />
		<br />
		<a-button type="primary" @click="testGet1('SuperAdmin', 1)"
			>测试Get请求参数加密</a-button
		>
		<br />
		<br />
		<a-button type="primary" @click="testGet2('SuperAdmin', 1)"
			>测试Get请求参数加密</a-button
		>
		<br />
		<br />
		<br />

		<div class="change-locale">
			<span style="margin-right: 16px">测试国际化</span>
		</div>
		<div class="locale-components">
			<div class="example">
				<a-pagination :total="50" show-size-changer />
			</div>
			<div class="example">
				<a-select show-search style="width: 200px">
					<a-select-option value="jack">jack</a-select-option>
					<a-select-option value="lucy">lucy</a-select-option>
				</a-select>
				<a-date-picker />
				<a-time-picker />
				<a-range-picker style="width: 200px" />
			</div>
			<div class="example">
				<a-button type="primary" @click="visible = true"
					>Show Modal</a-button
				>
				<a-button @click="info">Show info</a-button>
				<a-button @click="confirm">Show confirm</a-button>
				<a-popconfirm title="Question?">
					<a href="#">Click to confirm</a>
				</a-popconfirm>
			</div>
			<div class="example">
				<a-transfer
					:data-source="[]"
					show-search
					:target-keys="[]"
					:render="(item) => item.title"
				/>
			</div>
			<div class="site-config-provider-calendar-wrapper">
				<a-calendar :fullscreen="false" />
			</div>
			<div class="example">
				<a-table :data-source="[]" :columns="columns" />
			</div>
			<a-modal v-model:visible="visible" title="Locale Modal">
				<p>Locale Modal</p>
			</a-modal>
		</div>
	</div>
</template>

<script src="./index.ts" lang="ts"></script>

<style src="./index.less" lang="less"></style>
