<template>
    <div>
        <h1>{{ $t("views.login.你好啊！") }}</h1>
        <indexForm />
        <testForm />
    </div>
</template>

<script setup lang="ts" name="HelloWorld">
import indexForm from "../test/index.vue";
import testForm from "@/views/test/test.vue";
</script>
