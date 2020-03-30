using System;
using System.ComponentModel.DataAnnotations;

namespace ApiServer.Models
{
    #region AccountEditModel 用户信息编辑模型
    /// <summary>
    /// 用户信息编辑模型
    /// </summary>
    public class AccountEditModel
    {
        [Required(ErrorMessage = "必填信息")]
        public string Id { get; set; }
        [StringLength(50, MinimumLength = 1, ErrorMessage = "长度必须为1-50个字符")]
        public string Mail { get; set; }
        [StringLength(50, MinimumLength = 1, ErrorMessage = "长度必须为1-50个字符")]
        public string Name { get; set; }
        [StringLength(50, ErrorMessage = "长度必须为0-50个字符")]
        public string Password { get; set; }
        [StringLength(200, ErrorMessage = "长度必须为0-200个字符")]
        public string Description { get; set; }
        public string Location { get; set; }
        public string Phone { get; set; }
        public bool Frozened { get; set; }
        public bool IsAdmin { get; set; }
        public string ExpireTime { get; set; }
        public string ActivationTime { get; set; }
        public string DepartmentId { get; set; }
    }
    #endregion

    #region RegisterAccountModel 用户注册模型
    /// <summary>
    /// 用户注册模型
    /// </summary>
    public class RegisterAccountModel
    {
        [StringLength(50, MinimumLength = 1, ErrorMessage = "长度必须为1-50个字符")]
        public string Mail { get; set; }
        [StringLength(50, MinimumLength = 1, ErrorMessage = "长度必须为1-50个字符")]
        public string Name { get; set; }
        [StringLength(50, ErrorMessage = "长度必须为0-50个字符")]
        public string Password { get; set; }
        [StringLength(200, ErrorMessage = "长度必须为0-200个字符")]
        public string Description { get; set; }
        public string Location { get; set; }
        public string Phone { get; set; }
        public string OrganizationId { get; set; }
        public string DepartmentId { get; set; }
        //[Required(ErrorMessage = "必填信息")]
        public bool IsAdmin { get; set; }
        [Required(ErrorMessage = "必填信息")]
        public string ExpireTime { get; set; }
        [Required(ErrorMessage = "必填信息")]
        public string ActivationTime { get; set; }
    }
    #endregion

    #region ResetPasswordModel 重置密码编辑模型
    /// <summary>
    /// 重置密码编辑模型
    /// </summary>
    public class ResetPasswordModel
    {
        [Required(ErrorMessage = "必填信息")]
        public string UserId { get; set; }
        [StringLength(50, MinimumLength = 6, ErrorMessage = "长度必须大于6个字符")]
        public string Password { get; set; }
    }
    #endregion

    public class NewPasswordModel
    {
        [Required(ErrorMessage = "必填信息")]
        public string OldPassword { get; set; }
        [StringLength(50, MinimumLength = 6, ErrorMessage = "长度必须大于6个字符")]
        public string NewPassword { get; set; }
    }

    public class UserAdditionalRoleEditModel
    {
        [Required(ErrorMessage = "必填信息")]
        public string UserId { get; set; }
        public string AdditionalRoleIds { get; set; }
    }

