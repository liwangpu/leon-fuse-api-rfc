using ApiModel.Consts;
using ApiModel.Entities;
using ApiModel.Enums;
using ApiServer.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiServer.Repositories
{
    public class LayoutRepository : ResourceRepositoryBase<Layout, LayoutDTO>
    {
        public LayoutRepository(ApiDbContext context, ITreeRepository<PermissionTree> permissionTreeRep)
            : base(context, permissionTreeRep)
        {
        }

        /// <summary>
        /// 资源访问类型
        /// </summary>
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
        public async override Task<IQueryable<Layout>> _GetPermissionData(string accid, DataOperateEnum dataOp, bool withInActive = true)
        {
            IQueryable<Layout> query;


            var currentAcc = await _DbContext.Accounts.Select(x => new Account() { Id = x.Id, OrganizationId = x.OrganizationId, Type = x.Type }).FirstOrDefaultAsync(x => x.Id == accid);

            //数据状态
            if (withInActive)
                query = _DbContext.Set<Layout>();
            else
                query = _DbContext.Set<Layout>().Where(x => x.ActiveFlag == AppConst.I_DataState_Active);

            //超级管理员系列不走权限判断
            if (currentAcc.Type == AppConst.AccountType_SysAdmin || currentAcc.Type == AppConst.AccountType_SysService)
                return await Task.FromResult(query);



            if (dataOp == DataOperateEnum.Retrieve)
            {
                /*
                 * 读取操作
                 *      管理员:获取包括自己组织以及下级组织的有读取权限的数据
                 *      普通用户:获取自己组织的
                 */


                if (currentAcc.Type == AppConst.AccountType_BrandAdmin || currentAcc.Type == AppConst.AccountType_BrandMember)
                {
                    var organNode = await _DbContext.PermissionTrees.FirstOrDefaultAsync(x => x.ObjId == currentAcc.OrganizationId);
                    var organNodeQ = _PermissionTreeRepository.GetDescendantNode(organNode, new List<string>() { AppConst.S_NodeType_Organization }, true);
                    var organIds = organNodeQ.Select(x => x.ObjId).ToList();

                    query = from it in query
                            where organIds.Contains(it.OrganizationId)
                            select it;
                    return query;
                }

                if (currentAcc.Type == AppConst.AccountType_PartnerAdmin || currentAcc.Type == AppConst.AccountType_PartnerMember)
                {
                    var organNode = await _DbContext.PermissionTrees.FirstOrDefaultAsync(x => x.ObjId == currentAcc.OrganizationId);
                    var organNodeQ = _PermissionTreeRepository.GetDescendantNode(organNode, new List<string>() { AppConst.S_NodeType_Organization }, true);
                    var organIds = organNodeQ.Select(x => x.ObjId).ToList();

                    var permissionIdQ = _DbContext.ResourcePermissions.Where(x => organIds.Contains(x.OrganizationId) && x.ResType == ResType && x.OpRetrieve == 1);

                    query = from it in query
                            join ps in permissionIdQ on it.Id equals ps.ResId
                            select it;
                    return query;
                }

            }
            else
            {
                /*
                 * 改,删操作
                 *      管理员:只获取自己组织创建的
                 *      普通用户:获取自己创建的
                 */

                if (currentAcc.Type == AppConst.AccountType_BrandAdmin || currentAcc.Type == AppConst.AccountType_PartnerAdmin || currentAcc.Type == AppConst.AccountType_SupplierAdmin)
                {
                    var permissionIdQ = _DbContext.ResourcePermissions.Where(x => x.OrganizationId == currentAcc.OrganizationId && x.ResType == ResType && x.OpRetrieve == 1);

                    query = from it in query
                            join ps in permissionIdQ on it.Id equals ps.ResId
                            select it;
                    return query;
                }
            }

            //普通用户
            return await Task.FromResult(query.Where(x => x.Creator == currentAcc.Id));
        }
        #endregion
    }
}
