using ApiModel.Entities;
using ApiServer.Controllers.Common;
using ApiServer.Filters;
using ApiServer.Models;
using ApiServer.Repositories;
using BambooCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace ApiServer.Controllers
{
    //[Authorize]
    [Route("/[controller]")]
    public class MemberRegistryController : ListableController<MemberRegistry, MemberRegistryDTO>
    {

        #region 构造函数
        public MemberRegistryController(IRepository<MemberRegistry, MemberRegistryDTO> repository)
        : base(repository)
        {
        }
        #endregion

        #region Get 根据分页获取用户信息
        /// <summary>
        /// 根据分页获取用户信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedData<MemberRegistryDTO>), 200)]
        public async Task<IActionResult> Get([FromQuery] PagingRequestModel model)
        {

            return await _GetPagingRequest(model);
        }
        #endregion

        #region Get 根据Id获取用户信息
        /// <summary>
        /// 根据Id获取用户信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(MemberRegistryDTO), 200)]
        public async Task<IActionResult> Get(string id)
        {
            return await _GetByIdRequest(id);
        }
        #endregion

        #region Post 新建用户
        /// <summary>
        /// 新建用户
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateModel]
        [ProducesResponseType(typeof(MemberRegistryDTO), 200)]
        [ProducesResponseType(typeof(ValidationResultModel), 400)]
        public async Task<IActionResult> Post([FromBody]MemberRegistryCreateModel model)
        {
            var refMember = await _Repository._DbContext.MemberRegistries.FirstOrDefaultAsync(x => x.Mail == model.Mail.Trim());
            var member = refMember != null ? refMember : new MemberRegistry();
            if (refMember == null)
                member.Id = GuidGen.NewGUID();
            member.Name = model.Name;
            member.Description = model.Remark;
            member.Phone = model.Phone;
            member.Mail = model.Mail;
            member.Company = model.Company;
            member.Province = model.Province;
            member.City = model.City;
            member.Area = model.Area;
            member.Inviter = model.Inviter;
            if (refMember == null)
                member.CreatedTime = DateTime.Now;
            member.ModifiedTime = DateTime.Now;
            if (!string.IsNullOrWhiteSpace(member.Inviter))
            {
                var inviter = await _Repository._DbContext.Accounts.FirstOrDefaultAsync(x => x.Id == member.Inviter);
                if (inviter != null)
                    member.OrganizationId = inviter.OrganizationId;
            }
            if (refMember == null)
                _Repository._DbContext.MemberRegistries.Add(member);
            else
                _Repository._DbContext.MemberRegistries.Update(member);
            await _Repository._DbContext.SaveChangesAsync();
            return Ok();
        }
        #endregion
    }
}