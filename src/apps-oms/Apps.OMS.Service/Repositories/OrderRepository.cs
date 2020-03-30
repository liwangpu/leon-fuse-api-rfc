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
    public class OrderRepository : IRepository<Order>
    {
        protected readonly AppDbContext _Context;

        #region 构造函数
        public OrderRepository(AppDbContext context)
        {
            _Context = context;
        }
        #endregion

        public async Task<string> CanCreateAsync(Order data, string accountId)
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

        public async Task<string> CanUpdateAsync(Order data, string accountId)
        {
            return await Task.FromResult(string.Empty);
        }

        public async Task CreateAsync(Order data, string accountId)
        {
            data.Id = GuidGen.NewGUID();
            data.Creator = accountId;
            data.Modifier = accountId;
            data.CreatedTime = DateTime.Now;
            data.ModifiedTime = data.CreatedTime;
            data.ActiveFlag = AppConst.Active;
            if (data.OrderDetails != null && data.OrderDetails.Count > 0)
            {
                data.TotalNum = data.OrderDetails.Select(x => x.Num).Sum();
                data.TotalPrice = data.OrderDetails.Select(x => x.TotalPrice).Sum();
            }
            //生成订单编号
            var beginTime = new DateTime(data.CreatedTime.Year, data.CreatedTime.Month, data.CreatedTime.Day);
            var endTime = beginTime.AddDays(1);
            var orderCount = await _Context.Orders.Where(x => x.CreatedTime >= beginTime && x.CreatedTime < endTime).CountAsync();
            data.OrderNo = beginTime.ToString("yyyyMMdd") + (orderCount + 1).ToString().PadLeft(5, '0');
            //计算最优包装
            foreach (var item in data.OrderDetails)
                await ReCalculatePackage(item);
            _Context.Orders.Add(data);
            await _Context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id, string accountId)
        {
            var data = await _Context.Orders.FirstOrDefaultAsync(x => x.Id == id);
            if (data != null)
            {
                data.Modifier = accountId;
                data.ModifiedTime = data.CreatedTime;
                data.ActiveFlag = AppConst.InActive;
                _Context.Orders.Update(data);
                await _Context.SaveChangesAsync();
            }
        }

        public async Task<Order> GetByIdAsync(string id, string accountId)
        {
            var entity = await _Context.Orders.Include(x => x.OrderDetails).Include(x=>x.CustomizedProducts).Include(x => x.OrderFlowLogs).FirstOrDefaultAsync(x => x.Id == id);
            return entity;
        }

        public async Task<PagedData<Order>> SimplePagedQueryAsync(PagingRequestModel model, string accountId, Func<IQueryable<Order>, Task<IQueryable<Order>>> advanceQuery = null)
        {
            var query = _Context.Orders.AsQueryable();
            if (advanceQuery != null)
                query = await advanceQuery(query);
            //关键词过滤查询
            if (!string.IsNullOrWhiteSpace(model.Search))
                query = query.Where(d => d.Name.Contains(model.Search));
            var result = await query.Where(x => x.ActiveFlag == AppConst.Active).SimplePaging(model.Page, model.PageSize, model.OrderBy, "Name", model.Desc);
            return result;
        }

        public async Task UpdateAsync(Order data, string accountId)
        {
            data.Modifier = accountId;
            data.ModifiedTime = data.CreatedTime;
            data.ActiveFlag = AppConst.Active;
            if (data.OrderDetails != null && data.OrderDetails.Count > 0)
            {
                data.TotalNum = data.OrderDetails.Select(x => x.Num).Sum();
                data.TotalPrice = data.OrderDetails.Select(x => x.TotalPrice).Sum();
            }




            _Context.Orders.Update(data);
            await _Context.SaveChangesAsync();
        }

        /// <summary>
        /// 重新计算产品规格的包装(需要外部调用SaveChangesAsync)
        /// </summary>
        /// <param name="detail"></param>
        /// <returns></returns>
        public async Task ReCalculatePackage(OrderDetail detail)
        {
            var detailPcks = await _Context.OrderDetailPackages.Where(x => x.OrderDetail == detail).ToListAsync();
            //先把之前的产品包装清除
            foreach (var item in detailPcks)
                _Context.OrderDetailPackages.Remove(item);
            //规格包装信息
            var pcks = await _Context.ProductPackages.Where(x => x.ProductSpecId == detail.ProductSpecId && x.ActiveFlag == AppConst.Active).OrderByDescending(x => x.Num).ToListAsync();

            var packageCount = pcks.Count;
            var packageIndex = 0;
            var sum = detail.Num;

            while (sum > 0 && packageCount > 0)
            {
                var pck = pcks[packageIndex];
                var detailPck = new OrderDetailPackage();
                detailPck.ProductPackageId = pck.Id;
                detailPck.Id = GuidGen.NewGUID();
                detailPck.OrderDetail = detail;
                //如果是最后一个包装,不管够不够整除包装,也要装起来,小数也不要紧
                if (packageIndex == packageCount - 1)
                {
                    detailPck.Num = Math.Round((sum + 0.0m) / pck.Num, 2);
                    sum = 0;
                    _Context.OrderDetailPackages.Add(detailPck);
                }
                else
                {
                    if (sum >= pck.Num)
                    {
                        var remainder = sum % pck.Num;
                        detailPck.Num = (sum - remainder) / pck.Num;
                        sum = remainder;
                        _Context.OrderDetailPackages.Add(detailPck);
                    }
                    packageIndex++;
                }
            }
        }

    }
}
