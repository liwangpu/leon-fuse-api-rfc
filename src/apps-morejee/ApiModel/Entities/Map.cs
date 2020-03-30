using System.ComponentModel.DataAnnotations.Schema;

namespace ApiModel.Entities
{
    public class Map : ClientAssetEntity, IListable, IDTOTransfer<MapDTO>
    {
        public string FileAssetId { get; set; }
        public string Dependencies { get; set; }
        public string Properties { get; set; }
        public string Color { get; set; }
        [NotMapped]
        public string FileAssetUrl { get; set; }
        [NotMapped]
        public FileAsset FileAsset { get; set; }

        public MapDTO ToDTO()
        {
            var dto = new MapDTO();
            dto.Id = Id;
            dto.Name = Name;
            dto.PackageName = PackageName;
            dto.Dependencies = Dependencies;
            dto.Properties = Properties;
            dto.Description = Description;
            dto.OrganizationId = OrganizationId;
            dto.Creator = Creator;
            dto.Modifier = Modifier;
            dto.CreatedTime = CreatedTime;
            dto.ModifiedTime = ModifiedTime;
            dto.CreatorName = CreatorName;
            dto.ModifierName = ModifierName;
            dto.UnCookedAssetId = UnCookedAssetId;
            dto.CategoryName = CategoryName;
            dto.Icon = IconFileAssetUrl;
            dto.IconAssetId = Icon;
            dto.FileAssetId = FileAssetId;
            dto.FileAssetUrl = FileAssetUrl;
            if (FileAsset != null)
                dto.FileAsset = FileAsset.ToDTO();
            dto.Color = Color;
            return dto;
        }
    }

    public class MapDTO : ClientAssetEntity
    {
        public string Color { get; set; }
        public string IconAssetId { get; set; }
        public string FileAssetId { get; set; }
        public string Dependencies { get; set; }
        public string Properties { get; set; }
        public string FileAssetUrl { get; set; }
        public FileAssetDTO FileAsset { get; set; }
    }


}
