using System.ComponentModel.DataAnnotations;

namespace Apps.OMS.Export.Models
{
    public class CustomerCreateModel
    {
        [StringLength(50, MinimumLength = 1, ErrorMessage = "长度必须为1-50个字符")]
        public string Name { get; set; }
        [StringLength(200, ErrorMessage = "长度必须为0-200个字符")]
        public string Description { get; set; }
        public string Mail { get; set; }
        public string Telephone { get; set; }
        public string Cellphone { get; set; }
        public string Address { get; set; }
    }

}
