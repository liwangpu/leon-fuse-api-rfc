using System.ComponentModel.DataAnnotations.Schema;

namespace ApiModel.Entities
{
    #region ProductGroup 产品组
    /// <summary>
    /// 产品组
    /// </summary>
    public class ProductGroup : EntityBase, IListable, IDTOTransfer<ProductGroupDTO>
    {
        public string Serie { get; set; }
        public string Icon { get; set; }
        /// <summary>
        /// 轴心的精确位置
        /// </summary>
        public string PivotLocation { get; set; }
        /// <summary>
        /// 轴心九个点位枚举，TopLeft, TopCenter, TopRight, CenterLeft, Center, CenterRight, BottomLeft...
        /// </summary>
        public int PivotType { get; set; }
        /// <summary>
        /// 朝向的枚举， Forward, Back, Left, Right.  前后左右。 actor的正前方位Forward
        /// </summary>
        public int Orientation { get; set; }
        public string Color { get; set; }
        /// <summary>
        /// 结构为 List<ProductGroupItem>
        /// </summary>
        public string Items { get; set; }

        [NotMapped]
        public AssetCategory AssetCategory { get; set; }

        public ProductGroupDTO ToDTO()
        {
            var dto = new ProductGroupDTO();
            dto.Id = Id;
            dto.Name = Name;
            dto.Description = Description;
            dto.PivotLocation = PivotLocation;
            dto.PivotType = PivotType;
            dto.Orientation = Orientation;
            dto.Serie = Serie;
            dto.Items = Items;
            dto.OrganizationId = OrganizationId;
            dto.CategoryId = CategoryId;
            dto.Creator = Creator;
            dto.Modifier = Modifier;
            dto.CreatedTime = CreatedTime;
            dto.ModifiedTime = ModifiedTime;
            dto.CreatorName = CreatorName;
            dto.ModifierName = ModifierName;
            dto.ResourceType = ResourceType;
            dto.Icon = IconFileAssetUrl;
            dto.IconAssetId = Icon;
            dto.Color = Color;
            if (AssetCategory != null)
                dto.CategoryName = AssetCategory.Name;
            return dto;
        }
    }
    #endregion

    #region ProductGroupItem 产品组详细组成项
    /// <summary>
    /// 产品组详细组成项
    /// </summary>
    public class ProductGroupItem : EntityBase, IListable
    {
        public string Icon { get; set; }
        [NotMapped]
        
        public int No { get; set; }
        public string ProductId { get; set; }
        public string Rotation { get; set; }
        public string Location { get; set; }
        public string Parent { get; set; }
    }
    #endregion

    #region ProductGroupDTO 产品组DTO
    /// <summary>
    /// 产品组DTO
    /// </summary>
    public class ProductGroupDTO : EntityBase,IListable
    {
        public string Serie { get; set; }
        public string IconAssetId { get; set; }
        public string Items { get; set; }
        public string PivotLocation { get; set; }
        public int PivotType { get; set; }
        public int Orientation { get; set; }
        public string Icon { get; set; }
        public string Color { get; set; }
    }
    #endregion

}
