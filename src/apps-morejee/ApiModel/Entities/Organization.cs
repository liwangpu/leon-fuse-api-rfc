using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiModel.Entities
{
    public class Organization : EntityBase, IListable, IDTOTransfer<OrganizationDTO>
    {
        public string ParentId { get; set; }
        public string Icon { get; set; }
        public string Mail { get; set; }
        public string Location { get; set; }
        public string OwnerId { get; set; }
        public DateTime ExpireTime { get; set; }
        public DateTime ActivationTime { get; set; }
        public string OrganizationTypeId { get; set; }
        public List<Department> Departments { get; set; }
        public List<ClientAsset> ClientAssets { get; set; }
        public List<Product> Products { get; set; }
        public List<Solution> Solutions { get; set; }
        public List<Layout> Layouts { get; set; }
        public List<Order> Orders { get; set; }
        public List<AssetFolder> Folders { get; set; }
        public List<FileAsset> Files { get; set; }
        [NotMapped]
        public string OrganizationTypeName { get; set; }

        public OrganizationDTO ToDTO()
        {
            var dto = new OrganizationDTO();
            dto.Id = Id;
            dto.Name = Name;
            dto.Description = Description;
            dto.Mail = Mail;
            dto.ParentId = ParentId;
            dto.Location = Location;
            dto.Creator = Creator;
            dto.Modifier = Modifier;
            dto.CreatedTime = CreatedTime;
            dto.ModifiedTime = ModifiedTime;
            dto.CreatorName = CreatorName;
            dto.ModifierName = ModifierName;
            dto.Type = OrganizationTypeId;
            dto.TypeName = OrganizationTypeName;
            dto.OrganizationTypeId = OrganizationTypeId;
            dto.OrganizationTypeName = OrganizationTypeName;
            dto.ExpireTime = ExpireTime;
            dto.ActivationTime = ActivationTime;
            dto.Icon = IconFileAssetUrl;
            dto.IconAssetId = Icon;
            return dto;
        }
    }


    public class OrganizationDTO : EntityBase, IListable
    {
        public string ParentId { get; set; }
        public string IconAssetId { get; set; }
        public string Mail { get; set; }
        public string Location { get; set; }
        public string OwnerId { get; set; }
        public string Type { get; set; }
        public string TypeName { get; set; }
        public string OrganizationTypeId { get; set; }
        public string OrganizationTypeName { get; set; }
        public string Icon { get; set; }
        public DateTime ExpireTime { get; set; }
        public DateTime ActivationTime { get; set; }
        
    }
}
