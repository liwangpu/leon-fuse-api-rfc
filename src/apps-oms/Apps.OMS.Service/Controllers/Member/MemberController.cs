using Apps.Base.Common;
using Apps.Base.Common.Attributes;
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
using System.Threading.Tasks;

namespace Apps.OMS.Service.Controllers
{
    [Authorize]
    [Route("/[controller]")]
    [ApiController]
    public class MemberController : ListviewController<Member>
    {
        protected override AppDbContext _Context { get; }

        #region 构造函数
        public MemberController(IRepository<Member> repository, AppDbContext context, IOptions<AppConfig> settingsOptions)
            : base(repository, settingsOptions)
        {
            _Context = context;
        }
        #endregion

        #region Get 根据分页获取会员信息
        /// <summary>
        /// 根据分页获取会员信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedData<MemberDTO>), 200)]
        public async Task<IActionResult> Get([FromQuery] PagingRequestModel model)
        {
            var accountMicroService = new AccountMicroService(_AppConfig.APIGatewayServer);
            var nationalUrbanMicroService = new NationalUrbanMicroService(_AppConfig.APIGatewayServer);

            var toDTO = new Func<Member, Task<MemberDTO>>(async (entity) =>
            {
                var dto = new MemberDTO();
                dto.Id = entity.Id;
                dto.Name = entity.Name;
                dto.Description = entity.Description;
                dto.Creator = entity.Creator;
                dto.Modifier = entity.Modifier;
                dto.CreatedTime = entity.CreatedTime;
                dto.ModifiedTime = entity.ModifiedTime;
                dto.Province = entity.Province;
                dto.City = entity.City;
                dto.County = entity.County;
                dto.Company = entity.Company;

                await nationalUrbanMicroService.GetNameByIds(entity.Province, entity.City, entity.County, (provinceName, cityName, countyName) =>
                {
                    dto.ProvinceName = provinceName;
                    dto.CityName = cityName;
                    dto.CountyName = countyName;
                });

                var account = await accountMicroService.GetById(entity.AccountId);
                if (account != null)
                {
                    dto.Name = account.Name;
                    dto.Description = account.Description;
                    dto.Phone = account.Phone;
                    dto.Mail = account.Mail;
                    dto.Icon = account.Icon;
                }

                //dto.Inviter = entity.Inviter;
                //dto.InviterName = await accountMicroService.GetNameById(entity.Inviter);
                dto.Superior = entity.Superior;
                dto.SuperiorName = await accountMicroService.GetNameById(entity.Superior);

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

        #region Get 根据Id获取会员信息
        /// <summary>
        /// 根据Id获取会员信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(MemberDTO), 200)]
        public override async Task<IActionResult> Get(string id)
        {
            var accountMicroService = new AccountMicroService(_AppConfig.APIGatewayServer);
            var nationalUrbanMicroService = new NationalUrbanMicroService(_AppConfig.APIGatewayServer);

            var toDTO = new Func<Member, Task<MemberDTO>>(async (entity) =>
            {
                var dto = new MemberDTO();
                dto.Id = entity.Id;
                dto.Name = entity.Name;
                dto.Description = entity.Description;
                dto.Creator = entity.Creator;
                dto.Modifier = entity.Modifier;
                dto.CreatedTime = entity.CreatedTime;
                dto.ModifiedTime = entity.ModifiedTime;
                dto.Province = entity.Province;
                dto.City = entity.City;
                dto.County = entity.County;
                dto.Company = entity.Company;

                await nationalUrbanMicroService.GetNameByIds(entity.Province, entity.City, entity.County, (provinceName, cityName, countyName) =>
                {
                    dto.ProvinceName = provinceName;
                    dto.CityName = cityName;
                    dto.CountyName = countyName;
                });

                var account = await accountMicroService.GetById(entity.AccountId);
                if (account != null)
                {
                    dto.Name = account.Name;
                    dto.Description = account.Description;
                    dto.Phone = account.Phone;
                    dto.Mail = account.Mail;
                    dto.Icon = account.Icon;
                }

                dto.Inviter = entity.Inviter;
                dto.InviterName = await accountMicroService.GetNameById(entity.Inviter);
                dto.Superior = entity.Superior;
                //dto.SuperiorName = await accountMicroService.GetNameById(entity.Superior);

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

        #region Put 编辑会员信息
        /// <summary>
        /// 编辑会员信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [ValidateModel]
        [ProducesResponseType(typeof(ValidationResultModel), 400)]
        public async Task<IActionResult> Put([FromBody] MemberUpdateModel model)
        {
            var mapping = new Func<Member, Task<Member>>(async (entity) =>
            {
                entity.Province = model.Province;
                entity.City = model.City;
                entity.County = model.County;
                return await Task.FromResult(entity);
            });
            return await _PutRequest(model.Id, mapping);
        }
        #endregion
    }
}