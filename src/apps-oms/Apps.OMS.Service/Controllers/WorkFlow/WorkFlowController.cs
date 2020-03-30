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
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Apps.OMS.Service.Controllers
{
    /// <summary>
    /// 流程管理控制器
    /// </summary>
    [Authorize]
    [Route("/[controller]")]
    [ApiController]
    public class WorkFlowController : ServiceBaseController<WorkFlow>
    {
        protected AppConfig _AppConfig { get; }
        protected AppDbContext _Context { get; }

        #region 构造函数
        public WorkFlowController(IRepository<WorkFlow> repository, AppDbContext context, IOptions<AppConfig> settingsOptions)
            : base(repository)
        {
            _Context = context;
            _AppConfig = settingsOptions.Value;
        }
        #endregion

        #region Get 根据分页获取流程信息
        /// <summary>
        /// 根据分页获取流程信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedData<WorkFlowDTO>), 200)]
        public async Task<IActionResult> Get([FromQuery] PagingRequestModel model)
        {
            var accountMicroService = new AccountMicroService(_AppConfig.APIGatewayServer);
            var nationalUrbanMicroService = new NationalUrbanMicroService(_AppConfig.APIGatewayServer);

            var toDTO = new Func<WorkFlow, Task<WorkFlowDTO>>(async (entity) =>
            {
                var dto = new WorkFlowDTO();
                dto.Id = entity.Id;
                dto.Name = entity.Name;
                dto.Description = entity.Description;
                dto.Creator = entity.Creator;
                dto.Modifier = entity.Modifier;
                dto.CreatedTime = entity.CreatedTime;
                dto.ModifiedTime = entity.ModifiedTime;

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

        #region Get 根据Id获取流程信息
        /// <summary>
        /// 根据Id获取流程信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(WorkFlowDTO), 200)]
        public override async Task<IActionResult> Get(string id)
        {
            var accountMicroService = new AccountMicroService(_AppConfig.APIGatewayServer);
            var nationalUrbanMicroService = new NationalUrbanMicroService(_AppConfig.APIGatewayServer);

            var toDTO = new Func<WorkFlow, Task<WorkFlowDTO>>(async (entity) =>
            {
                var dto = new WorkFlowDTO();
                dto.Id = entity.Id;
                dto.Name = entity.Name;
                dto.Description = entity.Description;
                dto.Creator = entity.Creator;
                dto.Modifier = entity.Modifier;
                dto.CreatedTime = entity.CreatedTime;
                dto.ModifiedTime = entity.ModifiedTime;
                dto.ApplyOrgans = entity.ApplyOrgans;
                var wfItems = new List<WorkFlowItemDTO>();
                if (entity.WorkFlowItems != null && entity.WorkFlowItems.Count > 0)
                {
                    foreach (var it in entity.WorkFlowItems)
                    {
                        var wfitem = new WorkFlowItemDTO();
                        wfitem.Id = it.Id;
                        wfitem.Name = it.Name;
                        wfitem.Description = it.Description;
                        wfitem.CreatedTime = it.CreatedTime;
                        wfitem.Creator = it.Creator;
                        wfitem.ModifiedTime = it.ModifiedTime;
                        wfitem.Modifier = it.Modifier;
                        wfitem.FlowGrade = it.FlowGrade;
                        wfitem.OperateRoles = it.OperateRoles;
                        wfitem.SubWorkFlowId = it.SubWorkFlowId;
                        wfitem.AutoWorkFlow = it.AutoWorkFlow;
                        wfItems.Add(wfitem);
                    }
                }
                dto.WorkFlowItems = wfItems.OrderBy(x => x.FlowGrade).ToList();

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

        #region Post 新建工作流程信息
        /// <summary>
        /// 新建工作流程信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateModel]
        [ProducesResponseType(typeof(WorkFlowDTO), 200)]
        [ProducesResponseType(typeof(ValidationResultModel), 400)]
        public async Task<IActionResult> Post([FromBody]WorkFlowCreateModel model)
        {
            var mapping = new Func<WorkFlow, Task<WorkFlow>>(async (entity) =>
            {
                entity.Name = model.Name;
                entity.Description = model.Description;
                entity.ApplyOrgans = model.ApplyOrgans;
                return await Task.FromResult(entity);
            });
            return await _PostRequest(mapping);
        }
        #endregion

        #region Put 更新工作流程信息
        /// <summary>
        /// 更新工作流程信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [ValidateModel]
        [ProducesResponseType(typeof(WorkFlowDTO), 200)]
        [ProducesResponseType(typeof(ValidationResultModel), 400)]
        public async Task<IActionResult> Put([FromBody]WorkFlowUpdateModel model)
        {
            var mapping = new Func<WorkFlow, Task<WorkFlow>>(async (entity) =>
            {
                entity.Name = model.Name;
                entity.Description = model.Description;
                entity.ApplyOrgans = model.ApplyOrgans;
                return await Task.FromResult(entity);
            });
            return await _PutRequest(model.Id, mapping);
        }
        #endregion

        #region UpdateWorkFlowItem 更新详细工作流程
        /// <summary>
        /// 更新详细工作流程
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("UpdateWorkFlowItem")]
        [ValidateModel]
        [ProducesResponseType(typeof(WorkFlowDTO), 200)]
        [ProducesResponseType(typeof(ValidationResultModel), 400)]
        public async Task<IActionResult> UpdateWorkFlowItem([FromBody]WorkFlowItemEditModel model)
        {
            var mapping = new Func<WorkFlow, Task<WorkFlow>>(async (entity) =>
            {
                var workFlowItems = entity.WorkFlowItems != null ? entity.WorkFlowItems : new List<WorkFlowItem>();

                var destGradeIndex = model.FlowGrade;//目的index
                var originGradeIndex = !string.IsNullOrWhiteSpace(model.Id) ? workFlowItems.First(x => x.Id == model.Id).FlowGrade : 0;//原index

                if (!string.IsNullOrWhiteSpace(model.Id))
                {
                    for (var idx = workFlowItems.Count - 1; idx >= 0; idx--)
                    {
                        var curItem = workFlowItems[idx];

                        if (destGradeIndex < originGradeIndex)
                        {
                            //上移
                            /*
                             * 1)自己本身变成destIndex
                             * 2)原items中,大于等于destIndex且小于自己originIndex都加1
                             */
                            if (curItem.FlowGrade >= destGradeIndex && curItem.FlowGrade < originGradeIndex)
                                curItem.FlowGrade += 1;

                        }
                        else if (destGradeIndex > originGradeIndex)
                        {
                            //下移
                            /*
                             * 1)自己本身变成destIndex
                             * 2)原items中,大于originIndex且小于等于destIndex都减1
                             */
                            if (curItem.FlowGrade > originGradeIndex && curItem.FlowGrade <= destGradeIndex)
                                curItem.FlowGrade -= 1;
                        }
                        else { }

                        //以上提到的自己本身index变为destIndex
                        if (curItem.Id == model.Id)
                        {
                            curItem.Name = model.Name;
                            curItem.Description = model.Description;
                            curItem.OperateRoles = model.OperateRoles;
                            curItem.AutoWorkFlow = model.AutoWorkFlow;
                            curItem.FlowGrade = destGradeIndex;
                            curItem.SubWorkFlowId = model.SubWorkFlowId;
                            curItem.Modifier = CurrentAccountId;
                            curItem.ModifiedTime = DateTime.Now;
                        }
                    }
                }
                else
                {
                    var newFlowItem = new WorkFlowItem();
                    newFlowItem.Id = GuidGen.NewGUID();
                    newFlowItem.Name = model.Name;
                    newFlowItem.Description = model.Description;
                    newFlowItem.OperateRoles = model.OperateRoles;
                    newFlowItem.AutoWorkFlow = model.AutoWorkFlow;
                    newFlowItem.FlowGrade = workFlowItems.Count;
                    newFlowItem.SubWorkFlowId = model.SubWorkFlowId;
                    newFlowItem.Creator = CurrentAccountId;
                    newFlowItem.Modifier = CurrentAccountId;
                    newFlowItem.CreatedTime = DateTime.Now;
                    newFlowItem.ModifiedTime = newFlowItem.CreatedTime;
                    newFlowItem.WorkFlow = entity;
                    workFlowItems.Add(newFlowItem);
                }

                entity.WorkFlowItems = workFlowItems;
                entity.Modifier = CurrentAccountId;
                entity.ModifiedTime = DateTime.Now;
                return await Task.FromResult(entity);
            });
            return await _PutRequest(model.WorkFlowId, mapping, async (data) =>
             {
                 return await Get(data.Id);
             });
        }
        #endregion

        #region DeleteWorkFlowItem 删除详细工作流程
        /// <summary>
        /// 删除详细工作流程
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("DeleteWorkFlowItem")]
        [ValidateModel]
        [ProducesResponseType(typeof(WorkFlowDTO), 200)]
        [ProducesResponseType(typeof(ValidationResultModel), 400)]
        public async Task<IActionResult> DeleteWorkFlowItem([FromBody]WorkFlowItemDeleteModel model)
        {
            var mapping = new Func<WorkFlow, Task<WorkFlow>>(async (entity) =>
            {
                var workFlowItems = entity.WorkFlowItems != null ? entity.WorkFlowItems.Where(x => x.Id != model.Id).OrderBy(x => x.FlowGrade).ToList() : new List<WorkFlowItem>();
                for (int idx = 0, len = workFlowItems.Count; idx < len; idx++)
                {
                    var item = workFlowItems[idx];
                    item.FlowGrade = idx;
                }


                entity.WorkFlowItems = workFlowItems;
                entity.Modifier = CurrentAccountId;
                entity.ModifiedTime = DateTime.Now;
                return await Task.FromResult(entity);
            });
            return await _PutRequest(model.WorkFlowId, mapping);
        }
        #endregion

        #region Delete 删除工作流信息
        /// <summary>
        /// 删除工作流信息
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

        #region BatchDelete 批量删除工作流信息
        /// <summary>
        /// 批量删除工作流信息
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