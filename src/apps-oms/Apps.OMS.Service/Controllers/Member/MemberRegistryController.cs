using Apps.Base.Common;
using Apps.Base.Common.Attributes;
using Apps.Base.Common.Consts;
using Apps.Base.Common.Controllers;
using Apps.Base.Common.Interfaces;
using Apps.Base.Common.Models;
using Apps.Basic.Export.Models;
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
using System.Linq;
using System.Threading.Tasks;

namespace Apps.OMS.Service.Controllers
{
    [Authorize]
    [Route("/[controller]")]
    [ApiController]
    public class MemberRegistryController : ServiceBaseController<MemberRegistry>
    {
        protected AppConfig _AppConfig { get; }
        protected AppDbContext _Context { get; }
        protected IRepository<Member> _MemberRepository { get; }

        #region 构造函数
        public MemberRegistryController(IRepository<MemberRegistry> repository, IRepository<Member> memberRepository, AppDbContext context, IOptions<AppConfig> settingsOptions)
            : base(repository)
        {
            _Context = context;
            _MemberRepository = memberRepository;
            _AppConfig = settingsOptions.Value;
        }
        #endregion

        #region Get 根据分页获取会员邀请信息
        /// <summary>
        /// 根据分页获取会员邀请信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedData<MemberRegistryDTO>), 200)]
        public async Task<IActionResult> Get([FromQuery] PagingRequestModel model)
        {
            var accountMicroService = new AccountMicroService(_AppConfig.APIGatewayServer);
            var nationalUrbanMicroService = new NationalUrbanMicroService(_AppConfig.APIGatewayServer);

            var toDTO = new Func<MemberRegistry, Task<MemberRegistryDTO>>(async (entity) =>
            {
                var dto = new MemberRegistryDTO();
                dto.Id = entity.Id;
                dto.Name = entity.Name;
                dto.Description = entity.Description;
                dto.Creator = entity.Creator;
                dto.Modifier = entity.Modifier;
                dto.CreatedTime = entity.CreatedTime;
                dto.ModifiedTime = entity.ModifiedTime;
                dto.OrganizationId = entity.OrganizationId;
                dto.Province = entity.Province;
                dto.City = entity.City;
                dto.County = entity.County;
                dto.Phone = entity.Phone;
                dto.Mail = entity.Mail;
                dto.Company = entity.Company;
                dto.Inviter = entity.Inviter;
                dto.InviterName = await accountMicroService.GetNameById(entity.Inviter);
                await accountMicroService.GetNameByIds(entity.Creator, entity.Modifier, (creatorName, modifierName) =>
                {
                    dto.CreatorName = creatorName;
                    dto.ModifierName = modifierName;
                });
                await nationalUrbanMicroService.GetNameByIds(entity.Province, entity.City, entity.County, (provinceName, cityName, countyName) =>
                {
                    dto.ProvinceName = provinceName;
                    dto.CityName = cityName;
                    dto.CountyName = countyName;
                });
                return await Task.FromResult(dto);
            });

            var advanceQuery = new Func<IQueryable<MemberRegistry>, Task<IQueryable<MemberRegistry>>>(async (query) =>
            {
                query = query.Where(x => x.OrganizationId == CurrentAccountOrganizationId && x.IsApprove == false);
                return await Task.FromResult(query);
            });
            return await _PagingRequest(model, toDTO, advanceQuery);
        }
        #endregion

        #region Get 根据Id获取会员邀请信息
        /// <summary>
        /// 根据Id获取会员邀请信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(MemberRegistryDTO), 200)]
        public override async Task<IActionResult> Get(string id)
        {
            var accountMicroService = new AccountMicroService(_AppConfig.APIGatewayServer);
            var nationalUrbanMicroService = new NationalUrbanMicroService(_AppConfig.APIGatewayServer);

            var toDTO = new Func<MemberRegistry, Task<MemberRegistryDTO>>(async (entity) =>
            {
                var dto = new MemberRegistryDTO();
                dto.Id = entity.Id;
                dto.Name = entity.Name;
                dto.Description = entity.Description;
                dto.Creator = entity.Creator;
                dto.Modifier = entity.Modifier;
                dto.CreatedTime = entity.CreatedTime;
                dto.ModifiedTime = entity.ModifiedTime;
                dto.OrganizationId = entity.OrganizationId;
                dto.Province = entity.Province;
                dto.City = entity.City;
                dto.County = entity.County;
                dto.Phone = entity.Phone;
                dto.Mail = entity.Mail;
                dto.Company = entity.Company;
                dto.Inviter = entity.Inviter;
                dto.InviterName = await accountMicroService.GetNameById(entity.Inviter);
                await accountMicroService.GetNameByIds(entity.Creator, entity.Modifier, (creatorName, modifierName) =>
                {
                    dto.CreatorName = creatorName;
                    dto.ModifierName = modifierName;
                });
                await nationalUrbanMicroService.GetNameByIds(entity.Province, entity.City, entity.County, (provinceName, cityName, countyName) =>
                {
                    dto.ProvinceName = provinceName;
                    dto.CityName = cityName;
                    dto.CountyName = countyName;
                });
                return await Task.FromResult(dto);
            });
            return await _GetByIdRequest(id, toDTO);
        }
        #endregion

