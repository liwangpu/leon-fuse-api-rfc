using System;
using System.Collections.Generic;
using System.Text;

namespace Apps.OMS.Export.DTOs
{
    public class OrderDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Creator { get; set; }
        public string Modifier { get; set; }
        public string CreatorName { get; set; }
        public string ModifierName { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime ModifiedTime { get; set; }
        public string OrganizationId { get; set; }
        public string Url { get; set; }
        public string OrderNo { get; set; }
        public int TotalNum { get; set; }
        public decimal TotalPrice { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerPhone { get; set; }
        public string WorkFlowItemId { get; set; }
        public string WorkFlowItemName { get; set; }
        public string Data { get; set; }
        public string SolutionId { get; set; }
        public List<OrderDetailDTO> OrderDetails { get; set; }
        public List<OrderFlowLogDTO> OrderFlowLogs { get; set; }
        public List<OrderDetailCustomizedProductDTO> CustomizedProducts { get; set; }
    }

    public class OrderDetailDTO
    {
        public string Id { get; set; }
        public string Icon { get; set; }
        public string ProductSpecId { get; set; }
        public string ProductSpecName { get; set; }
        public string ProductId { get; set; }
        public string ProductCategoryId { get; set; }
        public string ProductName { get; set; }
        public string ProductCategoryName { get; set; }
        public string ProductUnit { get; set; }
        public string ProductBrand { get; set; }
        public string ProductDescription { get; set; }
        public int Num { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string Remark { get; set; }
        public string AttachmentIds { get; set; }
        public string Room { get; set; }
        public string Owner { get; set; }
        public string TPID { get; set; }
        public List<OrderDetailAttachmentDTO> Attachments { get; set; }
        public List<OrderDetailPackageDTO> Packages { get; set; }
    }

    public class OrderDetailCustomizedProductDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
    }

    public class OrderDetailPackageDTO
    {
        public string Id { get; set; }
        public string PackageName { get; set; }
        public int PackingUnit { get; set; }
        public decimal Num { get; set; }
    }

    public class OrderDetailAttachmentDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
    }

    public class OrderFlowLogDTO
    {
        public string Id { get; set; }
        public bool Approve { get; set; }
        public string Remark { get; set; }
        public string WorkFlowItemId { get; set; }
        public string WorkFlowItemName { get; set; }
        public string OrderId { get; set; }
        public DateTime CreatedTime { get; set; }
        public string Creator { get; set; }
        public string CreatorName { get; set; }
    }
}
