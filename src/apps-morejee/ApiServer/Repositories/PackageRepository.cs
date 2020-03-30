using ApiModel.Consts;
using ApiModel.Entities;
using ApiModel.Enums;
using ApiServer.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
namespace ApiServer.Repositories
{
    public class PackageRepository : ResourceRepositoryBase<Package, PackageDTO>
    {
        protected IRepository<ProductReplaceGroup, ProductReplaceGroupDTO> _ProductReplaceGroupRep;
        public PackageRepository(ApiDbContext context, ITreeRepository<PermissionTree> permissionTreeRep, IRepository<ProductReplaceGroup, ProductReplaceGroupDTO> replaceGroupRep)
            : base(context, permissionTreeRep)
        {
            _ProductReplaceGroupRep = replaceGroupRep;
        }

        public override ResourceTypeEnum ResourceTypeSetting
        {
            get
            {
                return ResourceTypeEnum.Organizational;
            }
        }

        public override int ResType => ResourceTypeConst.Package;

        #region GetByIdAsync 根据Id返回实体DTO数据信息
        /// <summary>
        /// 根据Id返回实体DTO数据信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override async Task<PackageDTO> GetByIdAsync(string id)
        {
            var data = await _GetByIdAsync(id);
            if (!string.IsNullOrWhiteSpace(data.Content))
            {
                data.ContentIns = !string.IsNullOrWhiteSpace(data.Content) ? JsonConvert.DeserializeObject<PackageContent>(data.Content) : new PackageContent();
                #region 匹配套餐中内容项
                if (data.ContentIns.Items != null && data.ContentIns.Items.Count > 0)
                {
                    for (int idx = data.ContentIns.Items.Count - 1; idx >= 0; idx--)
                    {
                        var cur = data.ContentIns.Items[idx];
                        if (string.IsNullOrWhiteSpace(cur.ProductSpecId))
                            continue;

                        var spec = await _DbContext.ProductSpec.Include(x => x.Product).FirstOrDefaultAsync(x => x.Id == cur.ProductSpecId);
                        if (spec != null)
                        {
                            cur.ProductSpecName = spec.Name;
                            cur.ProductName = spec.Product != null ? spec.Product.Name : "";
                            data.ContentIns.Items[idx] = cur;
                        }
                    }

                }
                #endregion

                #region 匹配套餐中区域内容项
                if (data.ContentIns.Areas != null && data.ContentIns.Areas.Count > 0)
                {

                    var areas = data.ContentIns.Areas;
                    for (int kdx = areas.Count - 1; kdx >= 0; kdx--)
                    {
                        var curArea = areas[kdx];
                        if (string.IsNullOrWhiteSpace(curArea.AreaAlias))
                        {
                            var referArea = await _DbContext.AreaTypes.FirstOrDefaultAsync(x => x.Id == curArea.AreaTypeId);
                            if (referArea != null)
                                curArea.AreaAlias = referArea.Name;
                        }


                        #region 匹配产品组
                        if (curArea.GroupsMap != null && curArea.GroupsMap.Count > 0)
                        {
                            var groups = new List<ProductGroupDTO>();
                            foreach (var item in curArea.GroupsMap)
                            {
                                var grp = await _DbContext.ProductGroups.FindAsync(item.Value);
                                if (grp != null)
                                {
                                    if (!string.IsNullOrWhiteSpace(grp.Icon))
                                    {
                                        var fs = await _DbContext.Files.FirstOrDefaultAsync(x => x.Id == grp.Icon);
                                        if (fs != null)
                                        {
                                            grp.IconFileAssetUrl = fs.Url;
                                        }
                                    }
                                    groups.Add(grp.ToDTO());
                                }

                            }
                            curArea.GroupsMapIns = groups;
                        }
                        #endregion

                        #region 匹配分类产品
                        if (curArea.ProductCategoryMap != null && curArea.ProductCategoryMap.Count > 0)
                        {
                            var products = new List<ProductDTO>();
                            foreach (var item in curArea.ProductCategoryMap)
                            {
                                var prd = await _DbContext.Products.FindAsync(item.Value);
                                if (prd != null)
                                {
                                    if (!string.IsNullOrWhiteSpace(prd.Icon))
                                    {
                                        var fs = await _DbContext.Files.FirstOrDefaultAsync(x => x.Id == prd.Icon);
                                        if (fs != null)
                                        {
                                            prd.IconFileAssetUrl = fs.Url;
                                        }
                                    }
                                    products.Add(prd.ToDTO());
                                }
                            }
                            curArea.ProductCategoryMapIns = products;
                        }
                        #endregion

                        #region 匹配材质
                        if (curArea.Materials != null && curArea.Materials.Count > 0)
                        {
                            var materials = new List<PackageMaterial>();
                            foreach (var item in curArea.Materials)
                            {
                                var mtl = await _DbContext.Materials.FindAsync(item.Value);
                                if (mtl != null)
                                {
                                    var model = new PackageMaterial();
                                    if (!string.IsNullOrWhiteSpace(mtl.Icon))
                                    {
                                        var fs = await _DbContext.Files.FindAsync(mtl.Icon);
                                        model.Icon = fs != null ? fs.Url : "";
                                    }

                                    model.MaterialId = mtl.Id;
                                    model.LastActorName = item.Key;
                                    model.ActorName = item.Key;
                                    if (model.ActorName == "待定")
                                        materials.Insert(0, model);
                                    else
                                        materials.Add(model);
                                }
                            }
                            curArea.MaterialIns = materials;
                        }
                        #endregion
                    }
                    data.ContentIns.Areas = areas.OrderBy(x => x.AreaAlias).ToList();

                }
                #endregion

                #region 匹配套餐中的产品替换组
                if (data.ContentIns.ReplaceGroups != null && data.ContentIns.ReplaceGroups.Count > 0)
                {
                    var rpGroups = new List<ProductReplaceGroupDTO>();
                    foreach (var rpId in data.ContentIns.ReplaceGroups)
                    {
                        var rpDto = await _ProductReplaceGroupRep.GetByIdAsync(rpId);
                        if (rpDto != null)
                            rpGroups.Add(rpDto);
                    }
                    data.ContentIns.ReplaceGroupIns = rpGroups;
                }
                #endregion

            }

            if (!string.IsNullOrWhiteSpace(data.Icon))
            {
                var fs = await _DbContext.Files.FirstOrDefaultAsync(x => x.Id == data.Icon);
                if (fs != null)
                {
                    data.IconFileAssetUrl = fs.Url;
                }
            }

            return data.ToDTO();
        }
        #endregion