        #region Post 新建会员邀请
        /// <summary>
        /// 新建会员邀请
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [ValidateModel]
        [ProducesResponseType(typeof(MemberRegistryDTO), 200)]
        [ProducesResponseType(typeof(ValidationResultModel), 400)]
        public async Task<IActionResult> Post([FromBody]MemberRegistryCreateModel model)
        {
            var accountMicroService = new AccountMicroService(_AppConfig.APIGatewayServer);
            var inviteUser = await accountMicroService.GetById(model.Inviter);
            if (inviteUser == null)
            {
                ModelState.AddModelError("Inviter", $"邀请人{model.Inviter}记录不存在");
                return new ValidationFailedResult(ModelState);
            }

            var refMember = await _Context.MemberRegistries.FirstOrDefaultAsync(x => x.Mail == model.Mail.Trim() && x.OrganizationId == inviteUser.OrganizationId);

            var mapping = new Func<MemberRegistry, Task<MemberRegistry>>(async (entity) =>
            {
                entity.Name = model.Name;
                entity.Description = model.Remark;
                entity.ActiveFlag = AppConst.Active;
                entity.Phone = model.Phone;
                entity.Mail = model.Mail;
                entity.Company = model.Company;
                entity.Province = model.Province;
                entity.City = model.City;
                entity.County = model.County;
                entity.BusinessCard = model.BusinessCard;
                entity.Inviter = model.Inviter;
                entity.Creator = model.Inviter;
                entity.Modifier = model.Inviter;
                entity.OrganizationId = inviteUser.OrganizationId;
                return await Task.FromResult(entity);
            });
            return await _PostRequest(refMember, mapping);
        }
        #endregion

        #region Put 更新会员邀请信息
        /// <summary>
        /// 更新会员邀请信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [ValidateModel]
        [ProducesResponseType(typeof(MemberRegistryDTO), 200)]
        [ProducesResponseType(typeof(ValidationResultModel), 400)]
        public async Task<IActionResult> Put([FromBody]MemberRegistryUpdateModel model)
        {
            var mapping = new Func<MemberRegistry, Task<MemberRegistry>>(async (entity) =>
            {
                entity.Name = model.Name;
                entity.Description = model.Remark;
                entity.Phone = model.Phone;
                entity.Mail = model.Mail;
                entity.Company = model.Company;
                entity.Province = model.Province;
                entity.City = model.City;
                entity.County = model.County;
                return await Task.FromResult(entity);
            });
            return await _PutRequest(model.Id, mapping);
        }
        #endregion

        #region ApproveRegistry 审核会员注册申请
        /// <summary>
        /// 审核会员注册申请
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("ApproveRegistry")]
        [HttpPost]
        [ValidateModel]
        [ProducesResponseType(typeof(Nullable), 200)]
        public async Task<IActionResult> ApproveRegistry([FromBody]MemberRegistryApproveModel model)
        {
            var accountMicroService = new AccountMicroService(_AppConfig.APIGatewayServer, Token);
            var mapping = new Func<MemberRegistry, Task<MemberRegistry>>(async (entity) =>
            {
                entity.IsApprove = true;
                entity.Approver = CurrentAccountId;
                return await Task.FromResult(entity);
            });
            var afterUpdated = new Func<MemberRegistry, Task>(async (entity) =>
            {
                var user = new AccountCreateModel();
                user.Mail = GuidGen.NewGUID();//使用guid作为邮件,以防邮件重复创建用户失败
                user.Phone = entity.Phone;
                user.Name = entity.Name;
                user.Password = AppConst.NormalPassword;
                user.Description = entity.Description;
                user.ActivationTime = DateTime.Now;
                user.ExpireTime = DateTime.Now.AddYears(1);
                var dto = await accountMicroService.CreateAccount(user);
                //创建会员基本信息
                if (dto != null)
                {
                    var member = new Member();
                    member.AccountId = dto.Id;
                    member.Province = entity.Province;
                    member.City = entity.City;
                    member.County = entity.County;
                    member.Company = entity.Company;
                    member.BusinessCard = entity.BusinessCard;
                    member.Inviter = entity.Inviter;
                    member.Superior = entity.Inviter;
                    await _MemberRepository.CreateAsync(member, CurrentAccountId);
                }

            });
            return await _PutRequest(model.Id, mapping, afterUpdated);
        }
        #endregion
    }
}