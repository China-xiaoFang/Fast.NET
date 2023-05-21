using Microsoft.Extensions.DependencyInjection;

namespace Fast.ServiceCollection.Util;

public static class RunStyleExtension
{
    public static void AddRunStyle(this IServiceCollection services, Action<RunStyleBuilder> configure)
    {
        var builder = new RunStyleBuilder();
        configure(builder);
    }
}

public class RunStyleBuilder
{
    public void UseDefault()
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