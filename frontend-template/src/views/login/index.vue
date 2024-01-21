<template>
    <div>
        <h1>{{ $t("views.login.你好啊！") }}</h1>
        <indexForm />
        <testForm />
        <el-button type="primary" @click="login()">尝试登录</el-button>
    </div>
</template>

<script setup lang="ts" name="HelloWorld">
import indexForm from "../test/index.vue";
import testForm from "@/views/test/test.vue";
import * as loginApi from "@/api/login";
import { LoginMethodEnum } from "@/api/modules/login-method-enum";

const login = ()=>{
    loginApi.login({
        account :"15288888888",
        password:"Fast888888",
        loginMethod:LoginMethodEnum.Account
    }).then(res =>{
        if(res.success){
            debugger;
        }else{
            console.log(res);
        }
    })
}
</script>
