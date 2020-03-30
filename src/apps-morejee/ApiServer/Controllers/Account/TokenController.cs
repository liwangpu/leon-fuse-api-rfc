using ApiModel.Entities;
using ApiServer.Data;
using ApiServer.Filters;
using ApiServer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiServer.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("/Token")]
    public class TokenController : Controller
    {
        protected readonly AppConfig _AppConfig;
        protected readonly ApiDbContext _Context;

        #region 构造函数
        public TokenController(ApiDbContext context, IOptions<AppConfig> settingsOptions)
        {
            _Context = context;
            _AppConfig = settingsOptions.Value;
        }
        #endregion

        #region RequestToken Token请求
        /// <summary>
        /// Token请求
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [ValidateModel]
        [HttpPost]
        public async Task<IActionResult> RequestToken([FromBody]TokenRequestModel model)
        {
            var account = await _Context.Accounts.Include(x => x.Organization).FirstOrDefaultAsync(x => x.Mail.ToLower() == model.Account.ToLower() || x.Phone == model.Account);
            if (account == null)
                return BadRequest(new ErrorRespondModel() { Message = "用户名或者密码有误" });

            if (account.Password != model.Password)
                return BadRequest(new ErrorRespondModel() { Message = "用户名或者密码有误" });

            if (account.Frozened)
                return BadRequest(new ErrorRespondModel() { Message = "账户已被冻结" });

            var now = DateTime.UtcNow;
            if (now < account.ActivationTime.AddDays(-1))
                return BadRequest(new ErrorRespondModel() { Message = "账户未启用" });

            if (now > account.Organization.ExpireTime)
            {
                return BadRequest(new ErrorRespondModel() { Message = "账户已失效" });
            }
            else
            {
                //普通用户,过期时间在组织过期时间之内
                if (account.Organization.OwnerId != account.Id)
                {
                    if (now > account.ExpireTime)
                        return BadRequest(new ErrorRespondModel() { Message = "账户已失效" });

                }
            }


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_AppConfig.JwtSettings.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            //var claims = new[] { new Claim(ClaimTypes.Name, account.Id) };

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, account.Id)
                , new Claim(ClaimTypes.Role, account.Type)
                , new Claim("OrganizationId", account.OrganizationId)
            };


            var expires = DateTime.Now.AddDays(_AppConfig.JwtSettings.ExpiresDay);
            var token = new JwtSecurityToken(
                issuer: _AppConfig.JwtSettings.Issuer,
                audience: _AppConfig.JwtSettings.Audience,
                claims: claims,
                notBefore: DateTime.Now,
                expires: expires,
                signingCredentials: creds);

            return Ok(new { Token = new JwtSecurityTokenHandler().WriteToken(token), Expires = expires.ToString("yyyy-MM-dd HH:mm:ss") });
        }
        #endregion
    }

}