        #region _GetPermissionData 获取权限数据
        /// <summary>
        /// 获取权限数据
        /// </summary>
        /// <param name="accid"></param>
        /// <param name="dataOp"></param>
        /// <param name="withInActive"></param>
        /// <returns></returns>
        public async override Task<IQueryable<Package>> _GetPermissionData(string accid, DataOperateEnum dataOp, bool withInActive = true)
        {
            IQueryable<Package> query;

            var currentAcc = await _DbContext.Accounts.Select(x => new Account() { Id = x.Id, OrganizationId = x.OrganizationId, Type = x.Type }).FirstOrDefaultAsync(x => x.Id == accid);

            //数据状态
            if (withInActive)
                query = _DbContext.Set<Package>();
            else
                query = _DbContext.Set<Package>().Where(x => x.ActiveFlag == AppConst.I_DataState_Active);

            //超级管理员系列不走权限判断
            if (currentAcc.Type == AppConst.AccountType_SysAdmin || currentAcc.Type == AppConst.AccountType_SysService)
                return await Task.FromResult(query);


            if (dataOp == DataOperateEnum.Retrieve)
            {
                /*
                 * 读取操作
                 *      管理员
                 *              品牌管理员/用户:获取自己组织的创建的产品
                 *       合伙人/供应商
                 *              管理员/用户:获取自己组织有读取权限的
                 */


                if (currentAcc.Type == AppConst.AccountType_BrandAdmin || currentAcc.Type == AppConst.AccountType_BrandMember)
                {
                    return query.Where(x => x.OrganizationId == currentAcc.OrganizationId);
                }
                else
                {
                    var permissionIdQ = _DbContext.ResourcePermissions.Where(x => x.OrganizationId == currentAcc.OrganizationId && x.ResType == ResType && x.OpRetrieve == 1);

                    query = from it in query
                            join ps in permissionIdQ on it.Id equals ps.ResId
                            select it;
                    return query;
                }
            }
            else
            {
                if (currentAcc.Type == AppConst.AccountType_BrandAdmin)
                {
                    return query.Where(x => x.OrganizationId == currentAcc.OrganizationId);
                }
            }

            return query.Take(0);
        }

        #endregion

        #region PagedSelectExpression
        /// <summary>
        /// PagedSelectExpression
        /// </summary>
        /// <returns></returns>
        public override Expression<Func<Package, Package>> PagedSelectExpression()
        {
            return x => new Package()
            {
                Id = x.Id,
                Name = x.Name,
                Icon = x.Icon,
                Description = x.Description,
                CategoryId = x.CategoryId,
                OrganizationId = x.OrganizationId,
                Creator = x.Creator,
                Modifier = x.Modifier,
                CreatedTime = x.CreatedTime,
                ModifiedTime = x.ModifiedTime,
                ResourceType = x.ResourceType
            };
        }
        #endregion
    }
}
