using Apps.Base.Common;
using Apps.Base.Common.Controllers;
using Apps.Base.Common.Interfaces;
using Apps.Base.Common.Models;
using Apps.Basic.Export.Services;
using Apps.OMS.Data.Entities;
using Apps.OMS.Export.DTOs;
using Apps.OMS.Export.Models;
using Apps.OMS.Service.Contexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Apps.OMS.Service.Controllers
{
    /// <summary>
    /// 积分参数设置控制器
    /// </summary>
    [Authorize]
    [Route("/[controller]")]
    [ApiController]
    public class MemberHierarchyParamController : ServiceBaseController<MemberHierarchyParam>
    {
        protected AppConfig _AppConfig { get; }
        protected AppDbContext _Context { get; }

        #region 构造函数
        public MemberHierarchyParamController(IRepository<MemberHierarchyParam> repository, AppDbContext context, IOptions<AppConfig> settingsOptions)
            : base(repository)
        {
            _Context = context;
            _AppConfig = settingsOptions.Value;
        }
        #endregion

        #region Get 根据分页获取会员等级积分计划信息
        /// <summary>
        /// 根据分页获取会员等级积分计划信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedData<MemberHierarchyParamDTO>), 200)]
        public async Task<IActionResult> Get([FromQuery] PagingRequestModel model)
        {
            var accountMicroService = new AccountMicroService(_AppConfig.APIGatewayServer);

            var toDTO = new Func<MemberHierarchyParam, Task<MemberHierarchyParamDTO>>(async (entity) =>
            {
                var dto = new MemberHierarchyParamDTO();
                dto.Id = entity.Id;
                dto.Name = entity.Name;
                dto.Description = entity.Description;
                dto.Creator = entity.Creator;
                dto.Modifier = entity.Modifier;
                dto.CreatedTime = entity.CreatedTime;
                dto.ModifiedTime = entity.ModifiedTime;

                var refSetting = await _Context.MemberHierarchySettings.Where(x => x.MemberHierarchyParamId == entity.Id && x.OrganizationId == CurrentAccountOrganizationId).FirstOrDefaultAsync();
                if (refSetting != null)
                    dto.Rate = refSetting.Rate;

                await accountMicroService.GetNameByIds(entity.Creator, entity.Modifier, (creatorName, modifierName) =>
                {
                    dto.CreatorName = creatorName;
                    dto.ModifierName = modifierName;
                });
                return await Task.FromResult(dto);
            });
            return await _PagingRequest(model, toDTO);
        }
        #endregion

        #region Get 根据Id获取会员等级积分计划信息
        /// <summary>
        /// 根据Id获取会员等级积分计划信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(MemberHierarchyParamDTO), 200)]
        public override async Task<IActionResult> Get(string id)
        {
            var accountMicroService = new AccountMicroService(_AppConfig.APIGatewayServer);

            var toDTO = new Func<MemberHierarchyParam, Task<MemberHierarchyParamDTO>>(async (entity) =>
            {
                var dto = new MemberHierarchyParamDTO();
                dto.Id = entity.Id;
                dto.Name = entity.Name;
                dto.Description = entity.Description;
                dto.Creator = entity.Creator;
                dto.Modifier = entity.Modifier;
                dto.CreatedTime = entity.CreatedTime;
                dto.ModifiedTime = entity.ModifiedTime;

                var refSetting = await _Context.MemberHierarchySettings.Where(x => x.MemberHierarchyParamId == entity.Id && x.OrganizationId == CurrentAccountOrganizationId).FirstOrDefaultAsync();
                if (refSetting != null)
                    dto.Rate = refSetting.Rate;

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

        #region GetHierarchySetting 获取组织积分计划比率信息
        /// <summary>
        /// 获取组织积分计划比率信息
        /// </summary>
        /// <param name="hierarchyId"></param>
        /// <returns></returns>
        [HttpGet("HierarchySetting/{hierarchyId}", Name = "GetHierarchySetting")]
        [ProducesResponseType(typeof(MemberHierarchyParamSettingDTO), 200)]
        public async Task<IActionResult> GetHierarchySetting(string hierarchyId)
        {
            var setting = await _Context.MemberHierarchySettings.Where(x => x.MemberHierarchyParamId == hierarchyId && x.OrganizationId == CurrentAccountOrganizationId).FirstOrDefaultAsync();
            if (setting == null)
            {
                setting = new MemberHierarchySetting();
                setting.OrganizationId = CurrentAccountOrganizationId;
                setting.MemberHierarchyParamId = hierarchyId;
                setting.Rate = 0;
            }
            var dto = new MemberHierarchyParamSettingDTO();
            dto.MemberHierarchyParamId = setting.MemberHierarchyParamId;
            dto.Rate = setting.Rate;
            return Ok(dto);
        }
        #endregion

        #region UpdateHierarchySetting 修改组织积分计划比率信息
        /// <summary>
        /// 修改组织积分计划比率信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("UpdateHierarchySetting")]
        public async Task<IActionResult> UpdateHierarchySetting([FromBody] MemberHierarchyParamSettingUpdateModel model)
        {
            var setting = await _Context.MemberHierarchySettings.Where(x => x.MemberHierarchyParamId == model.MemberHierarchyParamId && x.OrganizationId == CurrentAccountOrganizationId).FirstOrDefaultAsync();
            if (setting == null)
            {
                setting = new MemberHierarchySetting();
                setting.OrganizationId = CurrentAccountOrganizationId;
            }
            setting.MemberHierarchyParamId = model.MemberHierarchyParamId;
            setting.Rate = model.Rate;

            if (string.IsNullOrWhiteSpace(setting.Id))
            {
                setting.Id = GuidGen.NewGUID();
                _Context.MemberHierarchySettings.Add(setting);
            }
            else
            {
                _Context.MemberHierarchySettings.Update(setting);
            }
            await _Context.SaveChangesAsync();
            return Ok();
        }
        #endregion

        #region GetPointExchange 获取组织订单积分兑换率
        /// <summary>
        /// 获取组织订单积分兑换率
        /// </summary>
        /// <returns></returns>
        [HttpGet("PointExchange")]
        [ProducesResponseType(typeof(decimal), 200)]
        public async Task<IActionResult> GetPointExchange()
        {
            var exchange = await _Context.OrderPointExchanges.FirstOrDefaultAsync(x => x.OrganizationId == CurrentAccountOrganizationId);
            var rate = exchange != null ? exchange.Rate : 0;
            return Ok(rate);
        }
        #endregion

        #region UpdatePointExchange 设置组织订单积分兑换率
        /// <summary>
        /// 设置组织订单积分兑换率
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("PointExchange")]
        [ProducesResponseType(typeof(decimal), 200)]
        public async Task<IActionResult> UpdatePointExchange([FromBody] OrderPointExchangeUpdateModel model)
        {
            if (model.Rate <= 0)
            {
                ModelState.AddModelError("Rate", "兑换率必须大于0");
                return BadRequest(ModelState);
            }
            var exchange = await _Context.OrderPointExchanges.FirstOrDefaultAsync(x => x.OrganizationId == CurrentAccountOrganizationId);
            if (exchange == null)
            {
                exchange = new OrderPointExchange();
                exchange.OrganizationId = CurrentAccountOrganizationId;
            }
            exchange.Rate = model.Rate;
            if (!string.IsNullOrWhiteSpace(exchange.Id))
                _Context.OrderPointExchanges.Update(exchange);
            else
                _Context.OrderPointExchanges.Add(exchange);
            _Context.SaveChanges();
            return Ok();
        } 
        #endregion

    }
}