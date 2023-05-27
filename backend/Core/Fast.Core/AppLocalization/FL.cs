using System.Text;
using Fast.Core.AdminEnum;
using Fast.Core.AdminModel.Sys;
using Fast.Core.BaiduTranslator;
using Fast.Core.Cache;
using Fast.Iaas.Const;
using Furion.Localization;
using Microsoft.Extensions.DependencyInjection;

namespace Fast.Core.AppLocalization;

/// <summary>
/// 全局多语言静态类
/// 基于Furion扩展
/// </summary>
[SuppressSniffer]
public static class FL
{
    /// <summary>
    /// String 多语言
    /// </summary>
    /// <param name="name"></param>
    /// <param name="arguments"></param>
    /// <returns></returns>
    public static string Text(string name, params object[] arguments)
    {
        // 返回结果
        var result = name;

        // 当前语言
        var cultureName = L.GetSelectCulture().Culture.Name;

        // 检测是否为中文，这里占比使用70%即可
        var isChinese = ChineseCheck(name, 70);

        // 如果当前语言是中文，并且Name也是中文，直接返回Name
        if (cultureName == "zh-CN" && isChinese)
        {
            return result;
        }

        // 创建一个作用域
        Scoped.Create((_, scope) =>
        {
            var serviceScope = scope.ServiceProvider;
            // 获取缓存服务
            var _cache = serviceScope.GetService<ICache>();

            // 先从缓存中读取
            var sysAppLocalizationModels = _cache.Get<List<SysAppLocalizationOutput>>(CacheConst.SysAppLocalization);

            if (sysAppLocalizationModels == null || !sysAppLocalizationModels.Any())
            {
                // 获取服务
                var _db = serviceScope.GetService<ISqlSugarClient>();
                // 获取所有应用本地化配置信息
                sysAppLocalizationModels = _db.Queryable<SysAppLocalizationModel>().Select<SysAppLocalizationOutput>().ToList();

                // 放入缓存
                _cache.Set(CacheConst.SysAppLocalization, sysAppLocalizationModels);
            }

            // 如果不是中文，默认当作中文处理
            // 这里查到默认使用中英文匹配查找
            var sysAppLocalizationModel = sysAppLocalizationModels.FirstOrDefault(f => f.Chinese == name || f.English == name);
            if (sysAppLocalizationModel == null)
            {
                // 如果还是没有获取到，则根据是否为中文，使用百度翻译，翻译完成后保存数据库

                // 获取服务
                var _db = serviceScope.GetService<ISqlSugarClient>();

                // 百度翻译
                var from = BaiduTranslatorUtil.CN;
                var to = BaiduTranslatorUtil.EN;

                // 判断传入的字符串是否为中文
                if (!isChinese)
                {
                    // 不是中文默认使用 英文=>中文 处理
                    from = BaiduTranslatorUtil.EN;
                    to = BaiduTranslatorUtil.CN;
                }

                var translatorResultDto = BaiduTranslatorUtil.Translator(name, from, to);
                sysAppLocalizationModel = new SysAppLocalizationOutput {Chinese = name, English = name};
                // 判断是否存在翻译结果
                if (translatorResultDto?.Trans_Result is {Count: > 0})
                {
                    // 判断传入的字符串是否为中文
                    if (!isChinese)
                    {
                        sysAppLocalizationModel.Chinese = translatorResultDto.Trans_Result[0].Dst;
                        sysAppLocalizationModel.English = name;
                    }
                    else
                    {
                        sysAppLocalizationModel.Chinese = name;
                        sysAppLocalizationModel.English = translatorResultDto.Trans_Result[0].Dst;
                    }

                    // 存入数据库
                    _db.Insertable(new SysAppLocalizationModel
                    {
                        Chinese = sysAppLocalizationModel.Chinese,
                        English = sysAppLocalizationModel.English,
                        TranslationSource = TranslationSourceEnum.BaiduTranslate,
                        IsSystem = YesOrNotEnum.Y
                    }).ExecuteCommand();

                    // 删除缓存
                    _cache.Del(CacheConst.SysAppLocalization);
                }
            }

            result = cultureName switch
            {
                // 中文
                "zh-CN" => sysAppLocalizationModel.Chinese,
                // 英文
                "en-US" => sysAppLocalizationModel.English,
                _ => result
            };
        });

        return result;
    }

    /// <summary>
    /// 中文检测
    /// </summary>
    /// <param name="name"></param>
    /// <param name="percentage">占比</param>
    /// <returns></returns>
    public static bool ChineseCheck(string name, int percentage = 100)
    {
        // 获取字符串的长度
        var length = name.Length;

        // 英文字符总数
        var englishLen = 0.00M;

        // 根据任何字符在UniCode编码中都占用两个字节原理，中文要用两个，英文多占用一个，没用的字节为0
        var encoding = new UnicodeEncoding();
        var bytes = encoding.GetBytes(name);
        for (var i = 0; i < bytes.Length; i++)
        {
            i++;

            // 0和奇数字节等于0的就是英文字符
            if (bytes[i] == 0)
            {
                englishLen++;
            }
        }

        // 判断是否没有任何的英文字符
        if (englishLen == 0)
        {
            return true;
        }

        // 四舍五入，计算英文字符占比
        var englishPer = Math.Round(englishLen / length, 2);

        // (1 - 英文占比) * 100 大于等于百分比，则代表是中文
        return (1 - englishPer) * 100 >= percentage;
    }

    /// <summary>
    /// 系统应用本地化表Model类
    /// </summary>
    class SysAppLocalizationOutput
    {
        /// <summary>
        /// 中文
        /// </summary>
        public string Chinese { get; set; }

        /// <summary>
        /// 英文
        /// </summary>
        public string English { get; set; }
    }
}