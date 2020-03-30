using System.ComponentModel.DataAnnotations.Schema;

namespace ApiModel.Entities
{
    public class Navigation : EntityBase, IListable, IDTOTransfer<NavigationDTO>
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public string Icon { get; set; }
        public string Permission { get; set; }
        public string PagedModel { get; set; }
        public string Resource { get; set; }
        public string Field { get; set; }
        public string NodeType { get; set; }
        public bool IsInner { get; set; }
        public bool NewTapOpen { get; set; }
        public string QueryParams { get; set; }
        public NavigationDTO ToDTO()
        {
            var dto = new NavigationDTO();
            dto.Id = Id;
            dto.Name = Name;
            dto.Resource = Resource;
            dto.Url = Url;
            dto.Icon = Icon;
            dto.Permission = Permission;
            dto.PagedModel = PagedModel;
            dto.NodeType = NodeType;
            dto.Field = Field;
            dto.Title = Title;
            dto.IsInner = IsInner;
            dto.QueryParams = QueryParams;
            return dto;
        }
    }

    public class NavigationDTO : EntityBase, IListable
    {
        public bool IsInner { get; set; }
        public bool NewTapOpen { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string Icon { get; set; }
        public string Permission { get; set; }
        public string PagedModel { get; set; }
        public string Resource { get; set; }
        public string Field { get; set; }
        public string NodeType { get; set; }
        public string QueryParams { get; set; }
        
    }
}
