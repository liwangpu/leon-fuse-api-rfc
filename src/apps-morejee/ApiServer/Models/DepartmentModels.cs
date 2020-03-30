using System.ComponentModel.DataAnnotations;

namespace ApiServer.Models
{
    #region DepartmentEditModel 部门编辑模型
    /// <summary>
    /// 部门编辑模型
    /// </summary>
    public class DepartmentEditModel
    {
        [Required(ErrorMessage = "必填信息")]
        public string Id { get; set; }
        //[Required(ErrorMessage = "必填信息")]
        public string ParentId { get; set; }
        public string OrganizationId { get; set; }
        [StringLength(50, MinimumLength = 1, ErrorMessage = "长度必须为1-50个字符")]
        public string Name { get; set; }
        [StringLength(200, ErrorMessage = "长度必须为0-200个字符")]
        public string Description { get; set; }
    }
    #endregion

    #region DepartmentCreateModel 部门创建模型
    /// <summary>
    /// 部门创建模型
    /// </summary>
    public class DepartmentCreateModel
    {
        public string ParentId { get; set; }
        public string OrganizationId { get; set; }
        [StringLength(50, MinimumLength = 1, ErrorMessage = "长度必须为1-50个字符")]
        public string Name { get; set; }
        [StringLength(200, ErrorMessage = "长度必须为0-200个字符")]
        public string Description { get; set; }
    }
    #endregion
}
