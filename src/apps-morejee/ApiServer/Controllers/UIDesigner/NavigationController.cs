using ApiModel.Entities;
using ApiServer.Controllers.Common;
using ApiServer.Filters;
using ApiServer.Models;
using ApiServer.Repositories;
using BambooCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ApiServer.Controllers.UIDesigner
{
    [Authorize]
    [Route("/[controller]")]
    public class NavigationController : ListableController<Navigation, NavigationDTO>
    {

        #region 构造函数
        public NavigationController(IRepository<Navigation, NavigationDTO> repository)
        : base(repository)
        {
        }
        #endregion

        #region Get 根据分页查询信息获取导航概要信息
        /// <summary>
        /// 根据分页查询信息获取导航概要信息
        /// </summary>
        /// <param name="model"></param>
        /// <param name="nodeTypes">节点类型(逗号分隔)</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedData<NavigationDTO>), 200)]
        public async Task<IActionResult> Get([FromQuery] PagingRequestModel model, string nodeTypes)
        {
            var advanceQuery = new Func<IQueryable<Navigation>, Task<IQueryable<Navigation>>>(async (query) =>
            {
                if (!string.IsNullOrWhiteSpace(nodeTypes))
                {
                    var typeArr = nodeTypes.Split(",").ToList();
                    query = query.Where(x => typeArr.Contains(x.NodeType));
                }
                return await Task.FromResult(query);
            });
            return await _GetPagingRequest(model, null, advanceQuery);
        }
        #endregion

        #region Get 根据id获取导航信息
        /// <summary>
        /// 根据id获取导航信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(NavigationDTO), 200)]
        public async Task<IActionResult> Get(string id)
        {
            return await _GetByIdRequest(id);
        }
        #endregion

        #region Get 获取用户导航菜单
        /// <summary>
        /// 获取用户导航菜单
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        [Route("GetByRole")]
        [HttpGet]
        public async Task<IActionResult> GetByRole(string role)
        {
            var navs = _Repository._DbContext.Navigations.Select(x => x.ToDTO());
            return await Task.FromResult(Ok(navs));
        }
        #endregion

        #region Post 新建导航信息
        /// <summary>
        /// 新建导航信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateModel]
        [ProducesResponseType(typeof(NavigationDTO), 200)]
        [ProducesResponseType(typeof(ValidationResultModel), 400)]
        public async Task<IActionResult> Post([FromBody]NavigationCreateModel model)
        {
            var mapping = new Func<Navigation, Task<Navigation>>(async (entity) =>
            {
                entity.Title = model.Title;
                entity.Name = model.Name;
                entity.Description = model.Description;
                entity.Icon = model.Icon;
                entity.Url = model.Url;
                entity.Field = model.Field;
                entity.Permission = model.Permission;
                entity.PagedModel = model.PagedModel;
                entity.Resource = model.Resource;
                entity.NodeType = model.NodeType;
                entity.QueryParams = model.QueryParams;
                return await Task.FromResult(entity);
            });
            return await _PostRequest(mapping);
        }
        #endregion

        #region Put 更新导航信息
        /// <summary>
        /// 更新导航信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [ValidateModel]
        [ProducesResponseType(typeof(NavigationDTO), 200)]
        [ProducesResponseType(typeof(ValidationResultModel), 400)]
        public async Task<IActionResult> Put([FromBody]NavigationEditModel model)
        {
            var mapping = new Func<Navigation, Task<Navigation>>(async (entity) =>
            {
                entity.Title = model.Title;
                entity.Name = model.Name;
                entity.Description = model.Description;
                entity.Icon = model.Icon;
                entity.Url = model.Url;
                entity.Field = model.Field;
                entity.Permission = model.Permission;
                entity.PagedModel = model.PagedModel;
                entity.Resource = model.Resource;
                entity.NodeType = model.NodeType;
                entity.QueryParams = model.QueryParams;
                return await Task.FromResult(entity);
            });
            return await _PutRequest(model.Id, mapping);
        }
        #endregion

    }
}