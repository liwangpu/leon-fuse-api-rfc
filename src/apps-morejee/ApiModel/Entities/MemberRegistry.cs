using System.ComponentModel.DataAnnotations.Schema;
namespace ApiModel.Entities
{
    public class MemberRegistry : EntityBase, IListable, IDTOTransfer<MemberRegistryDTO>
    {
        public string Phone { get; set; }
        public string Mail { get; set; }
        public string Company { get; set; }
        public string Icon { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string Area { get; set; }
        public string BusinessCard { get; set; }
        public string Inviter { get; set; }

        public MemberRegistryDTO ToDTO()
        {
            var dto = new MemberRegistryDTO();
            dto.Id = Id;
            dto.Name = Name;
            dto.Phone = Phone;
            dto.Mail = Mail;
            dto.Company = Company;
            dto.Province = Province;
            dto.City = City;
            dto.Area = Area;
            dto.Inviter = Inviter;
            dto.BusinessCard = BusinessCard;
            dto.Description = Description;
            dto.CategoryId = CategoryId;
            dto.OrganizationId = OrganizationId;
            dto.Creator = Creator;
            dto.Modifier = Modifier;
            dto.CreatedTime = CreatedTime;
            dto.ModifiedTime = ModifiedTime;
            dto.CreatorName = CreatorName;
            dto.ModifierName = ModifierName;
            dto.Icon = Icon;
            return dto;
        }
    }

    public class MemberRegistryDTO : EntityBase, IListable
    {
        public string Phone { get; set; }
        public string Mail { get; set; }
        public string Company { get; set; }
        public string Icon { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string Area { get; set; }
        public string BusinessCard { get; set; }
        public string Inviter { get; set; }
        
    }
}
