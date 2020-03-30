using Apps.Base.Common.Interfaces;
using System;

namespace Apps.OMS.Data.Entities
{
    public class NationalUrban : IEntity
    {
        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string Creator { get; set; }
        /// <summary>
        /// 更新人
        /// </summary>
        public string Modifier { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedTime { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime ModifiedTime { get; set; }
        public string Description { get; set; }
        /// <summary>
        /// 代码
        /// </summary>
        public int CodeNumber { get; set; }
        /// <summary>
        /// 父级
        /// </summary>
        public string ParentId { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public string NationalUrbanType { get; set; }
    }
}
