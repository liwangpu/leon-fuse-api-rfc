using ApiModel.Consts;
using ApiModel.Entities;
using ApiServer.Controllers.Common;
using ApiServer.Filters;
using ApiServer.Models;
using ApiServer.Repositories;
using ApiServer.Services;
using BambooCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiServer.Controllers
{
    
    [Route("/[controller]")]
    public class FilesController : ListableController<FileAsset, FileAssetDTO>
    {
        private IHostingEnvironment hostEnv;
        private string uploadPath;


        #region 构造函数
        public FilesController(IRepository<FileAsset, FileAssetDTO> repository, IHostingEnvironment env)
        : base(repository)
        {
            hostEnv = env;
            uploadPath = Path.Combine(hostEnv.WebRootPath, "upload");
            if (Directory.Exists(uploadPath) == false)
                Directory.CreateDirectory(uploadPath);
        }
        #endregion

        #region Get 根据分页获取文件信息
        /// <summary>
        /// 根据分页获取文件信息
        /// </summary>
        /// <param name="model"></param>
        /// <param name="fileState"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedData<FileAssetDTO>), 200)]
        public async Task<IActionResult> Get([FromQuery] PagingRequestModel model, int? fileState = null)
        {
            var advanceQuery = new Func<IQueryable<FileAsset>, Task<IQueryable<FileAsset>>>(async (query) =>
            {
                if (fileState != null)
                    query = query.Where(x => x.FileState == fileState);
                query = query.Where(x => x.ActiveFlag == AppConst.I_DataState_Active);
                return await Task.FromResult(query);
            });
            return await _GetPagingRequest(model, null, advanceQuery);
        }
        #endregion

        #region Get 根据Id获取文件信息
        /// <summary>
        /// 根据Id获取文件信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(FileAssetDTO), 200)]
        public async Task<IActionResult> Get(string id)
        {
            return await _GetByIdRequest(id);
        }
        #endregion

        /// <summary>
        /// 根据MD5获取文件信息
        /// </summary>
        /// <param name="md5"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("MD5/{md5}")]
        [ProducesResponseType(typeof(FileAssetDTO), 200)]
        public async Task<IActionResult> GetByMD5(string md5)
        {
            var dto = new FileAssetDTO();
            var file = await _Repository._DbContext.Files.FirstOrDefaultAsync(x => x.Md5 == md5.ToLower());
            if (file == null)
                return NotFound();
            dto.Id = file.Id;
            dto.Name = file.Name;
            dto.Url = file.Url;
            dto.Size = file.Size;
            dto.FileExt = file.FileExt;
            dto.LocalPath = file.LocalPath;
            dto.Icon = file.Icon;
            dto.FileState = file.FileState;
            dto.Md5 = file.Md5;
            return Ok(dto);
        }

        #region Post 创建资源文件信息
        /// <summary>
        /// 创建资源文件信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateModel]
        [ProducesResponseType(typeof(FileAssetDTO), 200)]
        [ProducesResponseType(typeof(ValidationResultModel), 400)]
        public async Task<IActionResult> Post([FromBody]FileAssetCreateModel model)
        {
            var mapping = new Func<FileAsset, Task<FileAsset>>(async (entity) =>
            {
                entity.Name = model.Name;
                entity.Icon = model.Icon;
                entity.Description = model.Description;
                entity.Url = model.Url;
                entity.Md5 = model.Md5;
                entity.Size = model.Size;
                entity.FileState = model.FileState;
                entity.FileExt = model.FileExt;
                entity.LocalPath = model.LocalPath;
                entity.CategoryId = model.CategoryId;
                return await Task.FromResult(entity);
            });
            return await _PostRequest(mapping);
        }
        #endregion

        #region Put 更新资源文件信息
        /// <summary>
        /// 更新资源文件信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [ValidateModel]
        [ProducesResponseType(typeof(FileAssetDTO), 200)]
        [ProducesResponseType(typeof(ValidationResultModel), 400)]
        public async Task<IActionResult> Put([FromBody]FileAssetEditModel model)
        {
            var mapping = new Func<FileAsset, Task<FileAsset>>(async (entity) =>
            {
                entity.Id = model.Id;
                entity.Name = model.Name;
                entity.Icon = model.Icon;
                entity.Description = model.Description;
                entity.Url = model.Url;
                entity.Md5 = model.Md5;
                entity.Size = model.Size;
                entity.FileState = model.FileState;
                entity.FileExt = model.FileExt;
                entity.LocalPath = model.LocalPath;
                entity.CategoryId = model.CategoryId;
                return await Task.FromResult(entity);
            });
            return await _PutRequest(model.Id, mapping);
        }
        #endregion

        #region Upload 上传一个文件，文件放在body中。服务器会把此文件存在upload文件夹中，并在账号上创建一个FileAsset，并返回数据给客户端。
        /// <summary> 
        /// 上传一个文件，文件放在body中。服务器会把此文件存在upload文件夹中，并在账号上创建一个FileAsset，并返回数据给客户端。
        /// 文件的内容可以在Header里面附加，也可以再次使用PUT /files API来修改此FileAsset。
        /// 在Header里面附加的内容一定要用urlencode封装一下，否则遇到中文会被框架拦截，返回500错误。
        /// </summary>
        /// <returns></returns>
        [Route("Upload")]
        [HttpPost]
        public async Task<IActionResult> Upload()
        {
            Action<string> saveFile = (path) =>
            {
                using (StreamWriter sw = new StreamWriter(path))
                {
                    HttpContext.Request.Body.CopyTo(sw.BaseStream);
                }
            };
            var accid = AuthMan.GetAccountId(this);
            var account = await _Repository._DbContext.Accounts.FindAsync(accid);
            return await SaveUpload(saveFile, account);
        }
        #endregion

        #region UploadFormFile Form表单方式上传一个文件
        /// <summary>
        /// FormData上传一个文件
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [Route("UploadFormFile")]
        [HttpPost]
        public async Task<IActionResult> UploadFormFile(IFormFile file)
        {
            if (file == null)
                return BadRequest();

            Action<string> saveFile = (path) =>
            {
                using (FileStream fs = System.IO.File.Create(path))
                {
                    file.CopyTo(fs);
                    // 清空缓冲区数据
                    fs.Flush();
                }
            };
            var accid = AuthMan.GetAccountId(this);
            var account = await _Repository._DbContext.Accounts.FindAsync(accid);

            return await SaveUpload(saveFile, account);
        }
        #endregion

        #region UploadAttachment Form表单方式上传一个附件
        /// <summary>
        /// Form表单方式上传一个附件
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("UploadAttachment")]
        [HttpPost]
        public async Task<IActionResult> UploadAttachment(IFormFile file)
        {
            if (file == null)
                return BadRequest();

            Action<string> saveFile = (path) =>
            {
                using (FileStream fs = System.IO.File.Create(path))
                {
                    file.CopyTo(fs);
                    // 清空缓冲区数据
                    fs.Flush();
                }
            };
            var account = await _Repository._DbContext.Accounts.FindAsync(AppConst.BambooAdminId);

            return await SaveUpload(saveFile, account);
        }
        #endregion

        #region private SaveUpload 真实保存上传文件过程
        /// <summary>
        /// 真实保存上传文件过程
        /// </summary>
        /// <param name="saveFile"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        private async Task<IActionResult> SaveUpload(Action<string> saveFile, Account owner)
        {
            string fileState = "";
            string fileExt = "";
            string localPath = "";
            string description = "";
            Microsoft.Extensions.Primitives.StringValues headerVar;
            Request.Headers.TryGetValue("fileState", out headerVar); if (headerVar.Count > 0) fileState = headerVar[0].Trim();
            Request.Headers.TryGetValue("fileExt", out headerVar); if (headerVar.Count > 0) fileExt = headerVar[0].Trim();
            Request.Headers.TryGetValue("localPath", out headerVar); if (headerVar.Count > 0) localPath = headerVar[0].Trim();
            Request.Headers.TryGetValue("description", out headerVar); if (headerVar.Count > 0) description = headerVar[0].Trim();


            //几个头信息decode
            if (!string.IsNullOrWhiteSpace(fileState))
                fileState = System.Web.HttpUtility.UrlDecode(fileState);
            if (!string.IsNullOrWhiteSpace(fileExt))
                fileExt = System.Web.HttpUtility.UrlDecode(fileExt);
            if (!string.IsNullOrWhiteSpace(localPath))
                localPath = System.Web.HttpUtility.UrlDecode(localPath);
            if (!string.IsNullOrWhiteSpace(description))
                description = System.Web.HttpUtility.UrlDecode(description);

            //确保扩展名以 .开头，比如.jpg
            if (fileExt.Length > 0 && fileExt[0] != '.')
                fileExt = "." + fileExt;

            FileAsset res = new FileAsset();
            res.Id = GuidGen.NewGUID(); //先生成临时ID，用于保存文件
            res.Name = localPath.Length > 0 ? Path.GetFileName(localPath) : res.Id;
            res.Url = "/upload/" + res.Id;
            int fstate = 0;
            res.FileState = int.TryParse(fileState, out fstate) ? fstate : 0;
            res.FileExt = fileExt;
            res.LocalPath = localPath;
            res.Description = description;
            res.Creator = owner.Id;
            res.Modifier = owner.Id;
            res.OrganizationId = owner.OrganizationId;


            //保存
            string savePath = Path.Combine(uploadPath, res.Id);
            try
            {
                saveFile(savePath);
            }
            catch
            {
                res.Id = "";
                res.Url = "";
                return Ok(res);
            }
            FileInfo fi = new FileInfo(savePath);
            res.Size = fi.Length;
            res.Md5 = Md5.CalcFile(savePath); //计算md5
            res.Id = res.Md5; //将ID和url改为md5
            res.Url = "/upload/" + res.Id + res.FileExt;
            string renamedPath = Path.Combine(uploadPath, res.Id + res.FileExt);

            //图片文件生成缩略图
            var createThumbnails = new Action(() =>
            {
                if (res.FileExt.Equals(".jpg", StringComparison.CurrentCultureIgnoreCase) || res.FileExt.Equals(".png", StringComparison.CurrentCultureIgnoreCase) ||
                res.FileExt.Equals(".jpeg", StringComparison.CurrentCultureIgnoreCase) || res.FileExt.Equals(".gif", StringComparison.CurrentCultureIgnoreCase)
                || res.FileExt.Equals(".bmp", StringComparison.CurrentCultureIgnoreCase)
                )
                    ImageThumbnailCreator.SaveImageThumbnails(renamedPath);
            });

            // 检查是否已经上传过此文件
            var existRecord = await _Repository._DbContext.Set<FileAsset>().FindAsync(res.Id);
            if (existRecord != null)
            {
                // 数据库记录还在，但是文件不在了，重新保存下文件。
                if (System.IO.File.Exists(renamedPath) == false)
                {
                    //重命名文件
                    System.IO.File.Move(savePath, renamedPath);
                    createThumbnails();
                }
                else
                {
                    //删除冗余文件
                    System.IO.File.Delete(savePath);

                }
                return Ok(existRecord);
            }
            else // 没有上传记录
            {
                //没上传记录，但是已经有这个文件了，先删除已有的文件，使用用户的文件覆盖
                if (System.IO.File.Exists(renamedPath))
                {
                    System.IO.File.Delete(renamedPath);
                }
                System.IO.File.Move(savePath, renamedPath); //重命名文件
                createThumbnails();
            }

            await _Repository.CreateAsync(owner.Id, res); //记录到数据库

            return Ok(res);
        }
        #endregion
    }
}
