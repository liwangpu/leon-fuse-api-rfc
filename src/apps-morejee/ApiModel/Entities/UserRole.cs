using System.ComponentModel.DataAnnotations.Schema;

namespace ApiModel.Entities
{
    public class UserRole : EntityBase, IListable, IDTOTransfer<UserRoleDTO>
    {
        /// <summary>
        /// 适用组织类型,逗号分隔
        /// </summary>
        public string ApplyOrgans { get; set; }
        public bool IsInner { get; set; }
        public string Icon { get; set; }

        public UserRoleDTO ToDTO()
        {
            var dto = new UserRoleDTO();
            dto.Id = Id;
            dto.Name = Name;
            dto.ApplyOrgans = ApplyOrgans;
            dto.Description = Description;
            dto.OrganizationId = OrganizationId;
            dto.ActiveFlag = ActiveFlag;
            dto.Creator = Creator;
            dto.Modifier = Modifier;
            dto.CreatedTime = CreatedTime;
            dto.ModifiedTime = ModifiedTime;
            dto.CreatorName = CreatorName;
            dto.ModifierName = ModifierName;
            dto.IsInner = IsInner;
            return dto;
        }
    }

    public class UserRoleDTO : EntityBase, IListable
    {
        public bool IsInner { get; set; }
        public string Icon { get; set; }
        public string ApplyOrgans { get; set; }
        
    }
}
