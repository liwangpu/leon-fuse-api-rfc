using ApiModel.Consts;
using ApiModel.Entities;
using ApiModel.Enums;
using ApiServer.Controllers.Common;
using ApiServer.Filters;
using ApiServer.Models;
using ApiServer.Repositories;
using ApiServer.Services;
using BambooCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace ApiServer.Controllers
{
    [Authorize]
    [Route("/[controller]")]
    public class MapController : ListableController<Map, MapDTO>
    {
        private AppConfig appConfig;

        #region 构造函数
        public MapController(IRepository<Map, MapDTO> repository, IOptions<AppConfig> settingsOptions)
        : base(repository)
        {
            appConfig = settingsOptions.Value;
        }
        #endregion

        #region Get 根据分页查询信息获取地图概要信息
        /// <summary>
        /// 根据分页查询信息获取地图概要信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedData<MapDTO>), 200)]
        public async Task<IActionResult> Get([FromQuery] PagingRequestModel model)
        {
            var qMapping = new Action<List<string>>((query) =>
            {

            });
            return await _GetPagingRequest(model, qMapping);
        }
        #endregion

        #region Get 根据id获取地图信息
        /// <summary>
        /// 根据id获取地图信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(MapDTO), 200)]
        public async Task<IActionResult> Get(string id)
        {
            return await _GetByIdRequest(id);
        }
        #endregion

        #region Post 新建地图信息
        /// <summary>
        /// 新建地图信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateModel]
        [ProducesResponseType(typeof(MapDTO), 200)]
        [ProducesResponseType(typeof(ValidationResultModel), 400)]
        public async Task<IActionResult> Post([FromBody]MapCreateModel model)
        {
            var mapping = new Func<Map, Task<Map>>(async (entity) =>
            {
                entity.Name = model.Name;
                entity.Description = model.Description;
                entity.Icon = model.IconAssetId;
                entity.PackageName = model.PackageName;
                entity.UnCookedAssetId = model.UnCookedAssetId;
                entity.FileAssetId = model.FileAssetId;
                entity.Dependencies = model.Dependencies;
                entity.Properties = model.Properties;
                entity.Color = model.Color;
                entity.ResourceType = (int)ResourceTypeEnum.Organizational;

                return await Task.FromResult(entity);
            });
            return await _PostRequest(mapping);
        }
        #endregion

        #region Put 更新地图信息
        /// <summary>
        /// 更新地图信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [ValidateModel]
        [ProducesResponseType(typeof(MapDTO), 200)]
        [ProducesResponseType(typeof(ValidationResultModel), 400)]
        public async Task<IActionResult> Put([FromBody]MapEditModel model)
        {
            var mapping = new Func<Map, Task<Map>>(async (entity) =>
            {
                entity.Name = model.Name;
                entity.Description = model.Description;
                entity.PackageName = model.PackageName;
                entity.UnCookedAssetId = model.UnCookedAssetId;
                entity.FileAssetId = model.FileAssetId;
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

        #region TransToLayout 地图生成户型
        /// <summary>
        /// 地图生成户型
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("TransferToLayout")]
        public async Task<IActionResult> TransToLayout([FromBody] MapTransLayoutModel model)
        {
            var map = await _Repository._GetByIdAsync(model.MapId);
            var accid = AuthMan.GetAccountId(this);
            var layout = new Layout();
            layout.Id = GuidGen.NewGUID();
            layout.Name = map.Name;
            layout.Description = string.Format("auto generate at {0}", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
            layout.OrganizationId = map.OrganizationId;
            layout.Icon = map.Icon;
            layout.Creator = accid;
            layout.Modifier = accid;
            layout.CreatedTime = DateTime.Now;
            layout.ModifiedTime = DateTime.Now;
            layout.ActiveFlag = AppConst.I_DataState_Active;
            var layoutData = new MapTransLayoutDataModel();
            layoutData.Map = layout.Name;
            layoutData.MapId = map.Id;
            layout.Data = JsonConvert.SerializeObject(layoutData);
            _Repository._DbContext.Layouts.Add(layout);
            await _Repository._DbContext.SaveChangesAsync();
            return Ok();
        } 
        #endregion
    }
}