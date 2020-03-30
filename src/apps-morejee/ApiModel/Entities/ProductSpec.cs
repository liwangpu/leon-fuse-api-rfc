using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
namespace ApiModel.Entities
{
    /// <summary>
    /// 产品规格信息
    /// </summary>
    public class ProductSpec : EntityBase, IListable, IDTOTransfer<ProductSpecDTO>
    {
        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get; set; }
        /// <summary>
        /// 图册ids(逗号分隔的FileAssetId)
        /// </summary>
        public string Album { get; set; }
        /// <summary>
        /// 模型材质依赖信息(格式为SpecMeshMap,请看下文)
        /// </summary>
        public string StaticMeshIds { get; set; }
        /// <summary>
        /// 零售价，单位为元
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// 合伙人价格|渠道价，单位为元
        /// </summary>
        public decimal PartnerPrice { get; set; }
        /// <summary>
        /// 进货价，单位为元
        /// </summary>
        public decimal PurchasePrice { get; set; }
        /// <summary>
        /// 第三方ID，此产品在供应商自己的系统比如ERP的ID
        /// </summary>
        public string TPID { get; set; }
        public string Components { get; set; }
        /// <summary>
        /// 产品规格增加 string Slots属性是为了所有的产品可以自定义吸附和对齐或者组装用的插槽，方便产品之间进行组装。
        /// </summary>
        public string Slots { get; set; }
        public string Color { get; set; }

        /// <summary>
        /// 所在产品的ID
        /// </summary>
        public string ProductId { get; set; }
        public Product Product { get; set; }

        /**************** DTO专用容器 ****************/
        [NotMapped]
        public string Brand { get; set; }
        [NotMapped]
        public List<FileAsset> AlbumAsset { get; set; }
        [NotMapped]
        public List<StaticMesh> StaticMeshAsset { get; set; }

        public ProductSpec()
        {
            AlbumAsset = new List<FileAsset>();
            StaticMeshAsset = new List<StaticMesh>();
        }

        public ProductSpecDTO ToDTO()
        {
            var dto = new ProductSpecDTO();
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
            dto.Price = Price;
            dto.PartnerPrice = PartnerPrice;
            dto.PurchasePrice = PurchasePrice;
            dto.TPID = TPID;
            dto.ProductId = ProductId;
            dto.CategoryName = CategoryName;
            dto.Components = Components;
            dto.Slots = Slots;
            dto.StaticMeshIds = StaticMeshIds;
            dto.Icon = IconFileAssetUrl;
            dto.IconAssetId = Icon;
            dto.Color = Color;
            dto.Brand = Brand;
            if (AlbumAsset != null && AlbumAsset.Count > 0)
                dto.Album = AlbumAsset.Select(x => x.ToDTO()).ToList();
            if (StaticMeshAsset != null && StaticMeshAsset.Count > 0)
                dto.StaticMeshes = StaticMeshAsset.Select(x => x.ToDTO()).ToList();
            return dto;
        }
    }

    /// <summary>
    /// 产品规格信息DTO
    /// </summary>
    public class ProductSpecDTO : EntityBase, IListable
    {
        public string Color { get; set; }
        public string IconAssetId { get; set; }
        public decimal Price { get; set; }
        public decimal PartnerPrice { get; set; }
        public decimal PurchasePrice { get; set; }
        public string TPID { get; set; }
        public string ProductId { get; set; }
        public string StaticMeshIds { get; set; }
        public List<StaticMeshDTO> StaticMeshes { get; set; }
        public List<FileAssetDTO> Album { get; set; }
        public string Icon { get; set; }
        public string Slots { get; set; }
        public string Components { get; set; }
        public string Brand { get; set; }
    }

    /// <summary>
    /// 上传static mesh文件自动生成/关联 产品&规格信息DTO
    /// </summary>
    public class ProductSpecAutoRelatedDTO
    {
        public string ProductId { get; set; }
        public string ProductSpecId { get; set; }
    }

    /// <summary>
    /// 产品规格-模型和材质文件依赖,对应ProductSpec.StaticMeshIds(仅用来序列化反序列化字符串信息,非数据持久类)
    /// </summary>
    public class SpecMeshMap
    {
        public SpecMeshMap()
        {
            Items = new List<SpecMeshMapItem>();
        }
        public List<SpecMeshMapItem> Items { get; set; }
    }

    /// <summary>
    /// 产品规格-模型和材质文件依赖信息(仅用来序列化反序列化字符串信息,非数据持久类)
    /// </summary>
    public class SpecMeshMapItem
    {
        public SpecMeshMapItem()
        {
            MaterialIds = new List<string>();
        }

        /// <summary>
        /// 模型文件id
        /// </summary>
        public string StaticMeshId { get; set; }
        /// <summary>
        /// 所依赖材质文件Ids
        /// </summary>
        public List<string> MaterialIds { get; set; }
        public string Components { get; set; }
    }
}
