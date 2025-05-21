﻿// ------------------------------------------------------------------------
// Apache开源许可证
// 
// 版权所有 © 2018-Now 小方
// 
// 许可授权：
// 本协议授予任何获得本软件及其相关文档（以下简称“软件”）副本的个人或组织。
// 在遵守本协议条款的前提下，享有使用、复制、修改、合并、发布、分发、再许可、销售软件副本的权利：
// 1.所有软件副本或主要部分必须保留本版权声明及本许可协议。
// 2.软件的使用、复制、修改或分发不得违反适用法律或侵犯他人合法权益。
// 3.修改或衍生作品须明确标注原作者及原软件出处。
// 
// 特别声明：
// - 本软件按“原样”提供，不提供任何形式的明示或暗示的保证，包括但不限于对适销性、适用性和非侵权的保证。
// - 在任何情况下，作者或版权持有人均不对因使用或无法使用本软件导致的任何直接或间接损失的责任。
// - 包括但不限于数据丢失、业务中断等情况。
// 
// 免责条款：
// 禁止利用本软件从事危害国家安全、扰乱社会秩序或侵犯他人合法权益等违法活动。
// 对于基于本软件二次开发所引发的任何法律纠纷及责任，作者不承担任何责任。
// ------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

// ReSharper disable once CheckNamespace
namespace Fast.IaaS
{
    /// <summary>
    /// <see cref="ValidateExtension"/> 验证拓展类
    /// </summary>
    public static class ValidateExtension
    {
        /// <summary>
        /// 判断 string 是否为 Null
        /// </summary>
        /// <param name="value"><see cref="string"/>字符串</param>
        /// <returns><see cref="bool"/></returns>
        public static bool IsEmpty(this string value)
        {
            return value == null || string.IsNullOrEmpty(value);
        }

