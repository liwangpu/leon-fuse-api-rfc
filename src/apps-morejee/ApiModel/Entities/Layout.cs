using System.ComponentModel.DataAnnotations.Schema;

namespace ApiModel.Entities
{
    public class Layout : EntityBase, IAsset, IDTOTransfer<LayoutDTO>
    {
        public string Icon { get; set; }
        /// <summary>
        /// 户型数据,内容为类LayoutData的Json字符串。
        /// </summary>
        public string Data { get; set; }
        public string Color { get; set; }
        public LayoutDTO ToDTO()
        {
            var dto = new LayoutDTO();
            dto.Id = Id;
            dto.Name = Name;
            dto.Description = Description;
            dto.CategoryId = CategoryId;
            dto.OrganizationId = OrganizationId;
            dto.Creator = Creator;
            dto.Modifier = Modifier;
            dto.CreatedTime = CreatedTime;
            dto.ModifiedTime = ModifiedTime;
            dto.CreatorName = CreatorName;
            dto.ModifierName = ModifierName;
            dto.Data = Data;
            dto.CategoryId = CategoryId;
            dto.CategoryName = CategoryName;
            dto.Icon = IconFileAssetUrl;
            dto.IconAssetId = Icon;
            dto.Color = Color;
            return dto;
        }
    }

    public class LayoutDTO : EntityBase, IListable
    {
        public string IconAssetId { get; set; }
        public string Data { get; set; }
        public string Color { get; set; }
        public string Icon { get; set; }
    }
}
