using System.ComponentModel.DataAnnotations;

namespace ApiServer.Models
{
    #region FileAssetCreateModel 资源文件创建模型
    /// <summary>
    /// 资源文件创建模型
    /// </summary>
    public class FileAssetCreateModel
    {
        [StringLength(50, MinimumLength = 1, ErrorMessage = "长度必须为1-50个字符")]
        public string Name { get; set; }
        [StringLength(200, ErrorMessage = "长度必须为0-200个字符")]
        public string Description { get; set; }
        public string Url { get; set; }
        public string Md5 { get; set; }
        public long Size { get; set; }
        public string FileExt { get; set; }
        public string LocalPath { get; set; }
        public string FolderId { get; set; }
        public string CategoryId { get; set; }
        public string AccountId { get; set; }
        public string Icon { get; set; }
        public int FileState { get; set; }
    }
    #endregion

    #region FileAssetEditModel 资源文件编辑模型
    /// <summary>
    /// 资源文件编辑模型
    /// </summary>
    public class FileAssetEditModel
    {
        [Required(ErrorMessage = "必填信息")]
        public string Id { get; set; }
        [StringLength(50, MinimumLength = 1, ErrorMessage = "长度必须为1-50个字符")]
        public string Name { get; set; }
        [StringLength(200, ErrorMessage = "长度必须为0-200个字符")]
        public string Description { get; set; }
        public string Url { get; set; }
        public string Md5 { get; set; }
        public long Size { get; set; }
        public string FileExt { get; set; }
        public string LocalPath { get; set; }
        public string FolderId { get; set; }
        public string CategoryId { get; set; }
        public string AccountId { get; set; }
        public string Icon { get; set; }
        public int FileState { get; set; }
    }
    #endregion

    #region IconModel ICon信息上传模型
    /// <summary>
    /// ICon信息上传模型
    /// </summary>
    public class IconModel
    {
        [Required(ErrorMessage = "必填信息")]
        public string ObjId { get; set; }
        [Required(ErrorMessage = "必填信息")]
        public string AssetId { get; set; }
    }
    #endregion

    #region StaticMeshCreateModel 模型文件创建模型
    /// <summary>
    /// 模型文件创建模型
    /// </summary>
    public class StaticMeshCreateModel: ClientAssetCreateModel
    {
        public string IconAssetId { get; set; }
        public string Dependencies { get; set; }
        public string Properties { get; set; }
        public string SrcFileAssetId { get; set; }
        public string Materials { get; set; }
        public string Color { get; set; }
    }
    #endregion

    #region StaticMeshEditModel 模型文件编辑模型
    /// <summary>
    /// 模型文件编辑模型
    /// </summary>
    public class StaticMeshEditModel: ClientAssetEditModel
    {
        public string IconAssetId { get; set; }
        public string Dependencies { get; set; }
        public string Properties { get; set; }
        public string SrcFileAssetId { get; set; }
        public string Materials { get; set; }
        public string Color { get; set; }
    }
    #endregion

    #region StaticMeshUploadModel 规格模型上传模型
    /// <summary>
    /// 规格模型上传模型
    /// </summary>
    public class SpecStaticMeshUploadModel
    {
        [Required(ErrorMessage = "必填信息")]
        public string ProductSpecId { get; set; }
        [Required(ErrorMessage = "必填信息")]
        public string StaticMeshId { get; set; }
    }
    #endregion

    #region SpecMaterialUploadModel 规格材料上传模型
    /// <summary>
    /// 规格材料上传模型
    /// </summary>
    public class SpecMaterialUploadModel
    {
        [Required(ErrorMessage = "必填信息")]
        public string ProductSpecId { get; set; }
        [Required(ErrorMessage = "必填信息")]
        public string StaticMeshId { get; set; }
        [Required(ErrorMessage = "必填信息")]
        public string MaterialId { get; set; }
    }
    #endregion

}
