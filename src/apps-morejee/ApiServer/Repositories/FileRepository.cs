using System.Linq;
using System.Threading.Tasks;
using ApiModel.Consts;
using ApiModel.Entities;
using ApiModel.Enums;
using ApiServer.Data;
using Microsoft.EntityFrameworkCore;

namespace ApiServer.Repositories
{
    public class FileRepository : ListableRepository<FileAsset, FileAssetDTO>
    {

        public FileRepository(ApiDbContext context, ITreeRepository<PermissionTree> permissionTreeRep)
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

        public async override Task<IQueryable<FileAsset>> _GetPermissionData(string accid, DataOperateEnum dataOp, bool withInActive = false)
        {
            IQueryable<FileAsset> query;

            //数据状态
            if (withInActive)
                query = _DbContext.Set<FileAsset>();
            else
                query = _DbContext.Set<FileAsset>().Where(x => x.ActiveFlag == AppConst.I_DataState_Active);

            var currentAcc = await _DbContext.Accounts.Select(x => new Account() { Id = x.Id, OrganizationId = x.OrganizationId, Type = x.Type }).FirstOrDefaultAsync(x => x.Id == accid);

            if (dataOp != DataOperateEnum.Retrieve)
            {
                if (currentAcc.Type == UserRoleConst.SysAdmin)
                {
                    return query;
                }
                else if (currentAcc.Type == UserRoleConst.BrandAdmin)
                {
                    query = query.Where(x => x.OrganizationId == currentAcc.OrganizationId);
                    return query;
                }
                else if (currentAcc.Type == UserRoleConst.PartnerAdmin)
                {
                    query = query.Where(x => x.OrganizationId == currentAcc.OrganizationId);
                    return query;
                }
                else if (currentAcc.Type == UserRoleConst.SupplierAdmin)
                {
                    query = query.Where(x => x.OrganizationId == currentAcc.OrganizationId);
                    return query;
                }
                else { }
            }
            else
            {
                return query;
            }

            return await Task.FromResult(query.Where(x => x.Creator == currentAcc.Id));
        }
    }
}
