using Apps.Base.Common;
using Apps.Base.Common.Consts;
using Apps.Base.Common.Models;
using Apps.OMS.Export.DTOs;
using Apps.OMS.Export.Models;
using Apps.OMS.Service.Contexts;
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
    /// 省市区控制器
    /// </summary>
    [AllowAnonymous]
    [Route("/[controller]")]
    [ApiController]
    public class NationalUrbanController : ControllerBase
    {
        protected AppDbContext _Context { get; }

        #region 构造函数
        public NationalUrbanController(AppDbContext context, IOptions<AppConfig> settingsOptions)
        {
            _Context = context;
        }
        #endregion

        #region Get 根据查询获取省市区信息
        /// <summary>
        /// 根据查询获取省市区信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<NationalUrbanDTO>), 200)]
        public async Task<IActionResult> Get([FromQuery]NationalUrbanQueryModel model)
        {
            //如果没有输入省市区的类型,默认查询省信息
            if (string.IsNullOrWhiteSpace(model.NationalUrbanTypes))
                model.NationalUrbanTypes = NationalUrbanTypeConst.Province;

            var query = _Context.NationalUrbans.Select(x => x);
            if (!string.IsNullOrWhiteSpace(model.Search))
                query = query.Where(x => x.Name.Contains(model.Search));
            if (!string.IsNullOrWhiteSpace(model.NationalUrbanTypes))
            {
                var typeArr = model.NationalUrbanTypes.Split(",", StringSplitOptions.RemoveEmptyEntries);
                query = query.Where(x => typeArr.Contains(x.NationalUrbanType));
            }
            if (!string.IsNullOrEmpty(model.ParentId))
                query = query.Where(x => x.ParentId == model.ParentId);

            var res = await query.SimplePaging(model.Page, model.PageSize, model.OrderBy, "Name", model.Desc); ;
            var resDto = new PagedData<NationalUrbanDTO>();
            resDto.Page = res.Page;
            resDto.Size = res.Size;
            resDto.Total = res.Total;

            if (res.Data != null && res.Data.Count > 0)
            {
                var nationsDto = new List<NationalUrbanDTO>();
                foreach (var item in res.Data)
                {
                    var dto = new NationalUrbanDTO();
                    dto.Id = item.Id;
                    dto.Name = item.Name;
                    dto.ParentId = item.ParentId;
                    dto.NationalUrbanType = item.NationalUrbanType;
                    nationsDto.Add(dto);
                }
                resDto.Data = nationsDto;
            }
            return Ok(resDto);
        }
        #endregion

        #region Get 根据Id查询省市区信息
        /// <summary>
        /// 根据Id查询省市区信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(NationalUrbanDTO), 200)]
        public async Task<IActionResult> Get(string id)
        {
            var nation = await _Context.NationalUrbans.FirstOrDefaultAsync(x => x.Id == id);
            if (nation != null)
            {
                var dto = new NationalUrbanDTO();
                dto.Id = nation.Id;
                dto.Name = nation.Name;
                dto.ParentId = nation.ParentId;
                dto.NationalUrbanType = nation.NationalUrbanType;
                var children = await _Context.NationalUrbans.Where(x => x.ParentId == nation.Id).ToListAsync();
                if (children != null)
                {
                    var childrenDto = new List<NationalUrbanDTO>();
                    foreach (var item in children)
                    {
                        var childDto = new NationalUrbanDTO();
                        childDto.Id = item.Id;
                        childDto.Name = item.Name;
                        childDto.ParentId = item.ParentId;
                        childDto.NationalUrbanType = item.NationalUrbanType;
                        childrenDto.Add(childDto);
                    }
                    dto.Children = childrenDto;
                    return Ok(dto);
                }

            }
            return NotFound();
        }
        #endregion

        #region GetNameById 根据Id获取省市区名称
        /// <summary>
        /// 根据Id获取省市区名称
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("GetNameById")]
        [ProducesResponseType(typeof(string), 200)]
        public async Task<IActionResult> GetNameById(string id)
        {
            var acc = await _Context.NationalUrbans.FirstOrDefaultAsync(x => x.Id == id);
            return Ok(acc != null ? acc.Name : string.Empty);
        }
        #endregion

        #region GetNameByIds 根据Ids获取省市区名称集合
        /// <summary>
        /// 根据Ids获取省市区名称集合
        /// </summary>
        /// <param name="ids">逗号分隔的id</param>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("GetNameByIds")]
        [ProducesResponseType(typeof(List<string>), 200)]
        public async Task<IActionResult> GetNameByIds(string ids)
        {
            ids = string.IsNullOrWhiteSpace(ids) ? string.Empty : ids;
            var names = new List<string>();
            var idArr = ids.Split(",", StringSplitOptions.RemoveEmptyEntries);
            foreach (var id in idArr)
            {
                var acc = await _Context.NationalUrbans.FirstOrDefaultAsync(x => x.Id == id);
                names.Add(acc != null ? acc.Name : string.Empty);
            }
            return Ok(names);
        }
        #endregion
    }
}