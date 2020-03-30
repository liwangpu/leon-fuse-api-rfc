using ApiModel;
using ApiModel.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace ApiServer.Models
{
    public class PagingRequestModel
    {
        public string Q { get; set; }
        public string Search { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string OrderBy { get; set; }
        public bool Desc { get; set; }

        /// <summary>
        /// 明确指定过滤参数,在q语句完全支持前的过渡
        /// </summary>
        public List<QueryFilter> explicitFilters = new List<QueryFilter>();

        /// <summary>
        /// 从Q解析过滤参数
        /// </summary>
        /// <returns></returns>
        public List<QueryFilter> Filters()
        {
            var filters = new List<QueryFilter>();
            if (!string.IsNullOrWhiteSpace(Q))
            {

            }
            return filters;
        }
    }

    #region [CreateModel,EditModel Base]

    #region EntityCreateModel Entity基本创建模型
    /// <summary>
    /// Entity基本创建模型
    /// </summary>
    public class EntityCreateModel
    {
        [StringLength(50, MinimumLength = 1, ErrorMessage = "长度必须为1-50个字符")]
        public string Name { get; set; }
        [StringLength(200, ErrorMessage = "长度必须为0-200个字符")]
        public string Description { get; set; }
    }
    #endregion

    #region EntityEditModel Entity基本编辑模型
    /// <summary>
    /// Entity基本编辑模型
    /// </summary>
    public class EntityEditModel
    {
        [Required(ErrorMessage = "必填信息")]
        public string Id { get; set; }
        [StringLength(50, MinimumLength = 1, ErrorMessage = "长度必须为1-50个字符")]
        public string Name { get; set; }
        [StringLength(200, ErrorMessage = "长度必须为0-200个字符")]
        public string Description { get; set; }
    }
    #endregion

    #region ClientAssetCreateModel 客户端资源创建模型
    /// <summary>
    /// 客户端资源创建模型
    /// </summary>
    public class ClientAssetCreateModel : EntityCreateModel
    {
        public string FileAssetId { get; set; }
        public string PackageName { get; set; }
        public string UnCookedAssetId { get; set; }
    }
    #endregion

    #region ClientAssetEditModel 客户端资源编辑模型
    /// <summary> 
    /// 客户端资源编辑模型
    /// </summary>
    public class ClientAssetEditModel : EntityEditModel
    {
        public string FileAssetId { get; set; }
        public string PackageName { get; set; }
        public string UnCookedAssetId { get; set; }
    }
    #endregion

    #endregion

    #region CollectionCreateModel 收藏创建模型
    /// <summary>
    /// 收藏创建模型
    /// </summary>
    public class CollectionCreateModel
    {
        [Required(ErrorMessage = "必填信息")]
        public string TargetId { get; set; }
    }
    #endregion

    #region ColletionRequestModel 收藏查询请求信息
    /// <summary>
    /// 收藏查询请求信息
    /// </summary>
    public class ColletionRequestModel : PagingRequestModel
    {
        public string TargetId { get; set; }
        public string CategoryId { get; set; }
        //public bool IsInFolder { get; set; }
    }
    #endregion

    /// <summary>
    /// 偏好设置编辑模型
    /// </summary>
    public class PreferenceCreateModel
    {
        [Required(ErrorMessage = "必填信息")]
        public string Key { get; set; }
        [Required(ErrorMessage = "必填信息")]
        public string Value { get; set; }
    }

    /// <summary>
    /// 资源权限编辑模型
    /// </summary>
    public class ResPermissionEditModel
    {
        [Required(ErrorMessage = "必填信息")]
        public string OrgansPermission { get; set; }
        [Required(ErrorMessage = "必填信息")]
        public string ResIds { get; set; }
    }

    public class ResPermissionDetailModel
    {
        public string OrganId { get; set; }
        public string OperateIds { get; set; }
    }

    public class LeaveMessageModel
    {
        [StringLength(50, MinimumLength = 1, ErrorMessage = "长度必须为1-50个字符")]
        public string Subject { get; set; }
        [StringLength(50, MinimumLength = 1, ErrorMessage = "长度必须为1-50个字符")]
        public string Content { get; set; }
    }

}
