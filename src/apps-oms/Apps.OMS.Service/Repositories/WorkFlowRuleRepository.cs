using Apps.Base.Common;
using Apps.Base.Common.Interfaces;
using Apps.Base.Common.Models;
using Apps.OMS.Data.Entities;
using Apps.OMS.Service.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;


namespace Apps.OMS.Service.Repositories
{
    public class WorkFlowRuleRepository : IRepository<WorkFlowRule>
    {
        protected readonly AppDbContext _Context;

        #region 构造函数
        public WorkFlowRuleRepository(AppDbContext context)
        {
            _Context = context;
        }
        #endregion

        public async Task<string> CanCreateAsync(WorkFlowRule data, string accountId)
        {
           return await Task.FromResult(string.Empty);
        }

        public async Task<string> CanDeleteAsync(string id, string accountId)
        {
           return await Task.FromResult(string.Empty);
        }

        public async Task<string> CanGetByIdAsync(string id, string accountId)
        {
           return await Task.FromResult(string.Empty);
        }

        public async Task<string> CanUpdateAsync(WorkFlowRule data, string accountId)
        {
           return await Task.FromResult(string.Empty);
        }

        public async Task CreateAsync(WorkFlowRule data, string accountId)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteAsync(string id, string accountId)
        {
            throw new NotImplementedException();
        }

        public async Task<WorkFlowRule> GetByIdAsync(string id, string accountId)
        {
            var entity = await _Context.WorkFlowRules.FirstOrDefaultAsync(x => x.Id == id);
            return entity;
        }

        public async Task<PagedData<WorkFlowRule>> SimplePagedQueryAsync(PagingRequestModel model, string accountId, Func<IQueryable<WorkFlowRule>, Task<IQueryable<WorkFlowRule>>> advanceQuery = null)
        {
            var query = _Context.WorkFlowRules.AsQueryable();
            if (advanceQuery != null)
                query = await advanceQuery(query);
            //关键词过滤查询
            if (!string.IsNullOrWhiteSpace(model.Search))
                query = query.Where(d => d.Name.Contains(model.Search));
            var result = await query.SimplePaging(model.Page, model.PageSize, model.OrderBy, "Name", model.Desc);
            return result;
        }

        public async Task UpdateAsync(WorkFlowRule data, string accountId)
        {
            throw new NotImplementedException();
        }
    }
}
