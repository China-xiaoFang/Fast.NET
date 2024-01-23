/**
 * 验证方法
 */
import { buildFormValidatorInstance } from "@/utils/validate/interface";
import type { RuleType } from "async-validator";
import type { FormInstance, FormItemRule } from "element-plus";
import { i18n } from "@/lang";

/**
 * Form 表单的校验，并且滚动到对应的字段
 * @param formRef el-form Ref
 * @param fn 校验成功执行的代码
 * @param fail 校验失败执行的代码
 */
export const validateScrollToField = async (formRef: FormInstance, fn: () => void, fail?: () => void) => {
    if (!formRef) return;
    await formRef.validate((isValid: boolean, invalidFields?: any) => {
        if (isValid) {
            fn();
        } else {
            if (invalidFields) {
                fail && fail();
                formRef.scrollToField(Object.keys(invalidFields));
            }
        }
    });
};

/**
 * 构建表单验证规则
 * @param {buildFormValidatorInstance} paramsObj 参数对象
 * @returns
 */
export const buildFormValidator = ({ name, message, trigger = "blur" }: buildFormValidatorInstance): FormItemRule => {
    // 必填
    if (name == "required") {
        return {
            required: true,
            message: message,
            trigger: trigger,
        };
    }

    // 常见类型
    const validatorType = ["number", "integer", "float", "date", "url", "email"];
    if (validatorType.includes(name)) {
        return {
            type: name as RuleType,
            message: message,
            trigger: trigger,
        };
    }

    // 自定义验证方法
    const validatorCustomFun: anyObj = {
        account: (rule: any, value: string, callback: Function) => {
            if (!isAccount(value)) {
                return callback(new Error(i18n.global.t("utils.validate.请输入正确的账号")));
            }

            return callback();
        },
        password: (rule: any, value: string, callback: Function) => {
            if (!isPassword(value)) {
                return callback(new Error(i18n.global.t("utils.validate.请输入正确的密码")));
            }

            return callback();
        },
        mobile: (rule: any, value: string, callback: Function) => {
            if (!isMobile(value)) {
                return callback(new Error(i18n.global.t("utils.validate.请输入正确的手机号码")));
            }

            return callback();
        },
        idNumber: (rule: any, value: string, callback: Function) => {
            if (!isIdNumber(value)) {
                return callback(new Error(i18n.global.t("utils.validate.请输入正确的身份证号码")));
            }

            return callback();
        },
        editorRequired: (rule: any, value: string, callback: Function) => {
            if (value == "<p><br></p>") {
                return callback(new Error(i18n.global.t("utils.validate.内容不能为空")));
            }

            return callback();
        },
    };
    if (validatorCustomFun[name]) {
        return {
            required: name == "editorRequired" ? true : false,
            validator: validatorCustomFun[name],
            trigger: trigger,
            message: message,
        };
    }
    return {};
};

/**
 * 验证是否为外部链接
 * @description 判断是否为外部链接，支持http、https、mailto和tel协议
 * @param arg 参数，表示待验证的链接
 * @returns 返回一个布尔值，指定参数是否为外部链接
 */
export const isExternal = (arg: string): boolean => {
    // 使用正则表达式匹配http、https、mailto、tel和ftp开头的链接
    const reg = /^(https?:|mailto:|tel:|ftp:)/;
    return reg.test(arg);
};

/**
 * 验证是否为有效的URL
 * @description 判断是否为有效的URL，支持http和https协议
 * @param arg 参数，表示待验证的链接
 * @returns 返回一个布尔值，指定参数是否为有效的URL
 */
export const isValidURL = (arg: string): boolean => {
    // 使用正则表达式验证是否为有效的URL
    const reg =
        /^(https?|ftp):\/\/([a-zA-Z0-9.-]+(:[a-zA-Z0-9.&%$-]+)*@)*((25[0-5]|2[0-4][0-9]|1[0-9]{2}|[1-9][0-9]?)(\.(25[0-5]|2[0-4][0-9]|1[0-9]{2}|[1-9]?[0-9])){3}|([a-zA-Z0-9-]+\.)*[a-zA-Z0-9-]+\.(com|edu|gov|int|mil|net|org|biz|arpa|info|name|pro|aero|coop|museum|[a-zA-Z]{2}))(:[0-9]+)*(\/($|[a-zA-Z0-9.,?'\\+&%$#=~_-]+))*$/;
    return reg.test(arg);
};

/**
 * 是否为手机设备
 */
export const isMobileDevice = () => {
    return !!navigator.userAgent.match(
        /android|webos|ip(hone|ad|od)|opera (mini|mobi|tablet)|iemobile|windows.+(phone|touch)|mobile|fennec|kindle (Fire)|Silk|maemo|blackberry|playbook|bb10\; (touch|kbd)|Symbian(OS)|Ubuntu Touch/i
    );
};

/**
 * 是否为有效的手机号码
 * @description 判断是否为有效的手机号码
 * @param arg 参数，表示待验证的手机号码
 * @returns 返回一个布尔值，指定参数是否为有效的手机号码
 */
export const isMobile = (arg: string): boolean => {
    const reg = /^(1[3-9])\d{9}$/;
    return reg.test(arg);
};

/**
 * 是否为有效的身份证号码
 * @description 判断是否为有效的身份证号码，支持15 18 位
 * @param arg 参数，表示待验证的身份证号码
 * @returns 返回一个布尔值，指定参数是否为有效的身份证号码
 */
export const isIdNumber = (arg: string): boolean => {
    const reg = /(^\d{15}$)|(^\d{18}$)|(^\d{17}(\d|X|x)$)/;
    return reg.test(arg);
};

/**
 * 是否为有效的账号
 * @description 判断是否为有效的账号，3 ~ 15 位
 * @param arg 参数，表示待验证的账号
 * @returns 返回一个布尔值，指定参数是否为有效的账号
 */
export const isAccount = (arg: string): boolean => {
    const reg = /^[a-zA-Z][a-zA-Z0-9_]{2,15}$/;
    const reg1 = /^(1[3-9])\d{9}$/;
    return reg.test(arg) || reg1.test(arg);
};

/**
 * 是否为有效的密码
 * @description 判断是否为有效的密码，6 ~ 16 位
 * @param arg 参数，表示待验证的密码
 * @returns 返回一个布尔值，指定参数是否为有效的密码
 */
export const isPassword = (arg: string): boolean => {
    const reg = /^(?!.*[&<>"'\n\r]).{6,32}$/;
    return reg.test(arg);
};
