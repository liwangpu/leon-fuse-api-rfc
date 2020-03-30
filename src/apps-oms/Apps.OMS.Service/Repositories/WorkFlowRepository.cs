using Apps.Base.Common;
using Apps.Base.Common.Consts;
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
    public class WorkFlowRepository : IRepository<WorkFlow>
    {

        protected readonly AppDbContext _Context;

        #region 构造函数
        public WorkFlowRepository(AppDbContext context)
        {
            _Context = context;
        }
        #endregion

        public async Task<string> CanCreateAsync(WorkFlow data, string accountId)
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

        public async Task<string> CanUpdateAsync(WorkFlow data, string accountId)
        {
            return await Task.FromResult(string.Empty);
        }

        public async Task CreateAsync(WorkFlow data, string accountId)
        {
            data.Id = GuidGen.NewGUID();
            data.Creator = accountId;
            data.Modifier = accountId;
            data.CreatedTime = DateTime.Now;
            data.ModifiedTime = data.CreatedTime;
            data.ActiveFlag = AppConst.Active;
            _Context.WorkFlows.Add(data);
            await _Context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id, string accountId)
        {
            var data = await _Context.WorkFlows.FirstOrDefaultAsync(x => x.Id == id);
            if (data != null)
            {
                data.Modifier = accountId;
                data.ModifiedTime = data.CreatedTime;
                data.ActiveFlag = AppConst.InActive;
                _Context.WorkFlows.Update(data);
                await _Context.SaveChangesAsync();
            }
        }

        public async Task<WorkFlow> GetByIdAsync(string id, string accountId)
        {
            var entity = await _Context.WorkFlows.Include(x => x.WorkFlowItems).FirstOrDefaultAsync(x => x.Id == id);
            return entity;
        }

        public async Task<PagedData<WorkFlow>> SimplePagedQueryAsync(PagingRequestModel model, string accountId, Func<IQueryable<WorkFlow>, Task<IQueryable<WorkFlow>>> advanceQuery = null)
        {
            var query = _Context.WorkFlows.AsQueryable();
            if (advanceQuery != null)
                query = await advanceQuery(query);
            //关键词过滤查询
            if (!string.IsNullOrWhiteSpace(model.Search))
                query = query.Where(d => d.Name.Contains(model.Search));
            var result = await query.Where(x => x.ActiveFlag == AppConst.Active).SimplePaging(model.Page, model.PageSize, model.OrderBy, "Name", model.Desc);
            return result;
        }

        public async Task UpdateAsync(WorkFlow data, string accountId)
        {
            data.Modifier = accountId;
            data.ModifiedTime = data.CreatedTime;
            data.ActiveFlag = AppConst.Active;
            _Context.WorkFlows.Update(data);
            await _Context.SaveChangesAsync();
        }
    }
}
