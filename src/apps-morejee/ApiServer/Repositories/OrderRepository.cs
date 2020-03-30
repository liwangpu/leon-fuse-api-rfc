using ApiModel.Consts;
using ApiModel.Entities;
using ApiModel.Enums;
using ApiServer.Data;
using ApiServer.Models;
using ApiServer.Services;
using BambooCore;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiServer.Repositories
{
    public class OrderRepository : ListableRepository<Order, OrderDTO>
    {
        public AppConfig appConfig { get; }

        #region 构造函数
        public OrderRepository(ApiDbContext context, ITreeRepository<PermissionTree> permissionTreeRep, IOptions<AppConfig> settingsOptions)
    : base(context, permissionTreeRep)
        {
            appConfig = settingsOptions.Value;
        }
        #endregion

        public override ResourceTypeEnum ResourceTypeSetting
        {
            get
            {
                return ResourceTypeEnum.Organizational;
            }
        }

        public override async Task<PagedData<Order>> SimplePagedQueryAsync(PagingRequestModel model, string accid, Func<IQueryable<Order>, Task<IQueryable<Order>>> advanceQuery = null)
        {
            var result = await base.SimplePagedQueryAsync(model, accid, advanceQuery);
            var rootOrgan = await GetUserRootOrgan(accid);
            var ruleDetail = await _DbContext.WorkFlowRuleDetails.Where(x => x.OrganizationId == rootOrgan.Id).FirstOrDefaultAsync();
            if (ruleDetail != null)
            {
                var workFlowItems = await _DbContext.WorkFlowItems.Where(x => x.WorkFlow.Id == ruleDetail.WorkFlowId).ToListAsync();
                if (result.Data != null && result.Data.Count > 0)
                {
                    for (var idx = result.Data.Count - 1; idx >= 0; idx--)
                    {
                        var item = result.Data[idx];
                        if (!string.IsNullOrWhiteSpace(item.WorkFlowItemId))
                        {
                            var refFlowItem = workFlowItems.Where(x => x.Id == item.WorkFlowItemId).FirstOrDefault();
                            if (refFlowItem != null)
                                item.WorkFlowItemName = refFlowItem.Name;
                        }
                        else
                        {
                            var defaultFlowItem = workFlowItems.Where(x => x.FlowGrade <= 0).FirstOrDefault();
                            if (defaultFlowItem != null)
                            {
                                item.WorkFlowItemId = defaultFlowItem.Id;
                                item.WorkFlowItemName = defaultFlowItem.Name;
                            }
                        }
                    }
                }
            }

            return result;
        }

        #region GetByIdAsync 根据Id返回实体DTO数据信息
        /// <summary>
        /// 根据Id返回实体DTO数据信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override async Task<OrderDTO> GetByIdAsync(string id)
        {
            var data = await _DbContext.Orders.Include(x => x.OrderDetails).Include(x => x.OrderFlowLogs).Where(x => x.Id == id).FirstOrDefaultAsync();
            if (data.OrderDetails != null && data.OrderDetails.Count > 0)
            {
                for (int idx = data.OrderDetails.Count - 1; idx >= 0; idx--)
                {
                    var item = data.OrderDetails[idx];
                    data.TotalNum += item.Num;
                    data.TotalPrice += item.TotalPrice;
                    item.ProductSpec = await _DbContext.ProductSpec.Where(x => x.Id == item.ProductSpecId).Select(x => new ProductSpec() { Name = x.Name, ProductId = x.ProductId, Icon = x.Icon }).FirstOrDefaultAsync();
                    if (item.ProductSpec != null)
                    {
                        item.ProductSpec.Product = await _DbContext.Products.Where(x => x.Id == item.ProductSpec.ProductId).Select(x => new Product() { Id = x.Id, Name = x.Name, Description = x.Description, Unit = x.Unit, CategoryId = x.CategoryId }).FirstOrDefaultAsync();
                        if (item.ProductSpec.Product != null && !string.IsNullOrWhiteSpace(item.ProductSpec.Product.CategoryId))
                        {
                            item.ProductSpec.Product.CategoryName = await _DbContext.AssetCategories.Where(x => x.Id == item.ProductSpec.Product.CategoryId).Select(x => x.Name).FirstOrDefaultAsync();
                        }

                        if (!string.IsNullOrWhiteSpace(item.ProductSpec.Icon))
                        {
                            //item.ProductSpec.IconFileAsset = await _DbContext.Files.FindAsync(item.ProductSpec.Icon);
                            ////以第一个产品作为订单icon
                            //if (idx == 0)
                            //    data.IconFileAsset = item.ProductSpec.IconFileAsset;
                            if (!string.IsNullOrWhiteSpace(item.ProductSpec.Icon))
                            {
                                var fs = await _DbContext.Files.FirstOrDefaultAsync(x => x.Id == item.ProductSpec.Icon);
                                if (fs != null)
                                {
                                    item.ProductSpec.IconFileAssetUrl = fs.Url;
                                    item.ProductSpec.Icon = fs.Url;
                                }
                                if (idx == 0)
                                    data.IconFileAssetUrl = item.ProductSpec.IconFileAssetUrl;
                            }

                        }
                    }

                    if (!string.IsNullOrWhiteSpace(item.AttachmentIds))
                    {
                        var idArr = item.AttachmentIds.Split(",", StringSplitOptions.RemoveEmptyEntries);
                        var attaches = new List<OrderDetailAttachment>();
                        foreach (var fid in idArr)
                        {
                            var refFile = await _DbContext.Files.Where(x => x.Id == fid).FirstOrDefaultAsync();
                            if (refFile != null)
                            {
                                var attch = new OrderDetailAttachment();
                                attch.Id = refFile.Id;
                                attch.Name = refFile.Name;
                                attch.Url = refFile.Url;
                                attaches.Add(attch);
                            }
                        }
                        item.Attachments = attaches;
                    }
                }
            }

            if (data.OrderFlowLogs != null && data.OrderFlowLogs.Count > 0)
            {
                for (int idx = data.OrderFlowLogs.Count - 1; idx >= 0; idx--)
                {
                    var item = data.OrderFlowLogs[idx];
                    var refFlow = await _DbContext.WorkFlowItems.Where(x => x.Id == item.WorkFlowItemId).Select(x => new WorkFlowItem() { Name = x.Name }).FirstOrDefaultAsync();
                    if (refFlow != null)
                    {
                        item.WorkFlowItemName = refFlow.Name;
                        var opUser = await _DbContext.Accounts.Where(x => x.Id == item.Creator).Select(x => new Account() { Name = x.Name }).FirstOrDefaultAsync();
                        if (opUser != null)
                            item.CreatorName = opUser.Name;
                    }
                    else
                    {
                        data.OrderFlowLogs.RemoveAt(idx);
                    }
                    item.Order = null;//清除子order信息
                }
            }

            if (string.IsNullOrWhiteSpace(data.WorkFlowItemId))
            {
                var refRuleDetail = await _DbContext.WorkFlowRuleDetails.Where(x => x.OrganizationId == data.OrganizationId).FirstOrDefaultAsync();
                if (refRuleDetail != null)
                {
                    var defaultFlowItem = await _DbContext.WorkFlowItems.Where(x => x.WorkFlow.Id == refRuleDetail.WorkFlowId && x.FlowGrade <= 0).FirstOrDefaultAsync();
                    if (defaultFlowItem != null)
                    {
                        data.WorkFlowItemId = defaultFlowItem.Id;
                        data.WorkFlowItemName = defaultFlowItem.Name;
                    }
                }
            }
            else
            {
                var refFlowItem = await _DbContext.WorkFlowItems.Where(x => x.Id == data.WorkFlowItemId).FirstOrDefaultAsync();
                if (refFlowItem != null)
                {
                    data.WorkFlowItemId = refFlowItem.Id;
                    data.WorkFlowItemName = refFlowItem.Name;
                }
            }

            data.Url = appConfig.Plugins.OrderViewer + "?order=" + data.Id;
            data.CreatorName = await _DbContext.Accounts.Where(x => x.Id == data.Creator).Select(x => x.Name).FirstOrDefaultAsync();
            data.ModifierName = await _DbContext.Accounts.Where(x => x.Id == data.Modifier).Select(x => x.Name).FirstOrDefaultAsync();
            return data.ToDTO();
        }
        #endregion

        #region SatisfyCreateAsync 校验数据是否符合创建规范
        /// <summary>
        /// 校验数据是否符合创建规范
        /// </summary>
        /// <param name="accid"></param>
        /// <param name="data"></param>
        /// <param name="modelState"></param>
        /// <returns></returns>
        public override async Task SatisfyCreateAsync(string accid, Order data, ModelStateDictionary modelState)
        {
            if (data.OrderDetails == null && data.OrderDetails.Count <= 0)
            {
                modelState.AddModelError("OrderDetails", "订单不包含任何产品信息");
            }
            else
            {
                for (int idx = data.OrderDetails.Count - 1; idx >= 0; idx--)
                {
                    var item = data.OrderDetails[idx];
                    var exist = await _DbContext.ProductSpec.AnyAsync(x => x.Id == item.ProductSpecId);
                    if (!exist)
                    {
                        modelState.AddModelError("ProductSpecId", $"不存在Id为{item.ProductSpecId }的产品规格信息");
                        return;
                    }
                }
            }

        }
        #endregion

        #region CreateAsync 创建订单
        /// <summary>
        /// 创建订单
        /// </summary>
        /// <param name="accid"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public override async Task CreateAsync(string accid, Order data)
        {
            var currentAcc = await _DbContext.Accounts.FindAsync(accid);
            data.Id = GuidGen.NewGUID();
            data.Creator = accid;
            data.Modifier = accid;
            data.CreatedTime = DateTime.Now;
            data.ModifiedTime = data.CreatedTime;
            data.OrganizationId = currentAcc.OrganizationId;
            data.Url = appConfig.Plugins.OrderViewer + "?order=" + data.Id;
            if (data.OrderDetails != null && data.OrderDetails.Count > 0)
            {
                for (int idx = data.OrderDetails.Count - 1; idx >= 0; idx--)
                {
                    var item = data.OrderDetails[idx];
                    item.Id = GuidGen.NewGUID();
                    item.Creator = accid;
                    item.Modifier = accid;
                    item.CreatedTime = data.CreatedTime;
                    item.ModifiedTime = data.CreatedTime;
                    item.OrganizationId = currentAcc.OrganizationId;
                    //item.OrderDetailStateId = (int)OrderDetailStateEnum.Confirm;
                    item.ProductSpec = await _DbContext.ProductSpec.Where(x => x.Id == item.ProductSpecId).Select(x => new ProductSpec() { Name = x.Name, ProductId = x.ProductId }).FirstOrDefaultAsync();
                    if (item.ProductSpec != null)
                        item.ProductSpec.Product = await _DbContext.Products.Where(x => x.Id == item.ProductSpec.ProductId).Select(x => new Product() { Name = x.Name }).FirstOrDefaultAsync();

                }
            }
            //生成订单编号
            var beginTime = new DateTime(data.CreatedTime.Year, data.CreatedTime.Month, data.CreatedTime.Day);
            var endTime = beginTime.AddDays(1);
            var orderCount = await _DbContext.Orders.Where(x => x.CreatedTime >= beginTime && x.CreatedTime < endTime).CountAsync();
            data.OrderNo = beginTime.ToString("yyyyMMdd") + (orderCount + 1).ToString().PadLeft(5, '0');
            _DbContext.Orders.Add(data);
            await _DbContext.SaveChangesAsync();
        }
        #endregion

        public override async Task<IQueryable<Order>> _GetPermissionData(string accid, DataOperateEnum dataOp, bool withInActive = false)
        {
            var query = Enumerable.Empty<Order>().AsQueryable();

            var currentAcc = await _DbContext.Accounts.Select(x => new Account() { Id = x.Id, OrganizationId = x.OrganizationId, Type = x.Type }).FirstOrDefaultAsync(x => x.Id == accid);
            if (currentAcc == null)
                return query;

            //数据状态
            if (withInActive)
                query = _DbContext.Set<Order>();
            else
                query = _DbContext.Set<Order>().Where(x => x.ActiveFlag == AppConst.I_DataState_Active);







            //if (dataOp != DataOperateEnum.Retrieve)
            //{

            //    //超级管理员只管理内置角色
            //    if (currentAcc.Type == AppConst.AccountType_SysAdmin || currentAcc.Type == AppConst.AccountType_SysService)
            //    {
            //        query = query.Where(x => x.IsInner == true);
            //        return query;
            //    }


            //    //品牌商管理员管理自己建立的角色
            //    if (currentAcc.Type == AppConst.AccountType_BrandAdmin)
            //    {
            //        query = query.Where(x => x.OrganizationId == currentAcc.OrganizationId);
            //        return query;
            //    }


            //    return query.Where(x => x.Creator == currentAcc.Id);
            //}
            //query = query.Where(x => x.IsInner == true || x.OrganizationId == currentAcc.OrganizationId);
            return await Task.FromResult(query);
        }
    }
}
