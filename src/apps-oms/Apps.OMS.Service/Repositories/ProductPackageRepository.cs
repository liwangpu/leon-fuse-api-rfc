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
    public class ProductPackageRepository : IRepository<ProductPackage>
    {
        protected readonly AppDbContext _Context;

        #region 构造函数
        public ProductPackageRepository(AppDbContext context)
        {
            _Context = context;
        }
        #endregion

        public async Task<string> CanCreateAsync(ProductPackage data, string accountId)
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

        public async Task<string> CanUpdateAsync(ProductPackage data, string accountId)
        {
            return await Task.FromResult(string.Empty);
        }

        public async Task CreateAsync(ProductPackage data, string accountId)
        {
            data.Id = GuidGen.NewGUID();
            data.Creator = accountId;
            data.Modifier = accountId;
            data.CreatedTime = DateTime.Now;
            data.ModifiedTime = data.CreatedTime;
            data.ActiveFlag = AppConst.Active;
            _Context.ProductPackages.Add(data);
            await _Context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id, string accountId)
        {
            var entity = await _Context.ProductPackages.FirstOrDefaultAsync(x => x.Id == id);
            entity.ActiveFlag = AppConst.InActive;
            _Context.ProductPackages.Update(entity);
            await _Context.SaveChangesAsync();
        }

        public async Task<ProductPackage> GetByIdAsync(string id, string accountId)
        {
            var entity = await _Context.ProductPackages.FirstOrDefaultAsync(x => x.Id == id);
            return entity;
        }

        public async Task<PagedData<ProductPackage>> SimplePagedQueryAsync(PagingRequestModel model, string accountId, Func<IQueryable<ProductPackage>, Task<IQueryable<ProductPackage>>> advanceQuery = null)
        {
            var query = _Context.ProductPackages.AsQueryable();
            if (advanceQuery != null)
                query = await advanceQuery(query);
            //关键词过滤查询
            if (!string.IsNullOrWhiteSpace(model.Search))
                query = query.Where(d => d.Name.Contains(model.Search));
            var result = await query.SimplePaging(model.Page, model.PageSize, model.OrderBy, "Name", model.Desc);
            return result;
        }

        public async Task UpdateAsync(ProductPackage data, string accountId)
        {
            data.Modifier = accountId;
            data.ModifiedTime = data.CreatedTime;
            data.ActiveFlag = AppConst.Active;
            _Context.ProductPackages.Update(data);
            await _Context.SaveChangesAsync();
        }
    }
}
