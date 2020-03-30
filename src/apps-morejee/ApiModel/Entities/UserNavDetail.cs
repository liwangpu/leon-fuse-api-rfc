using System.ComponentModel.DataAnnotations.Schema;

namespace ApiModel.Entities
{
    public class UserNavDetail
    {
        public string Id { get; set; }
        public string ExcludeFiled { get; set; }
        public string ExcludePermission { get; set; }
        public string ExcludeQueryParams { get; set; }
        public int Grade { get; set; }
        public string ParentId { get; set; }
        public string RefNavigationId { get; set; }
        public string UserNavId { get; set; }
        public UserNav UserNav { get; set; }

        [NotMapped]
        public string Name { get; set; }
        [NotMapped]
        public string Title { get; set; }
        [NotMapped]
        public string Url { get; set; }
        [NotMapped]
        public string NodeType { get; set; }
        [NotMapped]
        public string Icon { get; set; }
        [NotMapped]
        public string Resource { get; set; }
        [NotMapped]
        public string Permission { get; set; }
        [NotMapped]
        public string PagedModel { get; set; }
        [NotMapped]
        public string Field { get; set; }
        [NotMapped]
        public string QueryParams { get; set; }
        [NotMapped]
        public bool NewTapOpen { get; set; }

    }
}
