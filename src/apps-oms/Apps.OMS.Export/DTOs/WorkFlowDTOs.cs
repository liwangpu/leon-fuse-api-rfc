using System;
using System.Collections.Generic;
using System.Text;

namespace Apps.OMS.Export.DTOs
{
    public class WorkFlowRuleDTO
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
        public bool IsInner { get; set; }
        public string WorkFlowId { get; set; }
    }

    public class WorkFlowDTO
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
        public string ApplyOrgans { get; set; }
        public List<WorkFlowItemDTO> WorkFlowItems { get; set; }
    }

    public class WorkFlowItemDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime ModifiedTime { get; set; }
        public string Creator { get; set; }
        public string Modifier { get; set; }
        public string SubWorkFlowId { get; set; }
        public string OperateRoles { get; set; }
        public int FlowGrade { get; set; }
        public bool AutoWorkFlow { get; set; }
    }

}
