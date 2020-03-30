using ApiModel.Consts;
using ApiModel.Entities;
using ApiModel.Enums;
using ApiServer.Data;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ApiServer.Repositories
{
    public class ProductSpecRepository : ListableRepository<ProductSpec, ProductSpecDTO>
    {
        public ProductSpecRepository(ApiDbContext context, ITreeRepository<PermissionTree> permissionTreeRep)
            : base(context, permissionTreeRep)
        {
        }

        public override ResourceTypeEnum ResourceTypeSetting
        {
            get
            {
                return ResourceTypeEnum.Organizational;
            }
        }

        public async override Task<IQueryable<ProductSpec>> _GetPermissionData(string accid, DataOperateEnum dataOp, bool withInActive = false)
        {
            var emptyQuery = Enumerable.Empty<ProductSpec>().AsQueryable();
            var query = emptyQuery;

            var currentAcc = await _DbContext.Accounts.Select(x => new Account() { Id = x.Id, OrganizationId = x.OrganizationId, Type = x.Type }).FirstOrDefaultAsync(x => x.Id == accid);
            if (currentAcc == null)
                return query;

            //数据状态
            if (withInActive)
                query = _DbContext.Set<ProductSpec>();
            else
                query = _DbContext.Set<ProductSpec>().Where(x => x.ActiveFlag == AppConst.I_DataState_Active);

            /*
             * 读取,修改,删除操作
             *      管理员:操作自己组织的创建的
             *      用户:操作自己创建的数据
             */


            if (currentAcc.Type == AppConst.AccountType_BrandAdmin|| currentAcc.Type == AppConst.AccountType_BrandMember || currentAcc.Type == AppConst.AccountType_PartnerAdmin || currentAcc.Type == AppConst.AccountType_PartnerMember || currentAcc.Type == AppConst.AccountType_SupplierAdmin || currentAcc.Type == AppConst.AccountType_SupplierMember)
            {
                return query.Where(x => x.OrganizationId == currentAcc.OrganizationId);
            }


            return query.Where(x => x.Creator == accid);
        }

        #region SatisfyCreateAsync 判断数据是否满足存储规范
        /// <summary>
        /// 判断数据是否满足存储规范
        /// </summary>
        /// <param name="accid"></param>
        /// <param name="data"></param>
        /// <param name="modelState"></param>
        /// <returns></returns>
        public override async Task SatisfyCreateAsync(string accid, ProductSpec data, ModelStateDictionary modelState)
        {
            var existProduct = false;
            if (data.Product != null)
            {
                existProduct = true;
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(data.ProductId))
                    existProduct = await _DbContext.Products.CountAsync(x => x.Id == data.ProductId && x.ActiveFlag == AppConst.I_DataState_Active) > 0;
            }

            if (!existProduct)
                modelState.AddModelError("ProductId", "对应产品记录不存在");
            await Task.FromResult(string.Empty);
        }
        #endregion

        #region SatisfyUpdateAsync 判断数据是否满足更新规范
        /// <summary>
        /// 判断数据是否满足更新规范
        /// </summary>
        /// <param name="accid"></param>
        /// <param name="data"></param>
        /// <param name="modelState"></param>
        /// <returns></returns>
        public override async Task SatisfyUpdateAsync(string accid, ProductSpec data, ModelStateDictionary modelState)
        {
            var existProduct = false;
            if (data.Product != null)
            {
                existProduct = true;
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(data.ProductId))
                    existProduct = await _DbContext.Products.CountAsync(x => x.Id == data.ProductId && x.ActiveFlag == AppConst.I_DataState_Active) > 0;
            }

            if (!existProduct)
                modelState.AddModelError("ProductId", "对应产品记录不存在");
            await Task.FromResult(string.Empty);
        }
        #endregion

        #region GetByIdAsync 根据Id返回实体DTO数据信息
        /// <summary>
        /// 根据Id返回实体DTO数据信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override async Task<ProductSpecDTO> GetByIdAsync(string id)
        {
            var res = await _GetByIdAsync(id);
            var product = await _DbContext.Products.FirstOrDefaultAsync(x => x.Id == res.ProductId);
            if (product != null)
            {
                res.Brand = product.Brand;
            }
            if (!string.IsNullOrWhiteSpace(res.Icon))
            {
                var fs = await _DbContext.Files.FirstOrDefaultAsync(x => x.Id == res.Icon);
                if (fs != null)
                {
                    res.IconFileAssetUrl = fs.Url;
                }
            }
            if (!string.IsNullOrWhiteSpace(res.Album))
            {
                var albumIds = res.Album.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
                for (int idx = albumIds.Count - 1; idx >= 0; idx--)
                {
                    var ass = await _DbContext.Files.FirstOrDefaultAsync(x => x.Id == albumIds[idx]);
                    if (ass != null)
                        res.AlbumAsset.Add(ass);
                }
            }
            if (!string.IsNullOrWhiteSpace(res.StaticMeshIds))
            {
                try
                {
                    var map = JsonConvert.DeserializeObject<SpecMeshMap>(res.StaticMeshIds);
                    for (int idx = map.Items.Count - 1; idx >= 0; idx--)
                    {
                        var refMesh = await _DbContext.StaticMeshs.FirstOrDefaultAsync(x => x.Id == map.Items[idx].StaticMeshId);
                        if (refMesh != null)
                        {
                            //var tmp = await _DbContext.Files.FirstOrDefaultAsync(x => x.Id == refMesh.FileAssetId);
                            //if (tmp != null)
                            //    refMesh.FileAsset = tmp;
                            if (!string.IsNullOrWhiteSpace(refMesh.Icon))
                            {
                                var fs = await _DbContext.Files.FirstOrDefaultAsync(x => x.Id == refMesh.Icon);
                                if (fs != null)
                                {
                                    refMesh.IconFileAssetUrl = fs.Url;
                                }
                            }
                            if (!string.IsNullOrWhiteSpace(refMesh.FileAssetId))
                            {
                                var fs = await _DbContext.Files.FirstOrDefaultAsync(x => x.Id == refMesh.FileAssetId);
                                if (fs != null)
                                {
                                    refMesh.FileAssetUrl = fs.Url;
                                    refMesh.FileAsset = fs;
                                }
                            }

                            res.StaticMeshAsset.Add(refMesh);
                        }
                    }
                }
                catch (Exception) { }
            }

            return res.ToDTO();
        }
        #endregion

        #region AddStaticMeshRelated 往产品规格里面添加模型依赖信息
        /// <summary>
        /// 往产品规格里面添加模型依赖信息
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="staticMeshId"></param>
        public void AddStaticMeshRelated(ProductSpec entity, string staticMeshId)
        {
            var map = string.IsNullOrWhiteSpace(entity.StaticMeshIds) ? new SpecMeshMap() : JsonConvert.DeserializeObject<SpecMeshMap>(entity.StaticMeshIds);
            var exist = map.Items.Where(x => x.StaticMeshId == staticMeshId).Count() > 0;
            if (!exist)
            {
                var item = new SpecMeshMapItem();
                item.StaticMeshId = staticMeshId;
                map.Items.Add(item);
            }
            entity.StaticMeshIds = JsonConvert.SerializeObject(map);
        }
        #endregion

        #region RemoveStaticMeshRelated 往产品规格里面移除模型依赖信息
        /// <summary>
        /// 往产品规格里面移除模型依赖信息
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="staticMeshId"></param>
        public void RemoveStaticMeshRelated(ProductSpec entity, string staticMeshId)
        {
            var map = string.IsNullOrWhiteSpace(entity.StaticMeshIds) ? new SpecMeshMap() : JsonConvert.DeserializeObject<SpecMeshMap>(entity.StaticMeshIds);
            for (int idx = map.Items.Count - 1; idx >= 0; idx--)
            {
                if (map.Items[idx].StaticMeshId == staticMeshId)
                    map.Items.RemoveAt(idx);
            }
            entity.StaticMeshIds = JsonConvert.SerializeObject(map);
        }
        #endregion
    }
}
