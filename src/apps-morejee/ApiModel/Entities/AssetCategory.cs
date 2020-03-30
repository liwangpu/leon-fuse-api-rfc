using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiModel.Entities
{
    public class AssetCategory : EntityBase, IListable, IDTOTransfer<AssetCategoryDTO>
    {
        public string Icon { get; set; }
        public string Type { get; set; }
        public string ParentId { get; set; }
        public int DisplayIndex { get; set; }
        public string CustomData { get; set; }
        public string Tag { get; set; }
        public bool Isolate { get; set; }
        public AssetCategoryDTO ToDTO()
        {
            AssetCategoryDTO dto = new AssetCategoryDTO();
            dto.Id = Id;
            dto.ParentId = ParentId;
            dto.Name = Name;
            dto.Value = Name;
            dto.Description = Description;
            dto.Icon = Icon;
            dto.Type = Type;
            dto.CustomData = CustomData;
            dto.Tag = Tag;
            dto.Isolate = Isolate;
            dto.DisplayIndex = DisplayIndex;
            dto.OrganizationId = OrganizationId;
            dto.Creator = Creator;
            dto.Modifier = Modifier;
            dto.CreatedTime = CreatedTime;
            dto.ModifiedTime = ModifiedTime;
            dto.CreatorName = CreatorName;
            dto.ModifierName = ModifierName;
            dto.IsRoot = string.IsNullOrWhiteSpace(ParentId);
            dto.Children = new List<AssetCategoryDTO>();
            return dto;
        }
    }

    public class AssetCategoryDTO : EntityBase
    {
        public string Value { get; set; }
        public string Icon { get; set; }
        public string ParentId { get; set; }
        public bool IsRoot { get; set; }
        /// <summary>
        /// 分类的类型，比如产品product, 材质material
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 在父级分类中的显示顺序，数值为0 - (childrencount - 1)
        /// </summary>
        public int DisplayIndex { get; set; }
        public string CustomData { get; set; }
        public string Tag { get; set; }
        public bool Isolate { get; set; }
        public List<AssetCategoryDTO> Children { get; set; }
    }

    public class AssetCategoryPack
    {
        public List<AssetCategoryDTO> Categories { get; set; }
    }
}
