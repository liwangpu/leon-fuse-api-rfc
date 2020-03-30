using Apps.Base.Common;
using Apps.Base.Common.Consts;
using Apps.Base.Common.Interfaces;
using Apps.Base.Common.Models;
using Apps.OMS.Data.Entities;
using Apps.OMS.Service.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Apps.OMS.Service.Repositories
{
    public class MemberRepository : IRepository<Member>
    {
        protected readonly AppDbContext _Context;

        #region 构造函数
        public MemberRepository(AppDbContext context)
        {
            _Context = context;
        }
        #endregion

        public async Task<string> CanCreateAsync(Member data, string accountId)
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

        public async Task<string> CanUpdateAsync(Member data, string accountId)
        {
            return await Task.FromResult(string.Empty);
        }

        public async Task CreateAsync(Member data, string accountId)
        {
            data.Id = GuidGen.NewGUID();
            data.Creator = accountId;
            data.Modifier = accountId;
            data.CreatedTime = DateTime.Now;
            data.ModifiedTime = data.CreatedTime;
            _Context.Members.Add(data);
            await _Context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id, string accountId)
        {
            throw new NotImplementedException();
        }

        public async Task<Member> GetByIdAsync(string id, string accountId)
        {
            var entity = await _Context.Members.FirstOrDefaultAsync(x => x.Id == id);
            return entity;
        }

        public async Task<PagedData<Member>> SimplePagedQueryAsync(PagingRequestModel model, string accountId, Func<IQueryable<Member>, Task<IQueryable<Member>>> advanceQuery = null)
        {
            var query = _Context.Members.AsQueryable();
            if (advanceQuery != null)
                query = await advanceQuery(query);
            //关键词过滤查询
            if (!string.IsNullOrWhiteSpace(model.Search))
                query = query.Where(d => d.Name.Contains(model.Search));
            var result = await query.SimplePaging(model.Page, model.PageSize, model.OrderBy, "Name", model.Desc);
            return result;
        }

        public async Task UpdateAsync(Member data, string accountId)
        {
            data.Modifier = accountId;
            data.ModifiedTime = data.CreatedTime;
            _Context.Members.Update(data);
            await _Context.SaveChangesAsync();
        }
    }
}
