using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiModel.Consts;
using ApiModel.Entities;
using ApiModel.Enums;
using ApiServer.Data;
using Microsoft.EntityFrameworkCore;

namespace ApiServer.Repositories
{
    public class AreaTypeRepository : ListableRepository<AreaType, AreaTypeDTO>
    {
        public AreaTypeRepository(ApiDbContext context, ITreeRepository<PermissionTree> permissionTreeRep)
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

        #region _GetPermissionData 获取权限数据
        /// <summary>
        /// 获取权限数据
        /// </summary>
        /// <param name="accid"></param>
        /// <param name="dataOp"></param>
        /// <param name="withInActive"></param>
        /// <returns></returns>
        public async override Task<IQueryable<AreaType>> _GetPermissionData(string accid, DataOperateEnum dataOp, bool withInActive = true)
        {
            IQueryable<AreaType> query;

            var currentAcc = await _DbContext.Accounts.Select(x => new Account() { Id = x.Id, OrganizationId = x.OrganizationId, Type = x.Type }).FirstOrDefaultAsync(x => x.Id == accid);

            //数据状态
            if (withInActive)
                query = _DbContext.Set<AreaType>();
            else
                query = _DbContext.Set<AreaType>().Where(x => x.ActiveFlag == AppConst.I_DataState_Active);

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
                var organNode = await _DbContext.PermissionTrees.FirstOrDefaultAsync(x => x.ObjId == currentAcc.OrganizationId);
                var organNodeQ =await _PermissionTreeRepository.GetAncestorNode(organNode, new List<string>() { AppConst.S_NodeType_Organization }, true);
                var organIds = organNodeQ.Select(x => x.ObjId).ToList();


                return query.Where(x => organIds.Contains(x.OrganizationId));
            }
            else
            {
                if (currentAcc.Type == AppConst.AccountType_BrandAdmin || currentAcc.Type == AppConst.AccountType_BrandMember)
                {
                    return query.Where(x => x.OrganizationId == currentAcc.OrganizationId);
                }
            }

            return query.Take(0);
        }

        #endregion
    }
}
