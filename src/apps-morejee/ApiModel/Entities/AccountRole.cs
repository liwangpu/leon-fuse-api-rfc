using System.ComponentModel.DataAnnotations.Schema;

namespace ApiModel.Entities
{
    public class AccountRole
    {
        public string Id { get; set; }
        public Account Account { get; set; }
        public string UserRoleId { get; set; }
        [NotMapped]
        public string UserRoleName { get; set; }
    }
}
