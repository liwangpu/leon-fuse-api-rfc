using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiModel.Entities
{
    public class Material : ClientAssetEntity, IAsset, IListable, IDTOTransfer<MaterialDTO>
    {
        public string FileAssetId { get; set; }
        public string Dependencies { get; set; }
        public string Parameters { get; set; }
        public string Color { get; set; }
        [NotMapped]
        public FileAsset FileAsset { get; set; }
        [JsonIgnore]
        [NotMapped]
        public string FileAssetUrl { get; set; }
        [NotMapped]
        public AssetCategory AssetCategory { get; set; }

        public MaterialDTO ToDTO()
        {
            var dto = new MaterialDTO();
            dto.Id = Id;
            dto.Name = Name;
            dto.PackageName = PackageName;
            dto.Description = Description;
            dto.FileAssetId = FileAssetId;
            dto.Dependencies = Dependencies;
            dto.Parameters = Parameters;
            dto.UnCookedAssetId = UnCookedAssetId;
            dto.OrganizationId = OrganizationId;
            dto.FolderName = FolderName;
            dto.Creator = Creator;
            dto.Modifier = Modifier;
            dto.CreatedTime = CreatedTime;
            dto.ModifiedTime = ModifiedTime;
            dto.CreatorName = CreatorName;
            dto.ModifierName = ModifierName;
            dto.CategoryName = CategoryName;
            dto.Url = FileAssetUrl;
            dto.Icon = IconFileAssetUrl;
            dto.IconAssetId = Icon;
            if (FileAsset != null)
                dto.FileAsset = FileAsset.ToDTO();
            if (AssetCategory != null)
                dto.CategoryName = AssetCategory.Name;
            dto.CategoryId = CategoryId;
            dto.Color = Color;
            return dto;
        }
    }

    public class MaterialDTO : ClientAssetEntity
    {
        public string Color { get; set; }
        public string IconAssetId { get; set; }
        public string FileAssetId { get; set; }
        public string Dependencies { get; set; }
        public string Parameters { get; set; }
        public string Url { get; set; }
        public FileAssetDTO FileAsset { get; set; }
    }
}
