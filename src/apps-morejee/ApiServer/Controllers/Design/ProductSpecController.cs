using ApiModel.Entities;
using ApiModel.Enums;
using ApiServer.Controllers.Common;
using ApiServer.Filters;
using ApiServer.Models;
using ApiServer.Repositories;
using ApiServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace ApiServer.Controllers
{
    /// <summary>
    /// 产品规格管理控制器
    /// </summary>
    
    [Route("/[controller]")]
    public class ProductSpecController : ListableController<ProductSpec, ProductSpecDTO>
    {
        public IRepository<Product, ProductDTO> _ProductRep { get; }

        #region 构造函数
        public ProductSpecController(IRepository<ProductSpec, ProductSpecDTO> repository, IRepository<Product, ProductDTO> productRep)
            : base(repository)
        {
            _ProductRep = productRep;
        }

        #endregion

        #region Get 根据id获取产品规格信息
        /// <summary>
        /// 根据id获取产品规格信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProductSpecDTO), 200)]
        public async Task<IActionResult> Get(string id)
        {
            return await _GetByIdRequest(id);
        }
        #endregion

        #region Post 新建产品规格信息
        /// <summary>
        /// 新建产品规格信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateModel]
        [ProducesResponseType(typeof(ProductSpecDTO), 200)]
        [ProducesResponseType(typeof(ValidationResultModel), 400)]
        public async Task<IActionResult> Post([FromBody]ProductSpecCreateModel model)
        {
            var mapping = new Func<ProductSpec, Task<ProductSpec>>(async (entity) =>
            {
                entity.Name = model.Name;
                entity.Description = model.Description;
                entity.ProductId = model.ProductId;
                entity.Price = model.Price;
                entity.PartnerPrice = model.PartnerPrice;
                entity.PurchasePrice = model.PurchasePrice;
                entity.Icon = model.IconAssetId;
                entity.Components = model.Components;
                entity.StaticMeshIds = model.StaticMeshIds;
                entity.Slots = model.Slots;
                entity.Color = model.Color;
                entity.ResourceType = (int)ResourceTypeEnum.Organizational;
                return await Task.FromResult(entity);
            });
            return await _PostRequest(mapping);
        }
        #endregion

        #region Put 编辑产品规格信息
        /// <summary>
        /// 编辑产品规格信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [ValidateModel]
        [ProducesResponseType(typeof(ProductSpecDTO), 200)]
        [ProducesResponseType(typeof(ValidationResultModel), 400)]
        public async Task<IActionResult> Put([FromBody]ProductSpecEditModel model)
        {
            var mapping = new Func<ProductSpec, Task<ProductSpec>>(async (entity) =>
            {
                entity.Name = model.Name;
                entity.Description = model.Description;
                entity.Price = model.Price;
                entity.PartnerPrice = model.PartnerPrice;
                entity.PurchasePrice = model.PurchasePrice;
                entity.Components = model.Components;
                entity.StaticMeshIds = model.StaticMeshIds;
                entity.Slots = model.Slots;
                entity.Color = model.Color;
                entity.TPID = model.TPID;
                if (!string.IsNullOrWhiteSpace(model.IconAssetId))
                    entity.Icon = model.IconAssetId;
                return await Task.FromResult(entity);
            });
            return await _PutRequest(model.Id, mapping);
        }
        #endregion

        #region GetBriefById 根据Id获取产品规格简洁的信息
        /// <summary>
        /// 根据Id获取产品规格简洁的信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("Brief/{id}")]
        [ProducesResponseType(typeof(ProductSpecDTO), 200)]
        public async Task<IActionResult> GetBriefById(string id)
        {
            var data = await _Repository._DbContext.ProductSpec.Include(x => x.Product).FirstOrDefaultAsync(x => x.Id == id);
            if (data == null)
                return NotFound();
            var dto = new ProductSpecDTO();
            dto.Id = data.Id;
            dto.Name = data.Name;
            dto.Description = data.Description;
            dto.OrganizationId = data.OrganizationId;
            dto.CreatedTime = data.CreatedTime;
            dto.ModifiedTime = data.ModifiedTime;
            dto.Creator = data.Creator;
            dto.Modifier = data.Modifier;
            dto.Price = data.Price;
            dto.PartnerPrice = data.PartnerPrice;
            dto.PurchasePrice = data.PurchasePrice;
            dto.TPID = data.TPID;
            dto.ProductId = data.ProductId;
            dto.Brand = data.Product.Brand;
            if (!string.IsNullOrWhiteSpace(data.Icon))
                dto.Icon = await _Repository._DbContext.Files.Where(x => x.Id == data.Icon).Select(x => x.Url).FirstOrDefaultAsync();

            return Ok(dto);
        }
        #endregion

        #region UploadStaticMesh 上传模型文件信息(非真实上传文件,文件已经提交到File,此处只是添加文件id到规格相关字段信息)
        /// <summary>
        /// 上传模型文件信息(非真实上传文件,文件已经提交到File,此处只是添加文件id到规格相关字段信息)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("UploadStaticMesh")]
        [HttpPut]
        [ValidateModel]
        [ProducesResponseType(typeof(ProductSpecDTO), 200)]
        [ProducesResponseType(typeof(ValidationResultModel), 400)]
        public async Task<IActionResult> UploadStaticMesh([FromBody]SpecStaticMeshUploadModel model)
        {
            var mapping = new Func<ProductSpec, Task<ProductSpec>>(async (entity) =>
            {
                (_Repository as ProductSpecRepository).AddStaticMeshRelated(entity, model.StaticMeshId);
                return await Task.FromResult(entity);
            });
            return await _PutRequest(model.ProductSpecId, mapping);
        }
        #endregion

        #region DeleteStaticMesh 删除模型文件信息(文件已经提交到File,删除不会对文件进行真实删除)
        /// <summary>
        /// 删除模型文件信息(文件已经提交到File,删除不会对文件进行真实删除)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("DeleteStaticMesh")]
        [HttpPut]
        [ValidateModel]
        [ProducesResponseType(typeof(ProductSpecDTO), 200)]
        [ProducesResponseType(typeof(ValidationResultModel), 400)]
        public async Task<IActionResult> DeleteStaticMesh([FromBody]SpecStaticMeshUploadModel model)
        {
            var mapping = new Func<ProductSpec, Task<ProductSpec>>(async (entity) =>
            {
                (_Repository as ProductSpecRepository).RemoveStaticMeshRelated(entity, model.StaticMeshId);
                return await Task.FromResult(entity);
            });
            return await _PutRequest(model.ProductSpecId, mapping);
        }
        #endregion

        #region UploadMaterial 上传材质文件信息(非真实上传文件,文件已经提交到File,此处只是添加文件id到规格相关字段信息)
        /// <summary>
        /// 上传材质文件信息(非真实上传文件,文件已经提交到File,此处只是添加文件id到规格相关字段信息)
        /// </summary>
        /// <param name="material"></param>
        /// <returns></returns>
        [Route("UploadMaterial")]
        [HttpPut]
        [ValidateModel]
        [ProducesResponseType(typeof(ProductSpecDTO), 200)]
        [ProducesResponseType(typeof(ValidationResultModel), 400)]
        public async Task<IActionResult> UploadMaterial([FromBody]SpecMaterialUploadModel material)
        {
            var mapping = new Func<ProductSpec, Task<ProductSpec>>(async (spec) =>
            {
                var map = string.IsNullOrWhiteSpace(spec.StaticMeshIds) ? new SpecMeshMap() : JsonConvert.DeserializeObject<SpecMeshMap>(spec.StaticMeshIds);
                for (int idx = map.Items.Count - 1; idx >= 0; idx--)
                {
                    if (map.Items[idx].StaticMeshId == material.StaticMeshId)
                    {
                        var metids = map.Items[idx].MaterialIds == null ? new List<string>() : map.Items[idx].MaterialIds;
                        metids.Add(material.MaterialId);
                        map.Items[idx].MaterialIds = metids;
                    }
                }
                spec.StaticMeshIds = JsonConvert.SerializeObject(map);
                return await Task.FromResult(spec);
            });
            return await _PutRequest(material.ProductSpecId, mapping);
        }
        #endregion

        #region DeleteMaterial 删除材质文件信息(文件已经提交到File,删除不会对文件进行真实删除)
        /// <summary>
        /// 删除材质文件信息(文件已经提交到File,删除不会对文件进行真实删除)
        /// </summary>
        /// <param name="material"></param>
        /// <returns></returns>
        [Route("DeleteMaterial")]
        [HttpPut]
        [ValidateModel]
        [ProducesResponseType(typeof(ProductSpecDTO), 200)]
        [ProducesResponseType(typeof(ValidationResultModel), 400)]
        public async Task<IActionResult> DeleteMaterial([FromBody]SpecMaterialUploadModel material)
        {
            var mapping = new Func<ProductSpec, Task<ProductSpec>>(async (spec) =>
            {
                var map = string.IsNullOrWhiteSpace(spec.StaticMeshIds) ? new SpecMeshMap() : JsonConvert.DeserializeObject<SpecMeshMap>(spec.StaticMeshIds);
                for (int idx = map.Items.Count - 1; idx >= 0; idx--)
                {
                    if (map.Items[idx].StaticMeshId == material.StaticMeshId)
                    {
                        var metids = map.Items[idx].MaterialIds;
                        for (int ndx = metids.Count - 1; ndx >= 0; ndx--)
                        {
                            if (metids[ndx] == material.MaterialId)
                                metids.RemoveAt(ndx);
                        }
                        map.Items[idx].MaterialIds = metids;
                    }
                }
                spec.StaticMeshIds = JsonConvert.SerializeObject(map);
                return await Task.FromResult(spec);
            });
            return await _PutRequest(material.ProductSpecId, mapping);
        }
        #endregion

        #region UploadAlbum 上传规格详细相册信息(非真实上传文件,文件已经提交到File,此处只是添加文件id到规格相关字段信息)
        /// <summary>
        /// 上传规格详细相册信息(非真实上传文件,文件已经提交到File,此处只是添加文件id到规格相关字段信息)
        /// </summary>
        /// <param name="icon"></param>
        /// <returns></returns>
        [Route("UploadAlbum")]
        [HttpPut]
        [ValidateModel]
        [ProducesResponseType(typeof(ProductSpecDTO), 200)]
        [ProducesResponseType(typeof(ValidationResultModel), 400)]
        public async Task<IActionResult> UploadAlbum([FromBody]IconModel icon)
        {
            var mapping = new Func<ProductSpec, Task<ProductSpec>>(async (spec) =>
            {
                var albumIds = string.IsNullOrWhiteSpace(spec.Album) ? new List<string>() : spec.Album.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
                albumIds.Add(icon.AssetId);
                spec.Album = string.Join(",", albumIds);
                return await Task.FromResult(spec);
            });
            return await _PutRequest(icon.ObjId, mapping);
        }
        #endregion

        #region DeleteAlbum 删除规格详细图片信息(文件已经提交到File,删除不会对文件进行真实删除)
        /// <summary>
        /// 删除规格详细图片信息(文件已经提交到File,删除不会对文件进行真实删除)
        /// </summary>
        /// <param name="icon"></param>
        /// <returns></returns>
        [Route("DeleteAlbum")]
        [HttpPut]
        [ValidateModel]
        [ProducesResponseType(typeof(ProductSpecDTO), 200)]
        [ProducesResponseType(typeof(ValidationResultModel), 400)]
        public async Task<IActionResult> DeleteAlbum([FromBody]IconModel icon)
        {
            var mapping = new Func<ProductSpec, Task<ProductSpec>>(async (spec) =>
            {
                var albumIds = string.IsNullOrWhiteSpace(spec.Album) ? new List<string>() : spec.Album.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
                for (int idx = albumIds.Count - 1; idx >= 0; idx--)
                {
                    if (albumIds[idx] == icon.AssetId)
                        albumIds.RemoveAt(idx);
                }
                spec.Album = string.Join(",", albumIds);
                return await Task.FromResult(spec);
            });
            return await _PutRequest(icon.ObjId, mapping);
        }
        #endregion

        #region AutoRelateSpec 根据模型文件Id自动创建/关联 产品以及产品规格信息
        /// <summary>
        /// 根据模型文件Id自动创建/关联 产品以及产品规格信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("AutoRelateSpec")]
        [HttpPost]
        [ValidateModel]
        [ProducesResponseType(typeof(ProductSpecAutoRelatedDTO), 200)]
        [ProducesResponseType(typeof(ValidationResultModel), 400)]
        public async Task<IActionResult> AutoRelateSpec([FromBody]ProductSpecAutoRelatedEditModel model)
        {
            if (!ModelState.IsValid)
                return new ValidationFailedResult(ModelState);

            var dto = new ProductSpecAutoRelatedDTO();
            var accid = AuthMan.GetAccountId(this);
            if (!string.IsNullOrWhiteSpace(model.ProductId))
            {
                var exist = await _ProductRep.ExistAsync(model.ProductId);
                if (!exist)
                {
                    ModelState.AddModelError("ProductId", "没有对应产品记录信息");
                    return new ValidationFailedResult(ModelState);
                }
                var canRead = await _ProductRep.CanReadAsync(accid, model.ProductId);
                if (!canRead)
                {
                    ModelState.AddModelError("ProductId", "没有操作权限");
                    return new ValidationFailedResult(ModelState);
                }
            }

            var staticMesh = await _Repository._DbContext.StaticMeshs.FindAsync(model.StaticMeshId);
            if (staticMesh == null)
            {
                ModelState.AddModelError("StaticMeshId", "没有对应模型记录信息");
                return new ValidationFailedResult(ModelState);
            }

            var product = await _ProductRep._GetByIdAsync(model.ProductId);
            if (!product.IsPersistence())
            {
                #region 创建产品
                product.Name = staticMesh.Name;
                product.Icon = staticMesh.Icon;
                product.Brand = model.Brand;
                await _ProductRep.SatisfyCreateAsync(accid, product, ModelState);
                if (!ModelState.IsValid)
                    return new ValidationFailedResult(ModelState);

                await _ProductRep.CreateAsync(accid, product);
                #endregion

                #region 创建规格
                var spec = new ProductSpec();
                spec.Name = staticMesh.Name;
                spec.Product = product;
                spec.ProductId = product.Id;
                spec.Icon = staticMesh.Icon;
                (_Repository as ProductSpecRepository).AddStaticMeshRelated(spec, model.StaticMeshId);
                await _Repository.SatisfyCreateAsync(accid, spec, ModelState);
                if (!ModelState.IsValid)
                    return new ValidationFailedResult(ModelState);

                await _Repository.CreateAsync(accid, spec);
                #endregion

                dto.ProductSpecId = spec.Id;
            }
            else
            {
                var specs = await (_ProductRep as ProductRepository).GetSpecByStaticMesh(product.Id, model.StaticMeshId);
                if (specs.Count == 0)
                {
                    #region 创建规格
                    var spec = new ProductSpec();
                    spec.Name = staticMesh.Name;
                    spec.Product = product;
                    spec.Icon = staticMesh.Icon;
                    (_Repository as ProductSpecRepository).AddStaticMeshRelated(spec, model.StaticMeshId);
                    await _Repository.SatisfyCreateAsync(accid, spec, ModelState);
                    if (!ModelState.IsValid)
                        return new ValidationFailedResult(ModelState);

                    await _Repository.CreateAsync(accid, spec);
                    #endregion

                    dto.ProductSpecId = spec.Id;
                }
                else
                {
                    dto.ProductSpecId = specs[0].Id;
                }
            }
            dto.ProductId = product.Id;
            return Ok(dto);
        }
        #endregion
    }
}