using ApiModel;
using ApiModel.Consts;
using ApiModel.Entities;
using ApiModel.Enums;
using ApiServer.Filters;
using ApiServer.Models;
using ApiServer.Repositories;
using ApiServer.Services;
using BambooCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiServer.Controllers.Common
{
    public class ListableController<T, DTO> : CommonController<T, DTO>
         where T : class, IListable, ApiModel.ICloneable, IDTOTransfer<DTO>, new()
          where DTO : class, IListable, new()
    {

        #region 构造函数
        public ListableController(IRepository<T, DTO> repository)
          : base(repository)
        {

        }
        #endregion

        #region ChangeICon 更新图标信息
        /// <summary>
        /// 更新图标信息
        /// </summary>
        /// <param name="icon"></param>
        /// <returns></returns>
        [Route("ChangeICon")]
        [HttpPut]
        [ValidateModel]
        [ProducesResponseType(typeof(ValidationResultModel), 400)]
        public async Task<IActionResult> ChangeICon([FromBody]IconModel icon)
        {
            var mapping = new Func<T, Task<T>>(async (spec) =>
            {
                spec.Icon = icon.AssetId;
                return await Task.FromResult(spec);
            });

            var handle = new Func<T, DTO, Task<IActionResult>>(async (entity, dto) =>
            {
                var new_dto = new DTO();
                new_dto.Id = entity.Id;
                new_dto.Name = entity.Name;
                new_dto.Icon = dto.Icon;
                return await Task.FromResult(Ok(new_dto));
            });
            return await _PutRequest(icon.ObjId, mapping, handle);
        }
        #endregion

        /*********************Collection*********************/

        #region _PostCollectionRequest 处理添加收藏数据请求
        /// <summary>
        /// 处理添加收藏数据请求
        /// </summary>
        /// <param name="mapping"></param>
        /// <returns></returns>
        protected async Task<IActionResult> _PostCollectionRequest(Func<Collection, Task<Collection>> mapping)
        {
            var accid = AuthMan.GetAccountId(this);
            var account = await _Repository._DbContext.Accounts.FindAsync(accid);
            var metadata = new Collection();
            //var canCreate = await _Repository.CanCreateAsync(accid);
            //if (!canCreate)
            //    return Forbid();
            var data = await mapping(metadata);
            data.Id = GuidGen.NewGUID();
            data.CreatedTime = DateTime.UtcNow;
            data.ModifiedTime = DateTime.UtcNow;
            data.Creator = accid;
            data.Modifier = accid;
            data.OrganizationId = account.OrganizationId;
            _Repository._DbContext.Collections.Add(data);
            await _Repository._DbContext.SaveChangesAsync();
            //await _Repository.SatisfyCreateAsync(accid, data, ModelState);
            //if (!ModelState.IsValid)
            //    return new ValidationFailedResult(ModelState);
            //await _Repository.CreateAsync(accid, data);
            //var dto = await _Repository.GetByIdAsync(metadata.Id);
            ////如果handle不为空,由handle掌控ActionResult
            //if (handle != null)
            //    return await handle(data);
            return Ok();
        }
        #endregion

        #region CreateCollection 添加收藏数据信息
        /// <summary>
        /// 添加收藏数据信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("Collection")]
        [HttpPost]
        [ValidateModel]
        [ProducesResponseType(typeof(ValidationResultModel), 400)]
        public async Task<IActionResult> CreateCollection([FromBody]CollectionCreateModel model)
        {
            var accid = AuthMan.GetAccountId(this);
            var t = new T();
            var colls = await _Repository._DbContext.Collections.Where(x => x.Creator == accid && x.Type == t.GetType().Name && x.TargetId == model.TargetId).ToListAsync();
            if (colls.Count > 0)
                return Ok();

            var mapping = new Func<Collection, Task<Collection>>(async (entity) =>
            {
                entity.TargetId = model.TargetId;
                entity.Type = t.GetType().Name;
                return await Task.FromResult(entity);
            });
            return await _PostCollectionRequest(mapping);
        }
        #endregion

        #region DeleteCollection 删除收藏数据信息
        /// <summary>
        /// 删除收藏数据信息
        /// </summary>
        /// <param name="targetId"></param>
        /// <returns></returns>
        [Route("Collection")]
        [HttpDelete]
        public virtual async Task<IActionResult> DeleteCollection(string targetId)
        {
            var accid = AuthMan.GetAccountId(this);
            var t = new T();
            var colls = await _Repository._DbContext.Collections.Where(x => x.Creator == accid && x.Type == t.GetType().Name && x.TargetId == targetId).ToListAsync();
            if (colls.Count == 0)
                return NotFound();
            foreach (var item in colls)
            {
                _Repository._DbContext.Collections.Remove(item);
            }
            await _Repository._DbContext.SaveChangesAsync();
            return Ok();
        }
        #endregion

        #region Collection 查询收藏列表数据信息
        /// <summary>
        /// 查询收藏列表数据信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("Collection")]
        [HttpGet]
        [ValidateModel]
        [ProducesResponseType(typeof(PagedData<PackageDTO>), 200)]
        public async Task<IActionResult> Collection([FromQuery] ColletionRequestModel model)
        {
            var t = typeof(T);
            var accid = AuthMan.GetAccountId(this);
            var query = _Repository._DbContext.Collections.Where(x => x.Creator == accid && x.ActiveFlag == AppConst.I_DataState_Active && x.Type == t.Name);
            if (!string.IsNullOrWhiteSpace(model.TargetId))
                query = query.Where(x => x.TargetId == model.TargetId);

            var dataQ = Enumerable.Empty<T>().AsQueryable();
            if (!string.IsNullOrWhiteSpace(model.CategoryId))
            {
                var curCategoryTree = await _Repository._DbContext.AssetCategoryTrees.FirstOrDefaultAsync(x => x.ObjId == model.CategoryId);
                var categoryQ = from it in _Repository._DbContext.AssetCategoryTrees
                                where it.NodeType == curCategoryTree.NodeType && it.OrganizationId == curCategoryTree.OrganizationId
                                && it.LValue >= curCategoryTree.LValue && it.RValue <= curCategoryTree.RValue
                                select it.ObjId;
                var catIds = await categoryQ.ToListAsync();

                dataQ = from it in _Repository._DbContext.Set<T>()
                        join cc in query on it.Id equals cc.TargetId
                        where catIds.Contains(it.CategoryId)
                        select it;
            }
            else
            {
                dataQ = from it in _Repository._DbContext.Set<T>()
                        join cc in query on it.Id equals cc.TargetId
                        select it;
            }

            var res = await dataQ.OrderBy(x => x.Id).SimplePaging(model.Page, model.PageSize);
            var datas = res.Data;

            var pagedData = new PagedData<DTO>() { Data = new List<DTO>(), Page = res.Page, Size = res.Size, Total = res.Total };

            for (int ddx = datas.Count - 1; ddx >= 0; ddx--)
            {
                var curData = datas[ddx];
                //if (string.IsNullOrWhiteSpace(curData.Icon))      
                //{
                //    var fs = await _Repository._DbContext.Files.FirstOrDefaultAsync(x => x.Id == curData.Icon);
                //    curData.Icon =;
                //}
                if (!string.IsNullOrWhiteSpace(curData.Icon))
                {
                    var fs = await _Repository._DbContext.Files.FirstOrDefaultAsync(x => x.Id == curData.Icon);
                    if (fs != null)
                    {
                        curData.IconFileAssetUrl = fs.Url;
                    }
                }
                var creator = await _Repository._DbContext.Accounts.FirstOrDefaultAsync(x => x.Creator == curData.Creator);
                curData.CreatorName = creator != null ? creator.Name : "";
                var modifier = await _Repository._DbContext.Accounts.FirstOrDefaultAsync(x => x.Modifier == curData.Modifier);
                curData.ModifierName = modifier != null ? modifier.Name : "";
                if (!string.IsNullOrWhiteSpace(curData.CategoryId))
                {
                    var cat = await _Repository._DbContext.AssetCategories.FirstOrDefaultAsync(x => x.Id == curData.CategoryId);
                    curData.CategoryName = cat != null ? cat.Name : "";
                }
                pagedData.Data.Add(curData.ToDTO());
            }

            return Ok(pagedData);
        }
        #endregion

        /*********************Preference*********************/

        #region GetPreference 获取偏好设置
        /// <summary>
        /// 获取偏好设置
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [Route("Preference")]
        [HttpGet]
        [ProducesResponseType(typeof(Preference), 200)]
        public async Task<IActionResult> GetPreference(string key)
        {
            var t = new T();
            var typeName = t.GetType().Name;
            var preference = await _Repository._DbContext.Preferences.FirstOrDefaultAsync(x => x.Type == typeName && x.Key == key);
            return Ok(preference != null ? preference : new Preference());
        } 
        #endregion

        #region CreatePreference 创建偏好设置
        /// <summary>
        /// 创建偏好设置
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("Preference")]
        [HttpPost]
        [ValidateModel]
        [ProducesResponseType(typeof(ValidationResultModel), 400)]
        [ProducesResponseType(typeof(Preference), 200)]
        public async Task<IActionResult> CreatePreference([FromBody]PreferenceCreateModel model)
        {
            var t = new T();
            var typeName = t.GetType().Name;
            var preference = await _Repository._DbContext.Preferences.FirstOrDefaultAsync(x => x.Type == typeName && x.Key == model.Key);
            if (preference != null)
            {
                preference.Value = model.Value;
                _Repository._DbContext.Update(preference);
            }
            else
            {
                preference = new Preference();
                preference.Id = GuidGen.NewGUID();
                preference.Type = typeName;
                preference.Key = model.Key;
                preference.Value = model.Value;
                _Repository._DbContext.Add(preference);
            }
            await _Repository._DbContext.SaveChangesAsync();
            return Ok(preference);
        }
        #endregion

        #region Share 分享数据信息
        /// <summary>
        /// 分享数据信息
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [Route("Share")]
        [HttpPut]
        public async Task<IActionResult> Share(string ids)
        {
            if (string.IsNullOrWhiteSpace(ids))
                return BadRequest();

            var idsArr = ids.Split(",", StringSplitOptions.RemoveEmptyEntries);
            foreach (var id in idsArr)
            {
                var entity = await _Repository._DbContext.Set<T>().FirstOrDefaultAsync(x => x.Id == id);
                if (entity != null)
                {
                    entity.ResourceType = (int)ResourceTypeEnum.Organizational_SubShare;
                    entity.ModifiedTime = DateTime.UtcNow;
                    _Repository._DbContext.Update<T>(entity);
                }
                await _Repository._DbContext.SaveChangesAsync();
            }
            return Ok();
        }
        #endregion

        #region CancelShare 取消分享数据
        /// <summary>
        /// 取消分享数据
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [Route("CancelShare")]
        [HttpPut]
        public async Task<IActionResult> CancelShare(string ids)
        {
            if (string.IsNullOrWhiteSpace(ids))
                return BadRequest();

            var idsArr = ids.Split(",", StringSplitOptions.RemoveEmptyEntries);
            foreach (var id in idsArr)
            {
                var entity = await _Repository._DbContext.Set<T>().FirstOrDefaultAsync(x => x.Id == id);
                if (entity != null)
                {
                    entity.ResourceType = (int)ResourceTypeEnum.Personal;
                    entity.ModifiedTime = DateTime.UtcNow;
                    _Repository._DbContext.Update<T>(entity);
                }
                await _Repository._DbContext.SaveChangesAsync();
            }
            return Ok();
        }
        #endregion
    }
}