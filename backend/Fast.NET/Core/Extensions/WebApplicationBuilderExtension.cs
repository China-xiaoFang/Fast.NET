// Apache开源许可证
//
// 版权所有 © 2018-2023 1.8K仔
//
// 特此免费授予获得本软件及其相关文档文件（以下简称“软件”）副本的任何人以处理本软件的权利，
// 包括但不限于使用、复制、修改、合并、发布、分发、再许可、销售软件的副本，
// 以及允许拥有软件副本的个人进行上述行为，但须遵守以下条件：
//
// 在所有副本或重要部分的软件中必须包括上述版权声明和本许可声明。
//
// 软件按“原样”提供，不提供任何形式的明示或暗示的保证，包括但不限于对适销性、适用性和非侵权的保证。
// 在任何情况下，作者或版权持有人均不对任何索赔、损害或其他责任负责，
// 无论是因合同、侵权或其他方式引起的，与软件或其使用或其他交易有关。

using Microsoft.AspNetCore.Builder;

namespace Fast.NET.Core.Extensions;

/// <summary>
/// <see cref="WebApplicationBuilder"/> 拓展类
/// </summary>
public static class WebApplicationBuilderExtension
{
    /// <summary>
    /// 框架初始化
    /// </summary>
    /// <param name="builder"><see cref="WebApplicationBuilder"/></param>
    /// <returns><see cref="WebApplicationBuilder"/></returns>
    public static WebApplicationBuilder Initialize(this WebApplicationBuilder builder)
    {
        // 运行控制台输出
        UseDefault();

        InternalContext.WebHostEnvironment = builder.Environment;

        // 初始化配置
        FastContext.ConfigureApplication(builder.WebHost, builder.Host);

        return builder;
    }

    static void UseDefault()
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine(@$"
        Fast.NET 程序启动时间：{DateTime.Now:yyyy-MM-dd HH:mm:ss}
        ");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(@"
          ______                _         _   _   ______   _______ 
         |  ____|              | |       | \ | | |  ____| |__   __|
         | |__     __ _   ___  | |_      |  \| | | |__       | |   
         |  __|   / _` | / __| | __|     | . ` | |  __|      | |   
         | |     | (_| | \__ \ | |_   _  | |\  | | |____     | |   
         |_|      \__,_| |___/  \__| (_) |_| \_| |______|    |_|   
                                                                   
        ");
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(@"
        Gitee：https://gitee.com/Net-18K/Fast.NET
        ");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(@"
        持续集百家所长，完善与丰富本框架基础设施，为.NET生态增加一种选择！

        期待您的PR，让.NET更好！
        ");
        Console.ResetColor();
    }
}