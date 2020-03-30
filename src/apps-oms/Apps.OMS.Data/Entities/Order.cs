using Apps.Base.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Apps.OMS.Data.Entities
{
    public class Order : IEntity, IListView
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
        public string OrderNo { get; set; }
        public int TotalNum { get; set; }
        public decimal TotalPrice { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerId { get; set; }
        public string SolutionId { get; set; }
        public string Data { get; set; }
        public Customer Customer { get; set; }
        public string WorkFlowItemId { get; set; }
        public string SubOrderIds { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
        public List<OrderFlowLog> OrderFlowLogs { get; set; }
        public List<OrderDetailCustomizedProduct> CustomizedProducts { get; set; }
        //public List<OrderPackage> OrderPackages { get; set; }
        [NotMapped]
        public string Icon { get; set; }
    }
}
