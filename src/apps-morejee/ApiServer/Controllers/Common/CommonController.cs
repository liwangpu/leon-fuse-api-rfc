using ApiModel;
using ApiModel.Entities;
using ApiServer.Filters;
using ApiServer.Models;
using ApiServer.Repositories;
using ApiServer.Services;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ApiServer.Controllers.Common
{
    public class CommonController<T, DTO> : Controller
         where T : class, IEntity, IDTOTransfer<DTO>, new()
           where DTO : class, IEntity, new()
    {
        protected IRepository<T, DTO> _Repository;
        //protected readonly 

        #region 构造函数
        public CommonController(IRepository<T, DTO> repository)
        {
            _Repository = repository;
        }
        #endregion

        #region _GetCurrentUserRootOrgan 获取用的用户根组织
        /// <summary>
        /// 获取用的用户根组织
        /// </summary>
        /// <returns></returns>
        protected async Task<Organization> _GetCurrentUserRootOrgan()
        {
            var accid = AuthMan.GetAccountId(this);
            var account = await _Repository._DbContext.Accounts.FirstOrDefaultAsync(x => x.Id == accid);
            if (account != null)
            {
                var rootNode = await _Repository._DbContext.PermissionTrees.FirstAsync(x => x.ObjId == account.OrganizationId);
                var organ = await _Repository._DbContext.Organizations.FirstOrDefaultAsync(x => x.Id == rootNode.RootOrganizationId);
                return organ;
            }
            return null;
        }
        #endregion

        #region _GetCurrentUserOrgan 获取当前用户的组织
        /// <summary>
        /// 获取当前用户的组织
        /// </summary>
        /// <returns></returns>
        protected async Task<Organization> _GetCurrentUserOrgan()
        {
            var accid = AuthMan.GetAccountId(this);
            var account = await _Repository._DbContext.Accounts.FirstOrDefaultAsync(x => x.Id == accid);
            if (account != null)
            {
                var organ = await _Repository._DbContext.Organizations.FirstOrDefaultAsync(x => x.Id == account.OrganizationId);
                return organ;
            }
            return null;
        }
        #endregion

        #region _GetCurrentUserOrganId 获取当前用户的组织Id
        /// <summary>
        /// 获取当前用户的组织Id
        /// </summary>
        protected async Task<string> _GetCurrentUserOrganId()
        {
            var accid = AuthMan.GetAccountId(this);
            var account = await _Repository._DbContext.Accounts.FirstOrDefaultAsync(x => x.Id == accid);
            if (account != null)
                return account.OrganizationId;
            return string.Empty;
        }
        #endregion

        #region _GetPagingRequest 根据查询参数获取分页信息
        /// <summary>
        /// 根据查询参数获取分页信息
        /// </summary>
        /// <param name="model"></param>
        /// <param name="qMapping"></param>
        /// <param name="advanceQuery"></param>
        /// <param name="literal"></param>
        /// <returns></returns>
        protected async Task<IActionResult> _GetPagingRequest(PagingRequestModel model, Action<List<string>> qMapping = null, Func<IQueryable<T>, Task<IQueryable<T>>> advanceQuery = null, Func<T, IList<T>, Task<T>> literal = null)
        {
            var accid = AuthMan.GetAccountId(this);
            var qs = new List<string>();
            qMapping?.Invoke(qs);
            if (qs.Count > 0)
            {
                var builder = new StringBuilder();
                foreach (var item in qs)
                    builder.AppendFormat(";{0}", item);
                model.Q = builder.ToString();
            }
            var result = await _Repository.SimplePagedQueryAsync(model, accid, advanceQuery);

            if (literal != null)
            {
                if (result.Data != null && result.Data.Count > 0)
                {
                    for (int idx = result.Data.Count - 1; idx >= 0; idx--)
                        result.Data[idx] = await literal(result.Data[idx], result.Data);
                }
            }
            return Ok(RepositoryBase<T, DTO>.PageQueryDTOTransfer(result));
        }
        #endregion

        #region _GetByIdRequest 根据id获取信息
        /// <summary>
        /// 根据id获取信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="handle"></param>
        /// <returns></returns>
        protected async Task<IActionResult> _GetByIdRequest(string id, Func<DTO, Task<IActionResult>> handle = null)
        {
            var accid = AuthMan.GetAccountId(this);
            var exist = await _Repository.ExistAsync(id);
            if (!exist)
                return NotFound();
            var canRead = await _Repository.CanReadAsync(accid, id);
            if (!canRead)
                return Forbid();

            var dto = await _Repository.GetByIdAsync(id);
            //如果handle不为空,由handle掌控ActionResult
            if (handle != null)
                return await handle(dto);
            return Ok(dto);
        }
        #endregion

        #region _PostRequest 处理Post请求信息
        /// <summary>
        /// 处理Post请求信息
        /// </summary>
        /// <param name="mapping"></param>
        /// <param name="handle"></param>
        /// <returns></returns>
        protected async Task<IActionResult> _PostRequest(Func<T, Task<T>> mapping, Func<T, Task<IActionResult>> handle = null)
        {
            var accid = AuthMan.GetAccountId(this);
            var metadata = new T();
            var canCreate = await _Repository.CanCreateAsync(accid);
            if (!canCreate)
                return Forbid();
            var data = await mapping(metadata);
            await _Repository.SatisfyCreateAsync(accid, data, ModelState);
            if (!ModelState.IsValid)
                return new ValidationFailedResult(ModelState);
            await _Repository.CreateAsync(accid, data);
            var dto = await _Repository.GetByIdAsync(metadata.Id);
            //如果handle不为空,由handle掌控ActionResult
            if (handle != null)
                return await handle(data);
            return Ok(dto);
        }
        #endregion

        #region _PutRequest 处理Put请求
        /// <summary>
        /// 处理Put请求
        /// </summary>
        /// <param name="id"></param>
        /// <param name="mapping"></param>
        /// <param name="handle"></param>
        /// <returns></returns>
        protected async Task<IActionResult> _PutRequest(string id, Func<T, Task<T>> mapping, Func<T, DTO, Task<IActionResult>> handle = null)
        {
            var exist = await _Repository.ExistAsync(id);
            if (!exist)
                return NotFound();
            var accid = AuthMan.GetAccountId(this);
            var permission = await _Repository.CanUpdateAsync(accid, id);
            if (!permission)
                return Forbid();
            var metadata = await _Repository._GetByIdAsync(id);
            var entity = await mapping(metadata);
            metadata.Id = id;
            await _Repository.SatisfyUpdateAsync(accid, entity, ModelState);
            if (!ModelState.IsValid)
                return new ValidationFailedResult(ModelState);
            await _Repository.UpdateAsync(accid, entity);
            var dto = await _Repository.GetByIdAsync(entity.Id);
            //如果handle不为空,由handle掌控ActionResult
            if (handle != null)
                return await handle(entity, dto);
            return Ok(dto);
        }
        #endregion

        #region _DeleteRequest 处理Delete请求
        /// <summary>
        /// 处理Delete请求
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected async Task<IActionResult> _DeleteRequest(string id)
        {
            var accid = AuthMan.GetAccountId(this);
            var exist = await _Repository.ExistAsync(id);
            if (!exist)
                return NotFound();

            var data = await _Repository._GetByIdAsync(id);
            await _Repository.SatisfyDeleteAsync(accid, data, ModelState);
            if (!ModelState.IsValid)
                return new ValidationFailedResult(ModelState);

            var canDelete = await _Repository.CanDeleteAsync(accid, id);
            if (!canDelete)
                return Forbid();
            await _Repository.DeleteAsync(accid, id);
            return Ok();
        }
        #endregion

        #region _ImportRequest 处理导入请求
        /// <summary>
        /// 处理导入请求
        /// </summary>
        /// <typeparam name="CSV"></typeparam>
        /// <param name="file"></param>
        /// <param name="importOp"></param>
        /// <param name="doneOp"></param>
        /// <returns></returns>
        protected async Task<IActionResult> _ImportRequest<CSV>(IFormFile file, Func<CSV, Task<string>> importOp, Action doneOp = null)
              where CSV : class, ImportData, new()
        {
            if (file == null)
                return BadRequest();

            var errorRecords = new List<CSV>();
            try
            {
                using (var fs = new MemoryStream())
                using (var sr = new StreamReader(fs, Encoding.UTF8))
                using (var reader = new CsvReader(sr))
                {
                    file.CopyTo(fs);
                    fs.Position = 0;
                    reader.Configuration.HeaderValidated = null;
                    reader.Configuration.MissingFieldFound = null;
                    var records = reader.GetRecords<CSV>().ToList();
                    foreach (var item in records)
                    {
                        var msg = await importOp(item);
                        if (!string.IsNullOrWhiteSpace(msg))
                        {
                            item.ErrorMsg = msg;
                            errorRecords.Add(item);
                        }
                    }

                }
                doneOp?.Invoke();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Import Data", ex.Message);
                return new ValidationFailedResult(ModelState);
            }

            if (errorRecords.Count > 0)
            {
                var ms = new MemoryStream();

                var config = new Configuration();
                config.Encoding = Encoding.UTF8;
                using (var stream = new MemoryStream())
                using (var writer = new StreamWriter(stream, Encoding.UTF8))
                using (var csv = new CsvWriter(writer, config))
                {
                    csv.WriteRecords(errorRecords);
                    writer.Flush();
                    stream.Position = 0;
                    stream.CopyTo(ms);
                    ms.Position = 0;
                }
                return File(ms, "application/octet-stream");
            }

            return Ok();
        }
        #endregion

        #region _ExportCSVTemplateRequest 处理导出模版请求
        /// <summary>
        /// 处理导出模版请求
        /// </summary>
        /// <typeparam name="CSV"></typeparam>
        /// <returns></returns>
        protected IActionResult _ExportCSVTemplateRequest<CSV>()
            where CSV : ClassMap, new()
        {
            var ms = new MemoryStream();
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream, Encoding.UTF8))
            using (var csv = new CsvWriter(writer))
            {
                csv.WriteHeader<CSV>();
                csv.NextRecord();
                writer.Flush();
                stream.Position = 0;
                stream.CopyTo(ms);
                ms.Position = 0;
            }
            return File(ms, "application/octet-stream");
        }
        #endregion

        #region _ExportDataRequest 处理导出数据请求
        /// <summary>
        /// 处理导出数据请求
        /// </summary>
        /// <typeparam name="CSV"></typeparam>
        /// <param name="model"></param>
        /// <param name="transMapping"></param>
        /// <param name="qMapping"></param>
        /// <param name="advanceQuery"></param>
        /// <returns></returns>
        protected async Task<IActionResult> _ExportDataRequest<CSV>(PagingRequestModel model, Func<DTO, Task<CSV>> transMapping, Action<List<string>> qMapping = null, Func<IQueryable<T>, Task<IQueryable<T>>> advanceQuery = null)
          where CSV : ClassMap, new()
        {
            model.Page = 0;
            model.PageSize = int.MaxValue;
            var accid = AuthMan.GetAccountId(this);
            var qs = new List<string>();
            qMapping?.Invoke(qs);
            if (qs.Count > 0)
            {
                var builder = new StringBuilder();
                foreach (var item in qs)
                    builder.AppendFormat(";{0}", item);
                model.Q = builder.ToString();
            }

            var list = new List<CSV>();
            var res = await _Repository.SimplePagedQueryAsync(model, accid, advanceQuery);
            var resource = RepositoryBase<T, DTO>.PageQueryDTOTransfer(res);
            if (resource.Data != null && resource.Data.Count > 0)
            {
                foreach (var item in resource.Data)
                {
                    var csData = await transMapping(item);
                    list.Add(csData);
                }
            }
            var ms = new MemoryStream();
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream, Encoding.UTF8))
            using (var csv = new CsvWriter(writer))
            {
                csv.WriteHeader<CSV>();
                csv.NextRecord();
                csv.WriteRecords(list);
                writer.Flush();
                stream.Position = 0;
                stream.CopyTo(ms);
                ms.Position = 0;
            }
            return File(ms, "application/octet-stream");
        }
        #endregion

        /**************** common method ****************/

        #region ExportData 根据分页查询信息导出查询数据
        ///// <summary>
        ///// 根据分页查询信息导出查询数据
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //[Route("Export")]
        //[HttpGet]
        //public virtual async Task<IActionResult> ExportData([FromQuery] PagingRequestModel model)
        //{
        //    var transMapping = new Func<T, Task<EntityExportCSV>>(async (entity) =>
        //    {
        //        var csData = new EntityExportCSV();
        //        csData.Name = entity.Name;
        //        csData.Description = entity.Description;
        //        csData.CreatedTime = entity.CreatedTime.ToString("yyyy-MM-dd hh:mm:ss");
        //        csData.ModifiedTime = entity.ModifiedTime.ToString("yyyy-MM-dd hh:mm:ss");
        //        csData.Creator = entity.Creator;
        //        csData.Modifier = entity.Modifier;
        //        return await Task.FromResult(csData);
        //    });
        //    return await _ExportDataRequest(model, transMapping);

        //}
        #endregion

        #region Delete 删除数据信息
        /// <summary>
        /// 删除用户信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public virtual async Task<IActionResult> Delete(string id)
        {
            return await _DeleteRequest(id);
        }
        #endregion

        #region BatchDelete 批量删除数据信息
        /// <summary>
        /// 批量删除数据信息
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [Route("BatchDelete")]
        [HttpDelete]
        public virtual async Task<IActionResult> BatchDelete(string ids)
        {
            if (!string.IsNullOrWhiteSpace(ids))
            {
                var accid = AuthMan.GetAccountId(this);
                var arr = ids.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                for (int idx = arr.Length - 1; idx >= 0; idx--)
                {
                    var curid = arr[idx];
                    var exist = await _Repository.ExistAsync(curid);
                    if (!exist)
                    {
                        ModelState.AddModelError("id", $"\"{curid}\"对应记录不存在");
                        return new ValidationFailedResult(ModelState);
                    }

                    var data = await _Repository._GetByIdAsync(curid);
                    await _Repository.SatisfyDeleteAsync(accid, data, ModelState);
                    if (!ModelState.IsValid)
                        return new ValidationFailedResult(ModelState);

                    var canDelete = await _Repository.CanDeleteAsync(accid, curid);
                    if (!canDelete)
                    {
                        ModelState.AddModelError("id", $"您没有权限删除\"{curid}\"信息");
                        return new ValidationFailedResult(ModelState);
                    }

                    await _Repository.DeleteAsync(accid, curid);
                }
                return Ok();
            }
            return BadRequest();
        }
        #endregion

        #region KeyWordSearchQ 基本关键词查询
        /// <summary>
        /// 基本关键词查询
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public static List<string> KeyWordSearchQ(string keyword)
        {
            var list = new List<string>();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                list.Add(string.Format("Name like {0}", keyword));
                //list.Add(string.Format("Description={0}", keyword));
            }
            return list;
        }
        #endregion

        #region interface ImportData
        /// <summary>
        /// CSV导入模型接口
        /// </summary>
        protected interface ImportData
        {
            string ErrorMsg { get; set; }
        }
        #endregion

        #region [EntityExportCSV] 默认的导出文件数据
        /// <summary>
        /// 默认的导出文件数据
        /// </summary>
        class EntityExportCSV : ClassMap<EntityExportCSV>
        {
            public EntityExportCSV()
                : base()
            {
                AutoMap();
            }

            public string Name { get; set; }
            public string Description { get; set; }
            public string CreatedTime { get; set; }
            public string ModifiedTime { get; set; }
            public string Creator { get; set; }
            public string Modifier { get; set; }
        }
        #endregion

    }
}