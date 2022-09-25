using Fast.Core.ServiceCollection;
using ServiceCollection = Fast.Core.ServiceCollection.ServiceCollection;

ServiceCollection.JWT = false;

WebApplication.CreateBuilder(args).Inject().RunProgram();