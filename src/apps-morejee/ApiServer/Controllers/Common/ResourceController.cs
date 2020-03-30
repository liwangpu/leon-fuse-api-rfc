using ApiModel;
using ApiModel.Entities;
using ApiModel.Enums;
using ApiServer.Filters;
using ApiServer.Models;
using ApiServer.Repositories;
using BambooCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
namespace ApiServer.Controllers.Common
{

    public abstract class ResourceController<T, DTO> : ListableController<T, DTO>
 where T : class, IListable, ApiModel.ICloneable, IDTOTransfer<DTO>, new()
  where DTO : class, IListable, new()
    {
        public abstract int ResType { get; }

        #region 构造函数
        public ResourceController(IRepository<T, DTO> repository)
          : base(repository)
        {

        }
        #endregion

        #region EditPermission 编辑资源权限
        /// <summary> 
        /// 编辑资源权限
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("EditPermission")]
        [HttpPut]
        [ValidateModel]
        [ProducesResponseType(typeof(bool), 200)]
        public async Task<IActionResult> EditPermission([FromBody]ResPermissionEditModel model)
        {
            var details = JsonConvert.DeserializeObject<List<ResPermissionDetailModel>>(model.OrgansPermission);
            var resIdArr = model.ResIds.Split(",", StringSplitOptions.RemoveEmptyEntries).ToList();
            if (details.Count > 0 && resIdArr.Count > 0)
            {
                foreach (var organ in details)
                {
                    var opIdArr = organ.OperateIds.Split(",", StringSplitOptions.RemoveEmptyEntries).Select(x => Convert.ToInt16(x)).ToList();
                    var bUpdateOp = opIdArr.Any(x => x == (int)DataOperateEnum.Update);
                    var bDeleteOp = opIdArr.Any(x => x == (int)DataOperateEnum.Delete);
                    var bRetrieveOp = bUpdateOp || bDeleteOp || opIdArr.Any(x => x == (int)DataOperateEnum.Retrieve);
                    foreach (var resId in resIdArr)
                    {
                        var referPermission = await _Repository._DbContext.ResourcePermissions.FirstOrDefaultAsync(x => x.OrganizationId == organ.OrganId && x.ResId == resId && x.ResType == ResType);
                        if (referPermission == null)
                            referPermission = new ResourcePermission();
                        referPermission.OpRetrieve = bRetrieveOp ? 1 : 0;
                        referPermission.OpUpdate = bUpdateOp ? 1 : 0;
                        referPermission.OpDelete = bDeleteOp ? 1 : 0;
                        referPermission.ResId = resId;
                        referPermission.OrganizationId = organ.OrganId;
                        referPermission.ResType = ResType;
                        if (referPermission.Id == null)
                        {
                            referPermission.Id = GuidGen.NewGUID();
                            _Repository._DbContext.ResourcePermissions.Add(referPermission);
                        }
                        else
                        {
                            _Repository._DbContext.ResourcePermissions.Update(referPermission);
                        }

                    }
                }
                await _Repository._DbContext.SaveChangesAsync();
            }
            return Ok();
        }
        #endregion

    }
}