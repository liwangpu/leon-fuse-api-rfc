using ApiModel.Consts;
using ApiModel.Entities;
using ApiModel.Enums;
using ApiServer.Controllers.Common;
using ApiServer.Data;
using ApiServer.Filters;
using ApiServer.Models;
using ApiServer.Repositories;
using BambooCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ApiServer.Controllers.Design
{
    
    [Route("/[controller]")]
    public class ProductReplaceGroupController : ListableController<ProductReplaceGroup, ProductReplaceGroupDTO>
    {
        private ApiDbContext _Context;

        #region 构造函数
        public ProductReplaceGroupController(IRepository<ProductReplaceGroup, ProductReplaceGroupDTO> repository, ApiDbContext context)
        : base(repository)
        {
            _Context = context;
        }
        #endregion

        #region _UpdateGroup 真实修改产品替换组
        /// <summary>
        /// 真实修改产品替换组
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        protected async Task<IActionResult> _UpdateGroup([FromBody]ProductReplaceGroupEditModel model)
        {
            var mapping = new Func<ProductReplaceGroup, Task<ProductReplaceGroup>>(async (entity) =>
            {
                entity.Name = model.Name;
                entity.Description = model.Description;
                entity.DefaultItemId = model.DefaultItemId;
                entity.GroupItemIds = model.ItemIds;
                //entity.CategoryId = model.CategoryId;
                return await Task.FromResult(entity);
            });
            return await _PutRequest(model.Id, mapping);
        }
        #endregion

        #region Get 根据分页查询信息获取产品替换组概要信息
        /// <summary>
        /// 根据分页查询信息获取产品替换组概要信息
        /// </summary>
        /// <param name="model"></param>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedData<ProductReplaceGroupDTO>), 200)]
        public async Task<IActionResult> Get([FromQuery] PagingRequestModel model, string categoryId = "")
        {
            var advanceQuery = new Func<IQueryable<ProductReplaceGroup>, Task<IQueryable<ProductReplaceGroup>>>(async (query) =>
            {
                #region 根据分类Id查询
                if (!string.IsNullOrWhiteSpace(categoryId))
                {
                    var curCategoryTree = await _Repository._DbContext.AssetCategoryTrees.FirstOrDefaultAsync(x => x.ObjId == categoryId);
                    //如果是根节点,把所有取出,不做分类过滤
                    if (curCategoryTree != null && curCategoryTree.LValue > 1)
                    {
                        var categoryIdsQ = from it in _Repository._DbContext.AssetCategoryTrees
                                           where it.NodeType == curCategoryTree.NodeType && it.OrganizationId == curCategoryTree.OrganizationId
                                           && it.LValue >= curCategoryTree.LValue && it.RValue <= curCategoryTree.RValue
                                           select it.ObjId;
                        var categoryIds = await categoryIdsQ.ToListAsync();

                        query = from it in query
                                join prod in _Repository._DbContext.Products on it.DefaultItemId equals prod.Id into prq
                                where categoryIds.Contains(prq.First().CategoryId)
                                select it;
                    }
                }
                #endregion
                query = query.Where(x => x.ActiveFlag == AppConst.I_DataState_Active);
                return await Task.FromResult(query);
            });
            return await _GetPagingRequest(model, null, advanceQuery);
        }
        #endregion

        #region Get 根据id获取产品替换组信息
        /// <summary>
        /// 根据id获取产品替换组信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProductReplaceGroupDTO), 200)]
        public async Task<IActionResult> Get(string id)
        {
            return await _GetByIdRequest(id);
        }
        #endregion

        #region Post 新建产品替换组信息
        /// <summary>
        /// 新建产品替换组信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateModel]
        [ProducesResponseType(typeof(ProductReplaceGroupDTO), 200)]
        [ProducesResponseType(typeof(ValidationResultModel), 400)]
        public async Task<IActionResult> Post([FromBody]ProductReplaceGroupCreateModel model)
        {
            var mapping = new Func<ProductReplaceGroup, Task<ProductReplaceGroup>>(async (entity) =>
            {
                entity.Name = model.Name;
                entity.Description = model.Description;
                entity.GroupItemIds = model.ItemIds;

                if (string.IsNullOrWhiteSpace(model.DefaultItemId))
                {
                    var ids = model.ItemIds.Split(",", StringSplitOptions.RemoveEmptyEntries);
                    entity.DefaultItemId = ids[0];
                }
                else
                    entity.DefaultItemId = model.DefaultItemId;
                entity.ResourceType = (int)ResourceTypeEnum.Organizational;
                return await Task.FromResult(entity);
            });
            return await _PostRequest(mapping);
        }
        #endregion

        #region Put 更新产品替换组信息
        /// <summary>
        /// 更新产品替换组信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [ValidateModel]
        [ProducesResponseType(typeof(ProductReplaceGroupDTO), 200)]
        [ProducesResponseType(typeof(ValidationResultModel), 400)]
        public async Task<IActionResult> Put([FromBody]ProductReplaceGroupEditModel model)
        {
            return await _UpdateGroup(model);
        }
        #endregion

        #region SetDefault 设置默认项
        /// <summary> 
        /// 设置默认项
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("SetDefault")]
        [HttpPut]
        [ValidateModel]
        [ProducesResponseType(typeof(ProductReplaceGroupDTO), 200)]
        [ProducesResponseType(typeof(ValidationResultModel), 400)]
        public async Task<IActionResult> SetDefault([FromBody]ProductReplaceGroupSetDefaultModel model)
        {
            var mapping = new Func<ProductReplaceGroup, Task<ProductReplaceGroup>>(async (entity) =>
            {
                entity.DefaultItemId = model.ItemId;
                return await Task.FromResult(entity);
            });
            return await _PutRequest(model.Id, mapping);
        }
        #endregion

        #region RemoveItem 删除产品项
        /// <summary>
        /// 删除产品项
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("RemoveItem")]
        [HttpPut]
        [ValidateModel]
        [ProducesResponseType(typeof(ProductReplaceGroupDTO), 200)]
        [ProducesResponseType(typeof(ValidationResultModel), 400)]
        public async Task<IActionResult> RemoveItem([FromBody]ProductReplaceGroupSetDefaultModel model)
        {
            //校验删除项是否是最后项,如果是的话软删除该记录
            var refer = await _Context.ProductReplaceGroups.FirstOrDefaultAsync(x => x.Id == model.Id && x.ActiveFlag == AppConst.I_DataState_Active);
            if (refer != null)
            {
                var itemArr = refer.GroupItemIds.Split(",", StringSplitOptions.RemoveEmptyEntries);
                if (itemArr.Length <= 1)
                {
                    refer.ActiveFlag = AppConst.I_DataState_InActive;
                    _Context.ProductReplaceGroups.Update(refer);
                    await _Context.SaveChangesAsync();
                    return Ok(new { });
                }
            }
            else
            {
                return NotFound();
            }


            var mapping = new Func<ProductReplaceGroup, Task<ProductReplaceGroup>>(async (entity) =>
            {
                var ids = entity.GroupItemIds.Split(",", StringSplitOptions.RemoveEmptyEntries).ToList();
                var changeIds = ids.Where(x => x != model.ItemId).ToList();
                if (model.ItemId == entity.DefaultItemId)
                {
                    if (changeIds != null && changeIds.Count > 0)
                        entity.DefaultItemId = changeIds[0];
                    else
                        entity.DefaultItemId = string.Empty;
                }
                if (changeIds != null && changeIds.Count > 0)
                    entity.GroupItemIds = string.Join(",", changeIds);
                else
                    entity.GroupItemIds = string.Empty;
                return await Task.FromResult(entity);
            });
            return await _PutRequest(model.Id, mapping);
        }
        #endregion

    }
}