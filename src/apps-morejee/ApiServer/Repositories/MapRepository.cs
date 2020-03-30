using System;
using System.Linq;
using System.Threading.Tasks;
using ApiModel.Consts;
using ApiModel.Entities;
using ApiModel.Enums;
using ApiServer.Data;
using ApiServer.Models;
using BambooCore;
using Microsoft.EntityFrameworkCore;

namespace ApiServer.Repositories
{
    public class MapRepository : ListableRepository<Map, MapDTO>
    {

        /// <summary>
        /// 资源访问类型
        /// </summary>
        public override ResourceTypeEnum ResourceTypeSetting
        {
            get
            {
                return ResourceTypeEnum.Organizational_SubShare;
            }
        }

        public MapRepository(ApiDbContext context, ITreeRepository<PermissionTree> permissionTreeRep)
            : base(context, permissionTreeRep)
        {

        }

        #region _GetPermissionData 获取权限数据
        /// <summary>
        /// 获取权限数据
        /// </summary>
        /// <param name="accid"></param>
        /// <param name="dataOp"></param>
        /// <param name="withInActive"></param>
        /// <returns></returns>
        public override async Task<IQueryable<Map>> _GetPermissionData(string accid, DataOperateEnum dataOp, bool withInActive = false)
        {
            IQueryable<Map> query;

            var currentAcc = await _DbContext.Accounts.Select(x => new Account() { Id = x.Id, OrganizationId = x.OrganizationId, Type = x.Type }).FirstOrDefaultAsync(x => x.Id == accid);

            //数据状态
            if (withInActive)
                query = _DbContext.Set<Map>();
            else
                query = _DbContext.Set<Map>().Where(x => x.ActiveFlag == AppConst.I_DataState_Active);

            //超级管理员系列不走权限判断
            if (currentAcc.Type == AppConst.AccountType_SysAdmin || currentAcc.Type == AppConst.AccountType_SysService)
                return await Task.FromResult(query);


            if (dataOp == DataOperateEnum.Retrieve)
            {
                return query;
            }
            else
            {
                if (currentAcc.Type == AppConst.AccountType_BrandAdmin || currentAcc.Type == AppConst.AccountType_BrandMember)
                    return query;

            }

            return query.Take(0);
        }
        #endregion

        #region override GetByIdAsync 根据Id返回实体DTO数据信息
        /// <summary>
        /// 根据Id返回实体DTO数据信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override async Task<MapDTO> GetByIdAsync(string id)
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
            if (!string.IsNullOrWhiteSpace(data.FileAssetId))
            {
                var fs = await _DbContext.Files.FirstOrDefaultAsync(x => x.Id == data.FileAssetId);
                if (fs != null)
                {
                    data.FileAssetUrl = fs.Url;
                    data.FileAsset = fs;
                }
            }

            return data.ToDTO();
        }
        #endregion

        public override async Task<PagedData<Map>> SimplePagedQueryAsync(PagingRequestModel model, string accid, Func<IQueryable<Map>, Task<IQueryable<Map>>> advanceQuery = null)
        {
            var result = await base.SimplePagedQueryAsync(model, accid, advanceQuery);
            if (result.Total > 0)
            {
                for (int idx = result.Data.Count - 1; idx >= 0; idx--)
                {
                    var curData = result.Data[idx];
                    curData.Dependencies = null;
                    curData.Properties = null;
                    if (!string.IsNullOrWhiteSpace(curData.Icon))
                    {
                        var fs = await _DbContext.Files.FirstOrDefaultAsync(x => x.Id == curData.Icon);
                        if (fs != null)
                        {
                            curData.IconFileAssetUrl = fs.Url;
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(curData.FileAssetId))
                    {
                        var fs = await _DbContext.Files.FirstOrDefaultAsync(x => x.Id == curData.FileAssetId);
                        if (fs != null)
                        {
                            curData.FileAssetUrl = fs.Url;
                        }
                    }

                }
            }
            return result;
        }
    }
}
