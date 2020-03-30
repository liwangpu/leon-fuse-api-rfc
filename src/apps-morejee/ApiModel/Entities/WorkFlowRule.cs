using System.ComponentModel.DataAnnotations.Schema;

namespace ApiModel.Entities
{
    public class WorkFlowRule : EntityBase, IListable, IDTOTransfer<WorkFlowRuleDTO>
    {
        public bool IsInner { get; set; }
        public string Keyword { get; set; }
        public string Icon { get; set; }   

        public WorkFlowRuleDTO ToDTO()
        {
            var dto = new WorkFlowRuleDTO();
            dto.Id = Id;
            dto.Name = Name;
            dto.Description = Description;
            dto.ActiveFlag = ActiveFlag;
            dto.Creator = Creator;
            dto.Modifier = Modifier;
            dto.CreatedTime = CreatedTime;
            dto.ModifiedTime = ModifiedTime;
            dto.CreatorName = CreatorName;
            dto.ModifierName = ModifierName;
            dto.Keyword = Keyword;
            dto.IsInner = IsInner;
            return dto;
        }
    }

    public class WorkFlowRuleDTO : EntityBase, IListable
    {
        public bool IsInner { get; set; }
        public string Keyword { get; set; }
        public string Icon { get; set; }
        
        public string WorkFlowName { get; set; }
    }
}
