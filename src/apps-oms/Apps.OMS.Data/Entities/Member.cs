using Apps.Base.Common.Interfaces;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Apps.OMS.Data.Entities
{
    public class Member : IEntity, IListView
    {
        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }
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
        [NotMapped]
        public string Description { get; set; }
        public string Company { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string County { get; set; }
        public string BusinessCard { get; set; }
        public string AccountId { get; set; }
        /// <summary>
        /// 邀请人
        /// </summary>
        public string Inviter { get; set; }
        /// <summary>
        /// 上级
        /// </summary>
        public string Superior { get; set; }
        [NotMapped]
        public string Name { get; set; }
        [NotMapped]
        public string Icon { get; set; }
    }
}
