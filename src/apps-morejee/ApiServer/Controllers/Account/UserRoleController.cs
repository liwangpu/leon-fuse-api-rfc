using ApiModel.Consts;
using ApiModel.Entities;
using ApiServer.Controllers.Common;
using ApiServer.Data;
using ApiServer.Filters;
using ApiServer.Models;
using ApiServer.Repositories;
using BambooCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ApiServer.Controllers
{
    
    [Route("/[controller]")]
    public class UserRoleController : ListableController<UserRole, UserRoleDTO>
    {
        protected readonly ApiDbContext _Context;

        #region 构造函数
        public UserRoleController(IRepository<UserRole, UserRoleDTO> repository)
        : base(repository)
        {

        }
        #endregion

        #region Get 根据分页查询信息获取用户角色概要信息
        /// <summary>
        /// 根据分页查询信息获取用户角色概要信息
        /// </summary>
        /// <param name="model"></param>
        /// <param name="organType"></param>
        /// <param name="excludeInner"></param>
        /// <param name="innerOnly"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedData<UserRoleDTO>), 200)]
        public async Task<IActionResult> Get([FromQuery] PagingRequestModel model, string organType, bool excludeInner = false, bool innerOnly = false)
        {
            var advanceQuery = new Func<IQueryable<UserRole>, Task<IQueryable<UserRole>>>(async (query) =>
            {
                if (!string.IsNullOrWhiteSpace(organType))
                    query = query.Where(x => x.ApplyOrgans.Contains(organType));

                if (excludeInner)
                    query = query.Where(x => x.IsInner == false);

                if (innerOnly)
                    query = query.Where(x => x.IsInner == true);

                query = query.Where(x => x.ActiveFlag == AppConst.I_DataState_Active);
                return await Task.FromResult(query);
            });
            return await _GetPagingRequest(model, null, advanceQuery);
        }
        #endregion

        #region Get 根据id获取用户角色信息
        /// <summary>
        /// 根据id获取用户角色信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserRoleDTO), 200)]
        public async Task<IActionResult> Get(string id)
        {
            return await _GetByIdRequest(id);
        }
        #endregion

        #region Get 获取用户角色信息
        ///// <summary>
        ///// 获取用户角色信息
        ///// </summary>
        ///// <param name="organType"></param>
        ///// <returns></returns>
        //[HttpGet]
        //public async Task<IActionResult> Get(string organType)
        //{
        //    var roles = new List<UserRole>();
        //    if (!string.IsNullOrWhiteSpace(organType))
        //        roles = await _Context.UserRoles.Where(x => x.Organization == organType).ToListAsync();
        //    else
        //        roles = await _Context.UserRoles.ToListAsync();
        //    return Ok(roles);
        //}
        #endregion

        #region Post 新建用户角色信息
        /// <summary>
        /// 新建用户角色信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateModel]
        [ProducesResponseType(typeof(UserRoleDTO), 200)]
        [ProducesResponseType(typeof(ValidationResultModel), 400)]
        public async Task<IActionResult> Post([FromBody]UserRoleCreateModel model)
        {
            var mapping = new Func<UserRole, Task<UserRole>>(async (entity) =>
            {
                var organId = await _GetCurrentUserOrganId();
                var organType = await _Repository._DbContext.Organizations.Where(x => x.Id == organId).Select(x => x.OrganizationTypeId).FirstOrDefaultAsync();
                entity.Name = model.Name;
                entity.Description = model.Description;
                entity.Icon = model.IconAssetId;
                entity.ApplyOrgans = organType;
                entity.OrganizationId = organId;
                return await Task.FromResult(entity);
            });
            return await _PostRequest(mapping);
        }
        #endregion

        #region Put 更新用户角色信息
        /// <summary>
        /// 更新用户角色信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [ValidateModel]
        [ProducesResponseType(typeof(UserRoleDTO), 200)]
        [ProducesResponseType(typeof(ValidationResultModel), 400)]
        public async Task<IActionResult> Put([FromBody]UserRoleEditModel model)
        {
            var mapping = new Func<UserRole, Task<UserRole>>(async (entity) =>
            {
                var organId = await _GetCurrentUserOrganId();
                var organType = await _Repository._DbContext.Organizations.Where(x => x.Id == organId).Select(x => x.OrganizationTypeId).FirstOrDefaultAsync();
                entity.Name = model.Name;
                entity.Description = model.Description;
                entity.Icon = model.IconAssetId;
                entity.ApplyOrgans = organType;
                entity.OrganizationId = organId;
                return await Task.FromResult(entity);
            });
            return await _PutRequest(model.Id, mapping);
        }
        #endregion

    }
}