using System.ComponentModel.DataAnnotations.Schema;
namespace ApiModel.Entities
{
    public class StaticMesh : ClientAssetEntity, IListable, IDTOTransfer<StaticMeshDTO>
    {
        public string FileAssetId { get; set; }
        public string Dependencies { get; set; }
        public string Properties { get; set; }
        public string SrcFileAssetId { get; set; }
        public string Color { get; set; }
        /// <summary>
        /// 字符型Materials,服务器不做处理,只是简单存储
        /// </summary>
        public string DyMaterials { get; set; }
        [NotMapped]
        public string FileAssetUrl { get; set; }
        [NotMapped]
        public FileAsset FileAsset { get; set; }

        public StaticMeshDTO ToDTO()
        {
            var dto = new StaticMeshDTO();
            dto.Id = Id;
            dto.Name = Name;
            dto.PackageName = PackageName;
            dto.Description = Description;
            dto.FileAssetId = FileAssetId;
            dto.Dependencies = Dependencies;
            dto.Properties = Properties;
            dto.UnCookedAssetId = UnCookedAssetId;
            dto.SrcFileAssetId = SrcFileAssetId;
            dto.OrganizationId = OrganizationId;
            dto.Creator = Creator;
            dto.Modifier = Modifier;
            dto.CreatedTime = CreatedTime;
            dto.ModifiedTime = ModifiedTime;
            dto.CreatorName = CreatorName;
            dto.ModifierName = ModifierName;
            dto.CategoryName = CategoryName;
            dto.Materials = DyMaterials;
            dto.Icon = IconFileAssetUrl;
            dto.IconAssetId = Icon;
            dto.FileAssetId = FileAssetId;
            dto.Url = FileAssetUrl;
            if (FileAsset != null)
                dto.FileAsset = FileAsset.ToDTO();
            dto.Color = Color;
            return dto;
        }
    }


    public class StaticMeshDTO : ClientAssetEntity
    {
        public string IconAssetId { get; set; }
        public string Dependencies { get; set; }
        public string Properties { get; set; }
        public string FileAssetId { get; set; }
        public string SrcFileAssetId { get; set; }
        public string Url { get; set; }
        public string Materials { get; set; }
        public string Color { get; set; }
        public FileAssetDTO FileAsset { get; set; }
    }
}
