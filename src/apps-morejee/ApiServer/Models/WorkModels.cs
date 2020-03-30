using System.ComponentModel.DataAnnotations;

namespace ApiServer.Models
{
    public class WorkFlowCreateModel
    {
        [StringLength(50, MinimumLength = 1, ErrorMessage = "长度必须为1-50个字符")]
        public string Name { get; set; }
        public string Description { get; set; }
        public string IconAssetId { get; set; }
        public string ApplyOrgans { get; set; }
    }

    public class WorkFlowEditModel
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

    public class WorkFlowRuleCreateModel
    {
        [StringLength(50, MinimumLength = 1, ErrorMessage = "长度必须为1-50个字符")]
        public string Name { get; set; }
        public string Description { get; set; }
        public string Keyword { get; set; }
    }

    public class WorkFlowRuleEditModel
    {
        [Required(ErrorMessage = "必填信息")]
        public string Id { get; set; }
        [StringLength(50, MinimumLength = 1, ErrorMessage = "长度必须为1-50个字符")]
        public string Name { get; set; }
        public string Description { get; set; }
        public string Keyword { get; set; }
    }

    public class WorkFlowRuleDefineModel
    {
        [Required(ErrorMessage = "必填信息")]
        public string Keyword { get; set; }
        [Required(ErrorMessage = "必填信息")]
        public string WorkFlowId { get; set; }
    }
}
