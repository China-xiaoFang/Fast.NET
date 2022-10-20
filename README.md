<div align="center">
    <p align="center">
        <img src="https://gitee.com/Net-18K/Fast.NET/raw/master/frontend/public/logn.png" height="100" alt="logo"/>
    </p>
</div>
<div align="center"><h1 align="center">Fast.NET</h1></div>

<div align="center"><h3 align="center">前后端分离架构，开箱即用，紧随前沿技术</h3></div>

<div align="center">

[![star](https://gitee.com/Net-18K/Fast.NET/badge/star.svg?theme=dark)](https://gitee.com/Net-18K/Fast.NET/stargazers)
[![fork](https://gitee.com/Net-18K/Fast.NET/badge/fork.svg?theme=dark)](https://gitee.com/Net-18K/Fast.NET/members)
[![GitHub license](https://img.shields.io/badge/license-Apache2-yellow)](https://gitee.com/Net-18K/fast.net/blob/master/LICENSE)

![今日诗词](https://v2.jinrishici.com/one.svg?font-size=20&spacing=2&color=Chocolate)
</div>

## 🍟 概述

* 基于.NET 6实现的通用权限管理平台（RBAC模式）。整合最新技术高效快速开发，前后端分离模式，开箱即用。
* 前端基于小诺Vue（antd）框架，整体RBAC基础数据结构+API接口风格采用小诺vue版本模式。
* 后台基于Furion框架，SqlSugar、多租户、分库读写分离、缓存、数据校验、鉴权、动态API、gRPC等众多黑科技集一身。
* 模块化架构设计，层次清晰，业务层推荐写到单独模块，框架升级不影响业务!
* 核心模块包括：用户、角色、职位、组织机构、菜单、字典、日志、多应用管理、文件管理、定时任务等功能。
* 代码量少、通俗易懂、功能强大、易扩展，轻松开发从现在开始！
* 集成工作流、SignalR等众多新功能。

```
如果对您有帮助，您可以点右上角 “Star” 收藏一下 ，获取第一时间更新，谢谢！
```

## 😁 分支说明
| 分支 | 详情 | 版本 | 环境 | 建议 |
| --- | ---- | ----- | ------ | ------- |
| master | 主分支 | 稳定版本 | 生产环境 | 如果需要Fork或者在此版本上进行修改，请拉取master分支的代码 |
| master_uat | 预生产分支 | Beat版本 | 演示环境 | master_uat是经过测试准备上线的版本，可以Fork，也可以学习，但是不建议这样的操作 |
| develop_test | 测试分支 | Test版本 | 测试环境 | develop_test是测试版本，此版本主要用于线上测试使用 |
| develop | 开发分支 | 迭代版本 | 开发环境 | develop是快速迭代版本，此版本的功能是未经过测试的代码，所以不建议用develop进行Fork和学习 |

## 快速启动

全栈工程师推荐idea

### 前端支撑
| 插件 | 版本   | 用途 |
|--- | ----- | ----- |
| node.js | 最新版 |  JavaScript运行环境 |

### 启动前端

```
npm install
```
```
npm run dev
```

### 后端支撑
| 插件 | 版本   | 用途 |
|--- | ----- | ----- |
| .NET 6 | 最新版 |  .NET运行环境 |

## 😎 框架来源

### 👉 原始版本【Admin.NET】（基于EF Core）

- [https://gitee.com/zuohuaijun/Admin.NET](https://gitee.com/zuohuaijun/Admin.NET)

### 👉 借鉴版本【Magic.NET】（基于SqlSugar）

- [https://gitee.com/zhengguojing/magic-net](https://gitee.com/zhengguojing/magic-net)

### 👉 衍生版本【Fast.NET】（基于SqlSugar）

- [https://gitee.com/Net-18K/fast.net](https://gitee.com/Net-18K/fast.net)

`如果集成其他ORM，请参照各自操作使用说明。系统默认SqlSugar不会处理其他ORM实体等，请自行处理。`

## 🥞 更新日志

更新日志 [点击查看](https://gitee.com/Net-18K/fast.net/commits/master)

## 🍿 在线体验

- 开发者租户：用户名：superAdmin，密码：123456          

- 地址1：[https://magic.18kboy.icu/](https://magic.18kboy.icu/) PS: 1核2G 1MB 配置，手下留情~

## 📖 帮助文档

### 👉后台文档：
* Furion后台框架文档 [https://furion.baiqian.ltd/docs](https://furion.baiqian.ltd/docs/)

### 👉前端文档：
* 小诺前端业务文档 [https://xiaonuo.vip/doc](https://xiaonuo.vip/doc)

1. Ant Design Pro of Vue 使用文档 [https://pro.antdv.com/docs/getting-started](https://pro.antdv.com/docs/getting-started)
2. Ant Design of Vue 组件文档 [https://1x.antdv.com/docs/vue/introduce-cn/](https://1x.antdv.com/docs/vue/introduce-cn/)
3. Vue 开发文档 [https://cn.vuejs.org/](https://cn.vuejs.org/)

### 👉关于signalr使用：

-  [wynnyo/vue-signalr: Signalr client for vue js (github.com)](https://github.com/wynnyo/vue-signalr)

😎通读以上文档，您就可以玩转本项目了（其实您已经是高手了）。项目使用上的问题，文档中基本都可以找到答案。

## 🍖 详细功能（待重新分析）

1. 主控面板、控制台页面，可进行工作台，分析页，统计等功能的展示。
2. 用户管理、对企业用户和系统管理员用户的维护，可绑定用户职务，机构，角色，数据权限等。
3. 应用管理、通过应用来控制不同维度的菜单展示。
4. 机构管理、公司组织架构维护，支持多层级结构的树形结构。
5. 职位管理、用户职务管理，职务可作为用户的一个标签，职务目前没有和权限等其他功能挂钩。
6. 菜单管理、菜单目录，菜单，和按钮的维护是权限控制的基本单位。
7. 角色管理、角色绑定菜单后，可限制相关角色的人员登录系统的功能范围。角色也可以绑定数据授权范围。
8. 字典管理、系统内各种枚举类型的维护。
9. 访问日志、用户的登录和退出日志的查看和管理。
10. 操作日志、用户的操作业务的日志的查看和管理。
11. 服务监控、服务器的运行状态，CPU、内存、网络等信息数据的查看。
12. 在线用户、当前系统在线用户的查看。
13. 公告管理、系统的公告的管理。
14. 文件管理、文件的上传下载查看等操作，文件可使用本地存储，阿里云oss，腾讯cos接入，支持拓展。
15. 定时任务、定时任务的维护，通过cron表达式控制任务的执行频率。
16. 系统配置、系统运行的参数的维护，参数的配置与系统运行机制息息相关。
17. 邮件发送、发送邮件功能。
18. 短信发送、短信发送功能，可使用阿里云sms，腾讯云sms，支持拓展。

## ⚡ 近期计划

- [x] 核心业务
- [x] 分库
- [x] 日志
- [x] 系统配置
- [x] 租户配置
- [x] 租户信息
- [x] 租户App授权
- [x] 租户分库
- [x] 菜单
- [x] 角色
- [x] 用户
- [x] ...

## 🥦 补充说明
* 基于.NET 6平台 Furion 开发框架与小诺 Vue 版本相结合！
* 基于Admin.NET(EFCore版本)和Magic.NET(SqlSugar版本)，但又不同于两者！
* 持续集百家所长，完善与丰富本框架基础设施，为.NET生态增加一种选择！
* 后期会推出基于此框架的相关应用场景案例，提供给大家使用！

## 🍻 贡献代码

`Fast.NET` 遵循 `Apache-2.0` 开源协议，欢迎大家提交 `PR` 或 `Issue`。

## 我为什么要为这个免费的开源库做出贡献？

我们都喜欢免费的开源库！ 但是有一个问题……

仅去年一年，我们就花费了N多个小时 维护我们所有的开源库。

我们需要你的帮助。 

您的贡献使我们能够将更多时间花在：错误修复、开发、文档和支持上。

## 我应该贡献多少？

任何数量都很感谢。

如果每个人都可以贡献一点点，这将帮助我们使 .NET 成为一个更好的生态！

另一个伟大的免费贡献方式是 传播 有关Admin.NET的信息。

非常感谢
感谢每一位贡献代码的朋友。

## 团队成员

| 成员 | 技术 | 昵称 | 
| :---: | :---: | :---: | 
| 1.8K仔 | 全栈 | 1.8K仔 | 

## 🍎 效果图
待开发中...

## 💐 特别鸣谢
- 👉 Furion：  [https://dotnetchina.gitee.io/furion](https://dotnetchina.gitee.io/furion)
- 👉 Admin.NET：  [https://gitee.com/zuohuaijun/Admin.NET](https://gitee.com/zuohuaijun/Admin.NET)
- 👉 Magic.NET：  [https://gitee.com/zhengguojing/magic-net](https://gitee.com/zhengguojing/magic-net)
- 👉 xiaonuo：[https://gitee.com/xiaonuobase/snowy](https://gitee.com/xiaonuobase/snowy)
- 👉 k-form-design：[https://gitee.com/kcz66/k-form-design](https://gitee.com/kcz66/k-form-design)
- 👉 MiniExcel：[https://gitee.com/dotnetchina/MiniExcel](https://gitee.com/dotnetchina/MiniExcel)
- 👉 SqlSugar：[https://gitee.com/dotnetchina/SqlSugar](https://gitee.com/dotnetchina/SqlSugar)

## 免责申明
    请勿用于违反我国法律的项目上 

如果对您有帮助，您可以点 "Star" 支持一下，这样才有持续下去的动力，谢谢！！！