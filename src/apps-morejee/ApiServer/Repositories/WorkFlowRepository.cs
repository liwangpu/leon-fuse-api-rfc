using ApiModel.Consts;
using ApiModel.Entities;
using ApiModel.Enums;
using ApiServer.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ApiServer.Repositories
{
    public class WorkFlowRepository : RepositoryBase<WorkFlow, WorkFlowDTO>, IRepository<WorkFlow, WorkFlowDTO>
    {
        public WorkFlowRepository(ApiDbContext context, ITreeRepository<PermissionTree> permissionTreeRep)
            : base(context, permissionTreeRep)
        {
        }

        public override async Task<WorkFlow> _GetByIdAsync(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                var data = await _DbContext.WorkFlows.Include(x => x.WorkFlowItems).Where(x => x.Id == id).FirstOrDefaultAsync();
                if (data != null)
                    return data;
            }
            return new WorkFlow();
        }

        public override async Task<WorkFlowDTO> GetByIdAsync(string id)
        {
            var data = await _DbContext.WorkFlows.Include(x => x.WorkFlowItems).Where(x => x.Id == id).FirstOrDefaultAsync();
            if (data.WorkFlowItems != null && data.WorkFlowItems.Count > 0)
            {
                data.WorkFlowItems = data.WorkFlowItems.OrderBy(x => x.FlowGrade).ToList();
                for (int idx = 0, len = data.WorkFlowItems.Count; idx < len; idx++)
                {
                    var item = data.WorkFlowItems[idx];
                    item.WorkFlow = null;
                    if (!string.IsNullOrWhiteSpace(item.SubWorkFlowId))
                    {
                        var refSubWorkFlow = await _DbContext.WorkFlows.FirstOrDefaultAsync(x => x.Id == item.SubWorkFlowId);
                        if (refSubWorkFlow != null)
                            item.SubWorkFlowName = refSubWorkFlow.Name;
                    }
                }
            }
            return data.ToDTO();
        }

        public override async Task<IQueryable<WorkFlow>> _GetPermissionData(string accid, DataOperateEnum dataOp, bool withInActive = false)
        {
            var query = Enumerable.Empty<WorkFlow>().AsQueryable();

            var currentAcc = await _DbContext.Accounts.Select(x => new Account() { Id = x.Id, OrganizationId = x.OrganizationId, Type = x.Type }).FirstOrDefaultAsync(x => x.Id == accid);
            if (currentAcc == null)
                return query;

            //数据状态
            if (withInActive)
                query = _DbContext.Set<WorkFlow>();
            else
                query = _DbContext.Set<WorkFlow>().Where(x => x.ActiveFlag == AppConst.I_DataState_Active);



            if (dataOp != DataOperateEnum.Retrieve)
            {

                //超级管理员
                if (currentAcc.Type == AppConst.AccountType_SysAdmin || currentAcc.Type == AppConst.AccountType_SysService)
                {
                    return query;
                }

                //品牌商管理员管理自己建立的角色
                if (currentAcc.Type == AppConst.AccountType_BrandAdmin)
                {
                    query = query.Where(x => x.OrganizationId == currentAcc.OrganizationId);
                    return query;
                }


                return query.Where(x => x.Creator == currentAcc.Id);
            }
            query = query.Where(x => x.OrganizationId == currentAcc.OrganizationId);
            return await Task.FromResult(query);
        }
    }
}
