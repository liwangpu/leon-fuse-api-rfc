using Apps.Base.Common.Interfaces;
using System;
using System.Collections.Generic;

namespace Apps.OMS.Data.Entities
{
    public class WorkFlow : IEntity
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
        public string OrganizationId { get; set; }
        public int ActiveFlag { get; set; }
        /// <summary>
        /// 适用组织类型,逗号分隔
        /// </summary>
        public string ApplyOrgans { get; set; }
        public List<WorkFlowItem> WorkFlowItems { get; set; }
    }

}
