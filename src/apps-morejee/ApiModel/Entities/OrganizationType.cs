using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiModel.Entities
{
    public class OrganizationType : EntityBase, IListable, IDTOTransfer<OrganizationTypeDTO>
    {
        public string Icon { get; set; }
        public bool IsInner { get; set; }
        public OrganizationTypeDTO ToDTO()
        {
            var dto = new OrganizationTypeDTO();
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
            dto.IsInner = IsInner;
            return dto;
        }
    }

    public class OrganizationTypeDTO : EntityBase, IListable
    {
        public string Icon { get; set; }
        public string ApplyOrgans { get; set; }
        public bool IsInner { get; set; }
    }

}
