using ApiModel.Consts;
using ApiModel.Entities;
using ApiServer.Controllers.Common;
using ApiServer.Filters;
using ApiServer.Models;
using ApiServer.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiServer.Controllers
{
    [Authorize]
    [Route("/[controller]")]
    public class CategoryController : CommonController<AssetCategory, AssetCategoryDTO>
    {

        #region 构造函数
        public CategoryController(IRepository<AssetCategory, AssetCategoryDTO> repository)
        : base(repository)
        {
        }
        #endregion

        #region _getParentCategoryAndSameLevelChildren 根据父节点Id获取父节点下第一级子节点分类
        /// <summary>
        /// 根据父节点Id获取父节点下第一级子节点分类
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        protected async Task<AssetCategoryDTO> _getParentCategoryAndSameLevelChildren(string parentId)
        {
            var parentCat = await _Repository._DbContext.AssetCategories.FirstAsync(d => d.Id == parentId && d.ActiveFlag == AppConst.I_DataState_Active);
            if (parentCat != null)
            {
                var dto = parentCat.ToDTO();
                dto.Children = new List<AssetCategoryDTO>();
                var sameLevelCats = await _Repository._DbContext.AssetCategories.Where(d => d.ParentId == parentId && d.ActiveFlag == AppConst.I_DataState_Active).Select(x => x.ToDTO()).OrderBy(x => x.DisplayIndex).ToListAsync();
                dto.Children.AddRange(sameLevelCats);
                return dto;
            }
            return new AssetCategoryDTO();
        }
        #endregion

        #region Get 根据id获取分类信息
        /// <summary>
        /// 根据id获取分类信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AssetCategoryDTO), 200)]
        public async Task<IActionResult> Get(string id)
        {
            return await _GetByIdRequest(id);
        }
        #endregion

        #region Get 获取整个类型(product, material)下的所有分类信息，已经整理成一个树结构
        /// <summary>
        /// 获取整个类型(product, material)下的所有分类信息，已经整理成一个树结构
        /// </summary>
        /// <param name="type"></param>
        /// <param name="organId"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(AssetCategoryDTO), 200)]
        public async Task<AssetCategoryDTO> Get(string type, string organId)
        {
            if (type == AppConst.S_Category_Product || type == AppConst.S_Category_Material || type == AppConst.S_Category_ProductGroup)
            {
                organId = (await _GetCurrentUserRootOrgan()).Id;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(organId))
                    organId = await _GetCurrentUserOrganId();
            }
            //if (string.IsNullOrEmpty(type))
            //    type = "product";
            return await (_Repository as AssetCategoryRepository).GetCategoryAsync(type, organId);
        }
        #endregion

        #region GetAll 获取所有分类信息
        /// <summary>
        /// 获取所有分类信息
        /// </summary>
        /// <param name="organId"></param>
        /// <returns></returns>
        [Route("all")]
        [HttpGet]
        [ProducesResponseType(typeof(AssetCategoryPack), 200)]
        public async Task<AssetCategoryPack> GetAll(string organId)
        {
            if (string.IsNullOrWhiteSpace(organId))
                organId = (await _GetCurrentUserRootOrgan()).Id;

            AssetCategoryPack pack = new AssetCategoryPack();
            pack.Categories = new List<AssetCategoryDTO>();

            //root node parentid == "".
            List<AssetCategory> rootlist = await _Repository._DbContext.AssetCategories.Where(d => d.ParentId == "" && d.OrganizationId == organId && d.ActiveFlag == AppConst.I_DataState_Active).ToListAsync();
            //List<AssetCategory> isolateRootlist = await _Repository._DbContext.AssetCategories.Where(d => d.Isolate == true && d.OrganizationId == organId && d.ActiveFlag == AppConst.I_DataState_Active).ToListAsync();
            //rootlist.AddRange(isolateRootlist);
            foreach (var root in rootlist)
            {
                AssetCategoryDTO cat = await (_Repository as AssetCategoryRepository).GetCategoryAsync(root.Type, organId);
                if (cat != null)
                    pack.Categories.Add(cat);
                //AssetCategoryDTO cat = await (_Repository as AssetCategoryRepository).GetCategoryIsolateAsync(root.Id, organId);
                //if (cat != null)
                //    pack.Categories.Add(cat);
            }
            return pack;
        }
        #endregion

        #region GetFlat 获取扁平结构的分类信息
        /// <summary>
        /// 获取扁平结构的分类信息
        /// </summary>
        /// <param name="type"></param>
        /// <param name="organId"></param>
        /// <returns></returns>
        [Route("Flat")]
        [HttpGet]
        [ProducesResponseType(typeof(List<AssetCategoryDTO>), 200)]
        public async Task<List<AssetCategoryDTO>> GetFlat(string type, string organId)
        {
            if (string.IsNullOrWhiteSpace(organId))
                organId = (await _GetCurrentUserRootOrgan()).Id;
            //已经在重构版本的里面改好,这里使用临时方案
            var existCat = _Repository._DbContext.AssetCategories.Any(x => x.OrganizationId == organId && x.Type == type);
            if (!existCat)
            {
                await Get(type, organId);
            }
            return await (_Repository as AssetCategoryRepository).GetFlatCategory(type, organId);
        }
        #endregion

        #region Post 创建一个分类
        /// <summary>
        /// 创建一个分类
        /// 必须指定一个父级ID，不能主动创建根节点，根节点在get时会自动创建。
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateModel]
        [ProducesResponseType(typeof(AssetCategoryDTO), 200)]
        [ProducesResponseType(typeof(ValidationResultModel), 400)]
        public async Task<IActionResult> Post([FromBody]AssetCategoryCreateModel model)
        {
            var mapping = new Func<AssetCategory, Task<AssetCategory>>(async (entity) =>
            {
                if (string.IsNullOrWhiteSpace(model.OrganizationId))
                    model.OrganizationId = await _GetCurrentUserOrganId();
                entity.Name = model.Name;
                entity.Description = model.Description;
                entity.ParentId = model.ParentId;
                entity.CustomData = model.CustomData;
                entity.Tag = model.Tag;
                entity.Isolate = model.Isolate;
                if (!string.IsNullOrWhiteSpace(model.ParentId))
                    entity.Type = (await _Repository._DbContext.AssetCategories.FirstAsync(d => d.Id == model.ParentId)).Type;
                else
                    entity.Type = model.Type;
                entity.OrganizationId = model.OrganizationId;
                entity.DisplayIndex = await _Repository._DbContext.AssetCategories.Where(d => d.ParentId == model.ParentId).CountAsync();
                return entity;
            });
            return await _PostRequest(mapping);
        }
        #endregion

        #region Put 修改一个分类的基本信息
        /// <summary>
        /// 修改一个分类的基本信息
        /// 名称，描述，图标。其他属性会被忽略，比如子分类，显示位置。
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [ValidateModel]
        [ProducesResponseType(typeof(AssetCategoryDTO), 200)]
        [ProducesResponseType(typeof(ValidationResultModel), 400)]
        public async Task<IActionResult> Put([FromBody]AssetCategoryEditModel model)
        {
            var mapping = new Func<AssetCategory, Task<AssetCategory>>(async (entity) =>
            {
                if (string.IsNullOrWhiteSpace(model.OrganizationId))
                    model.OrganizationId = await _GetCurrentUserOrganId();
                entity.Name = model.Name;
                entity.Description = model.Description;
                entity.ParentId = model.ParentId;
                entity.CustomData = model.CustomData;
                entity.Tag = model.Tag;
                entity.Isolate = model.Isolate;
                entity.OrganizationId = model.OrganizationId;
                return entity;
            });
            return await _PutRequest(model.Id, mapping);
        }
        #endregion

        #region Move 将一个分类移动到另外一个分类下作为其子分类
        /// <summary>
        /// 将一个分类移动到另外一个分类下作为其子分类
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <param name="targetId"></param>
        /// <returns></returns>
        [Route("Move")]
        [HttpPost]
        [ProducesResponseType(typeof(AssetCategoryDTO), 200)]
        public async Task<IActionResult> Move(string type, string id, string targetId)
        {
            string result = await (_Repository as AssetCategoryRepository).MoveAsync(type, id, targetId);
            if (result == "")
                return Ok();
            return BadRequest(result);
        }
        #endregion

        #region MoveUp 上移分类
        /// <summary> 
        /// 上移分类
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("MoveUp")]
        [HttpPut]
        [ProducesResponseType(typeof(AssetCategoryDTO), 200)]
        public async Task<IActionResult> MoveUp(string id)
        {
            var cat = await _Repository._DbContext.AssetCategories.FirstOrDefaultAsync(x => x.Id == id);
            if (cat != null && cat.DisplayIndex >= 1)
            {
                var interChangeCat = await _Repository._DbContext.AssetCategories.FirstOrDefaultAsync(x => x.ParentId == cat.ParentId && x.DisplayIndex == (cat.DisplayIndex - 1) && x.Type == cat.Type && x.ActiveFlag == AppConst.I_DataState_Active);
                if (interChangeCat != null)
                {
                    cat.DisplayIndex--;
                    interChangeCat.DisplayIndex++;
                    _Repository._DbContext.AssetCategories.Update(cat);
                    _Repository._DbContext.AssetCategories.Update(interChangeCat);
                    await _Repository._DbContext.SaveChangesAsync();
                    var dto = await _getParentCategoryAndSameLevelChildren(cat.ParentId);
                    return Ok(dto);
                }

            }
            return BadRequest();
        }
        #endregion

        #region MoveDown 下移分类
        /// <summary>
        /// 下移分类
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("MoveDown")]
        [HttpPut]
        [ProducesResponseType(typeof(AssetCategoryDTO), 200)]
        public async Task<IActionResult> MoveDown(string id)
        {
            var cat = await _Repository._DbContext.AssetCategories.FirstOrDefaultAsync(x => x.Id == id);
            if (cat != null)
            {
                var interChangeCat = await _Repository._DbContext.AssetCategories.FirstOrDefaultAsync(x => x.ParentId == cat.ParentId && x.DisplayIndex == (cat.DisplayIndex + 1) && x.Type == cat.Type && x.ActiveFlag == AppConst.I_DataState_Active);
                if (interChangeCat != null)
                {
                    cat.DisplayIndex++;
                    interChangeCat.DisplayIndex--;
                    _Repository._DbContext.AssetCategories.Update(cat);
                    _Repository._DbContext.AssetCategories.Update(interChangeCat);
                    await _Repository._DbContext.SaveChangesAsync();
                    var dto = await _getParentCategoryAndSameLevelChildren(cat.ParentId);
                    return Ok(dto);
                }

            }
            return BadRequest();
        }
        #endregion

        #region Transfer 将一个分类下的所有资源转移到另外一个分类中，目标分类必须是没有子分类的分类
        /// <summary>
        /// 将一个分类下的所有资源转移到另外一个分类中，目标分类必须是没有子分类的分类
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <param name="targetId"></param>
        /// <returns></returns>
        [Route("Transfer")]
        [HttpPost]
        public async Task<IActionResult> Transfer(string type, string id, string targetId)
        {
            string result = await (_Repository as AssetCategoryRepository).TransferAsync(type, id, targetId);
            if (result == "")
                return Ok();
            return BadRequest(result);
        }
        #endregion

        #region DisplayIndex 设置分类在父级分类中的显示顺序
        /// <summary>
        /// 设置分类在父级分类中的显示顺序，index从0到childrencount -1，会自动纠正非法index。
        /// 返回此分类的父级分类以及兄弟分类
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        [Route("DisplayIndex")]
        [HttpPost]
        [Produces(typeof(AssetCategoryDTO))]
        public async Task<IActionResult> DisplayIndex(string type, string id, int index)
        {
            var result = await (_Repository as AssetCategoryRepository).SetDisplayIndex(type, id, index);
            if (result == null)
                return BadRequest();

            return Ok(result);
        }
        #endregion
    }
}