    #region AccountProfileModel 用户账户基础信息
    /// <summary>
    /// 用户账户基础信息
    /// </summary>
    public class AccountProfileModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Mail { get; set; }
        public string Phone { get; set; }
        public string Avatar { get; set; }
        public string Location { get; set; }
        public string Brief { get; set; }
        public string OrganizationId { get; set; }
        public string Organization { get; set; }
        public string DepartmentId { get; set; }
        public string Department { get; set; }
        public string Role { get; set; }
        public DateTime ExpireTime { get; set; }
        public DateTime ActivationTime { get; set; }
    }
    #endregion

    #region OrganizationCreateModel 组织新建模型
    /// <summary>
    /// 组织新建模型
    /// </summary>
    public class OrganizationCreateModel
    {
        [StringLength(50, MinimumLength = 1, ErrorMessage = "长度必须为1-50个字符")]
        public string Name { get; set; }
        [StringLength(200, ErrorMessage = "长度必须为0-200个字符")]
        public string Description { get; set; }
        [StringLength(50, ErrorMessage = "长度必须为0-50个字符")]
        public string Mail { get; set; }
        [StringLength(50, ErrorMessage = "长度必须为0-50个字符")]
        public string Location { get; set; }
        [Required(ErrorMessage = "必填信息")]
        public DateTime ExpireTime { get; set; }
        [Required(ErrorMessage = "必填信息")]
        public DateTime ActivationTime { get; set; }
        [Required(ErrorMessage = "必填信息")]
        public string OrganizationTypeId { get; set; }
        public string ParentId { get; set; }
        public string IconAssetId { get; set; }
    }
    #endregion

    #region OrganizationEditModel 组织编辑模型
    /// <summary>
    /// 组织编辑模型
    /// </summary>
    public class OrganizationEditModel
    {
        [Required(ErrorMessage = "必填信息")]
        public string Id { get; set; }
        [StringLength(50, MinimumLength = 1, ErrorMessage = "长度必须为1-50个字符")]
        public string Name { get; set; }
        [StringLength(200, ErrorMessage = "长度必须为0-200个字符")]
        public string Description { get; set; }
        [StringLength(50, ErrorMessage = "长度必须为0-50个字符")]
        public string Mail { get; set; }
        [StringLength(50, ErrorMessage = "长度必须为0-50个字符")]
        public string Location { get; set; }
        [Required(ErrorMessage = "必填信息")]
        public DateTime ExpireTime { get; set; }
        [Required(ErrorMessage = "必填信息")]
        public DateTime ActivationTime { get; set; }
        [Required(ErrorMessage = "必填信息")]
        public string OrganizationTypeId { get; set; }
        public string ParentId { get; set; }
        public string IconAssetId { get; set; }
    }
    #endregion

    public class OrganizationTypeCreateModel
    {
        [StringLength(50, MinimumLength = 1, ErrorMessage = "长度必须为1-50个字符")]
        public string Name { get; set; }
        [StringLength(200, ErrorMessage = "长度必须为0-200个字符")]
        public string Description { get; set; }
    }

    public class OrganizationTypeEditModel
    {
        [Required(ErrorMessage = "必填信息")]
        public string Id { get; set; }
        [StringLength(50, MinimumLength = 1, ErrorMessage = "长度必须为1-50个字符")]
        public string Name { get; set; }
        [StringLength(200, ErrorMessage = "长度必须为0-200个字符")]
        public string Description { get; set; }
    }

    public class UserRoleCreateModel
    {
        [StringLength(50, MinimumLength = 1, ErrorMessage = "长度必须为1-50个字符")]
        public string Name { get; set; }
        [StringLength(200, ErrorMessage = "长度必须为0-200个字符")]
        public string Description { get; set; }
        public string IconAssetId { get; set; }
        //public string ApplyOrgans { get; set; }
    }

    public class UserRoleEditModel
    {
        [Required(ErrorMessage = "必填信息")]
        public string Id { get; set; }
        [StringLength(50, MinimumLength = 1, ErrorMessage = "长度必须为1-50个字符")]
        public string Name { get; set; }
        [StringLength(200, ErrorMessage = "长度必须为0-200个字符")]
        public string Description { get; set; }
        public string IconAssetId { get; set; }
        //public string ApplyOrgans { get; set; }
    }

    public class MemberRegistryCreateModel
    {
        [Required(ErrorMessage = "必填信息")]
        public string Name { get; set; }
        [Required(ErrorMessage = "必填信息")]
        public string Mail { get; set; }
        [Required(ErrorMessage = "必填信息")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "必填信息")]
        public string Company { get; set; }
        [Required(ErrorMessage = "必填信息")]
        public string Province { get; set; }
        [Required(ErrorMessage = "必填信息")]
        public string City { get; set; }
        public string Area { get; set; }
        [Required(ErrorMessage = "必填信息")]
        public string Inviter { get; set; }
        [StringLength(200, ErrorMessage = "长度必须为0-200个字符")]
        public string Remark { get; set; }
    }

}
