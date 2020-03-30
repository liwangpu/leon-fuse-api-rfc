using ApiModel.Consts;
using ApiModel.Entities;
using ApiServer.Controllers.Common;
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
    [Authorize]
    [Route("/[controller]")]
    public class ProductGroupController : ResourceController<ProductGroup, ProductGroupDTO>
    {
        public override int ResType => ResourceTypeConst.ProductGroup;

        #region 构造函数
        public ProductGroupController(IRepository<ProductGroup, ProductGroupDTO> repository)
            : base(repository)
        {
        }
        #endregion

        #region Get 根据分页查询信息获取产品组类型概要信息
        /// <summary>
        /// 根据分页查询信息获取产品组类型概要信息
        /// </summary>
        /// <param name="model"></param>
        /// <param name="categoryId"></param>
        /// <param name="classify"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedData<ProductGroupDTO>), 200)]
        public async Task<IActionResult> Get([FromQuery] PagingRequestModel model, string categoryId = "", bool classify = true)
        {
            var advanceQuery = new Func<IQueryable<ProductGroup>, Task<IQueryable<ProductGroup>>>(async (query) =>
            {
                if (classify)
                {
                    if (!string.IsNullOrWhiteSpace(categoryId))
                    {
                        var curCategoryTree = await _Repository._DbContext.AssetCategoryTrees.FirstOrDefaultAsync(x => x.ObjId == categoryId);
                        //如果是根节点,把所有取出,不做分类过滤
                        if (curCategoryTree != null && curCategoryTree.LValue > 1)
                        {
                            var categoryQ = from it in _Repository._DbContext.AssetCategoryTrees
                                            where it.NodeType == curCategoryTree.NodeType && it.OrganizationId == curCategoryTree.OrganizationId
                                            && it.LValue >= curCategoryTree.LValue && it.RValue <= curCategoryTree.RValue
                                            select it;
                            query = from it in query
                                    join cat in categoryQ on it.CategoryId equals cat.ObjId
                                    select it;
                        }
                    }
                }
                else
                {
                    query = query.Where(x => string.IsNullOrWhiteSpace(x.CategoryId));
                }
                query = query.Where(x => x.ActiveFlag == AppConst.I_DataState_Active);
                return await Task.FromResult(query);
            });


            return await _GetPagingRequest(model, null, advanceQuery);
        }
        #endregion

        #region Get 根据id获取产品组类型信息
        /// <summary>
        /// 根据id获取产品组类型信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProductGroupDTO), 200)]
        public async Task<IActionResult> Get(string id)
        {
            return await _GetByIdRequest(id);
        }
        #endregion

        #region Post 新建产品组类型信息
        /// <summary>
        /// 新建产品组类型信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateModel]
        [ProducesResponseType(typeof(ProductGroupDTO), 200)]
        [ProducesResponseType(typeof(ValidationResultModel), 400)]
        public async Task<IActionResult> Post([FromBody]ProductGroupCreateModel model)
        {
            var mapping = new Func<ProductGroup, Task<ProductGroup>>(async (entity) =>
            {
                entity.Name = model.Name;
                entity.Description = model.Description;
                entity.Icon = model.IconAssetId;
                entity.Items = model.Items;
                entity.PivotLocation = model.PivotLocation;
                entity.PivotType = model.PivotType;
                entity.Orientation = model.Orientation;
                entity.Serie = model.Serie;
                entity.CategoryId = model.CategoryId;
                entity.Color = model.Color;
                return await Task.FromResult(entity);
            });
            return await _PostRequest(mapping);
        }
        #endregion

        #region Put 更新产品组类型信息
        /// <summary>
        /// 更新产品组类型信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [ValidateModel]
        [ProducesResponseType(typeof(ProductGroupDTO), 200)]
        [ProducesResponseType(typeof(ValidationResultModel), 400)]
        public async Task<IActionResult> Put([FromBody]ProductGroupEditModel model)
        {
            var mapping = new Func<ProductGroup, Task<ProductGroup>>(async (entity) =>
            {
                entity.Name = model.Name;
                entity.Description = model.Description;
                if (!string.IsNullOrWhiteSpace(model.IconAssetId))
                    entity.Icon = model.IconAssetId;
                entity.Items = model.Items;
                entity.PivotLocation = model.PivotLocation;
                entity.PivotType = model.PivotType;
                entity.Orientation = model.Orientation;
                entity.Serie = model.Serie;
                entity.CategoryId = model.CategoryId;
                entity.Color = model.Color;
                return await Task.FromResult(entity);
            });
            return await _PutRequest(model.Id, mapping);
        }
        #endregion

        #region BulkChangeCategory 批量修改分类信息
        /// <summary>
        /// 批量修改分类信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("BulkChangeCategory")]
        [ValidateModel]
        [HttpPut]
        public async Task<IActionResult> BulkChangeCategory([FromBody]BulkChangeCategoryModel model)
        {
            if (!ModelState.IsValid)
                return new ValidationFailedResult(ModelState);
            var existCategory = await _Repository._DbContext.AssetCategories.CountAsync(x => x.Id == model.CategoryId) > 0;
            if (!existCategory)
            {
                ModelState.AddModelError("CategoryId", "对应记录不存在");
                return new ValidationFailedResult(ModelState);
            }
            var idArr = model.Ids.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            using (var transaction = _Repository._DbContext.Database.BeginTransaction())
            {
                try
                {
                    for (int idx = idArr.Length - 1; idx >= 0; idx--)
                    {
                        var id = idArr[idx];
                        var refProductGroup = await _Repository._DbContext.ProductGroups.FindAsync(id);
                        if (refProductGroup != null)
                        {
                            refProductGroup.CategoryId = model.CategoryId;
                            _Repository._DbContext.ProductGroups.Update(refProductGroup);
                        }
                        else
                        {
                            ModelState.AddModelError("ProductGroupId", "对应记录不存在");
                            return new ValidationFailedResult(ModelState);
                        }
                    }
                    _Repository._DbContext.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
            return Ok();
        }
        #endregion
    }
}