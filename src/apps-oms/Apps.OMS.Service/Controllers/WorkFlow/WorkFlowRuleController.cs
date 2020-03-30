using Apps.Base.Common;
using Apps.Base.Common.Attributes;
using Apps.Base.Common.Controllers;
using Apps.Base.Common.Interfaces;
using Apps.Base.Common.Models;
using Apps.Basic.Export.Services;
using Apps.OMS.Data.Entities;
using Apps.OMS.Export.DTOs;
using Apps.OMS.Export.Models;
using Apps.OMS.Export.Services;
using Apps.OMS.Service.Contexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace Apps.OMS.Service.Controllers
{
    /// <summary>
    /// 流程控制器
    /// </summary>
    [Authorize]
    [Route("/[controller]")]
    [ApiController]
    public class WorkFlowRuleController : ServiceBaseController<WorkFlowRule>
    {
        protected AppConfig _AppConfig { get; }
        protected AppDbContext _Context { get; }

        #region 构造函数
        public WorkFlowRuleController(IRepository<WorkFlowRule> repository, AppDbContext context, IOptions<AppConfig> settingsOptions)
            : base(repository)
        {
            _Context = context;
            _AppConfig = settingsOptions.Value;
        }
        #endregion

        #region Get 根据分页获取流程规则信息
        /// <summary>
        /// 根据分页获取流程规则信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedData<WorkFlowRuleDTO>), 200)]
        public async Task<IActionResult> Get([FromQuery] PagingRequestModel model)
        {
            var accountMicroService = new AccountMicroService(_AppConfig.APIGatewayServer);
            var nationalUrbanMicroService = new NationalUrbanMicroService(_AppConfig.APIGatewayServer);

            var toDTO = new Func<WorkFlowRule, Task<WorkFlowRuleDTO>>(async (entity) =>
            {
                var dto = new WorkFlowRuleDTO();
                dto.Id = entity.Id;
                dto.Name = entity.Name;
                dto.Description = entity.Description;
                dto.Creator = entity.Creator;
                dto.Modifier = entity.Modifier;
                dto.CreatedTime = entity.CreatedTime;
                dto.ModifiedTime = entity.ModifiedTime;
                dto.IsInner = entity.IsInner;
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

        #region Get 根据Id获取流程规则信息
        /// <summary>
        /// 根据Id获取流程规则信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(WorkFlowRuleDTO), 200)]
        public override async Task<IActionResult> Get(string id)
        {
            var accountMicroService = new AccountMicroService(_AppConfig.APIGatewayServer);
            var nationalUrbanMicroService = new NationalUrbanMicroService(_AppConfig.APIGatewayServer);

            var toDTO = new Func<WorkFlowRule, Task<WorkFlowRuleDTO>>(async (entity) =>
            {
                var dto = new WorkFlowRuleDTO();
                dto.Id = entity.Id;
                dto.Name = entity.Name;
                dto.Description = entity.Description;
                dto.Creator = entity.Creator;
                dto.Modifier = entity.Modifier;
                dto.CreatedTime = entity.CreatedTime;
                dto.ModifiedTime = entity.ModifiedTime;
                var refWorFlow = await _Context.WorkFlowRuleDetails.FirstOrDefaultAsync(x=>x.OrganizationId==CurrentAccountOrganizationId&&x.WorkFlowRuleId==entity.Id);
                if (refWorFlow!=null)
                    dto.WorkFlowId = refWorFlow.WorkFlowId;
                
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

        #region UpdateDetail 更新组织流程规格设置
        /// <summary>
        /// 更新组织流程规格设置
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("Detail")]
        [ValidateModel]
        public async Task<IActionResult> UpdateDetail([FromBody] WorkFlowRuleDetailUpdateModel model)
        {
            var exitRule = await _Context.WorkFlowRules.CountAsync(x => x.Id == model.WorkFlowRuleId) > 0;
            if (!exitRule)
            {
                ModelState.AddModelError("WorkFlowRuleId", $"{model.WorkFlowRuleId}记录不存在");
                return BadRequest(ModelState);
            }
            var exitFlow = await _Context.WorkFlows.CountAsync(x => x.Id == model.WorkFlowId) > 0;
            if (!exitFlow)
            {
                ModelState.AddModelError("WorkFlowId", $"{model.WorkFlowId}记录不存在");
                return BadRequest(ModelState);
            }

            var refDefine = await _Context.WorkFlowRuleDetails.FirstOrDefaultAsync(x => x.OrganizationId == CurrentAccountOrganizationId && x.WorkFlowRuleId == model.WorkFlowRuleId);
            if (refDefine == null)
            {
                refDefine = new WorkFlowRuleDetail();
                refDefine.OrganizationId = CurrentAccountOrganizationId;
                refDefine.WorkFlowRuleId = model.WorkFlowRuleId;
            }
            refDefine.WorkFlowId = model.WorkFlowId;
            if (string.IsNullOrWhiteSpace(refDefine.Id))
                _Context.WorkFlowRuleDetails.Add(refDefine);
            else
                _Context.WorkFlowRuleDetails.Update(refDefine);
            await _Context.SaveChangesAsync();
            return NoContent();
        } 
        #endregion
    }
}