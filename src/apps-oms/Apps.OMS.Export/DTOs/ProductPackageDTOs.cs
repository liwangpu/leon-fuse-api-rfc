using System;

namespace Apps.OMS.Export.DTOs
{
    public class ProductPackageDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Creator { get; set; }
        public string Modifier { get; set; }
        public string CreatorName { get; set; }
        public string ModifierName { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime ModifiedTime { get; set; }
        public string OrganizationId { get; set; }
        public int ActiveFlag { get; set; }
        public string ProductSpecId { get; set; }
        public int Num { get; set; }
    }
}
