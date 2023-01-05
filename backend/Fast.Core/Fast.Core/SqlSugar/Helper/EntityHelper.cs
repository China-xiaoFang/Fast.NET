using System.Reflection;
using Fast.Core.AdminFactory.EnumFactory;
using Fast.Core.SqlSugar.AttributeFilter;
using Fast.Core.SqlSugar.BaseModel;
using Fast.Core.SqlSugar.BaseModel.Interface;
using Fast.Core.SqlSugar.Dto;
using Furion.FriendlyException;

namespace Fast.Core.SqlSugar.Helper;

/// <summary>
/// 实体类帮助类
/// </summary>
public static class EntityHelper
{
    /// <summary>
    /// 缓存 Entity 类型
    /// </summary>
    private static List<SugarEntityTypeInfo> _cacheEntityTypeList { get; set; }

    /// <summary>
    /// 清空缓存 Entity 类型
    /// </summary>
    public static void ClearCacheEntityType()
    {
        _cacheEntityTypeList = null;
    }

    /// <summary>
    /// 反射获取所有的数据库Model Type
    /// </summary>
    /// <returns></returns>
    public static List<SugarEntityTypeInfo> ReflexGetAllTEntityList()
    {
        var excludeBaseTypes = new List<Type>
        {
            typeof(IBaseEntity),
            typeof(IBaseLogEntity),
            typeof(IBaseTenant),
            typeof(AutoIncrementEntity),
            typeof(BaseEntity),
            typeof(BaseLogEntity),
            typeof(BaseTenant),
            typeof(BaseTEntity),
            typeof(PrimaryKeyEntity),
        };

        // 先从缓存获取
        var entityTypeList = _cacheEntityTypeList;
        if (entityTypeList != null && entityTypeList.Any())
            return entityTypeList;

        // 获取所有实现了接口的类的类型
        var types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(sl => sl.GetTypes().Where(wh => wh.GetInterfaces().Contains(typeof(IDbEntity))))
            // 排除BaseEntity
            .Where(wh => !excludeBaseTypes.Contains(wh));

        // 遍历获取到的类型集合
        // 获取数据库类型，如果没有则默认是Admin库
        entityTypeList = types.Select(type => new SugarEntityTypeInfo(type.Name,
            type.GetCustomAttribute<DataBaseTypeAttribute>(true)?.SysDataBaseType ?? SysDataBaseTypeEnum.Admin, type)).ToList();
        // 放入缓存
        _cacheEntityTypeList = entityTypeList;

        return entityTypeList;
    }

    /// <summary>
    /// 反射获取所有的数据库Model
    /// </summary>
    /// <returns></returns>
    public static SugarEntityTypeInfo ReflexGetAllTEntity(string name)
    {
        var entityType = ReflexGetAllTEntityList().FirstOrDefault(f => f.ClassName == name);
        if (entityType == null)
        {
            throw Oops.Oh(ErrorCode.SugarModelError);
        }

        return entityType;
    }
}