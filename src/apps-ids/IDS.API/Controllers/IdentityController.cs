using Base.API;
using IDS.API.Application.Commands.Identities;
using IDS.API.Application.Queries.Identities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace IDS.API.Controllers
{
    /// <summary>
    /// 用户
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly IMediator mediator;

        #region ctor
        public IdentityController(IMediator mediator)
        {
            this.mediator = mediator;
        }
        #endregion

        #region Get 根据分页参数获取用户信息
        /// <summary>
        /// 根据分页参数获取用户信息
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagingQueryResult<IdentityPagingQueryDTO>), 200)]
        public async Task<IActionResult> Get([FromQuery] IdentityPagingQuery query)
        {
            var resule = await mediator.Send(query);
            return Ok(resule);
        }
        #endregion

        #region Get 根据Id获取用户详细信息
        /// <summary>
        /// 根据Id获取用户详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(IdentityQueryDTO), 200)]
        public async Task<IActionResult> Get(string id)
        {
            var dto = await mediator.Send(new IdentityQuery(id));
            return Ok(dto);
        }
        #endregion

        #region Post 创建用户
        /// <summary>
        /// 创建用户
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(IdentityQueryDTO), 200)]
        public async Task<IActionResult> Post([FromBody]IdentityCreateCommand command)
        {
            var id = await mediator.Send(command);
            return await Get(id);
        }
        #endregion

        #region Delete 删除用户
        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(202)]
        public async Task<IActionResult> Delete(string id)
        {
            await mediator.Send(new IdentityDeleteCommand(id));
            return NoContent();
        } 
        #endregion
    }
}