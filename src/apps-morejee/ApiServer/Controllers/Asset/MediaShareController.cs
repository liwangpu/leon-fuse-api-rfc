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

namespace ApiServer.Controllers.Asset
{
    
    [Route("/[controller]")]
    public class MediaShareController : CommonController<MediaShareResource, MediaShareResourceDTO>
    {
        #region 构造函数
        public MediaShareController(IRepository<MediaShareResource, MediaShareResourceDTO> repository)
        : base(repository)
        {

        }
        #endregion

        #region Get 根据分页查询信息获取媒体文件概要信息
        /// <summary>
        /// 根据分页查询信息获取媒体文件概要信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedData<MediaShareResourceDTO>), 200)]
        public async Task<IActionResult> Get([FromQuery] PagingRequestModel model)
        {
            var qMapping = new Action<List<string>>((query) =>
            {

            });
            return await _GetPagingRequest(model, qMapping);
        }
        #endregion

        #region Get 根据id获取媒体文件信息
        /// <summary>
        /// 根据id获取媒体文件信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(MediaShareResourceDTO), 200)]
        public async Task<IActionResult> Get(string id)
        {
            //var exist = await _Store.ExistAsync(id);
            //if (!exist)
            //    return NotFound();
            //var dto = await _Store.GetByIdAsync(id);
            //var curUtcTime = DateTime.UtcNow;
            //var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            //if (curUtcTime >= dtDateTime.AddSeconds(dto.StartShareTimeStamp).ToLocalTime())
            //{
            //    if (dto.StopShareTimeStamp > 0)
            //    {
            //        if (curUtcTime > dtDateTime.AddSeconds(dto.StopShareTimeStamp).ToLocalTime())
            //            return NotFound();
            //    }
            //}
            //return Ok(dto);
            return await _GetByIdRequest(id);
        }
        #endregion

        #region Post 新建媒体文件信息
        /// <summary>
        /// 新建媒体文件信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateModel]
        [ProducesResponseType(typeof(MediaShareResourceDTO), 200)]
        [ProducesResponseType(typeof(ValidationResultModel), 400)]
        public async Task<IActionResult> Post([FromBody]MediaShareResourceCreateModel model)
        {
            var mapping = new Func<MediaShareResource, Task<MediaShareResource>>(async (entity) =>
            {
                entity.Name = model.Name;
                entity.Description = model.Description;
                entity.MediaId = model.MediaId;
                entity.StartShareTimeStamp = model.StartShareTimeStamp;
                entity.StopShareTimeStamp = model.StopShareTimeStamp;
                entity.Password = model.Password;
                entity.ResourceType = (int)ResourceTypeEnum.NoLimit;
                return await Task.FromResult(entity);
            });
            return await _PostRequest(mapping);
        }
        #endregion

        #region Put 更新媒体文件信息
        /// <summary>
        /// 更新媒体文件信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [ValidateModel]
        [ProducesResponseType(typeof(MediaShareResourceDTO), 200)]
        [ProducesResponseType(typeof(ValidationResultModel), 400)]
        public async Task<IActionResult> Put([FromBody]MediaShareResourceEditModel model)
        {
            var mapping = new Func<MediaShareResource, Task<MediaShareResource>>(async (entity) =>
            {
                entity.Name = model.Name;
                entity.Description = model.Description;
                entity.StartShareTimeStamp = model.StartShareTimeStamp;
                entity.StopShareTimeStamp = model.StopShareTimeStamp;
                entity.Password = model.Password;
                return await Task.FromResult(entity);
            });
            return await _PutRequest(model.Id, mapping);
        }
        #endregion

        [AllowAnonymous]
        [Route("ViewShare")]
        [ValidateModel]
        [HttpPost]
        [ProducesResponseType(typeof(MediaShareResourceDTO), 200)]
        public async Task<IActionResult> ViewShare([FromBody] MediaShareRequestModel model)
        {
            var exist = await _Repository.ExistAsync(model.Id);
            if (!exist)
                return NotFound();
            var entity = await _Repository._GetByIdAsync(model.Id);
            var curUtcTime = DateTime.UtcNow;
            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            if (curUtcTime >= dtDateTime.AddSeconds(entity.StartShareTimeStamp).ToLocalTime())
            {
                if (entity.StopShareTimeStamp > 0)
                {
                    if (curUtcTime > dtDateTime.AddSeconds(entity.StopShareTimeStamp).ToLocalTime())
                        return NotFound();
                }

                if (!string.IsNullOrWhiteSpace(entity.Password))
                {
                    if (!string.IsNullOrWhiteSpace(model.Password))
                    {
                        if (entity.Password.Trim() != model.Password.Trim())
                            return Forbid();
                    }
                    else
                        return Forbid();
                }
            }
            var dto = await _Repository.GetByIdAsync(model.Id);
            return Ok(dto);
        }

    }
}