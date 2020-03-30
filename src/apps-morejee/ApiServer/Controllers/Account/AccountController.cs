using ApiModel.Consts;
using ApiModel.Entities;
using ApiServer.Controllers.Common;
using ApiServer.Filters;
using ApiServer.Models;
using ApiServer.Repositories;
using ApiServer.Services;
using BambooCommon;
using BambooCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiServer.Controllers
{
    /// <summary>
    /// 账户管理控制器
    /// </summary>
    [Authorize]
    [Route("/[controller]")]
    public class AccountController : ListableController<Account, AccountDTO>
    {

        #region 构造函数
        public AccountController(IRepository<Account, AccountDTO> repository)
        : base(repository)
        {
        }
        #endregion

        #region _GetAccountType 获取用户类型信息
        /// <summary>
        /// 获取用户类型信息
        /// </summary>
        /// <param name="isAdmin"></param>
        /// <param name="accountId"></param>
        /// <returns></returns>
        protected async Task<string> _GetAccountType(bool isAdmin, string accountId = "")
        {
            if (string.IsNullOrWhiteSpace(accountId))
                accountId = AuthMan.GetAccountId(this);
            var account = await _Repository._DbContext.Accounts.Include(x => x.Organization).FirstOrDefaultAsync(x => x.Id == accountId);
            if (account != null && account.Organization != null)
            {
                if (isAdmin)
                    return $"{account.Organization.OrganizationTypeId}admin";
                else
                    return $"{account.Organization.OrganizationTypeId}member";
            }
            return string.Empty;
        }
        #endregion

        #region Get 根据分页获取用户信息
        /// <summary>
        /// 根据分页获取用户信息
        /// </summary>
        /// <param name="model"></param>
        /// <param name="departmentId"></param>
        /// <param name="ignoreOwner"></param>
        /// <param name="currentOrganAccount"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedData<AccountDTO>), 200)]
        public async Task<IActionResult> Get([FromQuery] PagingRequestModel model, string departmentId, bool ignoreOwner = false, bool currentOrganAccount = true)
        {
            var organId = await _GetCurrentUserOrganId();

            var advanceQuery = new Func<IQueryable<Account>, Task<IQueryable<Account>>>(async (query) =>
            {
                if (!string.IsNullOrWhiteSpace(departmentId))
                {
                    query = query.Where(x => x.DepartmentId == departmentId);
                }
                if (ignoreOwner)
                {
                    var organ = await _Repository._DbContext.Organizations.FindAsync(organId);
                    if (organ != null)
                    {
                        query = query.Where(x => x.Id != organ.OwnerId);
                    }
                }
                if (currentOrganAccount)
                {
                    query = query.Where(x => x.OrganizationId == organId);
                }
                query = query.Where(x => x.ActiveFlag == AppConst.I_DataState_Active);
                return query;
            });
            return await _GetPagingRequest(model, null, advanceQuery);
        }
        #endregion

        #region Get 根据Id获取用户信息
        /// <summary>
        /// 根据Id获取用户信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AccountDTO), 200)]
        public async Task<IActionResult> Get(string id)
        {
            return await _GetByIdRequest(id);
        }
        #endregion

        #region GetNameById 根据Id获取用户姓名
        /// <summary>
        /// 根据Id获取用户姓名
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("GetNameById")]
        [ProducesResponseType(typeof(string), 200)]
        public async Task<IActionResult> GetNameById(string id)
        {
            var acc = await _Repository._DbContext.Accounts.FirstOrDefaultAsync(x => x.Id == id);
            return Ok(acc != null ? acc.Name : string.Empty);
        }
        #endregion

        #region GetNameByIds 根据Ids获取用户姓名集合
        /// <summary>
        /// 根据Ids获取用户姓名集合
        /// </summary>
        /// <param name="ids">逗号分隔的id</param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("GetNameByIds")]
        [ProducesResponseType(typeof(List<string>), 200)]
        public async Task<IActionResult> GetNameByIds(string ids)
        {
            ids = string.IsNullOrWhiteSpace(ids) ? string.Empty : ids;
            var names = new List<string>();
            var idArr = ids.Split(",", StringSplitOptions.RemoveEmptyEntries);
            foreach (var id in idArr)
            {
                var acc = await _Repository._DbContext.Accounts.FirstOrDefaultAsync(x => x.Id == id);
                names.Add(acc != null ? acc.Name : string.Empty);
            }
            return Ok(names);
        }
        #endregion

        #region Post 新建用户
        /// <summary>
        /// 新建用户
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateModel]
        [ProducesResponseType(typeof(AccountDTO), 200)]
        [ProducesResponseType(typeof(ValidationResultModel), 400)]
        public async Task<IActionResult> Post([FromBody]RegisterAccountModel model)
        {
            var mapping = new Func<Account, Task<Account>>(async (entity) =>
            {
                if (string.IsNullOrWhiteSpace(model.OrganizationId))
                    model.OrganizationId = await _GetCurrentUserOrganId();

                entity.Name = model.Name;
                entity.Description = model.Description;
                if (!string.IsNullOrWhiteSpace(model.Password))
                    entity.Password = Md5.CalcString(model.Password);
                if (!string.IsNullOrWhiteSpace(model.DepartmentId))
                    entity.DepartmentId = model.DepartmentId;
                entity.Mail = model.Mail;
                entity.Frozened = false;
                var t1 = DataHelper.ParseDateTime(model.ActivationTime);
                var t2 = DataHelper.ParseDateTime(model.ExpireTime);
                entity.ActivationTime = t1 != DateTime.MinValue ? t1 : DateTime.Now;
                entity.ExpireTime = t2 != DateTime.MinValue ? t2 : DateTime.Now.AddYears(10);
                entity.Type = await _GetAccountType(model.IsAdmin);
                entity.Location = model.Location;
                entity.Phone = model.Phone;
                entity.Mail = model.Mail;
                entity.OrganizationId = model.OrganizationId;

                return await Task.FromResult(entity);
            });
            return await _PostRequest(mapping);
        }
        #endregion

        #region Put 更新用户信息
        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [ValidateModel]
        [ProducesResponseType(typeof(AccountDTO), 200)]
        [ProducesResponseType(typeof(ValidationResultModel), 400)]
        public async Task<IActionResult> Put([FromBody]AccountEditModel model)
        {
            var mapping = new Func<Account, Task<Account>>(async (entity) =>
            {
                if (!string.IsNullOrWhiteSpace(model.Name))
                    entity.Name = model.Name;
                if (!string.IsNullOrWhiteSpace(model.Description))
                    entity.Description = model.Description;
                if (!string.IsNullOrWhiteSpace(model.Password))
                    entity.Password = Md5.CalcString(model.Password);
                if (!string.IsNullOrWhiteSpace(model.ActivationTime))
                    entity.ActivationTime = DataHelper.ParseDateTime(model.ActivationTime);
                if (!string.IsNullOrWhiteSpace(model.ExpireTime))
                    entity.ExpireTime = DataHelper.ParseDateTime(model.ExpireTime);

                if (!string.IsNullOrWhiteSpace(model.DepartmentId))
                {
                    entity.DepartmentId = model.DepartmentId;
                }
                //else
                //{
                //    entity.DepartmentId = null;
                //    entity.Department = null;
                //}

                entity.Mail = model.Mail;
                entity.Location = model.Location;
                entity.Phone = model.Phone;

                entity.Type = await _GetAccountType(model.IsAdmin, model.Id);
                return await Task.FromResult(entity);
            });
            return await _PutRequest(model.Id, mapping);
        }
        #endregion

        #region ChangePassword 修改密码
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("ChangePassword")]
        [HttpPut]
        [ValidateModel]
        [ProducesResponseType(typeof(ValidationResultModel), 400)]
        public async Task<IActionResult> ChangePassword([FromBody]NewPasswordModel model)
        {
            var accid = AuthMan.GetAccountId(this);
            Account acc = await _Repository._DbContext.Accounts.FindAsync(accid);
            if (acc.Password != model.OldPassword)
                ModelState.AddModelError("Password", "原密码输入有误");
            if (!ModelState.IsValid)
                return new ValidationFailedResult(ModelState);
            acc.Password = model.NewPassword;
            _Repository._DbContext.Update(acc);
            await _Repository._DbContext.SaveChangesAsync();
            return Ok();
        }
        #endregion

        #region ResetPassword 重置密码
        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("ResetPassword")]
        [HttpPut]
        [ValidateModel]
        [ProducesResponseType(typeof(ValidationResultModel), 400)]
        public async Task<IActionResult> ResetPassword([FromBody]ResetPasswordModel model)
        {
            var accid = AuthMan.GetAccountId(this);
            Account acc = await _Repository._DbContext.Accounts.FindAsync(model.UserId);
            acc.Password = model.Password;
            _Repository._DbContext.Update(acc);
            await _Repository._DbContext.SaveChangesAsync();
            return Ok();
        }
        #endregion

        #region GetProfile 获取账号信息
        /// <summary>
        /// 获取账号信息
        /// </summary>
        /// <returns></returns>
        [Route("Profile")]
        [HttpGet]
        [ProducesResponseType(typeof(AccountDTO), 200)]
        public async Task<IActionResult> GetProfile()
        {
            var accid = AuthMan.GetAccountId(this);
            var exist = await _Repository._DbContext.Accounts.AnyAsync(x => x.Id == accid);
            if (!exist) return NotFound();
            return await Get(accid);
        }
        #endregion

        #region UpdateAdditionalRole 更新用户附加角色信息
        /// <summary>
        /// 更新用户附加角色信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("UpdateAdditionalRole")]
        [HttpPut]
        [ValidateModel]
        [ProducesResponseType(typeof(ValidationResultModel), 400)]
        public async Task<IActionResult> UpdateAdditionalRole([FromBody]UserAdditionalRoleEditModel model)
        {
            var accid = AuthMan.GetAccountId(this);
            Account acc = await _Repository._DbContext.Accounts.Where(x => x.Id == model.UserId).FirstOrDefaultAsync();
            //清除之前的角色信息
            var originRoles = _Repository._DbContext.AccountRoles.Where(x => x.Account == acc);
            foreach (var originItem in originRoles)
            {
                _Repository._DbContext.Remove(originItem);
            }
            await _Repository._DbContext.SaveChangesAsync();

            var newRoles = new List<AccountRole>();
            var roleIdArr = model.AdditionalRoleIds.Split(",", StringSplitOptions.RemoveEmptyEntries).Distinct().ToList();
            foreach (var roleId in roleIdArr)
            {
                var newRole = new AccountRole();
                newRole.Id = GuidGen.NewGUID();
                newRole.Account = acc;
                newRole.UserRoleId = roleId;
                newRoles.Add(newRole);
            }
            acc.AdditionRoles = newRoles;
            _Repository._DbContext.Update(acc);
            await _Repository._DbContext.SaveChangesAsync();
            return Ok();
        }
        #endregion

        #region GetNavigationData 获取这个账号的网站后台导航菜单配置
        /// <summary>
        /// 获取这个账号的网站后台导航菜单配置
        /// </summary>
        /// <returns></returns>
        [Route("Navigation")]
        [HttpGet]
        [ProducesResponseType(typeof(NavigationModel), 200)]
        public async Task<NavigationModel> GetNavigationData()
        {
            var accid = AuthMan.GetAccountId(this);
            Account acc = await _Repository._DbContext.Accounts.FindAsync(accid);
            if (acc != null)
            {
                NavigationModel mm;
                if (SiteConfig.Instance.GetItem("navi_" + acc.Type, out mm))
                    return mm;
            }
            return null;
        }
        #endregion
    }
}