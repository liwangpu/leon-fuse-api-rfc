using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Base.API;
using IDS.API.Application.Queries.Organizations;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IDS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizationController : ControllerBase
    {
        private readonly IMediator mediator;

        #region ctor
        public OrganizationController(IMediator mediator)
        {
            this.mediator = mediator;
        }
        #endregion

        #region Get 根据分页参数获取组织信息
        /// <summary>
        /// 根据分页参数获取用户信息
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagingQueryResult<OrganizationPagingQueryDTO>), 200)]
        public async Task<IActionResult> Get([FromQuery] OrganizationPagingQueryDTO query)
        {
            var resule = await mediator.Send(query);
            return Ok(resule);
        }
        #endregion
    }
}