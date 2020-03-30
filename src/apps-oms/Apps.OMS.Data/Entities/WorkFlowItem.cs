using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Apps.OMS.Data.Entities
{
    public class WorkFlowItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime ModifiedTime { get; set; }
        public string Creator { get; set; }
        public string Modifier { get; set; }
        public string WorkFlowId { get; set; }
        public WorkFlow WorkFlow { get; set; }
        public string SubWorkFlowId { get; set; }
        public string OperateRoles { get; set; }
        public int FlowGrade { get; set; }
        public bool AutoWorkFlow { get; set; }
    }
}
