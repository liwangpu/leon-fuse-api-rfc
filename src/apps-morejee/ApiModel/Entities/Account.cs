using ApiModel.Consts;
using BambooCommon;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiModel.Entities
{
    public class Account : EntityBase, IListable, ICloneable, IDTOTransfer<AccountDTO>
    {
        public string Icon { get; set; }
        public string Mail { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public bool MailValid { get; set; }
        public bool PhoneValid { get; set; }
        public bool Frozened { get; set; }
        public string DepartmentId { get; set; }
        public string Location { get; set; }
        /// <summary>
        /// 账号类型，系统管理员，普通用户，供应商，设计公司等等
        /// </summary>
        public string Type
        {
            get; set;
        }
        /// <summary>
        /// 账号有效期，登陆时间小于这个有效期则无法登陆
        /// </summary>
        public DateTime ExpireTime { get; set; }
        /// <summary>
        /// 账号启用时间，如果当前登陆时间小于启用时间，则不能登陆。
        /// </summary>
        public DateTime ActivationTime { get; set; }
        public Organization Organization { get; set; }
        public Department Department { get; set; }
        public List<AccountRole> AdditionRoles { get; set; }

        [NotMapped]
        public string OrganizationName { get; set; }
        [NotMapped]
        public string OrganizationIcon { get; set; }
        /// <summary>
        /// 权限记录，记录能访问的所有类型资源的所有权限设置。不在此列出的则无法访问。
        /// </summary>

        public List<PermissionItem> Permissions { get; set; }

        public AccountDTO ToDTO()
        {
            var dto = new AccountDTO();
            dto.Id = Id;
            dto.Name = Name;
            dto.Description = Description;
            dto.Mail = Mail;
            dto.Location = Location;
            dto.OrganizationId = OrganizationId;
            dto.DepartmentId = DepartmentId;
            dto.Phone = Phone;
            dto.Frozened = Frozened;
            dto.Type = Type;
            dto.RoleId = Type;
            //临时方案,已经在重构版本里面改好
            if (Type == UserRoleConst.SysAdmin)
                dto.RoleName = "系统超级管理员";
            else if (Type == UserRoleConst.SysService)
                dto.RoleName = "系统客服";
            else if (Type == UserRoleConst.BrandAdmin)
                dto.RoleName = "品牌商管理员";
            else if (Type == UserRoleConst.BrandMember)
                dto.RoleName = "品牌商用户";
            else if (Type == UserRoleConst.PartnerAdmin)
                dto.RoleName = "代理商管理员";
            else if (Type == UserRoleConst.PartnerMember)
                dto.RoleName = "代理商用户";
            else if (Type == UserRoleConst.SupplierAdmin)
                dto.RoleName = "供应商管理员";
            else if (Type == UserRoleConst.SupplierMember)
                dto.RoleName = "供应商用户";
            else { }
            dto.ExpireTime = ExpireTime;
            dto.ActivationTime = ActivationTime;
            dto.Creator = Creator;
            dto.Modifier = Modifier;
            dto.CreatedTime = CreatedTime;
            dto.ModifiedTime = ModifiedTime;
            dto.CreatorName = CreatorName;
            dto.ModifierName = ModifierName;
            dto.OrganizationId = OrganizationId;
            dto.OrganizationName = OrganizationName;
            dto.OrganizationIcon = OrganizationIcon;
            dto.CategoryName = CategoryName;
            dto.AdditionRoles = AdditionRoles;
            if (Department != null)
                dto.DepartmentName = Department.Name;
            if (Type.Contains("admin"))
                dto.IsAdmin = true;
            dto.Icon = IconFileAssetUrl;
            dto.IconAssetId = Icon;
            return dto;
        }
    }


    public class AccountDTO : EntityBase, IListable
    {
        public string OrganizationName { get; set; }
        public string OrganizationIcon { get; set; }
        public string Icon { get; set; }
        public string Mail { get; set; }
        public string Location { get; set; }
        public string DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string Phone { get; set; }
        public bool Frozened { get; set; }
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public string Type { get; set; }
        public bool IsAdmin { get; set; }
        public string IconAssetId { get; set; }
        public DateTime ExpireTime { get; set; }
        public DateTime ActivationTime { get; set; }
        public List<AccountRole> AdditionRoles { get; set; }
    }
}
