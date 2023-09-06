#nullable enable
using System;

namespace Fast.Iaas.Attributes
{
    /// <summary>
    /// 枚举特性
    /// 用于区分是否可以写入枚举字典的特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Enum)]
    public class FastEnumAttribute : Attribute
    {
        /// <summary>
        /// 中文名称
        /// </summary>
        public string? ChName { get; set; }

        /// <summary>
        /// 英文名称
        /// </summary>
        public string? EnName { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string? Remark { get; set; }

        public FastEnumAttribute()
        {
        }

        public FastEnumAttribute(string? chName, string? enName, string? remark)
        {
            ChName = chName;
            EnName = enName;
            Remark = remark;
        }

        public FastEnumAttribute(string? chName, string? enName)
        {
            ChName = chName;
            EnName = enName;
            Remark = chName;
        }

        public FastEnumAttribute(string? chName)
        {
            ChName = chName;
            Remark = chName;
        }
    }
}