using ApiModel.Consts;
using ApiModel.Entities;
using ApiModel.Enums;
using ApiServer.Controllers.Common;
using ApiServer.Filters;
using ApiServer.Models;
using ApiServer.Repositories;
using BambooCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiServer.Controllers
{
    /// <summary>
    /// 套餐管理控制器
    /// </summary>
    [Authorize]
    [Route("/[controller]")]
    public class PackageController : ResourceController<Package, PackageDTO>
    {
        public override int ResType => ResourceTypeConst.Package;

        #region 构造函数
        public PackageController(IRepository<Package, PackageDTO> repository)
            : base(repository)
        {
        }
        #endregion

        #region Get 根据分页查询信息获取套餐概要信息
        /// <summary>
        /// 根据分页查询信息获取套餐概要信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedData<PackageDTO>), 200)]
        public async Task<IActionResult> Get([FromQuery] PagingRequestModel model)
        {
            return await _GetPagingRequest(model);
        }
        #endregion

        #region Get 根据id获取套餐信息
        /// <summary>
        /// 根据id获取套餐信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PackageDTO), 200)]
        public async Task<IActionResult> Get(string id)
        {
            return await _GetByIdRequest(id);
        }
        #endregion

        #region Post 新建套餐信息
        /// <summary>
        /// 新建套餐信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateModel]
        [ProducesResponseType(typeof(PackageDTO), 200)]
        [ProducesResponseType(typeof(ValidationResultModel), 400)]
        public async Task<IActionResult> Post([FromBody]PackageCreateModel model)
        {
            var mapping = new Func<Package, Task<Package>>(async (entity) =>
            {
                entity.Name = model.Name;
                entity.Description = model.Description;
                entity.Content = model.Content;
                entity.Icon = model.IconAssetId;
                entity.Color = model.Color;
                entity.ResourceType = (int)ResourceTypeEnum.Organizational;
                return await Task.FromResult(entity);
            });
            return await _PostRequest(mapping);
        }
        #endregion

        #region Put 更新套餐信息
        /// <summary>
        /// 更新套餐信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [ValidateModel]
        [ProducesResponseType(typeof(PackageDTO), 200)]
        [ProducesResponseType(typeof(ValidationResultModel), 400)]
        public async Task<IActionResult> Put([FromBody]PackageEditModel model)
        {
            var mapping = new Func<Package, Task<Package>>(async (entity) =>
            {
                entity.Name = model.Name;
                entity.Description = model.Description;
                entity.Content = model.Content;
                entity.Color = model.Color;
                if (!string.IsNullOrWhiteSpace(model.IconAssetId))
                    entity.Icon = model.IconAssetId;
                return await Task.FromResult(entity);
            });
            return await _PutRequest(model.Id, mapping);
        }
        #endregion

        #region EditAreaType 编辑套餐区域信息
        /// <summary>
        /// 编辑套餐区域信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("EditAreaType")]
        [HttpPut]
        [ValidateModel]
        [ProducesResponseType(typeof(PackageDTO), 200)]
        [ProducesResponseType(typeof(ValidationResultModel), 400)]
        public async Task<IActionResult> EditAreaType([FromBody]PackageAreaTypeEditModel model)
        {
            var mapping = new Func<Package, Task<Package>>(async (entity) =>
            {
                var bExist = false;
                entity.ContentIns = !string.IsNullOrWhiteSpace(entity.Content) ? JsonConvert.DeserializeObject<PackageContent>(entity.Content) : new PackageContent();

                var areas = entity.ContentIns != null && entity.ContentIns.Areas != null && entity.ContentIns.Areas.Count > 0 ? entity.ContentIns.Areas : new List<PackageArea>();
                for (int idx = areas.Count - 1; idx >= 0; idx--)
                {
                    var curItem = areas[idx];
                    if (curItem.Id == model.Id)
                    {
                        curItem.AreaAlias = model.AreaAlias;
                        curItem.AreaTypeId = model.AreaTypeId;
                        bExist = true;
                        break;
                    }
                }

                if (!bExist)
                {
                    areas.Add(new PackageArea() { AreaAlias = model.AreaAlias, AreaTypeId = model.AreaTypeId, Id = GuidGen.NewGUID() });
                }
                entity.ContentIns.Areas = areas;
                entity.Content = JsonConvert.SerializeObject(entity.ContentIns);
                return await Task.FromResult(entity);
            });
            return await _PutRequest(model.PackageId, mapping);
        }
        #endregion

        #region DeleteAreaType 删除套餐区域信息
        /// <summary>
        /// 删除套餐区域信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("DeleteAreaType")]
        [HttpPut]
        [ValidateModel]
        [ProducesResponseType(typeof(PackageDTO), 200)]
        [ProducesResponseType(typeof(ValidationResultModel), 400)]
        public async Task<IActionResult> DeleteAreaType([FromBody]PackageAreaTypeDeleteModel model)
        {
            var mapping = new Func<Package, Task<Package>>(async (entity) =>
            {
                entity.ContentIns = !string.IsNullOrWhiteSpace(entity.Content) ? JsonConvert.DeserializeObject<PackageContent>(entity.Content) : new PackageContent();

                var areas = entity.ContentIns != null && entity.ContentIns.Areas != null && entity.ContentIns.Areas.Count > 0 ? entity.ContentIns.Areas : new List<PackageArea>();
                for (int idx = areas.Count - 1; idx >= 0; idx--)
                {
                    var curItem = areas[idx];
                    if (curItem.Id == model.Id)
                    {
                        areas.RemoveAt(idx);
                        break;
                    }
                }
                entity.ContentIns.Areas = areas;
                entity.Content = JsonConvert.SerializeObject(entity.ContentIns);
                return await Task.FromResult(entity);
            });
            return await _PutRequest(model.PackageId, mapping);
        }
        #endregion

        #region AddProductGroup 添加套餐区域产品组信息
        /// <summary>
        /// 添加套餐区域产品组信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("AddProductGroup")]
        [HttpPut]
        [ValidateModel]
        [ProducesResponseType(typeof(PackageDTO), 200)]
        [ProducesResponseType(typeof(ValidationResultModel), 400)]
        public async Task<IActionResult> AddProductGroup([FromBody]PackageProductGroupCreateModel model)
        {
            var mapping = new Func<Package, Task<Package>>(async (entity) =>
            {
                entity.ContentIns = !string.IsNullOrWhiteSpace(entity.Content) ? JsonConvert.DeserializeObject<PackageContent>(entity.Content) : new PackageContent();
                var areas = entity.ContentIns != null && entity.ContentIns.Areas != null && entity.ContentIns.Areas.Count > 0 ? entity.ContentIns.Areas : new List<PackageArea>();
                for (int idx = areas.Count - 1; idx >= 0; idx--)
                {
                    var curItem = areas[idx];
                    if (curItem.Id == model.AreaId)
                    {
                        var groupDic = curItem.GroupsMap != null ? curItem.GroupsMap : new Dictionary<string, string>();
                        var grp = await _Repository._DbContext.ProductGroups.FindAsync(model.ProductGroupId);
                        groupDic[grp.CategoryId] = model.ProductGroupId;
                        curItem.GroupsMap = groupDic;
                        break;
                    }
                }
                entity.Content = JsonConvert.SerializeObject(entity.ContentIns);
                return await Task.FromResult(entity);
            });
            return await _PutRequest(model.PackageId, mapping);
        }
        #endregion

        #region DeleteProductGroup 删除套餐区域产品组信息
        /// <summary>
        /// 删除套餐区域产品组信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("DeleteProductGroup")]
        [HttpPut]
        [ValidateModel]
        [ProducesResponseType(typeof(PackageDTO), 200)]
        [ProducesResponseType(typeof(ValidationResultModel), 400)]
        public async Task<IActionResult> DeleteProductGroup([FromBody]PackageProductGroupDeleteModel model)
        {
            var mapping = new Func<Package, Task<Package>>(async (entity) =>
            {
                entity.ContentIns = !string.IsNullOrWhiteSpace(entity.Content) ? JsonConvert.DeserializeObject<PackageContent>(entity.Content) : new PackageContent();
                var areas = entity.ContentIns != null && entity.ContentIns.Areas != null && entity.ContentIns.Areas.Count > 0 ? entity.ContentIns.Areas : new List<PackageArea>();
                for (int idx = areas.Count - 1; idx >= 0; idx--)
                {
                    var curItem = areas[idx];
                    if (curItem.Id == model.AreaId)
                    {
                        var groupDic = curItem.GroupsMap != null ? curItem.GroupsMap : new Dictionary<string, string>();
                        for (int nxd = 0; nxd >= 0; nxd--)
                        {
                            var curKv = groupDic.ElementAt(nxd);
                            var grp = await _Repository._DbContext.ProductGroups.FindAsync(model.ProductGroupId);
                            if (curKv.Key == grp.CategoryId)
                                groupDic.Remove(curKv.Key);
                        }
                        curItem.GroupsMap = groupDic;
                        break;
                    }
                }
                entity.Content = JsonConvert.SerializeObject(entity.ContentIns);
                return await Task.FromResult(entity);
            });
            return await _PutRequest(model.PackageId, mapping);
        }
        #endregion

        #region AddCategoryProduct 添加套餐区域分类产品信息
        /// <summary>
        /// 添加套餐区域分类产品信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("AddCategoryProduct")]
        [HttpPut]
        [ValidateModel]
        [ProducesResponseType(typeof(PackageDTO), 200)]
        [ProducesResponseType(typeof(ValidationResultModel), 400)]
        public async Task<IActionResult> AddCategoryProduct([FromBody]PackageCategoryProductCreateModel model)
        {
            var mapping = new Func<Package, Task<Package>>(async (entity) =>
            {
                entity.ContentIns = !string.IsNullOrWhiteSpace(entity.Content) ? JsonConvert.DeserializeObject<PackageContent>(entity.Content) : new PackageContent();
                var areas = entity.ContentIns != null && entity.ContentIns.Areas != null && entity.ContentIns.Areas.Count > 0 ? entity.ContentIns.Areas : new List<PackageArea>();
                for (int idx = areas.Count - 1; idx >= 0; idx--)
                {
                    var curItem = areas[idx];
                    if (curItem.Id == model.AreaId)
                    {
                        var cateogoryDic = curItem.ProductCategoryMap != null ? curItem.ProductCategoryMap : new Dictionary<string, string>();
                        var cat = await _Repository._DbContext.Products.FindAsync(model.ProductId);
                        cateogoryDic[cat.CategoryId] = model.ProductId;
                        curItem.ProductCategoryMap = cateogoryDic;
                        break;
                    }
                }
                entity.Content = JsonConvert.SerializeObject(entity.ContentIns);
                return await Task.FromResult(entity);
            });
            return await _PutRequest(model.PackageId, mapping);
        }
        #endregion

        #region DeleteCategoryProduct 删除套餐区域分类产品信息
        /// <summary>
        /// 删除套餐区域分类产品信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("DeleteCategoryProduct")]
        [HttpPut]
        [ValidateModel]
        [ProducesResponseType(typeof(PackageDTO), 200)]
        [ProducesResponseType(typeof(ValidationResultModel), 400)]
        public async Task<IActionResult> DeleteCategoryProduct([FromBody]PackageCategoryProductDeleteModel model)
        {
            var mapping = new Func<Package, Task<Package>>(async (entity) =>
            {
                entity.ContentIns = !string.IsNullOrWhiteSpace(entity.Content) ? JsonConvert.DeserializeObject<PackageContent>(entity.Content) : new PackageContent();
                var areas = entity.ContentIns != null && entity.ContentIns.Areas != null && entity.ContentIns.Areas.Count > 0 ? entity.ContentIns.Areas : new List<PackageArea>();
                for (int idx = areas.Count - 1; idx >= 0; idx--)
                {
                    var curItem = areas[idx];
                    if (curItem.Id == model.AreaId)
                    {
                        var cateogoryDic = curItem.ProductCategoryMap != null ? curItem.ProductCategoryMap : new Dictionary<string, string>();
                        for (int nxd = 0; nxd >= 0; nxd--)
                        {
                            var curKv = cateogoryDic.ElementAt(nxd);
                            if (curKv.Value == model.ProductId)
                                cateogoryDic.Remove(curKv.Key);
                        }
                        break;
                    }
                }
                entity.Content = JsonConvert.SerializeObject(entity.ContentIns);
                return await Task.FromResult(entity);
            });
            return await _PutRequest(model.PackageId, mapping);
        }
        #endregion

        #region EditMaterial 新增/编辑套餐材质信息
        /// <summary>
        /// 新增/编辑套餐材质信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("EditMaterial")]
        [HttpPut]
        [ValidateModel]
        [ProducesResponseType(typeof(PackageDTO), 200)]
        [ProducesResponseType(typeof(ValidationResultModel), 400)]
        public async Task<IActionResult> EditMaterial([FromBody]PackageMaterialCreateModel model)
        {
            var mapping = new Func<Package, Task<Package>>(async (entity) =>
            {
                entity.ContentIns = !string.IsNullOrWhiteSpace(entity.Content) ? JsonConvert.DeserializeObject<PackageContent>(entity.Content) : new PackageContent();
                var areas = entity.ContentIns != null && entity.ContentIns.Areas != null && entity.ContentIns.Areas.Count > 0 ? entity.ContentIns.Areas : new List<PackageArea>();
                for (int idx = areas.Count - 1; idx >= 0; idx--)
                {
                    var curItem = areas[idx];
                    if (curItem.Id == model.AreaId)
                    {
                        var materialDic = curItem.Materials != null ? curItem.Materials : new Dictionary<string, string>();
                        if (!string.IsNullOrWhiteSpace(model.LastActorName) && materialDic[model.LastActorName] != null)
                            materialDic.Remove(model.LastActorName);
                        materialDic[!string.IsNullOrWhiteSpace(model.ActorName) ? model.ActorName : "待定"] = model.MaterialId;
                        curItem.Materials = materialDic;
                        break;
                    }
                }
                entity.Content = JsonConvert.SerializeObject(entity.ContentIns);
                return await Task.FromResult(entity);
            });
            return await _PutRequest(model.PackageId, mapping);
        }
        #endregion

        #region DeleteMaterial 删除套餐材质信息
        /// <summary>
        /// 删除套餐材质信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("DeleteMaterial")]
        [HttpPut]
        [ValidateModel]
        [ProducesResponseType(typeof(PackageDTO), 200)]
        [ProducesResponseType(typeof(ValidationResultModel), 400)]
        public async Task<IActionResult> DeleteMaterial([FromBody]PackageMaterialDeleteModel model)
        {
            var mapping = new Func<Package, Task<Package>>(async (entity) =>
            {
                entity.ContentIns = !string.IsNullOrWhiteSpace(entity.Content) ? JsonConvert.DeserializeObject<PackageContent>(entity.Content) : new PackageContent();
                var areas = entity.ContentIns != null && entity.ContentIns.Areas != null && entity.ContentIns.Areas.Count > 0 ? entity.ContentIns.Areas : new List<PackageArea>();
                for (int idx = areas.Count - 1; idx >= 0; idx--)
                {
                    var curItem = areas[idx];
                    if (curItem.Id == model.AreaId)
                    {
                        var materialDic = curItem.Materials != null ? curItem.Materials : new Dictionary<string, string>();
                        for (int nxd = 0; nxd >= 0; nxd--)
                        {
                            var curKv = materialDic.ElementAt(nxd);
                            if (curKv.Key == model.LastActorName && curKv.Value == model.MaterialId)
                            {
                                materialDic.Remove(curKv.Key);
                                break;
                            }
                        }
                        curItem.Materials = materialDic;
                        break;
                    }
                }
                entity.Content = JsonConvert.SerializeObject(entity.ContentIns);
                return await Task.FromResult(entity);
            });
            return await _PutRequest(model.PackageId, mapping);
        }
        #endregion

        #region AddProductReplaceGroup 添加套餐产品替换组信息
        /// <summary>
        /// 添加套餐产品替换组信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("AddProductReplaceGroup")]
        [HttpPut]
        [ValidateModel]
        [ProducesResponseType(typeof(PackageDTO), 200)]
        [ProducesResponseType(typeof(ValidationResultModel), 400)]
        public async Task<IActionResult> AddProductReplaceGroup([FromBody]PackageProductReplaceGroupCreateModel model)
        {
            var mapping = new Func<Package, Task<Package>>(async (entity) =>
            {
                entity.ContentIns = !string.IsNullOrWhiteSpace(entity.Content) ? JsonConvert.DeserializeObject<PackageContent>(entity.Content) : new PackageContent();
                var replaceGroups = entity.ContentIns != null && entity.ContentIns.ReplaceGroups != null ? entity.ContentIns.ReplaceGroups : new List<string>();
                var idsArr = model.ReplaceGroupIds.Split(",", StringSplitOptions.RemoveEmptyEntries);
                replaceGroups.AddRange(idsArr);
                entity.ContentIns.ReplaceGroups = replaceGroups.Select(x => x).Distinct().ToList();
                entity.Content = JsonConvert.SerializeObject(entity.ContentIns);
                return await Task.FromResult(entity);
            });
            return await _PutRequest(model.PackageId, mapping);
        }
        #endregion

        #region DeleteMaterial 删除套餐产品替换组信息
        /// <summary>
        /// 删除套餐产品替换组信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("DeleteProductReplaceGroup")]
        [HttpPut]
        [ValidateModel]
        [ProducesResponseType(typeof(PackageDTO), 200)]
        [ProducesResponseType(typeof(ValidationResultModel), 400)]
        public async Task<IActionResult> DeleteProductReplaceGroup([FromBody]PackageProductReplaceGroupDeleteModel model)
        {
            var mapping = new Func<Package, Task<Package>>(async (entity) =>
            {
                entity.ContentIns = !string.IsNullOrWhiteSpace(entity.Content) ? JsonConvert.DeserializeObject<PackageContent>(entity.Content) : new PackageContent();
                var replaceGroups = entity.ContentIns != null && entity.ContentIns.ReplaceGroups != null ? entity.ContentIns.ReplaceGroups : new List<string>();
                entity.ContentIns.ReplaceGroups = replaceGroups.Where(x => x != model.ItemId).ToList();
                entity.Content = JsonConvert.SerializeObject(entity.ContentIns);
                return await Task.FromResult(entity);
            });
            return await _PutRequest(model.PackageId, mapping);
        }
        #endregion

        #region ChangeContent 更新套餐详情信息
        /////// <summary>
        ///// 更新套餐详情信息
        ///// </summary>
        ///// <param name="id"></param>
        ///// <param name="content"></param>
        ///// <returns></returns>
        //[Route("ChangeContent")]
        //[HttpPost]
        //public async Task<IActionResult> ChangeContent(string id, [FromBody]OrderContent content)
        //{
        //    var mapping = new Func<Package, Task<Package>>(async (entity) =>
        //    {
        //        entity.Content = JsonConvert.SerializeObject(content);
        //        return await Task.FromResult(entity);
        //    });
        //    return await _PutRequest(id, mapping);
        //}
        #endregion
    }
}