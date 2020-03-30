using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiModel.Entities
{
    #region Package 套餐
    /// <summary>
    /// 套餐
    /// </summary>
    public class Package : EntityBase, IListable, IDTOTransfer<PackageDTO>
    {
        /// <summary>
        /// 图标Asset Id
        /// </summary>
        public string Icon { get; set; }
        /// <summary>
        /// 套餐内容, 内容为PackageContent对象的json字符串
        /// </summary>
        public string Content { get; set; }
        public string Color { get; set; }
        [NotMapped]
        public PackageContent ContentIns { get; set; }
        #region ToDTO
        public PackageDTO ToDTO()
        {
            var dto = new PackageDTO();
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
            dto.Content = Content;
            dto.CategoryName = CategoryName;
            dto.ResourceType = ResourceType;
            if (ContentIns != null)
                dto.ContentIns = ContentIns;
            dto.Icon = IconFileAssetUrl;
            dto.IconAssetId = Icon;
            dto.Color = Color;
            return dto;
        }
        #endregion
    }
    #endregion

    #region PackageDTO 套餐DTO
    /// <summary>
    /// 套餐DTO
    /// </summary>
    public class PackageDTO : EntityBase,IListable
    {
        public string IconAssetId { get; set; }
        public string State { get; set; }
        public string Content { get; set; }
        public PackageContent ContentIns { get; set; }
        public string Icon { get; set; }
        public string Color { get; set; }
    }
    #endregion

    #region PackageContent 套餐内容(仅供序列化使用,非数据库实体)
    /// <summary>
    /// 套餐内容(仅供序列化使用,非数据库实体)
    /// </summary>
    public class PackageContent
    {
        public List<PackageArea> Areas { get; set; }
        public List<PackageContentItem> Items { get; set; }
        public decimal TotalPrice { get; set; }
        public string Remark { get; set; }
        public List<string> ReplaceGroups { get; set; } //替换组，每个组里面是一堆家具， 如果以此套餐来限定方案的话，方案里创建和替换物品时就只能在替换组里面选择。
        public List<ProductReplaceGroupDTO> ReplaceGroupIns { get; set; }
    }
    #endregion

    #region PackageContentItem 套餐项(仅供序列化使用,非数据库实体)
    /// <summary>
    /// 套餐项(仅供序列化使用,非数据库实体)
    /// </summary>
    public class PackageContentItem
    {
        public string ProductSpecId { get; set; }
        public int Num { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string Remark { get; set; }
        public string ProductName { get; set; }
        public string ProductSpecName { get; set; }
    }
    #endregion

    #region PackageProductSet 套餐中的产品组，这一堆产品是一个组合。暂时无用
    /// <summary>
    /// 套餐中的产品组，这一堆产品是一个组合。暂时无用
    /// </summary>
    public class PackageProductSet
    {
        public string DefaultId { get; set; } //默认的产品ID
        public List<string> Products { get; set; } //产品ID集合，这一堆产品里面可以互相替换
    }
    #endregion

    public class PackageMaterial
    {
        public string ActorName { get; set; }
        public string LastActorName { get; set; }
        public string Icon { get; set; }
        public string MaterialId { get; set; }
    }

    #region PackageArea 套餐的区域配置，配置套餐中的一个区域（狭义的区域也可以理解为房间）
    /// <summary>
    /// 套餐的区域配置，配置套餐中的一个区域（狭义的区域也可以理解为房间）
    /// </summary>
    public class PackageArea
    {
        public string Id { get; set; }
        public string AreaTypeId { get; set; } //区域类型的ID
        public string AreaAlias { get; set; } //别名，如果有两个或以上的同类型区域，可以通过别名来区别。比如三个卧室的情况

        public Dictionary<string, string> GroupsMap { get; set; } //<GroupName, GroupId>物品组， 每个名字的组位应该用哪一个物品组
        public Dictionary<string, string> ProductCategoryMap { get; set; } //<ProductCategoryId, ProductId>. 产品的按分类指定数据， 这个区域里面 单人沙发用的哪一款， 茶几用的哪一款。
        public Dictionary<string, string> Materials { get; set; } //<ActorName, MaterialId> 墙体，地面，天花等模型的材质. ActorName是场景中actor的名称，可以不写精确的名称只写一部分。比如wall, wall-main, wall-south之类的。wall1,wall2就都会匹配上


        public List<ProductGroupDTO> GroupsMapIns { get; set; }
        public List<ProductDTO> ProductCategoryMapIns { get; set; }
        public List<PackageMaterial> MaterialIns { get; set; }

    }
    #endregion


}
