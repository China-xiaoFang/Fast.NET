/*
 * Author 1.8K仔
 * Fast.Core 常用Using引用
 * 此文件为了减少代码，将所有的Using引用，全部放在此处
 */

global using System.Text;
global using System.Web;
global using System.Reflection;
global using System.ComponentModel;
global using System.Security.Cryptography;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using System.Linq.Expressions;
global using System.Linq.Dynamic.Core;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.Extensions.Caching.Memory;
global using Microsoft.Extensions.Options;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;
global using Furion;
global using Furion.ConfigurableOptions;
global using Furion.DependencyInjection;
global using Furion.FriendlyException;
global using Furion.DynamicApiController;
global using static Furion.App;
global using SqlSugar;
global using Yitter.IdGenerator;
global using Fast.Iaas;
global using Fast.Iaas.Util;
global using Fast.Iaas.Extension;
global using Mapster;
global using Fast.Core;
global using Fast.Core.AdminFactory.BaseModelFactory;
global using Fast.Core.AdminFactory.BaseModelFactory.Const;
global using Fast.Core.AdminFactory.BaseModelFactory.Dto;
global using Fast.Core.AdminFactory.BaseModelFactory.Interface;
global using Fast.Core.AdminFactory.EnumFactory;
global using Fast.Core.AdminFactory.ModelFactory;
global using Fast.Core.AdminFactory.ModelFactory.Basis;
global using Fast.Core.AdminFactory.ModelFactory.Sys;
global using Fast.Core.AdminFactory.ModelFactory.Tenant;
global using Fast.Core.AttributeFilter;
global using Fast.Core.AttributeFilter.Http;
global using HttpGetAttribute = Fast.Core.AttributeFilter.Http.HttpGetAttribute;
global using HttpPostAttribute = Fast.Core.AttributeFilter.Http.HttpPostAttribute;
global using HttpPutAttribute = Fast.Core.AttributeFilter.Http.HttpPutAttribute;
global using HttpDeleteAttribute = Fast.Core.AttributeFilter.Http.HttpDeleteAttribute;
global using Fast.Core.Cache;
global using Fast.Core.SqlSugar;
global using Fast.Core.Util;
global using Fast.Core.Util.Captcha;
global using Fast.Core.Util.Captcha.ClickWord;
global using Fast.Core.Util.Captcha.General;
global using Fast.Core.Util.Http;
global using Fast.Core.Util.Json;
global using Fast.Core.Util.Json.Extension;
global using Fast.Core.Util.Json.JsonConverter;
global using Fast.Core.Util.MiniExcel;
global using Fast.Core.Util.MiniExcel.AttributeFilter;
global using Fast.Core.Util.MiniExcel.Extension;
global using Fast.Core.Util.SnowflakeId.Extension;
global using Fast.Core.Util.Restful.Extension;
global using Fast.Core.Util.Restful.Internal;