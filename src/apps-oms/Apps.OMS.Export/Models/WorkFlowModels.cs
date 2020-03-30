using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Apps.OMS.Export.Models
{
    public class WorkFlowCreateModel
    {
        [StringLength(50, MinimumLength = 1, ErrorMessage = "长度必须为1-50个字符")]
        public string Name { get; set; }
        public string Description { get; set; }
        public string IconAssetId { get; set; }
        public string ApplyOrgans { get; set; }
    }


    public class WorkFlowUpdateModel
    {
        [Required(ErrorMessage = "必填信息")]
        public string Id { get; set; }
        [StringLength(50, MinimumLength = 1, ErrorMessage = "长度必须为1-50个字符")]
        public string Name { get; set; }
        public string Description { get; set; }
        public string IconAssetId { get; set; }
        public string ApplyOrgans { get; set; }
    }

    public class WorkFlowItemEditModel
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "必填信息")]
        public string WorkFlowId { get; set; }
        [StringLength(50, MinimumLength = 1, ErrorMessage = "长度必须为1-50个字符")]
        public string Name { get; set; }
        public string Description { get; set; }
        public string SubWorkFlowId { get; set; }
        public string OperateRoles { get; set; }
        public int FlowGrade { get; set; }
        public bool AutoWorkFlow { get; set; }
    }

    public class WorkFlowItemDeleteModel
    {
        [Required(ErrorMessage = "必填信息")]
        public string Id { get; set; }
        [Required(ErrorMessage = "必填信息")]
        public string WorkFlowId { get; set; }
    }

    public class WorkFlowRuleDetailUpdateModel
    {
        [Required(ErrorMessage = "必填信息")]
        public string WorkFlowRuleId { get; set; }
        [Required(ErrorMessage = "必填信息")]
        public string WorkFlowId { get; set; }
    }
}
