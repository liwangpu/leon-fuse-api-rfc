using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
namespace ApiModel.Entities
{
    public class Product : EntityBase, IAsset, IDTOTransfer<ProductDTO>
    {
        public string Icon { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }
        /// <summary>
        /// 是否已经发布
        /// </summary>
        public bool IsPublish { get; set; }
        public string Color { get; set; }
        public string Brand { get; set; }
        /// <summary>
        /// 规格
        /// </summary>
        public List<ProductSpec> Specifications { get; set; }

        [NotMapped]
        public AssetCategory AssetCategory { get; set; }

        public ProductDTO ToDTO()
        {
            var dto = new ProductDTO();
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
            dto.CategoryName = CategoryName;
            dto.Unit = Unit;
            dto.IsPublish = IsPublish;
            dto.Color = Color;
            dto.Brand = Brand;
            if (Specifications != null && Specifications.Count > 0)
            {
                var defaultSpec = Specifications.OrderByDescending(x => x.CreatedTime).First();
                dto.Price = defaultSpec.Price;
                dto.PartnerPrice = defaultSpec.PartnerPrice;
                dto.PurchasePrice = defaultSpec.PurchasePrice;
                dto.Specifications = Specifications.Select(x => x.ToDTO()).ToList();
                dto.TPID = defaultSpec.TPID;
            }
            dto.Icon = IconFileAssetUrl;
            dto.IconAssetId = Icon;
            if (AssetCategory != null)
                dto.CategoryName = AssetCategory.Name;
            return dto;
        }
    }

    public class ProductDTO : EntityBase, IListable
    {
        public string Color { get; set; }
        public string Brand { get; set; }
        public string IconAssetId { get; set; }
        public decimal Price { get; set; }
        public decimal PartnerPrice { get; set; }
        public decimal PurchasePrice { get; set; }
        public List<ProductSpecDTO> Specifications { get; set; }
        public string Icon { get; set; }
        public string Unit { get; set; }
        public bool IsPublish { get; set; }
        public string TPID { get; set; }
    }

}
