using ApiModel.Entities;
using ApiServer.Data;
using ApiServer.Filters;
using ApiServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace ApiServer.Controllers.Asset
{
    
    [Route("/[controller]")]
    public class AssetTransferController : Controller
    {
        protected readonly ApiDbContext _Context;

        #region 构造函数
        public AssetTransferController(ApiDbContext context)
        {
            _Context = context;
        }
        #endregion

        #region Post 转移资源文件
        /// <summary>
        /// 转移资源文件
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateModel]
        [ProducesResponseType(typeof(AssetCategoryDTO), 200)]
        [ProducesResponseType(typeof(ValidationResultModel), 400)]
        public async Task<IActionResult> Post([FromBody]AssetTransferDataGroupModel model)
        {
            using (var transaction = _Context.Database.BeginTransaction())
            {
                try
                {
                    foreach (var gitem in model.Groups)
                    {
                        var accExist = (await _Context.Accounts.FirstOrDefaultAsync(x => x.Id == gitem.TargetAcc)) != null;
                        //校验用户id
                        if (!accExist)
                        {
                            ModelState.AddModelError("targetAcc", $"{gitem.TargetAcc}记录不存在");
                            throw new Exception();
                        }

                        foreach (var dic in gitem.Assets)
                        {
                            var idsArr = string.IsNullOrWhiteSpace(dic.Value) ? new string[] { } : dic.Value.Split(",", StringSplitOptions.RemoveEmptyEntries);

                            if (dic.Key.ToLower().IndexOf("file") > -1)
                            {
                                if (!string.IsNullOrWhiteSpace(dic.Value))
                                {
                                    foreach (var id in idsArr)
                                    {
                                        var it = await _Context.Files.FirstOrDefaultAsync(x => x.Id == id);
                                        if (it == null)
                                        {
                                            ModelState.AddModelError("file", $"{dic.Value}记录不存在");
                                            throw new Exception();
                                        }
                                        it.Creator = gitem.TargetAcc;
                                        _Context.Files.Update(it);
                                        await _Context.SaveChangesAsync();
                                    }
                                }
                            }
                            else if (dic.Key.ToLower().IndexOf("product") > -1)
                            {
                                if (!string.IsNullOrWhiteSpace(dic.Value))
                                {
                                    foreach (var id in idsArr)
                                    {
                                        var it = await _Context.Products.FirstOrDefaultAsync(x => x.Id == id);
                                        if (it == null)
                                        {
                                            ModelState.AddModelError("product", $"{dic.Value}记录不存在");
                                            throw new Exception();
                                        }
                                        it.Creator = gitem.TargetAcc;
                                        _Context.Products.Update(it);
                                        await _Context.SaveChangesAsync();
                                    }
                                }
                            }
                            else if (dic.Key.ToLower().IndexOf("material") > -1)
                            {
                                if (!string.IsNullOrWhiteSpace(dic.Value))
                                {
                                    foreach (var id in idsArr)
                                    {
                                        var it = await _Context.Materials.FirstOrDefaultAsync(x => x.Id == id);
                                        if (it == null)
                                        {
                                            ModelState.AddModelError("material", $"{dic.Value}记录不存在");
                                            throw new Exception();
                                        }
                                        it.Creator = gitem.TargetAcc;
                                        _Context.Materials.Update(it);
                                        await _Context.SaveChangesAsync();
                                    }
                                }
                            }
                            else { }
                        }

                    }
                    transaction.Commit();
                }
                catch (Exception)
                {
                    return new ValidationFailedResult(ModelState);
                }
            }
            return Ok();
        }
        #endregion
    }
}