using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Linq;
namespace ApiModel.Entities
{
    public class ProductReplaceGroup : EntityBase, IListable, IDTOTransfer<ProductReplaceGroupDTO>
    {
        public string Icon { get; set; }
        public string DefaultItemId { get; set; }
        public string GroupItemIds { get; set; }

        [NotMapped]
        public Product DefaultItem { get; set; }
        [NotMapped]
        public List<Product> GroupItems { get; set; }

        public ProductReplaceGroupDTO ToDTO()
        {
            var dto = new ProductReplaceGroupDTO();
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
            dto.CategoryId = CategoryId;
            dto.CategoryName = CategoryName;
            dto.DefaultItemId = DefaultItemId;
            dto.GroupItemIds = GroupItemIds;

            dto.Icon = IconFileAssetUrl;
            dto.IconAssetId = Icon;
            if (DefaultItem != null)
                dto.DefaultItem = DefaultItem.ToDTO();

            if (GroupItems != null && GroupItems.Count > 0)
                dto.GroupItems = GroupItems.Select(x => x.ToDTO()).ToList();


            return dto;
        }
    }


    public class ProductReplaceGroupDTO : EntityBase,IListable
    {
        public string IconAssetId { get; set; }
        public string DefaultItemId { get; set; }
        public string GroupItemIds { get; set; }
        public ProductDTO DefaultItem { get; set; }
        public List<ProductDTO> GroupItems { get; set; }
        public string Icon { get; set; }
        
    }

}
