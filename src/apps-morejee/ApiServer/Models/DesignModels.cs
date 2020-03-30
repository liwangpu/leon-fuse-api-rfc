using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ApiServer.Models
{
    #region ProductCreateModel 产品新建模型
    /// <summary>
    /// 产品新建模型
    /// </summary>
    public class ProductCreateModel
    {
        [StringLength(50, MinimumLength = 1, ErrorMessage = "长度必须为1-50个字符")]
        public string Name { get; set; }
        [StringLength(200, ErrorMessage = "长度必须为0-200个字符")]
        public string Description { get; set; }
        public string Unit { get; set; }
        public string FolderId { get; set; }
        public string CategoryId { get; set; }
        public string IconAssetId { get; set; }
        public bool IsPublish { get; set; }
        public string Color { get; set; }
        public string Brand { get; set; }
    }
    #endregion

    #region ProductEditModel 产品编辑模型
    /// <summary>
    /// 产品编辑模型
    /// </summary>
    public class ProductEditModel
    {
        [Required(ErrorMessage = "必填信息")]
        public string Id { get; set; }
        [StringLength(50, MinimumLength = 1, ErrorMessage = "长度必须为1-50个字符")]
        public string Name { get; set; }
        [StringLength(200, ErrorMessage = "长度必须为0-200个字符")]
        public string Description { get; set; }
        public string Unit { get; set; }
        public string FolderId { get; set; }
        public string CategoryId { get; set; }
        public string IconAssetId { get; set; }
        public bool IsPublish { get; set; }
        public string Color { get; set; }
        public string Brand { get; set; }
    }
    #endregion

    #region ProductSpecCreateModel 产品规格新建模型
    /// <summary>
    /// 产品规格新建模型
    /// </summary>
    public class ProductSpecCreateModel
    {
        [StringLength(50, MinimumLength = 1, ErrorMessage = "长度必须为1-50个字符")]
        public string Name { get; set; }
        [StringLength(200, ErrorMessage = "长度必须为0-200个字符")]
        public string Description { get; set; }
        [Required(ErrorMessage = "必填信息")]
        public string ProductId { get; set; }
        public string CategoryId { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "价格信息有误")]
        public decimal Price { get; set; }
        public decimal PartnerPrice { get; set; }
        public decimal PurchasePrice { get; set; }
        public string IconAssetId { get; set; }
        public string Components { get; set; }
        public string Slots { get; set; }
        public string StaticMeshIds { get; set; }
        public string Color { get; set; }
    }
    #endregion

    #region ProductSpecEditModel 产品规格编辑模型
    /// <summary>
    /// 产品规格编辑模型
    /// </summary>
    public class ProductSpecEditModel
    {
        [Required(ErrorMessage = "必填信息")]
        public string Id { get; set; }
        [StringLength(50, MinimumLength = 1, ErrorMessage = "长度必须为1-50个字符")]
        public string Name { get; set; }
        [StringLength(200, ErrorMessage = "长度必须为0-200个字符")]
        public string Description { get; set; }
        public string ProductId { get; set; }
        public string CategoryId { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "价格信息有误")]
        public decimal Price { get; set; }
        public decimal PartnerPrice { get; set; }
        public decimal PurchasePrice { get; set; }
        public string IconAssetId { get; set; }
        public string Components { get; set; }
        public string Slots { get; set; }
        public string StaticMeshIds { get; set; }
        public string Color { get; set; }
        public string TPID { get; set; }
    }
    #endregion

    #region BulkChangeCategoryModel 批量修改分类模型
    /// <summary>
    /// 批量修改分类模型
    /// </summary>
    public class BulkChangeCategoryModel
    {
        [Required(ErrorMessage = "必填信息")]
        public string Ids { get; set; }
        [Required(ErrorMessage = "必填信息")]
        public string CategoryId { get; set; }
    }
    #endregion

    #region ProductSpecAutoRelatedEditModel 根据规格自动创建或者关联产品规格编辑模型
    /// <summary>
    /// 根据规格自动创建或者关联产品规格编辑模型
    /// </summary>
    public class ProductSpecAutoRelatedEditModel
    {
        [Required(ErrorMessage = "必填信息")]
        public string StaticMeshId { get; set; }
        public string ProductId { get; set; }
        public string Brand { get; set; }
    }
    #endregion

    #region SolutionCreateModel 解决方案新建模型
    /// <summary>
    /// 解决方案新建模型
    /// </summary>
    public class SolutionCreateModel
    {
        [StringLength(50, MinimumLength = 1, ErrorMessage = "长度必须为1-50个字符")]
        public string Name { get; set; }
        [StringLength(200, ErrorMessage = "长度必须为0-200个字符")]
        public string Description { get; set; }
        public string Data { get; set; }
        public string CategoryId { get; set; }
        public string LayoutId { get; set; }
        public string IconAssetId { get; set; }
        public bool IsSnapshot { get; set; }
        public string SnapshotData { get; set; }
        public string Color { get; set; }
    }
    #endregion

    #region SolutionEditModel 解决方案编辑模型
    /// <summary>
    /// 解决方案编辑模型
    /// </summary>
    public class SolutionEditModel
    {
        [Required(ErrorMessage = "必填信息")]
        public string Id { get; set; }
        [StringLength(50, MinimumLength = 1, ErrorMessage = "长度必须为1-50个字符")]
        public string Name { get; set; }
        [StringLength(200, ErrorMessage = "长度必须为0-200个字符")]
        public string Description { get; set; }
        public string Data { get; set; }
        public string CategoryId { get; set; }
        public string LayoutId { get; set; }
        public string IconAssetId { get; set; }
        public bool IsSnapshot { get; set; }
        public string SnapshotData { get; set; }
        public string Color { get; set; }
    }
    #endregion

    public class SolutionSnapshotEditModel
    {
        public string SolutionId { get; set; }
        public string SnapshotData { get; set; }
    }

    #region MaterialCreateModel 材质创建模型
    /// <summary>
    /// 材质创建模型
    /// </summary>
    public class MaterialCreateModel : ClientAssetCreateModel
    {
        public string Dependencies { get; set; }
        public string Parameters { get; set; }
        public string CategoryId { get; set; }
        public string IconAssetId { get; set; }
        public string Color { get; set; }
    }
    #endregion

    #region MaterialEditModel 材质编辑模型
    /// <summary>
    /// 材质编辑模型
    /// </summary>
    public class MaterialEditModel : ClientAssetEditModel
    {
        public string Icon { get; set; }
        public string Dependencies { get; set; }
        public string Parameters { get; set; }
        public string CategoryId { get; set; }
        public string IconAssetId { get; set; }
        public string Color { get; set; }
    }
    #endregion


    public class OrderDetailCreateModel
    {
        public string ProductSpecId { get; set; }
        public int Num { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string Remark { get; set; }
    }

    #region OrderCreateModel 订单创建模型
    /// <summary>
    /// 订单创建模型
    /// </summary>
    public class OrderCreateModel
    {
        [StringLength(50, MinimumLength = 1, ErrorMessage = "长度必须为1-50个字符")]
        public string Name { get; set; }
        [StringLength(200, ErrorMessage = "长度必须为0-200个字符")]
        public string Description { get; set; }

        public List<OrderDetailCreateModel> Content { get; set; }
        ////[Required(ErrorMessage = "必填信息")]
        //public string Content { get; set; }
        //public string State { get; set; }
        //public string IconAssetId { get; set; }
        //public DateTime StateTime { get; set; }
    }
    #endregion

    #region OrderEditModel 订单创建模型
    /// <summary>
    /// 订单创建模型
    /// </summary>
    public class OrderEditModel
    {
        [Required(ErrorMessage = "必填信息")]
        public string Id { get; set; }
        [StringLength(50, MinimumLength = 1, ErrorMessage = "长度必须为1-50个字符")]
        public string Name { get; set; }
        [StringLength(200, ErrorMessage = "长度必须为0-200个字符")]
        public string Description { get; set; }
        //[Required(ErrorMessage = "必填信息")]
        public string Content { get; set; }
        public string State { get; set; }
        public string IconAssetId { get; set; }
        public DateTime StateTime { get; set; }
    }
    #endregion

    public class OrderWorkFlowAuditEditModel
    {
        [Required(ErrorMessage = "必填信息")]
        public string OrderId { get; set; }
        [Required(ErrorMessage = "必填信息")]
        public string WorkFlowItemId { get; set; }
        public bool Approve { get; set; }
        public string Remark { get; set; }
    }

    public class OrderCustomerEditModel
    {
        [Required(ErrorMessage = "必填信息")]
        public string OrderId { get; set; }
        [Required(ErrorMessage = "必填信息")]
        public string CustomerName { get; set; }
        [Required(ErrorMessage = "必填信息")]
        public string CustomerPhone { get; set; }
        [Required(ErrorMessage = "必填信息")]
        public string CustomerAddress { get; set; }
    }

    public class OrderDetailEditModel
    {
        [Required(ErrorMessage = "必填信息")]
        public string OrderId { get; set; }
        [Required(ErrorMessage = "必填信息")]
        public string Id { get; set; }
        public int Num { get; set; }
        public decimal TotalPrice { get; set; }
        public string Remark { get; set; }
        public string AttachIds { get; set; }
    }

    #region PackageCreateModel 套餐创建模型
    /// <summary>
    /// 套餐创建模型
    /// </summary>
    public class PackageCreateModel
    {
        [StringLength(50, MinimumLength = 1, ErrorMessage = "长度必须为1-50个字符")]
        public string Name { get; set; }
        [StringLength(200, ErrorMessage = "长度必须为0-200个字符")]
        public string Description { get; set; }
        public string Content { get; set; }
        public string State { get; set; }
        public string IconAssetId { get; set; }
        public DateTime StateTime { get; set; }
        public string Color { get; set; }
    }
    #endregion

    #region PackageEditModel 套餐编辑模型
    /// <summary>
    /// 套餐编辑模型
    /// </summary>
    public class PackageEditModel
    {
        [Required(ErrorMessage = "必填信息")]
        public string Id { get; set; }
        [StringLength(50, MinimumLength = 1, ErrorMessage = "长度必须为1-50个字符")]
        public string Name { get; set; }
        [StringLength(200, ErrorMessage = "长度必须为0-200个字符")]
        public string Description { get; set; }
        public string Content { get; set; }
        public string State { get; set; }
        public string IconAssetId { get; set; }
        public DateTime StateTime { get; set; }
        public string Color { get; set; }
    }
    #endregion

    #region PackageAreaTypeEditModel 套餐区域编辑模型
    /// <summary>
    /// 套餐区域编辑模型
    /// </summary>
    public class PackageAreaTypeEditModel
    {
        [Required(ErrorMessage = "必填信息")]
        public string PackageId { get; set; }
        public string Id { get; set; }
        public string AreaTypeId { get; set; }
        public string AreaAlias { get; set; }
    }
    #endregion

    #region PackageAreaTypeDeleteModel 套餐区域删除模型
    /// <summary>
    /// 套餐区域删除模型
    /// </summary>
    public class PackageAreaTypeDeleteModel
    {
        [Required(ErrorMessage = "必填信息")]
        public string PackageId { get; set; }
        [Required(ErrorMessage = "必填信息")]
        public string Id { get; set; }
    }
    #endregion

    #region PackageProductGroupCreateModel 套餐产品组创建模型
    /// <summary>
    /// 套餐产品组创建模型
    /// </summary>
    public class PackageProductGroupCreateModel
    {
        [Required(ErrorMessage = "必填信息")]
        public string PackageId { get; set; }
        [Required(ErrorMessage = "必填信息")]
        public string AreaId { get; set; }
        [Required(ErrorMessage = "必填信息")]
        public string ProductGroupId { get; set; }
    }
    #endregion

    #region PackageProductGroupDeleteModel 套餐详情产品组删除模型
    /// <summary>
    /// 套餐详情产品组删除模型
    /// </summary>
    public class PackageProductGroupDeleteModel
    {
        [Required(ErrorMessage = "必填信息")]
        public string PackageId { get; set; }
        [Required(ErrorMessage = "必填信息")]
        public string AreaId { get; set; }
        [Required(ErrorMessage = "必填信息")]
        public string ProductGroupId { get; set; }
    }
    #endregion

    #region PackageCategoryProductCreateModel 套餐详情分类产品创建模型
    /// <summary>
    /// 套餐详情分类产品创建模型
    /// </summary>
    public class PackageCategoryProductCreateModel
    {
        [Required(ErrorMessage = "必填信息")]
        public string PackageId { get; set; }
        [Required(ErrorMessage = "必填信息")]
        public string AreaId { get; set; }
        [Required(ErrorMessage = "必填信息")]
        public string ProductId { get; set; }
    }
    #endregion

    #region PackageCategoryProductDeleteModel 套餐详情分类产品删除模型
    /// <summary>
    /// 套餐详情分类产品删除模型
    /// </summary>
    public class PackageCategoryProductDeleteModel
    {
        [Required(ErrorMessage = "必填信息")]
        public string PackageId { get; set; }
        [Required(ErrorMessage = "必填信息")]
        public string AreaId { get; set; }
        [Required(ErrorMessage = "必填信息")]
        public string ProductId { get; set; }
    }
    #endregion

    #region PackageMaterialCreateModel 套餐详细材质创建模型
    /// <summary>
    /// 套餐详细材质创建模型
    /// </summary>
    public class PackageMaterialCreateModel
    {
        public string LastActorName { get; set; }
        public string ActorName { get; set; }
        [Required(ErrorMessage = "必填信息")]
        public string PackageId { get; set; }
        [Required(ErrorMessage = "必填信息")]
        public string AreaId { get; set; }
        [Required(ErrorMessage = "必填信息")]
        public string MaterialId { get; set; }
    }
    #endregion

    #region PackageMaterialDeleteModel 套餐详情材质删除模型
    /// <summary>
    /// 套餐详情材质删除模型
    /// </summary>
    public class PackageMaterialDeleteModel
    {
        [Required(ErrorMessage = "必填信息")]
        public string LastActorName { get; set; }
        [Required(ErrorMessage = "必填信息")]
        public string PackageId { get; set; }
        [Required(ErrorMessage = "必填信息")]
        public string AreaId { get; set; }
        [Required(ErrorMessage = "必填信息")]
        public string MaterialId { get; set; }
    }
    #endregion

    #region PackageProductReplaceGroupCreateModel 套餐详情产品替换组创建模型
    /// <summary>
    /// 套餐详情产品替换组创建模型
    /// </summary>
    public class PackageProductReplaceGroupCreateModel
    {
        [Required(ErrorMessage = "必填信息")]
        public string PackageId { get; set; }
        [Required(ErrorMessage = "必填信息")]
        public string ReplaceGroupIds { get; set; }
    }
    #endregion

    #region PackageProductReplaceGroupDeleteModel 套餐详情产品替换组删除模型
    /// <summary> 
    /// 套餐详情产品替换组删除模型
    /// </summary>
    public class PackageProductReplaceGroupDeleteModel
    {
        [Required(ErrorMessage = "必填信息")]
        public string PackageId { get; set; }
        [Required(ErrorMessage = "必填信息")]
        public string ItemId { get; set; }
    }
    #endregion

    #region PackageReplaceGroupCreateModel 套餐详情替换组创建模型
    /// <summary>
    /// 套餐详情替换组创建模型
    /// </summary>
    public class PackageReplaceGroupCreateModel
    {
        [Required(ErrorMessage = "必填信息")]
        public string PackageId { get; set; }
        [Required(ErrorMessage = "必填信息")]
        public string AreaId { get; set; }
        [Required(ErrorMessage = "必填信息")]
        public string ProductIds { get; set; }
    }
    #endregion

    #region PackageReplaceGroupDeleteModel 套餐详情替换组删除模型
    /// <summary>
    /// 套餐详情替换组删除模型
    /// </summary>
    public class PackageReplaceGroupDeleteModel
    {
        [Required(ErrorMessage = "必填信息")]
        public string PackageId { get; set; }
        [Required(ErrorMessage = "必填信息")]
        public string AreaId { get; set; }
        [Required(ErrorMessage = "必填信息")]
        public string ProductId { get; set; }
    }
    #endregion

    #region PackageReplaceGroupSetDefaultModel 套餐详情替换组默认项设置模型
    /// <summary>
    /// 套餐详情替换组默认项设置模型
    /// </summary>
    public class PackageReplaceGroupSetDefaultModel
    {
        [Required(ErrorMessage = "必填信息")]
        public string PackageId { get; set; }
        [Required(ErrorMessage = "必填信息")]
        public string AreaId { get; set; }
        [Required(ErrorMessage = "必填信息")]
        public string DefaultId { get; set; }
        [Required(ErrorMessage = "必填信息")]
        public string ProductId { get; set; }
    }
    #endregion

    #region MapCreateModel 地图创建模型
    /// <summary>
    /// 地图创建模型
    /// </summary>
    public class MapCreateModel : ClientAssetCreateModel
    {
        public string Dependencies { get; set; }
        public string Properties { get; set; }
        public string IconAssetId { get; set; }
        public string Color { get; set; }
    }
    #endregion

    #region MapEditModel 地图编辑模型
    /// <summary>
    /// 地图编辑模型
    /// </summary>
    public class MapEditModel : ClientAssetEditModel
    {
        public string Dependencies { get; set; }
        public string Properties { get; set; }
        public string IconAssetId { get; set; }
        public string Color { get; set; }
    }
    #endregion

    #region MapTransLayoutModel 地图生成户型模型
    /// <summary>
    /// 地图生成户型模型
    /// </summary>
    public class MapTransLayoutModel
    {
        public string MapId { get; set; }
    }
    #endregion

    #region MapTransLayoutDataModel 地图生成户型模型Data
    /// <summary>
    /// 地图生成户型模型Data
    /// </summary>
    public class MapTransLayoutDataModel
    {
        public string Map { get; set; }  //指向客户端使用的地图的名称, Map对象的PackageName的值的最后一个斜杠后面的部分。比如 /Game/Maps/tls-all. 那么Map的值就为tls-all  
        public string MapId { get; set; }  //引用的map的对象ID  
    }
    #endregion


    #region LayoutCreateModel 户型创建模型
    /// <summary>
    /// 户型创建模型
    /// </summary>
    public class LayoutCreateModel
    {
        [StringLength(50, MinimumLength = 1, ErrorMessage = "长度必须为1-50个字符")]
        public string Name { get; set; }
        [StringLength(200, ErrorMessage = "长度必须为0-200个字符")]
        public string Description { get; set; }
        public string IconAssetId { get; set; }
        public string CategoryId { get; set; }
        public string Data { get; set; }
        public string Color { get; set; }
    }
    #endregion

    #region LayoutEditModel 户型编辑模型
    /// <summary>
    /// 户型编辑模型
    /// </summary>
    public class LayoutEditModel
    {
        [Required(ErrorMessage = "必填信息")]
        public string Id { get; set; }
        [StringLength(50, MinimumLength = 1, ErrorMessage = "长度必须为1-50个字符")]
        public string Name { get; set; }
        [StringLength(200, ErrorMessage = "长度必须为0-200个字符")]
        public string Description { get; set; }
        public string IconAssetId { get; set; }
        public string CategoryId { get; set; }
        public string Data { get; set; }
        public string Color { get; set; }
    }
    #endregion

    #region TextureCreateModel 贴图创建模型
    /// <summary>
    /// 贴图创建模型
    /// </summary>
    public class TextureCreateModel : ClientAssetCreateModel
    {
        public string IconAssetId { get; set; }
        public string Dependencies { get; set; }
        public string Properties { get; set; }
        public string Color { get; set; }
    }
    #endregion

    #region TextureEditModel 贴图编辑模型
    /// <summary>
    /// 贴图编辑模型
    /// </summary>
    public class TextureEditModel : ClientAssetEditModel
    {
        public string IconAssetId { get; set; }
        public string Dependencies { get; set; }
        public string Properties { get; set; }
        public string Color { get; set; }
    }
    #endregion

    #region MediaCreateModel 媒体文件创建模型
    /// <summary>
    /// 媒体文件创建模型
    /// </summary>
    public class MediaCreateModel : EntityCreateModel
    {
        public string FileAssetId { get; set; }
        public string IconAssetId { get; set; }
        public string Rotation { get; set; }
        public string Location { get; set; }
        public string SolutionId { get; set; }
        public string Type { get; set; }
    }
    #endregion

    #region MediaEditModel 媒体文件编辑模型
    /// <summary>
    /// 媒体文件编辑模型
    /// </summary>
    public class MediaEditModel : EntityEditModel
    {
        public string FileAssetId { get; set; }
        public string IconAssetId { get; set; }
        public string Rotation { get; set; }
        public string Location { get; set; }
        public string SolutionId { get; set; }
        public string Type { get; set; }
    }
    #endregion

    #region MediaShareResourceCreateModel 媒体分享信息创建模型
    /// <summary>
    /// 媒体分享信息创建模型
    /// </summary>
    public class MediaShareResourceCreateModel : EntityCreateModel
    {
        [Required(ErrorMessage = "必填信息")]
        public string MediaId { get; set; }
        [Required(ErrorMessage = "必填信息")]
        public long StartShareTimeStamp { get; set; }
        public long StopShareTimeStamp { get; set; }
        public string Password { get; set; }
    }
    #endregion

    #region MediaShareResourceEditModel 媒体分享信息编辑模型
    /// <summary>
    /// 媒体分享信息编辑模型
    /// </summary>
    public class MediaShareResourceEditModel : EntityEditModel
    {
        [Required(ErrorMessage = "必填信息")]
        public long StartShareTimeStamp { get; set; }
        public long StopShareTimeStamp { get; set; }
        public string Password { get; set; }
    }
    #endregion

    #region MediaShareRequestModel 共享资源文件查看请求模型
    /// <summary>
    /// 共享资源文件查看请求模型
    /// </summary>
    public class MediaShareRequestModel
    {
        public string Id { get; set; }
        public string MediaType { get; set; }
        public string Password { get; set; }
    }
    #endregion

    #region AreaTypeCreateModel 区域类型创建模型
    /// <summary>
    /// 区域类型创建模型
    /// </summary>
    public class AreaTypeCreateModel : EntityCreateModel
    {
        public string IconAssetId { get; set; }
    }
    #endregion

    #region AreaTypeEditModel 区域类型编辑模型
    /// <summary>
    /// 区域类型编辑模型
    /// </summary>
    public class AreaTypeEditModel : EntityEditModel
    {
        public string IconAssetId { get; set; }
    }
    #endregion


    public class ProductGroupCreateModel : EntityCreateModel
    {
        public string Serie { get; set; }
        public string IconAssetId { get; set; }
        public string Items { get; set; }
        public string PivotLocation { get; set; }
        public int PivotType { get; set; }
        public int Orientation { get; set; }
        public string CategoryId { get; set; }
        public string Color { get; set; }
    }

    public class ProductGroupEditModel : EntityEditModel
    {
        public string Serie { get; set; }
        public string IconAssetId { get; set; }
        public string Items { get; set; }
        public string PivotLocation { get; set; }
        public int PivotType { get; set; }
        public int Orientation { get; set; }
        public string CategoryId { get; set; }
        public string Color { get; set; }
    }


    public class AssetCategoryCreateModel : EntityCreateModel
    {
        [Required(ErrorMessage = "必填信息")]
        public string Type { get; set; }
        //[Required(ErrorMessage = "必填信息")]
        public string ParentId { get; set; }
        public string OrganizationId { get; set; }
        public string CustomData { get; set; }
        public string Tag { get; set; }
        public bool Isolate { get; set; }
    }

    public class AssetCategoryEditModel : EntityEditModel
    {
        public string ParentId { get; set; }
        public string OrganizationId { get; set; }
        public string CustomData { get; set; }
        public string Tag { get; set; }
        public bool Isolate { get; set; }
    }

    public class ProductReplaceGroupCreateModel : EntityCreateModel
    {
        [Required(ErrorMessage = "必填信息")]
        public string ItemIds { get; set; }
        public string DefaultItemId { get; set; }
    }

    public class ProductReplaceGroupEditModel : EntityEditModel
    {
        [Required(ErrorMessage = "必填信息")]
        public string ItemIds { get; set; }
        public string DefaultItemId { get; set; }
    }

    public class ProductReplaceGroupSetDefaultModel
    {
        [Required(ErrorMessage = "必填信息")]
        public string Id { get; set; }
        [Required(ErrorMessage = "必填信息")]
        public string ItemId { get; set; }
    }

}
