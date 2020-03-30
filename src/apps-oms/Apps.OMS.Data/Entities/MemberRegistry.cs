using Apps.Base.Common.Interfaces;
using System;
namespace Apps.OMS.Data.Entities
{
    public class MemberRegistry : IEntity
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
        /// <summary>
        /// 数据激活状态标记
        /// </summary>
        public int ActiveFlag { get; set; }
        /// <summary>
        /// 组织Id
        /// </summary>
        public string OrganizationId { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        public string Phone { get; set; }
        public string Mail { get; set; }
        public string Company { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string County { get; set; }
        public string BusinessCard { get; set; }
        public string Inviter { get; set; }
        /// <summary>
        /// 是否审批
        /// </summary>
        public bool IsApprove { get; set; }
        /// <summary>
        /// 审批人
        /// </summary>
        public string Approver { get; set; }
    }
}
