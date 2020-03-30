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
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiServer.Controllers.Design
{
    /// <summary>
    /// 解决方案控制器
    /// </summary>
    [Authorize]
    [Route("/[controller]")]
    public class SolutionController : ResourceController<Solution, SolutionDTO>
    {
        public override int ResType => ResourceTypeConst.Solution;

        #region 构造函数
        public SolutionController(IRepository<Solution, SolutionDTO> repository)
            : base(repository)
        {
        }
        #endregion

        #region Get 根据分页查询信息获取解决方案概要信息
        /// <summary>
        /// 根据分页查询信息获取解决方案概要信息
        /// </summary>
        /// <param name="model"></param>
        /// <param name="onlyShare"></param>
        /// <param name="mine"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedData<SolutionDTO>), 200)]
        public async Task<IActionResult> Get([FromQuery] PagingRequestModel model, bool onlyShare = false, bool mine = false)
        {
            var advanceQuery = new Func<IQueryable<Solution>, Task<IQueryable<Solution>>>(async (query) =>
            {
                if (onlyShare)
                {
                    query = query.Where(x => x.ResourceType == (int)ResourceTypeEnum.Organizational_SubShare);
                }
                if (mine)
                {
                    var accid = AuthMan.GetAccountId(this);
                    query = query.Where(x => x.Creator == accid);
                }

                query = query.Where(x => x.ActiveFlag == AppConst.I_DataState_Active && x.IsSnapshot == false);
                return await Task.FromResult(query);
            });

            return await _GetPagingRequest(model, null, advanceQuery);
        }
        #endregion

        #region Get 根据id获取解决方案信息
        /// <summary>
        /// 根据id获取解决方案信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(SolutionDTO), 200)]
        public async Task<IActionResult> Get(string id)
        {
            return await _GetByIdRequest(id);
        }
        #endregion

        #region Post 新建解决方案信息
        /// <summary>
        /// 新建解决方案信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateModel]
        [ProducesResponseType(typeof(Solution), 200)]
        public async Task<IActionResult> Post([FromBody]SolutionCreateModel model)
        {
            var mapping = new Func<Solution, Task<Solution>>(async (entity) =>
            {
                entity.Name = model.Name;
                entity.Description = model.Description;
                entity.Icon = model.IconAssetId;
                if (!string.IsNullOrWhiteSpace(model.LayoutId))
                    entity.LayoutId = model.LayoutId;
                entity.CategoryId = model.CategoryId;
                entity.Data = model.Data;
                entity.IsSnapshot = model.IsSnapshot;
                entity.SnapshotData = model.SnapshotData;
                entity.Color = model.Color;
                return await Task.FromResult(entity);
            });
            return await _PostRequest(mapping);
        }
        #endregion

        #region Put 更新解决方案信息
        /// <summary>
        /// 更新解决方案信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [ValidateModel]
        [ProducesResponseType(typeof(Solution), 200)]
        public async Task<IActionResult> Put([FromBody]SolutionEditModel model)
        {
            var mapping = new Func<Solution, Task<Solution>>(async (entity) =>
            {
                entity.Name = model.Name;
                entity.Description = model.Description;
                if (!string.IsNullOrWhiteSpace(model.LayoutId))
                    entity.LayoutId = model.LayoutId;
                if (!string.IsNullOrWhiteSpace(model.IconAssetId))
                    entity.Icon = model.IconAssetId;
                entity.CategoryId = model.CategoryId;
                entity.IsSnapshot = model.IsSnapshot;
                entity.SnapshotData = model.SnapshotData;
                entity.Data = model.Data;
                entity.Color = model.Color;
                return await Task.FromResult(entity);
            });
            return await _PutRequest(model.Id, mapping);
        }
        #endregion

        #region UpdateSnapshotData 更新方案快照信息
        /// <summary>
        /// 更新方案快照信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("UpdateSnapshotData")]
        [HttpPut]
        [ValidateModel]
        public async Task<IActionResult> UpdateSnapshotData([FromBody]SolutionSnapshotEditModel model)
        {
            var refSolution = await _Repository._DbContext.Solutions.Where(x => x.Id == model.SolutionId).FirstOrDefaultAsync();
            if (refSolution == null)
                return NotFound();
            refSolution.SnapshotData = model.SnapshotData;
            _Repository._DbContext.Solutions.Update(refSolution);
            await _Repository._DbContext.SaveChangesAsync();
            return Ok();
        } 
        #endregion
    }
}
