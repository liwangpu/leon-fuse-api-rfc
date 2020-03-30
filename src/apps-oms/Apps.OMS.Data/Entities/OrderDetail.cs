using Apps.Base.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Apps.OMS.Data.Entities
{
    public class OrderDetail : IEntity
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
        public string ProductSpecId { get; set; }
        public int Num { get; set; }
        public decimal UnitPrice { get; set; }
        public string Remark { get; set; }
        public string Room { get; set; }
        public string Owner { get; set; }
        public string AttachmentIds { get; set; }
        public string OrderId { get; set; }
        public Order Order { get; set; }
        public List<OrderDetailPackage> Packages { get; set; }
        [NotMapped]
        public decimal TotalPrice
        {
            get
            {
                return Math.Round(UnitPrice * Num, 2, MidpointRounding.AwayFromZero);
            }
        }
    }
}
