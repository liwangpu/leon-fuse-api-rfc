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
namespace ApiServer.Controllers.Design
{
    /// <summary>
    /// 贴图管理控制器
    /// </summary>
    [Authorize]
    [Route("/[controller]")]
    public class TextureController : ListableController<Texture, TextureDTO>
    {
        #region 构造函数
        public TextureController(IRepository<Texture, TextureDTO> repository)
            : base(repository)
        {
        }
        #endregion

        #region Get 根据分页查询信息获取贴图概要信息
        /// <summary>
        /// 根据分页查询信息获取贴图概要信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedData<TextureDTO>), 200)]
        public async Task<IActionResult> Get([FromQuery] PagingRequestModel model)
        {
            var qMapping = new Action<List<string>>((query) =>
            {

            });
            return await _GetPagingRequest(model, qMapping);
        }
        #endregion

        #region Get 根据id获取贴图信息
        /// <summary>
        /// 根据id获取贴图信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TextureDTO), 200)]
        public async Task<IActionResult> Get(string id)
        {
            return await _GetByIdRequest(id);
        }
        #endregion

        #region Post 新建贴图信息
        /// <summary>
        /// 新建贴图信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateModel]
        [ProducesResponseType(typeof(TextureDTO), 200)]
        [ProducesResponseType(typeof(ValidationResultModel), 400)]
        public async Task<IActionResult> Post([FromBody]TextureCreateModel model)
        {
            var mapping = new Func<Texture, Task<Texture>>(async (entity) =>
            {
                entity.Name = model.Name;
                entity.Description = model.Description;
                entity.Icon = model.IconAssetId;
                entity.FileAssetId = model.FileAssetId;
                entity.PackageName = model.PackageName;
                entity.UnCookedAssetId = model.UnCookedAssetId;
                entity.Dependencies = model.Dependencies;
                entity.Properties = model.Properties;
                entity.Color = model.Color;
                entity.ResourceType = (int)ResourceTypeEnum.Organizational;
                return await Task.FromResult(entity);
            });
            return await _PostRequest(mapping);
        }
        #endregion

        #region Put 更新贴图信息
        /// <summary>
        /// 更新贴图信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [ValidateModel]
        [ProducesResponseType(typeof(TextureDTO), 200)]
        [ProducesResponseType(typeof(ValidationResultModel), 400)]
        public async Task<IActionResult> Put([FromBody]TextureEditModel model)
        {
            var mapping = new Func<Texture, Task<Texture>>(async (entity) =>
            {
                entity.Name = model.Name;
                entity.Description = model.Description;
                entity.PackageName = model.PackageName;
                entity.FileAssetId = model.FileAssetId;
                entity.UnCookedAssetId = model.UnCookedAssetId;
                if (!string.IsNullOrWhiteSpace(model.IconAssetId))
                    entity.Icon = model.IconAssetId;
                entity.Dependencies = model.Dependencies;
                entity.Properties = model.Properties;
                entity.Color = model.Color;
                return await Task.FromResult(entity);
            });
            return await _PutRequest(model.Id, mapping);
        }
        #endregion

    }
}