        /// <summary>
        /// 判断 Enum 是否为 Null
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="value">枚举值</param>
        /// <returns><see cref="bool"/></returns>
        public static bool IsEmpty<TEnum>(this TEnum value) where TEnum : struct, Enum
        {
            // 判断是否有效
            if (!Enum.IsDefined(typeof(TEnum), value))
            {
                return true;
            }

            // 判断是否为默认值
            if (EqualityComparer<TEnum>.Default.Equals(value, default))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 判断 Enum 是否为 Null
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="value">枚举值</param>
        /// <returns><see cref="bool"/></returns>
        public static bool IsEmpty<TEnum>(this TEnum? value) where TEnum : struct, Enum
        {
            // 判断是否为空
            if (!value.HasValue)
            {
                return true;
            }

            return value.Value.IsEmpty();
        }

        /// <summary>
        /// 判断 Enum 是否为 Null 或者 0
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="value">枚举值</param>
        /// <returns><see cref="bool"/></returns>
        public static bool IsNullOrZero<TEnum>(this TEnum value) where TEnum : struct, Enum
        {
            // 判断是否为空
            if (value.IsEmpty())
            {
                return true;
            }

            // 这里全部转为 Long 类型处理，避免 Int 长度过短，导致报错
            return value.ParseToLong() == 0;
        }

        /// <summary>
        /// 判断 Enum 是否为 Null 或者 0
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="value">枚举值</param>
        /// <returns><see cref="bool"/></returns>
        public static bool IsNullOrZero<TEnum>(this TEnum? value) where TEnum : struct, Enum
        {
            // 判断是否为空
            if (value.IsEmpty())
            {
                return true;
            }

            // 这里全部转为 Long 类型处理，避免 Int 长度过短，导致报错
            return value.ParseToLong() == 0;
        }

        /// <summary>
        /// 判断 byte 是否为 Null 或者 0
        /// </summary>
        /// <param name="value"><see cref="int"/>值</param>
        /// <returns><see cref="bool"/></returns>
        public static bool IsNullOrZero(this byte? value)
        {
            return value switch
            {
                null => true,
                0 => true,
                _ => false
            };
        }

        /// <summary>
        /// 判断 int 是否为 Null 或者 0
        /// </summary>
        /// <param name="value"><see cref="int"/>值</param>
        /// <returns><see cref="bool"/></returns>
        public static bool IsNullOrZero(this int? value)
        {
            return value switch
            {
                null => true,
                0 => true,
                _ => false
            };
        }

        /// <summary>
        /// 判断 double 是否为 Null 或者 0
        /// </summary>
        /// <param name="value"><see cref="double"/>值</param>
        /// <returns><see cref="bool"/></returns>
        public static bool IsNullOrZero(this double? value)
        {
            return value switch
            {
                null => true,
                0 => true,
                _ => false
            };
        }

        /// <summary>
        /// 判断 decimal 是否为 Null 或者 0
        /// </summary>
        /// <param name="value"><see cref="decimal"/>值</param>
        /// <returns><see cref="bool"/></returns>
        public static bool IsNullOrZero(this decimal? value)
        {
            return value switch
            {
                null => true,
                0 => true,
                _ => false
            };
        }

        /// <summary>
        /// 判断 float 是否为 Null 或者 0
        /// </summary>
        /// <param name="value"><see cref="decimal"/>值</param>
        /// <returns><see cref="bool"/></returns>
        public static bool IsNullOrZero(this float? value)
        {
            return value switch
            {
                null => true,
                0 => true,
                _ => false
            };
        }

        /// <summary>
        /// 判断 long 是否为 Null 或者 0
        /// </summary>
        /// <param name="value"><see cref="long"/>值</param>
        /// <returns><see cref="bool"/></returns>
        public static bool IsNullOrZero(this long? value)
        {
            return value switch
            {
                null => true,
                0 => true,
                _ => false
            };
        }

        #region 验证输入字符串为数字(带小数)

        /// <summary>
        /// 验证输入字符串为带小数点正数
        /// </summary>
        /// <param name="str">输入字符</param>
        /// <returns>返回一个bool类型的值</returns>
        public static bool IsNumber(this string str)
        {
            return Regex.IsMatch(str, "^([0]|([1-9]+\\d{0,}?))(.[\\d]+)?$");
        }

        /// <summary>
        /// 验证输入字符串为带小数点正负数
        /// </summary>
        /// <param name="str">输入字符</param>
        /// <returns>返回一个bool类型的值</returns>
        public static bool IsDecimalNumber(this string str)
        {
            return Regex.IsMatch(str, "^-?\\d+$|^(-?\\d+)(\\.\\d+)?$");
        }

        #endregion

        #region 验证中国电话格式是否有效，格式010-85849685

        /// <summary>
        /// 验证中国电话格式是否有效，格式010-85849685
        /// </summary>
        /// <param name="str">输入字符</param>
        /// <returns>返回一个bool类型的值</returns>
        public static bool IsTel(this string str)
        {
            return Regex.IsMatch(str, @"^(0[0-9]{2,3}\-)?([2-9][0-9]{6,7})+(\-[0-9]{1,4})?$", RegexOptions.IgnoreCase);
        }

        #endregion

        #region 验证输入字符串为电话号码

        /// <summary>
        /// 验证输入字符串为电话号码
        /// </summary>
        /// <param name="str">输入字符</param>
        /// <returns>返回一个bool类型的值</returns>
        public static bool IsPhone(this string str)
        {
            return Regex.IsMatch(str, @"(^(\d{2,4}[-_－—]?)?\d{3,8}([-_－—]?\d{3,8})?([-_－—]?\d{1,7})?$)|(^0?1[35]\d{9}$)");
            //弱一点的验证：  @"\d{3,4}-\d{7,8}"         
        }

        #endregion

        #region 验证是否是有效传真号码

        /// <summary>
        /// 验证是否是有效传真号码
        /// </summary>
        /// <param name="str">输入字符</param>
        /// <returns>返回一个bool类型的值</returns>
        public static bool IsFax(this string str)
        {
            return Regex.IsMatch(str, @"^[+]{0,1}(\d){1,3}[ ]?([-]?((\d)|[ ]){1,12})+$");
        }

        #endregion

        #region 验证手机号是否合法

        /// <summary>
        /// 验证手机号是否合法 号段为13,14,15,16,17,18,19  0，86开头将自动识别
        /// </summary>
        /// <param name="str">输入字符</param>
        /// <returns>返回一个bool类型的值</returns>
        public static bool IsMobile(this string str)
        {
            if (!str.StartsWith("1"))
            {
                str = str.TrimStart('8', '6').TrimStart('0');
            }

            return Regex.IsMatch(str, @"^(13|14|15|16|17|18|19)\d{9}$");
        }

        #endregion

        #region 验证身份证是否有效

        /// <summary>
        /// 验证身份证是否有效
        /// </summary>
        /// <param name="str">输入字符</param>
        /// <returns>返回一个bool类型的值</returns>
        public static bool IsIdCard(this string str)
        {
            return str.Length switch
            {
                18 => str.IsIdCard18(),
                15 => str.IsIdCard15(),
                _ => false
            };
        }

        /// <summary>
        /// 验证输入字符串为18位的身份证号码
        /// </summary>
        /// <param name="str">输入字符</param>
        /// <returns>返回一个bool类型的值</returns>
        public static bool IsIdCard18(this string str)
        {
            if (long.TryParse(str.Remove(17), out var n) == false || n < Math.Pow(10, 16) ||
                long.TryParse(str.Replace('x', '0').Replace('X', '0'), out _) == false)
            {
                return false; //数字验证
            }

            const string address =
                "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            if (!address.Contains(str.Remove(2)))
            {
                return false; //省份验证
            }

            var birth = str.Substring(6, 8).Insert(6, "-").Insert(4, "-");
            if (DateTime.TryParse(birth, out _) == false)
            {
                return false; //生日验证
            }

            var arrVarIfyCode = "1,0,x,9,8,7,6,5,4,3,2".Split(',');
            var wi = "7,9,10,5,8,4,2,1,6,3,7,9,10,5,8,4,2".Split(',');
            var ai = str.Remove(17).ToCharArray();
            var sum = 0;
            for (var i = 0; i < 17; i++)
            {
                sum += int.Parse(wi[i]) * int.Parse(ai[i].ToString());
            }

            Math.DivRem(sum, 11, out var y);
            return arrVarIfyCode[y] == str.Substring(17, 1).ToLower();
        }

        /// <summary>
        /// 验证输入字符串为15位的身份证号码
        /// </summary>
        /// <param name="str">输入字符</param>
        /// <returns>返回一个bool类型的值</returns>
        public static bool IsIdCard15(this string str)
        {
            if (long.TryParse(str, out var n) == false || n < Math.Pow(10, 14))
            {
                return false; //数字验证
            }

            const string address =
                "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            if (!address.Contains(str.Remove(2)))
            {
                return false; //省份验证
            }

            var birth = str.Substring(6, 6).Insert(4, "-").Insert(2, "-");
            return DateTime.TryParse(birth, out _);
        }

        #endregion

        #region 验证是否是有效邮箱地址

        /// <summary>
        /// 验证是否是有效邮箱地址
        /// </summary>
        /// <param name="str">输入字符</param>
        /// <returns>返回一个bool类型的值</returns>
        public static bool IsEmail(this string str)
        {
            return Regex.IsMatch(str,
                @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }

        /// <summary>
        /// 验证是否是有效QQ邮箱地址
        /// </summary>
        /// <param name="str">输入字符</param>
        /// <returns>返回一个bool类型的值</returns>
        public static bool IsQqEmail(this string str)
        {
            return Regex.IsMatch(str, @"[1-9]\d{4,10}@qq\.com");
        }

        #endregion

        #region 验证是否只含有汉字

        /// <summary>
        /// 验证是否只含有汉字
        /// </summary>
        /// <param name="strLn">输入字符</param>
        /// <returns></returns>
        public static bool IsOnlyChinese(this string strLn)
        {
            return Regex.IsMatch(strLn, @"^[\u4e00-\u9fa5]+$");
        }

        #endregion

        #region 是否有多余的字符 防止SQL注入

        /// <summary>
        /// 是否有多余的字符 防止SQL注入
        /// </summary>
        /// <param name="str">输入字符</param>
        /// <returns></returns>
        public static bool IsBadString(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return false;
            //列举一些特殊字符串
            const string badChars =
                "@,*,#,$,!,+,',=,--,%,^,&,?,(,), <,>,[,],{,},/,\\,;,:,\",\"\",delete,update,drop,alert,select";
            var arrBadChar = badChars.Split(',');
            return arrBadChar.Any(t => !str.Contains(t));
        }

        #endregion

        #region 是否由数字、26个英文字母或者下划线組成的字串

        /// <summary>
        /// 是否由数字、26个英文字母或者下划线組成的字串 
        /// </summary>
        /// <param name="str">输入字符</param>
        /// <returns></returns>
        public static bool IsNzx(this string str)
        {
            return Regex.Match(str, "^[0-9a-zA-Z_]+$").Success;
        }

        #endregion

        #region 由数字、26个英文字母、汉字組成的字串

        /// <summary>
        /// 由数字、26个英文字母、汉字組成的字串
        /// </summary>
        /// <param name="str">输入字符</param>
        /// <returns></returns>
        public static bool IsAlphaNumericChinese(this string str)
        {
            return Regex.Match(str, @"^[0-9a-zA-Z\u4e00-\u9fa5]+$").Success;
        }

        #endregion

        #region 由数字、26个英文字母組成的字串

        /// <summary>
        /// 是否由数字、26个英文字母組成的字串
        /// </summary>
        /// <param name="str">输入字符</param>
        /// <returns></returns>
        public static bool IsAlphaNumeric(this string str)
        {
            return Regex.Match(str, @"^[0-9a-zA-Z]+$").Success;
        }

        #endregion

        #region 验证输入字符串为邮政编码

        /// <summary>
        /// 验证输入字符串为邮政编码
        /// </summary>
        /// <param name="str">输入字符</param>
        /// <returns>返回一个bool类型的值</returns>
        public static bool IsPostCode(this string str)
        {
            return Regex.IsMatch(str, @"\d{6}");
        }

        #endregion

        #region 检查对象的输入长度

        /// <summary>
        /// 检查对象的输入长度
        /// </summary>
        /// <param name="str">输入字符</param>
        /// <param name="length">指定的长度</param>
        /// <returns>false 太长，true -太短</returns>
        public static bool CheckLength(this string str, int length)
        {
            if (str.Length > length)
            {
                return false; //长度太长
            }

            return str.Length >= length;
        }

        #endregion

        #region 判断用户输入是否为日期

        /// <summary>
        /// 判断用户输入是否为日期
        /// </summary>
        /// <param name="str">输入字符</param>
        /// <returns>返回一个bool类型的值</returns>
        /// <remarks>
        /// 可判断格式如下（其中-可替换为/，不影响验证):
        /// YYYY | YYYY-MM | YYYYMM | YYYY-MM-DD | YYYYMMDD | YYYY-MM-DD HH:MM:SS | YYYY-MM-DD HH:MM:SS.FFF
        /// </remarks>
        public static bool IsDateTime(this string str)
        {
            // 检查输入是否为空
            if (null == str)
            {
                return false;
            }

            // 定义一个正则表达式，用于验证日期格式
            const string regexDate =
                @"[1-2]{1}[0-9]{3}((-|\/|\.){1}(([0]?[1-9]{1})|(1[0-2]{1}))((-|\/|\.){1}((([0]?[1-9]{1})|([1-2]{1}[0-9]{1})|(3[0-1]{1}))(( ([0-1]{1}[0-9]{1})|2[0-3]{1}):([0-5]{1}[0-9]{1}):([0-5]{1}[0-9]{1})(\.[0-9]{3})?)?)?)?)?$";

            // 使用正则表达式来验证输入字符串是否符合日期格式
            if (!Regex.IsMatch(str, regexDate))
                return false;

            // 移除所有非数字字符，只保留数字部分
            var cleanStr = new string(str.Where(char.IsDigit).ToArray());

            // 检查日期长度
            if (cleanStr.Length == 4 || cleanStr.Length == 6 || cleanStr.Length == 8)
            {
                // 提取年份部分
                var year = int.Parse(cleanStr[..4]);

                // 如果字符串长度大于等于6，提取月份部分
                if (cleanStr.Length >= 6)
                {
                    var month = int.Parse(cleanStr.Substring(4, 2));

                    // 验证月份是否在合法范围内
                    if (month < 1 || month > 12)
                        return false;

                    // 如果字符串长度为8，提取日部分
                    if (cleanStr.Length == 8)
                    {
                        var day = int.Parse(cleanStr.Substring(6, 2));

                        // 验证日是否在合法范围内，考虑月份的天数
                        if (day < 1 || day > DateTime.DaysInMonth(year, month))
                            return false;
                    }
                }

                // 符合日期格式的条件
                return true;
            }

            // 如果不符合任何日期格式，返回false
            return false;
        }

        #endregion
    }
}