using System.ComponentModel.DataAnnotations.Schema;

namespace ApiModel.Entities
{
    public class AreaType : EntityBase, IListable, IDTOTransfer<AreaTypeDTO>
    {
        public string Icon { get; set; }

        public AreaTypeDTO ToDTO()
        {
            var dto = new AreaTypeDTO();
            dto.Id = Id;
            dto.Name = Name;
            dto.Description = Description;
            dto.OrganizationId = OrganizationId;
            dto.Creator = Creator;
            dto.Modifier = Modifier;
            dto.CreatedTime = CreatedTime;
            dto.ModifiedTime = ModifiedTime;
            dto.CreatorName = CreatorName;
            dto.ModifierName = ModifierName;
            dto.Icon = IconFileAssetUrl;
            dto.IconAssetId = Icon;
            return dto;
        }
    }

    public class AreaTypeDTO : EntityBase, IListable
    {
        public string IconAssetId { get; set; }
        public string Icon { get; set; }
        
    }
}
