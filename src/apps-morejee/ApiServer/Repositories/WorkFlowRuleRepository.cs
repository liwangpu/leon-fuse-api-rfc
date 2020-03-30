using ApiModel.Consts;
using ApiModel.Entities;
using ApiModel.Enums;
using ApiServer.Data;
using ApiServer.Models;
using BambooCore;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiServer.Repositories
{
    public class WorkFlowRuleRepository : RepositoryBase<WorkFlowRule, WorkFlowRuleDTO>, IRepository<WorkFlowRule, WorkFlowRuleDTO>
    {
        public WorkFlowRuleRepository(ApiDbContext context, ITreeRepository<PermissionTree> permissionTreeRep)
            : base(context, permissionTreeRep)
        {
        }

        public override async Task<IQueryable<WorkFlowRule>> _GetPermissionData(string accid, DataOperateEnum dataOp, bool withInActive = false)
        {
            var query = Enumerable.Empty<WorkFlowRule>().AsQueryable();

            var currentAcc = await _DbContext.Accounts.Select(x => new Account() { Id = x.Id, OrganizationId = x.OrganizationId, Type = x.Type }).FirstOrDefaultAsync(x => x.Id == accid);
            if (currentAcc == null)
                return query;

            //数据状态
            if (withInActive)
                query = _DbContext.Set<WorkFlowRule>();
            else
                query = _DbContext.Set<WorkFlowRule>().Where(x => x.ActiveFlag == AppConst.I_DataState_Active);



            if (dataOp != DataOperateEnum.Retrieve)
            {


                return query.Where(x => x.Creator == currentAcc.Id);
            }
            return await Task.FromResult(query);
        }
        //public override async Task<WorkFlowRuleDTO> GetByIdAsync(string id)
        //{
        //    var data = await _DbContext.WorkFlowRules.Where(x => x.Id == id).FirstOrDefaultAsync();
        //    if (!string.IsNullOrWhiteSpace(data.Keyword))
        //    {
        //        var detail=await _DbContext.WorkFlowRuleDetails.Where(x=>x.KeyWord)
        //    }
        //    return data.ToDTO();
        //}
    }
}
