using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Apps.OMS.Export.Models
{
    #region OrderCreateModel 订单创建模型
    /// <summary>
    /// 订单创建模型
    /// </summary>
    public class OrderCreateModel
    {
        [StringLength(50, MinimumLength = 1, ErrorMessage = "长度必须为1-50个字符")]
        public string Name { get; set; }
        public string SolutionId { get; set; }
        [StringLength(200, ErrorMessage = "长度必须为0-200个字符")]
        public string Description { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerPhone { get; set; }
        public string Data { get; set; }
        public List<OrderDetailCreateModel> Content { get; set; }
        public List<CustomizedProduct> CustomizedProduct { get; set; }
    }
    #endregion

    #region OrderBasicInfoUpdateModel 订单基本信息编辑模型
    /// <summary>
    /// 订单基本信息编辑模型
    /// </summary>
    public class OrderBasicInfoUpdateModel
    {
        [Required(ErrorMessage = "必填信息")]
        public string Id { get; set; }
        [StringLength(50, MinimumLength = 1, ErrorMessage = "长度必须为1-50个字符")]
        public string Name { get; set; }
        [StringLength(200, ErrorMessage = "长度必须为0-200个字符")]
        public string Description { get; set; }
    }
    #endregion

    #region OrderCustomerInfoUpdateModel 订单客户基本信息更新模型
    /// <summary>
    /// 订单客户基本信息更新模型
    /// </summary>
    public class OrderCustomerInfoUpdateModel
    {
        [Required(ErrorMessage = "必填信息")]
        public string Id { get; set; }
        [StringLength(50, MinimumLength = 1, ErrorMessage = "长度必须为1-50个字符")]
        public string CustomerName { get; set; }
        [StringLength(50, MinimumLength = 1, ErrorMessage = "长度必须为1-50个字符")]
        public string CustomerAddress { get; set; }
        [StringLength(50, MinimumLength = 1, ErrorMessage = "长度必须为1-50个字符")]
        public string CustomerPhone { get; set; }
    }
    #endregion

    #region OrderUpdateModel 订单创建模型
    /// <summary>
    /// 订单创建模型
    /// </summary>
    public class OrderUpdateModel
    {
        [Required(ErrorMessage = "必填信息")]
        public string Id { get; set; }
        [StringLength(50, MinimumLength = 1, ErrorMessage = "长度必须为1-50个字符")]
        public string Name { get; set; }
        [StringLength(200, ErrorMessage = "长度必须为0-200个字符")]
        public string Description { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerPhone { get; set; }
        public List<OrderDetailCreateModel> Content { get; set; }
    }
    #endregion

    public class OrderCustomerUpdateModel
    {
        [Required(ErrorMessage = "必填信息")]
        public string OrderId { get; set; }
        [StringLength(50, MinimumLength = 1, ErrorMessage = "长度必须为1-50个字符")]
        public string Name { get; set; }
        [StringLength(200, ErrorMessage = "长度必须为0-200个字符")]
        public string Description { get; set; }
        public string Mail { get; set; }
        public string Telephone { get; set; }
        public string Cellphone { get; set; }
        public string Address { get; set; }
    }

    public class OrderDetailCreateModel
    {
        public string ProductSpecId { get; set; }
        public int Num { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string Remark { get; set; }
        public string Room { get; set; }
        public string Owner { get; set; }
    }

    public class CustomizedProduct
    {
        public string Name { get; set; }
        public string Icon { get; set; }
    }

    public class OrderDetailEditModel
    {
        [Required(ErrorMessage = "必填信息")]
        public string OrderId { get; set; }
        [Required(ErrorMessage = "必填信息")]
        public string Id { get; set; }
        public int Num { get; set; }
        public decimal TotalPrice { get; set; }
        public string Remark { get; set; }
        public string AttachIds { get; set; }
    }

    public class OrderWorkFlowAuditEditModel
    {
        [Required(ErrorMessage = "必填信息")]
        public string OrderId { get; set; }
        [Required(ErrorMessage = "必填信息")]
        public string WorkFlowItemId { get; set; }
        public bool Approve { get; set; }
        public string Remark { get; set; }
    }
}
