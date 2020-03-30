using System;

namespace Apps.OMS.Export.DTOs
{
    public class MemberHierarchyParamDTO
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
        public decimal Rate { get; set; }
    }

    public class MemberHierarchyParamSettingDTO
    {
        public string MemberHierarchyParamId { get; set; }
        public decimal Rate { get; set; }
    }
}
