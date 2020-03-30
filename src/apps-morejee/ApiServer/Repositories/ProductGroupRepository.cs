using ApiModel.Consts;
using ApiModel.Entities;
using ApiModel.Enums;
using ApiServer.Data;
using ApiServer.Models;
using BambooCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace ApiServer.Repositories
{
    public class ProductGroupRepository : ResourceRepositoryBase<ProductGroup, ProductGroupDTO>
    {
        public ProductGroupRepository(ApiDbContext context, ITreeRepository<PermissionTree> permissionTreeRep)
            : base(context, permissionTreeRep)
        {
        }

        public override int ResType => ResourceTypeConst.ProductGroup;

        public override ResourceTypeEnum ResourceTypeSetting
        {
            get
            {
                return ResourceTypeEnum.Organizational;
            }
        }

        #region GetByIdAsync 根据Id返回实体DTO数据信息
        /// <summary>
        /// 根据Id返回实体DTO数据信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override async Task<ProductGroupDTO> GetByIdAsync(string id)
        {
            var data = await _GetByIdAsync(id);
            if (!string.IsNullOrWhiteSpace(data.Icon))
            {
                var fs = await _DbContext.Files.FirstOrDefaultAsync(x => x.Id == data.Icon);
                if (fs != null)
                {
                    data.IconFileAssetUrl = fs.Url;
                }
            }
            if (!string.IsNullOrWhiteSpace(data.CategoryId))
                data.AssetCategory = await _DbContext.AssetCategories.FindAsync(data.CategoryId);
            return data.ToDTO();
        }
        #endregion

        #region override SimplePagedQueryAsync
        /// <summary>
        /// SimplePagedQueryAsync
        /// </summary>
        /// <param name="model"></param>
        /// <param name="accid"></param>
        /// <param name="advanceQuery"></param>
        /// <returns></returns>
        public override async Task<PagedData<ProductGroup>> SimplePagedQueryAsync(PagingRequestModel model, string accid, Func<IQueryable<ProductGroup>, Task<IQueryable<ProductGroup>>> advanceQuery = null)
        {
            var result = await base.SimplePagedQueryAsync(model, accid, advanceQuery);

            if (result.Total > 0)
            {
                for (int idx = result.Data.Count - 1; idx >= 0; idx--)
                {
                    var curData = result.Data[idx];

                    if (!string.IsNullOrWhiteSpace(curData.Icon))
                    {
                        var fs = await _DbContext.Files.FirstOrDefaultAsync(x => x.Id == curData.Icon);
                        if (fs != null)
                        {
                            curData.IconFileAssetUrl = fs.Url;
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(curData.CategoryId))
                        curData.AssetCategory = await _DbContext.AssetCategories.FindAsync(curData.CategoryId);
                }
            }
            return result;
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
        public async override Task<IQueryable<ProductGroup>> _GetPermissionData(string accid, DataOperateEnum dataOp, bool withInActive = true)
        {
            IQueryable<ProductGroup> query;

            var currentAcc = await _DbContext.Accounts.Select(x => new Account() { Id = x.Id, OrganizationId = x.OrganizationId, Type = x.Type }).FirstOrDefaultAsync(x => x.Id == accid);

            //数据状态
            if (withInActive)
                query = _DbContext.Set<ProductGroup>();
            else
                query = _DbContext.Set<ProductGroup>().Where(x => x.ActiveFlag == AppConst.I_DataState_Active);

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
                if (currentAcc.Type == AppConst.AccountType_BrandAdmin || currentAcc.Type == AppConst.AccountType_BrandMember)
                {
                    return query.Where(x => x.OrganizationId == currentAcc.OrganizationId);
                }
            }

            //普通用户
            return await Task.FromResult(query.Take(0));
        }

        #endregion
    }
}
