using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiModel.Entities
{
    public class Department : EntityBase, IListable, IDTOTransfer<DepartmentDTO>
    {
        public string ParentId { get; set; }
        public Department Parent { get; set; }
        public Organization Organization { get; set; }
        public string Icon { get; set; }  

        public DepartmentDTO ToDTO()
        {
            var dto = new DepartmentDTO();
            dto.Id = Id;
            dto.Name = Name;
            dto.Description = Description;
            dto.OrganizationId = OrganizationId;
            dto.ParentId = ParentId;
            dto.Creator = Creator;
            dto.Modifier = Modifier;
            dto.CreatedTime = CreatedTime;
            dto.ModifiedTime = ModifiedTime;
            dto.CreatorName = CreatorName;
            dto.ModifierName = ModifierName;
            return dto;
        }
    }


    public class DepartmentDTO :EntityBase,IListable
    {
        public string ParentId { get; set; }
        public string Icon { get; set; }
        
    }
}
