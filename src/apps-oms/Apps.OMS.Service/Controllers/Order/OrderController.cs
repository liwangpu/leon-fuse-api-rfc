using Apps.Base.Common;
using Apps.Base.Common.Attributes;
using Apps.Base.Common.Interfaces;
using Apps.Base.Common.Models;
using Apps.Basic.Export.Services;
using Apps.FileSystem.Export.Services;
using Apps.MoreJee.Export.Services;
using Apps.OMS.Data.Consts;
using Apps.OMS.Data.Entities;
using Apps.OMS.Export.DTOs;
using Apps.OMS.Export.Models;
using Apps.OMS.Service.Contexts;
using Apps.OMS.Service.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace Apps.OMS.Service.Controllers
{
    /// <summary>
    /// 订单控制器
    /// </summary>
    [Authorize]
    [Route("/[controller]")]
    [ApiController]
    public class OrderController : ListviewController<Order>
    {
        protected override AppDbContext _Context { get; }

        #region 构造函数
        public OrderController(IRepository<Order> repository, AppDbContext context, IOptions<AppConfig> settingsOptions)
            : base(repository, settingsOptions)
        {
            _Context = context;
        }
        #endregion

        #region Get 根据分页获取订单信息
        /// <summary>
        /// 根据分页获取订单信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedData<OrderDTO>), 200)]
        public async Task<IActionResult> Get([FromQuery] PagingRequestModel model)
        {
            var accountMicroService = new AccountMicroService(_AppConfig.APIGatewayServer, Token);

            var toDTO = new Func<Order, Task<OrderDTO>>(async (entity) =>
            {
                var dto = new OrderDTO();
                dto.Id = entity.Id;
                dto.Name = entity.Name;
                dto.Description = entity.Description;
                dto.Creator = entity.Creator;
                dto.Modifier = entity.Modifier;
                dto.CreatedTime = entity.CreatedTime;
                dto.ModifiedTime = entity.ModifiedTime;
                dto.OrganizationId = entity.OrganizationId;
                dto.TotalNum = entity.TotalNum;
                dto.TotalPrice = Math.Round(entity.TotalPrice, 2, MidpointRounding.AwayFromZero);
                dto.OrderNo = entity.OrderNo;
                dto.CustomerName = entity.CustomerName;
                dto.CustomerPhone = entity.CustomerPhone;
                dto.CustomerAddress = entity.CustomerAddress;
                dto.Url = $"{_AppConfig.Plugins.OrderViewer}?order={entity.Id}";

                #region 订单状态
                if (string.IsNullOrWhiteSpace(entity.WorkFlowItemId))
                {
                    var refRule = await _Context.WorkFlowRuleDetails.FirstOrDefaultAsync(x => x.OrganizationId == CurrentAccountOrganizationId && x.WorkFlowRuleId == WorkFlowRuleConst.OrderWorkFlow);
                    if (refRule != null)
                    {
                        var dfItem = await _Context.WorkFlowItems.FirstOrDefaultAsync(x => x.WorkFlowId == refRule.WorkFlowId && x.FlowGrade == 0);
                        dto.WorkFlowItemId = dfItem.Id;
                        dto.WorkFlowItemName = dfItem.Name;
                    }
                }
                else
                {
                    dto.WorkFlowItemId = entity.WorkFlowItemId;
                    var dfitem = await _Context.WorkFlowItems.FirstOrDefaultAsync(x => x.Id == entity.WorkFlowItemId);
                    if (dfitem != null)
                        dto.WorkFlowItemName = dfitem.Name;

                }
                #endregion

                await accountMicroService.GetNameByIds(entity.Creator, entity.Modifier, (creatorName, modifierName) =>
                {
                    dto.CreatorName = creatorName;
                    dto.ModifierName = modifierName;
                });
                return await Task.FromResult(dto);
            });
            var user = await accountMicroService.GetProfile();
            var advanceQuery = new Func<IQueryable<Order>, Task<IQueryable<Order>>>(async (query) =>
            {
                query = query.Where(x => x.OrganizationId == user.OrganizationId);
                if (user.RoleId == "brandmember")
                {
                    query = query.Where(x => x.Creator == user.Id);
                }
                return query.OrderByDescending(x=>x.ModifiedTime);
            });
            return await _PagingRequest(model, toDTO, advanceQuery);
        }
        #endregion

        #region Get 根据Id获取订单信息
        /// <summary>
        /// 根据Id获取订单信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(OrderDTO), 200)]
        public override async Task<IActionResult> Get(string id)
        {
            var accountMicroService = new AccountMicroService(_AppConfig.APIGatewayServer);
            var productSpecMicroService = new ProductSpecMicroService(_AppConfig.APIGatewayServer);
            var productMicroService = new ProductMicroService(_AppConfig.APIGatewayServer);
            var fileMicroServer = new FileMicroService(_AppConfig.APIGatewayServer);

            var toDTO = new Func<Order, Task<OrderDTO>>(async (entity) =>
            {
                var dto = new OrderDTO();
                dto.Id = entity.Id;
                dto.Name = entity.Name;

                dto.Description = entity.Description;
                dto.Creator = entity.Creator;
                dto.Modifier = entity.Modifier;
                dto.CreatedTime = entity.CreatedTime;
                dto.ModifiedTime = entity.ModifiedTime;
                dto.OrganizationId = entity.OrganizationId;
                dto.TotalNum = entity.TotalNum;
                //dto.TotalPrice = entity.TotalPrice;
                dto.OrderNo = entity.OrderNo;
                dto.CustomerName = entity.CustomerName;
                dto.CustomerPhone = entity.CustomerPhone;
                dto.CustomerAddress = entity.CustomerAddress;
                dto.Data = entity.Data;
                dto.SolutionId = entity.SolutionId;
                dto.Url = $"{_AppConfig.Plugins.OrderViewer}?order={entity.Id}";

                #region 订单状态
                if (string.IsNullOrWhiteSpace(entity.WorkFlowItemId))
                {
                    var refRule = await _Context.WorkFlowRuleDetails.FirstOrDefaultAsync(x => x.OrganizationId == CurrentAccountOrganizationId && x.WorkFlowRuleId == WorkFlowRuleConst.OrderWorkFlow);
                    if (refRule != null)
                    {
                        var dfItem = await _Context.WorkFlowItems.FirstOrDefaultAsync(x => x.WorkFlowId == refRule.WorkFlowId && x.FlowGrade == 0);
                        dto.WorkFlowItemId = dfItem.Id;
                        dto.WorkFlowItemName = dfItem.Name;
                    }
                }
                else
                {
                    dto.WorkFlowItemId = entity.WorkFlowItemId;
                    var dfitem = await _Context.WorkFlowItems.FirstOrDefaultAsync(x => x.Id == entity.WorkFlowItemId);
                    if (dfitem != null)
                        dto.WorkFlowItemName = dfitem.Name;

                }
                #endregion

                #region OrderDetails
                var details = new List<OrderDetailDTO>();
                if (entity.OrderDetails != null && entity.OrderDetails.Count > 0)
                {
                    decimal totalPrice = 0;
                    foreach (var it in entity.OrderDetails)
                    {
                        var ditem = new OrderDetailDTO();
                        ditem.Id = it.Id;
                        ditem.Remark = it.Remark;
                        ditem.Num = it.Num;
                        ditem.UnitPrice = Math.Round(it.UnitPrice, 2, MidpointRounding.AwayFromZero);
                        ditem.TotalPrice = Math.Round(it.UnitPrice * it.Num, 2, MidpointRounding.AwayFromZero);
                        ditem.AttachmentIds = it.AttachmentIds;
                        ditem.Room = it.Room;
                        ditem.Owner = it.Owner;
                        totalPrice += ditem.TotalPrice;
                        var pckDtos = new List<OrderDetailPackageDTO>();
                        var pcks = await _Context.OrderDetailPackages.Where(x => x.OrderDetailId == it.Id).OrderByDescending(x => x.Num).ToListAsync();
                        foreach (var pck in pcks)
                        {
                            var pckDto = new OrderDetailPackageDTO();
                            var refPackage = await _Context.ProductPackages.FirstOrDefaultAsync(x => x.Id == pck.ProductPackageId);
                            if (refPackage != null)
                            {
                                pckDto.Id = pck.Id;
                                pckDto.PackageName = refPackage.Name;
                                pckDto.PackingUnit = refPackage.Num;
                                pckDto.Num = pck.Num;
                                pckDtos.Add(pckDto);
                            }

                        }
                        ditem.Packages = pckDtos;

                        await productSpecMicroService.GetBriefById(it.ProductSpecId, (spec) =>
                          {
                              ditem.ProductSpecId = spec.Id;
                              ditem.ProductSpecName = spec.Name;
                              ditem.ProductId = spec.ProductId;
                              ditem.Icon = spec.Icon;
                              ditem.TPID = spec.TPID;
                          });

                        await productMicroService.GetBriefById(ditem.ProductId, (prod) =>
                         {
                             ditem.ProductName = prod.Name;
                             ditem.ProductCategoryId = prod.CategoryId;
                             ditem.ProductCategoryName = prod.CategoryName;
                             ditem.ProductUnit = prod.Unit;
                             ditem.ProductBrand = prod.Brand;
                             ditem.ProductDescription = prod.Description;
                         });

                        if (!string.IsNullOrWhiteSpace(ditem.AttachmentIds))
                        {
                            var fsIdArr = ditem.AttachmentIds.Split(",", StringSplitOptions.RemoveEmptyEntries);
                            var fsList = new List<OrderDetailAttachmentDTO>();
                            foreach (var fid in fsIdArr)
                            {
                                await fileMicroServer.GetById(fid, (fs) =>
                                  {
                                      var fdto = new OrderDetailAttachmentDTO();
                                      fdto.Id = fs.Id;
                                      fdto.Name = fs.Name;
                                      fdto.Url = fs.Url;
                                      fsList.Add(fdto);
                                  });
                            }
                            ditem.Attachments = fsList;
                        }

                        details.Add(ditem);
                    }
                    dto.TotalPrice = totalPrice;
                }
                dto.OrderDetails = details;
                #endregion

                #region CustomizedProduct
                var customizedProducts = new List<OrderDetailCustomizedProductDTO>();
                if (entity.CustomizedProducts != null & entity.CustomizedProducts.Count > 0)
                {
                    foreach (var it in entity.CustomizedProducts)
                    {
                        var ditem = new OrderDetailCustomizedProductDTO();
                        ditem.Id = it.Id;
                        ditem.Name = it.Name;
                        ditem.Icon = it.Icon;
                        customizedProducts.Add(ditem);
                    }
                }
                dto.CustomizedProducts = customizedProducts;
                #endregion

                #region OrderFlowLogs
                var logs = new List<OrderFlowLogDTO>();
                if (entity.OrderFlowLogs != null && entity.OrderFlowLogs.Count > 0)
                {
                    foreach (var log in entity.OrderFlowLogs)
                    {
                        var logDto = new OrderFlowLogDTO();
                        logDto.Id = log.Id;
                        logDto.Approve = log.Approve;
                        logDto.Remark = log.Remark;
                        logDto.WorkFlowItemId = log.WorkFlowItemId;
                        logDto.WorkFlowItemName = await _Context.WorkFlowItems.Where(x => x.Id == log.WorkFlowItemId).Select(x => x.Name).FirstOrDefaultAsync();
                        logDto.Creator = log.Creator;
                        logDto.CreatedTime = log.CreatedTime;
                        await accountMicroService.GetNameByIds(entity.Creator, (creatorName) =>
                        {
                            logDto.CreatorName = creatorName;
                        });
                        logs.Add(logDto);
                    }
                }
                dto.OrderFlowLogs = logs;
                #endregion

                await accountMicroService.GetNameByIds(entity.Creator, entity.Modifier, (creatorName, modifierName) =>
                {
                    dto.CreatorName = creatorName;
                    dto.ModifierName = modifierName;
                });
                return await Task.FromResult(dto);
            });
            return await _GetByIdRequest(id, toDTO);
        }
        #endregion

        #region GetOrganOrderFlow 获取订单的流程信息
        /// <summary>
        /// 获取订单的流程信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("OrganOrderFlow")]
        public async Task<IActionResult> GetOrganOrderFlow()
        {
            var refRule = await _Context.WorkFlowRuleDetails.FirstOrDefaultAsync(x => x.OrganizationId == CurrentAccountOrganizationId && x.WorkFlowRuleId == WorkFlowRuleConst.OrderWorkFlow);
            if (refRule != null)
            {
                var workflow = await _Context.WorkFlows.Include(x => x.WorkFlowItems).FirstOrDefaultAsync(x => x.Id == refRule.WorkFlowId);
                if (workflow != null)
                    return Ok(workflow);

            }
            return NotFound();
        }
        #endregion

        #region Post 创建订单
        /// <summary>
        /// 创建订单
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ValidateModel]
        [ProducesResponseType(typeof(OrderDTO), 200)]
        [ProducesResponseType(typeof(ValidationResultModel), 400)]
        public async Task<IActionResult> Post([FromBody]OrderCreateModel model)
        {
            var mapping = new Func<Order, Task<Order>>(async (entity) =>
            {
                entity.Name = model.Name;
                entity.Description = model.Description;
                entity.OrganizationId = CurrentAccountOrganizationId;
                entity.CustomerName = model.CustomerName;
                entity.CustomerPhone = model.CustomerPhone;
                entity.CustomerAddress = model.CustomerAddress;
                entity.SolutionId = model.SolutionId;
                entity.Data = model.Data;

                var details = new List<OrderDetail>();
                if (model.Content != null && model.Content.Count > 0)
                {
                    model.Content.ForEach(item =>
                    {
                        var detail = new OrderDetail();
                        detail.Id = GuidGen.NewGUID();
                        detail.ProductSpecId = item.ProductSpecId;
                        detail.Num = item.Num;
                        //detail.UnitPrice = item.UnitPrice;
                        detail.UnitPrice = Math.Round(item.UnitPrice, 2, MidpointRounding.AwayFromZero);
                        detail.Remark = item.Remark;
                        detail.Room = item.Room;
                        detail.Owner = item.Owner;
                        detail.CreatedTime = DateTime.Now;
                        detail.ModifiedTime = detail.CreatedTime;
                        detail.Creator = CurrentAccountId;
                        detail.Modifier = CurrentAccountId;
                        detail.OrganizationId = CurrentAccountOrganizationId;
                        details.Add(detail);
                    });
                }
                entity.OrderDetails = details;

                var customizedProducts = new List<OrderDetailCustomizedProduct>();
                if (model.CustomizedProduct != null && model.CustomizedProduct.Count > 0)
                {
                    model.CustomizedProduct.ForEach(item =>
                    {
                        var customiedProd = new OrderDetailCustomizedProduct();
                        customiedProd.Id = GuidGen.NewGUID();
                        customiedProd.Name = item.Name;
                        customiedProd.Icon = item.Icon;
                        customizedProducts.Add(customiedProd);
                    });
                }
                entity.CustomizedProducts = customizedProducts;


                return await Task.FromResult(entity);
            });
            return await _PostRequest(mapping);
        }
        #endregion

        #region UpdateBasicInfo 更新订单基础信息
        /// <summary>
        /// 更新订单基础信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("BasicInfo")]
        [ValidateModel]
        [ProducesResponseType(typeof(ValidationResultModel), 400)]
        public async Task<IActionResult> UpdateBasicInfo([FromBody] OrderBasicInfoUpdateModel model)
        {
            var mapping = new Func<Order, Task<Order>>(async (entity) =>
            {
                entity.Name = model.Name;
                entity.Description = model.Description;
                return await Task.FromResult(entity);
            });
            return await _PutRequest(model.Id, mapping);
        }
        #endregion

        #region UpdateCustomerInfo 更新订单客户基础信息
        /// <summary>
        /// 更新订单客户基础信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("CustomerInfo")]
        [ValidateModel]
        [ProducesResponseType(typeof(ValidationResultModel), 400)]
        public async Task<IActionResult> UpdateCustomerInfo([FromBody] OrderCustomerInfoUpdateModel model)
        {
            var mapping = new Func<Order, Task<Order>>(async (entity) =>
            {
                entity.CustomerName = model.CustomerName;
                entity.CustomerPhone = model.CustomerPhone;
                entity.CustomerAddress = model.CustomerAddress;
                return await Task.FromResult(entity);
            });
            return await _PutRequest(model.Id, mapping);
        }
        #endregion

        #region UpdateOrderDetail 更新订单详细信息
        /// <summary>
        /// 更新订单详细信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("OrderDetail")]
        [ValidateModel]
        [ProducesResponseType(typeof(ValidationResultModel), 400)]
        [ProducesResponseType(typeof(OrderDetailDTO), 200)]
        public async Task<IActionResult> UpdateOrderDetail([FromBody] OrderDetailEditModel model)
        {
            var dto = new OrderDetailDTO();
            var detail = await _Context.OrderDetails.Where(x => x.Id == model.Id).FirstOrDefaultAsync();
            if (detail == null) return NotFound();

            detail.Num = model.Num;
            detail.Remark = model.Remark;
            if (!string.IsNullOrWhiteSpace(model.AttachIds))
            {
                var idArr = model.AttachIds.Trim().Split(",", StringSplitOptions.RemoveEmptyEntries);
                detail.AttachmentIds = string.Join(",", idArr);
            }
            else
                detail.AttachmentIds = string.Empty;
            await (_Repository as OrderRepository).ReCalculatePackage(detail);
            _Context.OrderDetails.Update(detail);
            _Context.SaveChanges();

            #region 获取产品包装规格
            {
                var pckDtos = new List<OrderDetailPackageDTO>();
                var pcks = await _Context.OrderDetailPackages.Where(x => x.OrderDetailId == detail.Id).OrderByDescending(x => x.Num).ToListAsync();
                foreach (var pck in pcks)
                {
                    var pckDto = new OrderDetailPackageDTO();
                    var refPackage = await _Context.ProductPackages.FirstOrDefaultAsync(x => x.Id == pck.ProductPackageId);
                    if (refPackage != null)
                    {
                        pckDto.Id = pck.Id;
                        pckDto.PackageName = refPackage.Name;
                        pckDto.PackingUnit = refPackage.Num;
                        pckDto.Num = pck.Num;
                        pckDtos.Add(pckDto);
                    }

                }
                dto.Packages = pckDtos;
            }
            #endregion

            return Ok(dto);
        }
        #endregion

        #region AuditOrder 订单审核
        /// <summary>
        /// 订单审核
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("AuditOrder")]
        [HttpPut]
        [ValidateModel]
        [ProducesResponseType(typeof(OrderDTO), 200)]
        [ProducesResponseType(typeof(ValidationResultModel), 400)]
        public async Task<IActionResult> AuditOrder([FromBody]OrderWorkFlowAuditEditModel model)
        {
            var mapping = new Func<Order, Task<Order>>(async (entity) =>
            {
                var logs = await _Context.OrderFlowLogs.Where(x => x.Order == entity).ToListAsync();
                var operateLog = new OrderFlowLog();
                operateLog.Id = GuidGen.NewGUID();
                operateLog.Remark = model.Remark;
                operateLog.CreatedTime = DateTime.Now;
                operateLog.Creator = CurrentAccountId;
                operateLog.Approve = model.Approve;
                operateLog.WorkFlowItemId = model.WorkFlowItemId;
                operateLog.Order = entity;
                _Context.OrderFlowLogs.Add(operateLog);
                await _Context.SaveChangesAsync();
                var workFlowItem = await _Context.WorkFlowItems.Include(x => x.WorkFlow).Where(x => x.Id == model.WorkFlowItemId).FirstOrDefaultAsync();

                if (workFlowItem != null)
                {
                    var workFlow = await _Context.WorkFlows.Include(x => x.WorkFlowItems).Where(x => x == workFlowItem.WorkFlow).FirstOrDefaultAsync();

                    if (model.Approve)
                    {
                        var nextWorkFlowItem = workFlow.WorkFlowItems.Where(x => x.FlowGrade == workFlowItem.FlowGrade + 1).FirstOrDefault();
                        if (nextWorkFlowItem != null)
                            entity.WorkFlowItemId = nextWorkFlowItem.Id;
                    }
                    else
                    {
                        var lastWorkFlowItem = workFlow.WorkFlowItems.Where(x => x.FlowGrade == workFlowItem.FlowGrade - 1).FirstOrDefault();
                        if (lastWorkFlowItem != null)
                            entity.WorkFlowItemId = lastWorkFlowItem.Id;
                    }
                }

                return await Task.FromResult(entity);
            });
            return await _PutRequest(model.OrderId, mapping, async (data) =>
             {
                 return await Get(data.Id);
             });
        }
        #endregion

        #region Delete 删除订单信息
        /// <summary>
        /// 删除订单信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(Nullable), 200)]
        public async Task<IActionResult> Delete(string id)
        {
            return await _DeleteRequest(id);
        }
        #endregion

        #region BatchDelete 批量删除订单信息
        /// <summary>
        /// 批量删除订单信息
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpDelete("BatchDelete")]
        [ProducesResponseType(typeof(Nullable), 200)]
        public async Task<IActionResult> BatchDelete(string ids)
        {
            return await _BatchDeleteRequest(ids);
        }
        #endregion
    }
}