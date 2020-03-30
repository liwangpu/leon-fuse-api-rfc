using Apps.Base.Common;
using Apps.Base.Common.Attributes;
using Apps.Base.Common.Consts;
using Apps.Base.Common.Controllers;
using Apps.Base.Common.Interfaces;
using Apps.OMS.Data.Entities;
using Apps.OMS.Export.DTOs;
using Apps.OMS.Export.Models;
using Apps.OMS.Service.Contexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Apps.OMS.Service.Controllers
{
    [Authorize]
    [Route("/[controller]")]
    [ApiController]
    public class ProductPackageController : ServiceBaseController<ProductPackage>
    {
        protected AppConfig _AppConfig { get; }
        protected AppDbContext _Context { get; }

        #region 构造函数
        public ProductPackageController(IRepository<ProductPackage> repository, AppDbContext context, IOptions<AppConfig> settingsOptions)
            : base(repository)
        {
            _Context = context;
            _AppConfig = settingsOptions.Value;
        }
        #endregion

        #region Get 根据Id获取产品包装信息
        /// <summary>
        /// 根据Id获取产品包装信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProductPackageDTO), 200)]
        public async override Task<IActionResult> Get(string id)
        {
            var pck = await _Context.ProductPackages.FirstOrDefaultAsync(x => x.Id == id);
            return Ok(pck);
        } 
        #endregion

        #region Get 根据产品规格Id获取产品包装信息
        /// <summary>
        /// 根据产品规格Id获取产品包装信息
        /// </summary>
        /// <param name="productSpecId"></param>
        /// <returns></returns>
        [HttpGet("GetByProductSpecId/{productSpecId}")]
        [ProducesResponseType(typeof(List<ProductPackageDTO>), 200)]
        public async Task<IActionResult> GetByProductSpecId(string productSpecId)
        {
            var pcks = await _Context.ProductPackages.Where(x => x.OrganizationId == CurrentAccountOrganizationId && x.ProductSpecId == productSpecId && x.ActiveFlag == AppConst.Active).ToListAsync();


            return Ok(pcks);
        }
        #endregion

        #region Post 新建产品包装信息
        /// <summary>
        /// 新建产品包装信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateModel]
        [ProducesResponseType(typeof(ProductPackageDTO), 200)]
        [ProducesResponseType(typeof(ValidationResultModel), 400)]
        public async Task<IActionResult> Post([FromBody]ProductPackageCreateModel model)
        {
            var mapping = new Func<ProductPackage, Task<ProductPackage>>(async (entity) =>
            {
                entity.Name = model.Name;
                entity.Description = model.Description;
                entity.Num = model.Num;
                entity.ProductSpecId = model.ProductSpecId;
                entity.OrganizationId = CurrentAccountOrganizationId;

                return await Task.FromResult(entity);
            });
            return await _PostRequest(mapping);
        }
        #endregion

        #region Put 更新产品包装信息
        /// <summary>
        /// 更新产品包装信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [ValidateModel]
        [ProducesResponseType(typeof(WorkFlowDTO), 200)]
        [ProducesResponseType(typeof(ValidationResultModel), 400)]
        public async Task<IActionResult> Put([FromBody]ProductPackageUpdateModel model)
        {
            var mapping = new Func<ProductPackage, Task<ProductPackage>>(async (entity) =>
            {
                entity.Name = model.Name;
                entity.Description = model.Description;
                entity.Num = model.Num;
                return await Task.FromResult(entity);
            });
            return await _PutRequest(model.Id, mapping);
        }
        #endregion

        #region Delete 删除产品包装信息
        /// <summary>
        /// 删除产品包装信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(Nullable), 200)]
        public async Task<IActionResult> Delete(string id)
        {
            return await _DeleteRequest(id);
        } 
        #endregion
    }
}