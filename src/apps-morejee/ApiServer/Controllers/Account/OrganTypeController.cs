using ApiModel.Consts;
using ApiModel.Entities;
using ApiServer.Controllers.Common;
using ApiServer.Filters;
using ApiServer.Models;
using ApiServer.Repositories;
using BambooCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;


namespace ApiServer.Controllers
{
    
    [Route("/[controller]")]
    public class OrganTypeController : ListableController<OrganizationType, OrganizationTypeDTO>
    {
        #region 构造函数
        public OrganTypeController(IRepository<OrganizationType, OrganizationTypeDTO> repository)
         : base(repository)
        {
        }
        #endregion

        #region Get 根据分页查询信息获取组织类型信息
        /// <summary>
        /// 根据分页查询信息获取组织类型信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedData<OrganizationTypeDTO>), 200)]
        public async Task<IActionResult> Get([FromQuery] PagingRequestModel model)
        {
            return await _GetPagingRequest(model);
        }
        #endregion

        #region Get 根据id获取组织类型信息
        /// <summary>
        /// 根据id获取组织类型信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(OrganizationTypeDTO), 200)]
        public async Task<IActionResult> Get(string id)
        {
            return await _GetByIdRequest(id);
        }
        #endregion

        #region Post 新建组织类型信息
        /// <summary>
        /// 新建组织类型信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateModel]
        [ProducesResponseType(typeof(OrganizationTypeDTO), 200)]
        [ProducesResponseType(typeof(ValidationResultModel), 400)]
        public async Task<IActionResult> Post([FromBody]OrganizationTypeCreateModel model)
        {
            var mapping = new Func<OrganizationType, Task<OrganizationType>>(async (entity) =>
            {
                entity.Name = model.Name;
                entity.Description = model.Description;
                return await Task.FromResult(entity);
            });
            return await _PostRequest(mapping);
        }
        #endregion

        #region Put 更新组织类型信息
        /// <summary>
        /// 更新组织类型信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [ValidateModel]
        [ProducesResponseType(typeof(OrganizationTypeDTO), 200)]
        [ProducesResponseType(typeof(ValidationResultModel), 400)]
        public async Task<IActionResult> Put([FromBody]OrganizationTypeEditModel model)
        {
            var mapping = new Func<OrganizationType, Task<OrganizationType>>(async (entity) =>
            {
                entity.Name = model.Name;
                entity.Description = model.Description;
                return await Task.FromResult(entity);
            });
            return await _PutRequest(model.Id, mapping);
        }
        #endregion
    }
}