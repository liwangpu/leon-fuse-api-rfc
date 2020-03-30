using System.ComponentModel.DataAnnotations;

namespace ApiServer.Models
{
    public class NavigationCreateModel
    {
        //[StringLength(50, MinimumLength = 1, ErrorMessage = "长度必须为1-50个字符")]
        public string Name { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        //[StringLength(50, MinimumLength = 1, ErrorMessage = "长度必须为1-50个字符")]
        public string Url { get; set; }
        //[StringLength(50, MinimumLength = 1, ErrorMessage = "长度必须为1-50个字符")]
        public string Icon { get; set; }
        //[StringLength(50, MinimumLength = 1, ErrorMessage = "长度必须为1-50个字符")]
        public string Permission { get; set; }
        //[StringLength(50, MinimumLength = 1, ErrorMessage = "长度必须为1-50个字符")]
        public string NodeType { get; set; }
        public string PagedModel { get; set; }
        public string Resource { get; set; }
        public string Field { get; set; }
        public string QueryParams { get; set; }
    }

    public class NavigationEditModel
    {
        [Required(ErrorMessage = "必填信息")]
        public string Id { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }
        //[StringLength(50, MinimumLength = 1, ErrorMessage = "长度必须为1-50个字符")]
        public string Name { get; set; }
        //[StringLength(50, MinimumLength = 1, ErrorMessage = "长度必须为1-50个字符")]
        //[StringLength(50, MinimumLength = 1, ErrorMessage = "长度必须为1-50个字符")]
        public string Url { get; set; }
        //[StringLength(50, MinimumLength = 1, ErrorMessage = "长度必须为1-50个字符")]
        public string Icon { get; set; }
        //[StringLength(50, MinimumLength = 1, ErrorMessage = "长度必须为1-50个字符")]
        public string Permission { get; set; }
        //[StringLength(50, MinimumLength = 1, ErrorMessage = "长度必须为1-50个字符")]
        public string NodeType { get; set; }
        public string PagedModel { get; set; }
        public string Resource { get; set; }
        public string Field { get; set; }
        public string QueryParams { get; set; }
    }


    public class UserNavCreateModel
    {
        public string Name { get; set; }
        public string Role { get; set; }
        public string Description { get; set; }
    }

    public class UserNavEditModel
    {
        [Required(ErrorMessage = "必填信息")]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public string Description { get; set; }
    }

    public class UserNavDetailEditModel
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "必填信息")]
        public string UserNavId { get; set; }
        [Required(ErrorMessage = "必填信息")]
        public string RefNavigationId { get; set; }
        public string Grade { get; set; }
        public string ParentId { get; set; }
        public string ExcludeFiled { get; set; }
        public string ExcludePermission { get; set; }
        public string ExcludeQueryParams { get; set; }
    }

    public class UserNavDetailDeleteModel
    {
        [Required(ErrorMessage = "必填信息")]
        public string Id { get; set; }
        [Required(ErrorMessage = "必填信息")]
        public string UserNavId { get; set; }
    }
}
