using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiModel.Entities
{
    public class WorkFlow : EntityBase, IListable, IDTOTransfer<WorkFlowDTO>
    {
        public string Icon { get; set; }
        /// <summary>
        /// 适用组织类型,逗号分隔
        /// </summary>
        public string ApplyOrgans { get; set; }
        public List<WorkFlowItem> WorkFlowItems { get; set; }

        public WorkFlowDTO ToDTO()
        {
            var dto = new WorkFlowDTO();
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
            dto.ApplyOrgans = ApplyOrgans;
            if (WorkFlowItems != null && WorkFlowItems.Count > 0)
                dto.WorkFlowItems = WorkFlowItems;
            else
                dto.WorkFlowItems = new List<WorkFlowItem>();
            return dto;
        }
    }

    public class WorkFlowDTO : EntityBase, IListable
    {
        public string Icon { get; set; }
        public string ApplyOrgans { get; set; }
        
        public List<WorkFlowItem> WorkFlowItems { get; set; }
    }
}
