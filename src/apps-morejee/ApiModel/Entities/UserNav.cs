using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Collections.Generic;

namespace ApiModel.Entities
{
    public class UserNav : EntityBase, IListable, IDTOTransfer<UserNavDTO>
    {
        public string Role { get; set; }
        public List<UserNavDetail> UserNavDetails { get; set; }
        [NotMapped]
        public string Icon { get; set; }
        [NotMapped]
        public string RoleName { get; set; }

        public UserNavDTO ToDTO()
        {
            var dto = new UserNavDTO();
            dto.Id = Id;
            dto.Name = Name;
            dto.Description = Description;
            dto.Role = Role;
            dto.RoleId = Role;
            dto.RoleName = RoleName;
            dto.UserNavDetails = UserNavDetails;
            return dto;
        }
    }


    public class UserNavDTO : EntityBase, IListable
    {
        public string Role { get; set; }
        public string RoleId { get; set; }
        public string RoleName { get; set; }

        public string Icon { get; set; }
        public List<UserNavDetail> UserNavDetails { get; set; }
    }
}
