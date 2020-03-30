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
using System.Linq;
using ApiModel.Consts;
using Microsoft.EntityFrameworkCore;
using ApiServer.Services;
using System.Collections.Generic;

namespace ApiServer.Controllers
{
    /// <summary>
    /// 部门管理控制器
    /// </summary>
    [Authorize]
    [Route("/[controller]")]
    public class DepartmentController : ListableController<Department, DepartmentDTO>
    {

        #region 构造函数
        public DepartmentController(IRepository<Department, DepartmentDTO> repository)
        : base(repository)
        {
        }
        #endregion

        #region Get 根据分页查询信息获取部门导航概要信息
        /// <summary>
        /// 根据分页查询信息获取部门导航概要信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedData<DepartmentDTO>), 200)]
        public async Task<IActionResult> Get([FromQuery] PagingRequestModel model)
        {
            return await _GetPagingRequest(model);
        }
        #endregion

        #region Get 根据id获取部门信息
        /// <summary>
        /// 根据id获取部门信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(DepartmentDTO), 200)]
        public async Task<IActionResult> Get(string id)
        {
            return await _GetByIdRequest(id);
        }
        #endregion

        #region Post 新建部门信息
        /// <summary>
        /// 新建部门信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateModel]
        [ProducesResponseType(typeof(DepartmentDTO), 200)]
        [ProducesResponseType(typeof(ValidationResultModel), 400)]
        public async Task<IActionResult> Post([FromBody]DepartmentCreateModel model)
        {
            var mapping = new Func<Department, Task<Department>>(async (entity) =>
            {
                if (string.IsNullOrWhiteSpace(model.OrganizationId))
                    model.OrganizationId = await _GetCurrentUserOrganId();
                if (string.IsNullOrWhiteSpace(model.ParentId))
                {
                    var defaultDepartment = await _Repository._DbContext.Departments.FirstOrDefaultAsync(x => x.OrganizationId == model.OrganizationId && string.IsNullOrWhiteSpace(x.ParentId) && x.ActiveFlag == AppConst.I_DataState_Active);
                    if (defaultDepartment != null)
                        model.ParentId = defaultDepartment.Id;
                }

                entity.Name = model.Name;
                entity.Description = model.Description;
                entity.ParentId = model.ParentId;
                entity.OrganizationId = model.OrganizationId;
                return entity;
            });
            return await _PostRequest(mapping);
        }
        #endregion

        #region Put 编辑部门信息
        /// <summary>
        /// 编辑部门信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [ValidateModel]
        [ProducesResponseType(typeof(DepartmentDTO), 200)]
        [ProducesResponseType(typeof(ValidationResultModel), 400)]
        public async Task<IActionResult> Put([FromBody]DepartmentEditModel model)
        {
            var mapping = new Func<Department, Task<Department>>(async (entity) =>
            {
                entity.Name = model.Name;
                entity.ParentId = model.ParentId;
                entity.Description = model.Description;
                return await Task.FromResult(entity);
            });
            return await _PutRequest(model.Id, mapping);
        }
        #endregion

        #region GetByOrgan 根据组织id获取部门信息
        /// <summary>
        /// 根据组织id获取部门信息
        /// </summary>
        /// <param name="organId"></param>
        /// <returns></returns>
        [Route("ByOrgan")]
        [HttpGet]
        [ProducesResponseType(typeof(List<DepartmentDTO>), 200)]
        public async Task<IActionResult> GetByOrgan(string organId)
        {
            if (string.IsNullOrWhiteSpace(organId))
                organId = await _GetCurrentUserOrganId();
            var dtos = await (_Repository as DepartmentRepository).GetByOrgan(organId);
            return Ok(dtos);
        }
        #endregion

    }
}