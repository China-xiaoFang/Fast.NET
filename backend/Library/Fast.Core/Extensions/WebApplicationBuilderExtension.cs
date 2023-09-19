using System;
using Microsoft.AspNetCore.Builder;

namespace Fast.Core.Extensions;

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

        InternalApp.WebHostEnvironment = builder.Environment;

        // 初始化配置
        InternalApp.ConfigureApplication(builder.WebHost, builder.Host);

        return builder;
    }

    private static void UseDefault()
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
        Gitee：https://gitee.com/Net-18K/fast.net
        ");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(@"
        持续集百家所长，完善与丰富本框架基础设施，为.NET生态增加一种选择！

        期待您的PR，让.NET更好！
        ");
    }
}