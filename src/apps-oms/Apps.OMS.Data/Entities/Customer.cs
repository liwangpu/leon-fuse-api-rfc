using Apps.Base.Common.Interfaces;
using System;

namespace Apps.OMS.Data.Entities
{
    public class Customer : IEntity, IListView
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
        public string Mail { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public string Telephone { get; set; }
        public string Cellphone { get; set; }
        public string Address { get; set; }
        public string OrganizationId { get; set; }
    }
}
