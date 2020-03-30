using ApiModel.Entities;
using ApiModel.Enums;
using ApiServer.Controllers.Common;
using ApiServer.Filters;
using ApiServer.Models;
using ApiServer.Repositories;
using BambooCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiServer.Controllers
{
    /// <summary>
    /// 户型管理控制器
    /// </summary>
    [Authorize]
    [Route("/[controller]")]
    public class LayoutController : ListableController<Layout, LayoutDTO>
    {
        #region 构造函数
        public LayoutController(IRepository<Layout, LayoutDTO> repository)
        : base(repository)
        { }
        #endregion

        #region Get 根据分页查询信息获取户型概要信息
        /// <summary>
        /// 根据分页查询信息获取户型概要信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedData<LayoutDTO>), 200)]
        public async Task<IActionResult> Get([FromQuery] PagingRequestModel model)
        {
            var qMapping = new Action<List<string>>((query) =>
            {

            });
            return await _GetPagingRequest(model, qMapping);
        }
        #endregion

        #region Get 根据id获取户型信息
        /// <summary>
        /// 根据id获取户型信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(LayoutDTO), 200)]
        public async Task<IActionResult> Get(string id)
        {
            return await _GetByIdRequest(id);
        }
        #endregion

        #region Post 新建户型信息
        /// <summary>
        /// 新建户型信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateModel]
        [ProducesResponseType(typeof(LayoutDTO), 200)]
        [ProducesResponseType(typeof(ValidationResultModel), 400)]
        public async Task<IActionResult> Post([FromBody]LayoutCreateModel model)
        {
            var mapping = new Func<Layout, Task<Layout>>(async (entity) =>
            {
                entity.Name = model.Name;
                entity.Description = model.Description;
                entity.Icon = model.IconAssetId;
                entity.CategoryId = model.CategoryId;
                entity.Data = model.Data;
                entity.Color = model.Color;
                entity.ResourceType = (int)ResourceTypeEnum.Organizational;
                return await Task.FromResult(entity);
            });
            return await _PostRequest(mapping);
        }
        #endregion

        #region Put 更新户型信息
        /// <summary>
        /// 更新户型信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [ValidateModel]
        [ProducesResponseType(typeof(LayoutDTO), 200)]
        [ProducesResponseType(typeof(ValidationResultModel), 400)]
        public async Task<IActionResult> Put([FromBody]LayoutEditModel model)
        {
            var mapping = new Func<Layout, Task<Layout>>(async (entity) =>
            {
                entity.Name = model.Name;
                entity.Description = model.Description;
                if (!string.IsNullOrWhiteSpace(model.IconAssetId))
                    entity.Icon = model.IconAssetId;
                entity.CategoryId = model.CategoryId;
                entity.Data = model.Data;
                entity.Color = model.Color;
                return await Task.FromResult(entity);
            });
            return await _PutRequest(model.Id, mapping);
        }
        #endregion

        #region UpdateData 更新Data数据信息
        /// <summary>
        /// 更新Data数据信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        [Route("UpdateData")]
        [HttpPut]
        public async Task<IActionResult> UpdateData(string id, [FromBody]string data)
        {
            var mapping = new Func<Layout, Task<Layout>>(async (entity) =>
            {
                entity.Data = data;
                return await Task.FromResult(entity);
            });
            return await _PutRequest(id, mapping);
        }
        #endregion

    }
}
