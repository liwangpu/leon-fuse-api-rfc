using System.ComponentModel.DataAnnotations;

namespace Apps.OMS.Export.Models
{
    public class ProductPackageCreateModel
    {
        [Required(ErrorMessage = "必填信息")]
        public string ProductSpecId { get; set; }
        [StringLength(50, MinimumLength = 1, ErrorMessage = "名称长度必须为1-50个字符")]
        public string Name { get; set; }
        [StringLength(200, ErrorMessage = "描述长度必须为0-200个字符")]
        public string Description { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "数量必须大于等于1")]
        public int Num { get; set; }
    }

    public class ProductPackageUpdateModel
    {
        [Required(ErrorMessage = "必填信息")]
        public string Id { get; set; }
        [Required(ErrorMessage = "必填信息")]
        public string ProductSpecId { get; set; }
        [StringLength(50, MinimumLength = 1, ErrorMessage = "名称长度必须为1-50个字符")]
        public string Name { get; set; }
        [StringLength(200, ErrorMessage = "描述长度必须为0-200个字符")]
        public string Description { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "数量必须大于等于1")]
        public int Num { get; set; }
    }
}
