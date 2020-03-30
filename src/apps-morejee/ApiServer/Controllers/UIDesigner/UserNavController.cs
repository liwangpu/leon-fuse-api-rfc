using ApiModel.Entities;
using ApiServer.Controllers.Common;
using ApiServer.Filters;
using ApiServer.Models;
using ApiServer.Repositories;
using ApiServer.Services;
using BambooCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiServer.Controllers.UIDesigner
{
    
    [Route("/[controller]")]
    public class UserNavController : ListableController<UserNav, UserNavDTO>
    {
        #region 构造函数
        public UserNavController(IRepository<UserNav, UserNavDTO> repository)
         : base(repository)
        {
        }
        #endregion

        #region Get 根据分页查询信息获取角色导航概要信息
        /// <summary>
        /// 根据分页查询信息获取角色导航概要信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedData<UserNavDTO>), 200)]
        public async Task<IActionResult> Get([FromQuery] PagingRequestModel model)
        {
            return await _GetPagingRequest(model);
        }
        #endregion

        #region Get 根据id获取导航信息
        /// <summary>
        /// 根据id获取导航信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserNavDTO), 200)]
        public async Task<IActionResult> Get(string id)
        {
            return await _GetByIdRequest(id);
        }
        #endregion

        #region Post 新建角色导航信息
        /// <summary>
        /// 新建角色导航信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateModel]
        [ProducesResponseType(typeof(UserNavDTO), 200)]
        [ProducesResponseType(typeof(ValidationResultModel), 400)]
        public async Task<IActionResult> Post([FromBody]UserNavCreateModel model)
        {
            var mapping = new Func<UserNav, Task<UserNav>>(async (entity) =>
            {
                entity.Name = model.Name;
                entity.Description = model.Description;
                entity.Role = model.Role;
                return await Task.FromResult(entity);
            });
            return await _PostRequest(mapping);
        }
        #endregion

        #region Put 更新角色导航信息
        /// <summary>
        /// 更新角色导航信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [ValidateModel]
        [ProducesResponseType(typeof(UserNavDTO), 200)]
        [ProducesResponseType(typeof(ValidationResultModel), 400)]
        public async Task<IActionResult> Put([FromBody]UserNavEditModel model)
        {
            var mapping = new Func<UserNav, Task<UserNav>>(async (entity) =>
            {
                entity.Name = model.Name;
                entity.Description = model.Description;
                entity.Role = model.Role;
                return await Task.FromResult(entity);
            });
            return await _PutRequest(model.Id, mapping);
        }
        #endregion

        #region GetUserNav 获取当前用户导航菜单
        /// <summary>
        /// 获取用户导航菜单
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        [Route("GetUserNav")]
        [HttpGet]
        public async Task<IActionResult> GetUserNav(string role)
        {
            var accid = AuthMan.GetAccountId(this);
            var account = await _Repository._DbContext.Accounts.FirstOrDefaultAsync(x => x.Id == accid);
            var userNav = await _Repository._DbContext.UserNavs.Include(x => x.UserNavDetails).Where(x => x.Role == account.Type).FirstOrDefaultAsync();
            if (userNav == null) return Ok(new List<UserNavDetail>());
            var dto = await _Repository.GetByIdAsync(userNav.Id);
            return Ok(dto.UserNavDetails);
        }
        #endregion

        #region UpdateUserNavDetail 更新角色导航详细项信息
        /// <summary>
        /// 更新角色导航详细项信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("UpdateUserNavDetail")]
        [HttpPut]
        [ValidateModel]
        [ProducesResponseType(typeof(UserNavDTO), 200)]
        [ProducesResponseType(typeof(ValidationResultModel), 400)]
        public async Task<IActionResult> UpdateUserNavDetail([FromBody]UserNavDetailEditModel model)
        {
            var mapping = new Func<UserNav, Task<UserNav>>(async (entity) =>
            {
                var details = await _Repository._DbContext.UserNavDetails.Where(x => x.UserNav.Id == model.UserNavId).ToListAsync();
                var refDetail = !string.IsNullOrWhiteSpace(model.Id) ? details.Where(x => x.Id == model.Id).FirstOrDefault() : new UserNavDetail();
                if (refDetail == null)
                    refDetail = new UserNavDetail();


                if (string.IsNullOrWhiteSpace(model.Id))
                {
                    if (string.IsNullOrWhiteSpace(model.ParentId))
                        refDetail.Grade = details.Where(x => string.IsNullOrWhiteSpace(x.ParentId)).Count();
                    else
                        refDetail.Grade = details.Where(x => x.ParentId == model.ParentId).Count();
                }
                refDetail.UserNav = entity;
                refDetail.ParentId = model.ParentId;
                refDetail.RefNavigationId = model.RefNavigationId;
                refDetail.ExcludeFiled = model.ExcludeFiled;
                refDetail.ExcludePermission = model.ExcludePermission;
                refDetail.ExcludeQueryParams = model.ExcludeQueryParams;

                if (!string.IsNullOrWhiteSpace(model.Id))
                {
                    _Repository._DbContext.UserNavDetails.Update(refDetail);
                }
                else
                {
                    refDetail.Id = GuidGen.NewGUID();
                    _Repository._DbContext.UserNavDetails.Add(refDetail);
                }
                await _Repository._DbContext.SaveChangesAsync();
                return await Task.FromResult(entity);
            });
            return await _PutRequest(model.UserNavId, mapping);
        }
        #endregion

        #region DeleteUserNavDetail 删除角色导航详细项信息
        /// <summary>
        /// 删除角色导航详细项信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("DeleteUserNavDetail")]
        [HttpPut]
        [ValidateModel]
        [ProducesResponseType(typeof(UserNavDTO), 200)]
        [ProducesResponseType(typeof(ValidationResultModel), 400)]
        public async Task<IActionResult> DeleteUserNavDetail([FromBody]UserNavDetailDeleteModel model)
        {
            var mapping = new Func<UserNav, Task<UserNav>>(async (entity) =>
            {
                var details = await _Repository._DbContext.UserNavDetails.Where(x => x.UserNav.Id == model.UserNavId).ToListAsync();
                var deleteItem = details.Where(x => x.Id == model.Id).First();

                var deleteQueue = new Queue<UserNavDetail>();
                deleteQueue.Enqueue(deleteItem);

                do
                {
                    var it = deleteQueue.Dequeue();
                    var subs = details.Where(x => x.ParentId == it.Id).ToList();
                    subs.ForEach(t =>
                    {
                        deleteQueue.Enqueue(t);
                        details.Remove(t);
                        _Repository._DbContext.UserNavDetails.Remove(t);
                    });
                    details.Remove(it);
                    _Repository._DbContext.UserNavDetails.Remove(it);
                }
                while (deleteQueue.Count > 0);

                //重新排序同等级的导航栏信息
                var sameRankDetails = details.Where(x => x.ParentId == deleteItem.ParentId).OrderBy(x => x.Grade).ToList();
                for (int idx = sameRankDetails.Count - 1; idx >= 0; idx--)
                {
                    var item = sameRankDetails[idx];
                    item.Grade = idx;
                    _Repository._DbContext.UserNavDetails.Update(item);
                }

                await _Repository._DbContext.SaveChangesAsync();
                return await Task.FromResult(entity);
            });
            return await _PutRequest(model.UserNavId, mapping);
        }
        #endregion
    }
}