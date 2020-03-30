using System;
using System.ComponentModel.DataAnnotations;

namespace ApiModel.Entities
{
    /// <summary>
    /// 账号的第三方登陆信息
    /// </summary>
    public class AccountOpenId
    {
        [Key]
        /// <summary>
        /// 第三方平台的oauth openid，用来关联用户身份
        /// </summary>
        public string OpenId { get; set; }
        /// <summary>
        /// 平台名, google, facebook, qq, wechat等等
        /// </summary>
        public string Platform { get; set; }
        /// <summary>
        /// 创建绑定的时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        public string AccountId { get; set; }
        public Account Account { get; set; }
    }
}
