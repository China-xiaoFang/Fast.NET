<template>
	<div class="login_background">
		<div class="login_background_front"></div>
		<div class="login_main">
			<div class="login_config">
				<a-dropdown>
					<global-outlined />
					<template #overlay>
						<a-menu>
							<a-menu-item v-for="item in lang" :key="item.value" :command="item" :class="{
								selected: config.lang === item.value,
							}" @click="configLang(item.value)">
								{{ item.name }}
							</a-menu-item>
						</a-menu>
					</template>
				</a-dropdown>
			</div>
			<div class="login-form">
				<a-card>
					<div class="login-header">
						<div class="logo">
							<img :alt="webSiteInfo.chName" :src="webSiteInfo.logoUrl" />
							<label>{{ webSiteInfo.chName }}</label>
						</div>
					</div>
					<a-tabs v-model:activeKey="activeKey">
						<a-tab-pane key="userAccount" :tab="$t('login.accountPassword')">
							<a-form ref="loginForm" :model="ruleForm" :rules="rules">
								<a-form-item name="account">
									<a-input v-model:value="ruleForm.account" :placeholder="
										$t('login.accountPlaceholder')
									" size="large" @keyup.enter="login">
										<template #prefix>
											<UserOutlined class="login-icon-gray" />
										</template>
									</a-input>
								</a-form-item>
								<a-form-item name="password">
									<a-input-password v-model:value="ruleForm.password" :placeholder="$t('login.PWPlaceholder')"
										size="large" autocomplete="off" @keyup.enter="login">
										<template #prefix>
											<LockOutlined class="login-icon-gray" />
										</template>
									</a-input-password>
								</a-form-item>
								<!-- <a-form-item name="validCode" v-if="captchaOpen">
									<a-row :gutter="8">
										<a-col :span="17">
											<a-input v-model:value="
												ruleForm.validCode
											" :placeholder="
	$t('login.validPlaceholder')
" size="large">
												<template #prefix>
													<verified-outlined class="login-icon-gray" />
												</template>
											</a-input>
										</a-col>
										<a-col :span="7">
											<img :src="validCodeBase64" class="login-validCode-img" @click="loginCaptcha" />
										</a-col>
									</a-row>
								</a-form-item> -->

								<a-form-item>
									<a href="/findpwd" style="color: #0d84ff">{{ $t("login.forgetPassword") }}？</a>
								</a-form-item>
								<a-form-item>
									<a-button type="primary" class="w-full" :loading="isLoading" round size="large" @click="login">{{
										$t("login.signIn") }}
									</a-button>
								</a-form-item>
							</a-form>
						</a-tab-pane>
						<!-- <a-tab-pane key="userSms" :tab="$t('login.phoneSms')" force-render>
							<phone-login-form />
						</a-tab-pane> -->
					</a-tabs>
					<!-- <three-login /> -->
				</a-card>
			</div>
		</div>
	</div>
</template>

<script src="./index.ts" lang="ts"></script>

<style src="./index.less" lang="less"></style>
