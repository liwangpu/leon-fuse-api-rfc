﻿using ApiModel.Consts;
using ApiModel.Entities;
using ApiModel.Enums;
using ApiServer.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ApiServer.Repositories
{
    public class TextureRepository : ListableRepository<Texture, TextureDTO>
    {
        public TextureRepository(ApiDbContext context, ITreeRepository<PermissionTree> permissionTreeRep) : base(context, permissionTreeRep)
        {
        }

        public override ResourceTypeEnum ResourceTypeSetting
        {
            get
            {
                return ResourceTypeEnum.Organizational_SubShare;
            }
        }

        #region _GetPermissionData 获取权限数据
        /// <summary>
        /// 获取权限数据
        /// </summary>
        /// <param name="accid"></param>
        /// <param name="dataOp"></param>
        /// <param name="withInActive"></param>
        /// <returns></returns>
        public override async Task<IQueryable<Texture>> _GetPermissionData(string accid, DataOperateEnum dataOp, bool withInActive = false)
        {
            IQueryable<Texture> query;

            var currentAcc = await _DbContext.Accounts.Select(x => new Account() { Id = x.Id, OrganizationId = x.OrganizationId, Type = x.Type }).FirstOrDefaultAsync(x => x.Id == accid);

            //数据状态
            if (withInActive)
                query = _DbContext.Set<Texture>();
            else
                query = _DbContext.Set<Texture>().Where(x => x.ActiveFlag == AppConst.I_DataState_Active);

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
        public override async Task<TextureDTO> GetByIdAsync(string id)
        {
            var data = await _GetByIdAsync(id);

            //if (!string.IsNullOrWhiteSpace(data.Icon))
            //    data.IconFileAsset = await _DbContext.Files.FindAsync(data.Icon);
            //if (!string.IsNullOrWhiteSpace(data.FileAssetId))
            //    data.FileAsset = await _DbContext.Files.FindAsync(data.Icon);
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
    }
}
