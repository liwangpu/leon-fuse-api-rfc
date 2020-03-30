using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ApiServer.Models
{
    public class AssetTransferDataModel
    {
        /// <summary>
        /// 目标账号
        /// </summary>
        [Required(ErrorMessage = "必填信息")]
        public string TargetAcc { get; set; }
        /// <summary>
        /// key=资源类型比如files,product,material, value=逗号连接的id数组字符串，比如"id1,id2,id3"
        /// </summary>
        [Required(ErrorMessage = "必填信息")]
        public Dictionary<string, string> Assets { get; set; }
    }

    public class AssetTransferDataGroupModel
    {
        [Required(ErrorMessage = "必填信息")]
        public List<AssetTransferDataModel> Groups { get; set; }
    }

}
