using ApiModel.Consts;
using ApiModel.Entities;
using ApiModel.Enums;
using ApiServer.Controllers.Common;
using ApiServer.Filters;
using ApiServer.Libraries;
using ApiServer.Models;
using ApiServer.Repositories;
using ApiServer.Services;
using BambooCore;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace ApiServer.Controllers
{
    [Authorize]
    [Route("/[controller]")]
    public class ProductsController : ResourceController<Product, ProductDTO>
    {
        private readonly ISettingRepository settingRepository;
        private readonly IHostingEnvironment env;

        public override int ResType => ResourceTypeConst.Product;

        #region 构造函数
        public ProductsController(IRepository<Product, ProductDTO> repository, ISettingRepository settingRepository, IHostingEnvironment env)
            : base(repository)
        {
            this.settingRepository = settingRepository;
            this.env = env;
        }
        #endregion

        #region Get 根据分页查询信息获取产品概要信息
        /// <summary>
        /// 根据分页查询信息获取产品概要信息
        /// </summary>
        /// <param name="model"></param>
        /// <param name="categoryId"></param>
        /// <param name="categoryName"></param>
        /// <param name="classify"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedData<ProductDTO>), 200)]
        public async Task<IActionResult> Get([FromQuery] PagingRequestModel model, string categoryId = "", string categoryName = "", bool classify = true)
        {
            var advanceQuery = new Func<IQueryable<Product>, Task<IQueryable<Product>>>(async (query) =>
            {
                if (classify)
                {
                    #region 根据分类Id查询
                    if (!string.IsNullOrWhiteSpace(categoryId))
                    {
                        var curCategoryTree = await _Repository._DbContext.AssetCategoryTrees.FirstOrDefaultAsync(x => x.ObjId == categoryId);
                        //如果是根节点,把所有取出,不做分类过滤
                        //if (curCategoryTree != null && curCategoryTree.LValue > 1)
                        //{
                        var categoryQ = from it in _Repository._DbContext.AssetCategoryTrees
                                        where it.NodeType == curCategoryTree.NodeType && it.OrganizationId == curCategoryTree.OrganizationId
                                        && it.LValue >= curCategoryTree.LValue && it.RValue <= curCategoryTree.RValue
                                        select it;
                        query = from it in query
                                join cat in categoryQ on it.CategoryId equals cat.ObjId
                                select it;
                        //}
                    }
                    //else
                    //{
                    //    query = query.Where(x => !string.IsNullOrWhiteSpace(x.CategoryId));
                    //}
                    #endregion

                    #region 根据分类名称查询
                    if (!string.IsNullOrWhiteSpace(categoryName))
                    {
                        var accid = AuthMan.GetAccountId(this);
                        var account = await _Repository._DbContext.Accounts.FindAsync(accid);
                        var categoryIds = _Repository._DbContext.AssetCategories.Where(x => x.Type == AppConst.S_Category_Product && x.OrganizationId == account.OrganizationId && x.Name.Contains(categoryName)).Select(x => x.Id);
                        query = query.Where(x => categoryIds.Contains(x.CategoryId));
                    }
                    #endregion
                }
                else
                {
                    query = query.Where(x => string.IsNullOrWhiteSpace(x.CategoryId));
                }
                query = query.Where(x => x.ActiveFlag == AppConst.I_DataState_Active);
                return await Task.FromResult(query);
            });

            return await _GetPagingRequest(model, null, advanceQuery);
        }
        #endregion

        #region Get 根据id获取产品信息
        /// <summary>
        /// 根据id获取产品信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProductDTO), 200)]
        public async Task<IActionResult> Get(string id)
        {
            return await _GetByIdRequest(id);
        }
        #endregion

        #region Post 新建产品信息
        /// <summary>
        /// 新建产品信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateModel]
        [ProducesResponseType(typeof(ProductDTO), 200)]
        [ProducesResponseType(typeof(ValidationResultModel), 400)]
        public async Task<IActionResult> Post([FromBody]ProductCreateModel model)
        {
            var mapping = new Func<Product, Task<Product>>(async (entity) =>
            {
                entity.Name = model.Name;
                entity.Description = model.Description;
                entity.Icon = model.IconAssetId;
                entity.Unit = model.Unit;
                entity.CategoryId = model.CategoryId;
                entity.IsPublish = model.IsPublish;
                entity.Color = model.Color;
                entity.Brand = model.Brand;
                entity.ResourceType = (int)ResourceTypeEnum.Organizational;


                #region 自动创建默认的规格
                {
                    var accid = AuthMan.GetAccountId(this);
                    var account = await _Repository._DbContext.Accounts.FirstOrDefaultAsync(x => x.Id == accid);
                    var defaultSpec = new ProductSpec();
                    defaultSpec.Id = GuidGen.NewGUID();
                    defaultSpec.Name = entity.Name;
                    defaultSpec.Product = entity;
                    defaultSpec.OrganizationId = account.OrganizationId;
                    defaultSpec.Creator = accid;
                    defaultSpec.Modifier = accid;
                    defaultSpec.CreatedTime = DateTime.UtcNow;
                    defaultSpec.ModifiedTime = DateTime.UtcNow;
                    entity.Specifications = new List<ProductSpec>() { defaultSpec };
                }
                #endregion
                return await Task.FromResult(entity);
            });
            return await _PostRequest(mapping);
        }
        #endregion

        #region Put 更新产品信息
        /// <summary>
        /// 更新产品信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [ValidateModel]
        [ProducesResponseType(typeof(ProductDTO), 200)]
        [ProducesResponseType(typeof(ValidationResultModel), 400)]
        public async Task<IActionResult> Put([FromBody]ProductEditModel model)
        {
            var mapping = new Func<Product, Task<Product>>(async (entity) =>
            {
                entity.Name = model.Name;
                entity.CategoryId = model.CategoryId;
                entity.Description = model.Description;
                entity.IsPublish = model.IsPublish;
                entity.Color = model.Color;
                entity.Brand = model.Brand;
                if (!string.IsNullOrWhiteSpace(model.IconAssetId))
                    entity.Icon = model.IconAssetId;
                entity.Unit = model.Unit;
                entity.CategoryId = model.CategoryId;

                return await Task.FromResult(entity);
            });
            return await _PutRequest(model.Id, mapping);
        }
        #endregion

        #region BulkChangeCategory 批量修改产品分类信息
        /// <summary>
        /// 批量修改产品分类信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("BulkChangeCategory")]
        [ValidateModel]
        [HttpPut]
        public async Task<IActionResult> BulkChangeCategory([FromBody]BulkChangeCategoryModel model)
        {
            if (!ModelState.IsValid)
                return new ValidationFailedResult(ModelState);
            var existCategory = await _Repository._DbContext.AssetCategories.CountAsync(x => x.Id == model.CategoryId) > 0;
            if (!existCategory)
            {
                ModelState.AddModelError("CategoryId", "对应记录不存在");
                return new ValidationFailedResult(ModelState);
            }
            var idArr = model.Ids.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            using (var transaction = _Repository._DbContext.Database.BeginTransaction())
            {
                try
                {
                    for (int idx = idArr.Length - 1; idx >= 0; idx--)
                    {
                        var id = idArr[idx];
                        var refProduct = await _Repository._DbContext.Products.FindAsync(id);
                        if (refProduct != null)
                        {
                            refProduct.CategoryId = model.CategoryId;
                            _Repository._DbContext.Products.Update(refProduct);
                        }
                        else
                        {
                            ModelState.AddModelError("ProductId", "对应记录不存在");
                            return new ValidationFailedResult(ModelState);
                        }
                    }
                    _Repository._DbContext.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
            return Ok();
        }
        #endregion

        #region ImportProductAndCategory 根据CSV批量分类产品
        /// <summary>
        /// 根据CSV批量分类产品
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [Route("ImportProductAndCategory")]
        [HttpPut]
        public async Task<IActionResult> ImportProductAndCategory(IFormFile file)
        {






            //var accid = AuthMan.GetAccountId(this);
            //var currentAcc = await _Repository._DbContext.Accounts.FindAsync(accid);
            //var psProductQuery = await _Repository._GetPermissionData(accid, DataOperateEnum.Update);
            ////var psCategoryQuery = _Repository._DbContext.AssetCategories.Where(x => x.Type == AppConst.S_Category_Product && x.ActiveFlag == AppConst.I_DataState_Active && x.OrganizationId == currentAcc.OrganizationId);
            //var importOp = new Func<ProductAndCategoryImportCSV, Task<string>>(async (data) =>
            //{
            //    var mapProductCount = await psProductQuery.Where(x => x.Name.Trim() == data.ProductName.Trim()).CountAsync();
            //    if (mapProductCount == 0)
            //        return "没有找到该产品或您没有权限修改该条数据";
            //    if (mapProductCount > 1)
            //        return "产品名称有重复,请手动分配该产品";
            //    //var mapCategoryCount = await psCategoryQuery.Where(x => x.Name == data.CategoryName).CountAsync();
            //    //if (mapCategoryCount == 0)
            //    //    return "没有找到该分类,请确认分类名称是否有误";
            //    //if (mapCategoryCount > 1)
            //    //    return "分类名称有重复,请手动分配该产品";

            //    var refProduct = await psProductQuery.Where(x => x.Name.Trim() == data.ProductName.Trim()).Include(x => x.Specifications).FirstAsync();
            //    //var refCategory = await psCategoryQuery.Where(x => x.Name.Trim() == data.CategoryName.Trim()).FirstAsync();
            //    //refProduct.CategoryId = refCategory.Id;
            //    refProduct.Description = data.Description;
            //    refProduct.Unit = data.Unit;
            //    refProduct.Brand = data.Brand;

            //    if (refProduct.Specifications != null && refProduct.Specifications.Count > 0)
            //    {
            //        var defaultSpec = refProduct.Specifications[0];
            //        defaultSpec.Price = data.Price;
            //        defaultSpec.PartnerPrice = data.PartnerPrice;
            //        defaultSpec.PurchasePrice = data.PurchasePrice;
            //    }

            //    _Repository._DbContext.Products.Update(refProduct);
            //    return await Task.FromResult(string.Empty);
            //});

            //var doneOp = new Action(async () =>
            //{
            //    await _Repository._DbContext.SaveChangesAsync();
            //});
            //return await _ImportRequest(file, importOp, doneOp);
            var fs = new MemoryStream();
            file.CopyTo(fs);
            using (var package = new ExcelPackage(fs))
            {
                var workbox = package.Workbook;
                var sheet1 = workbox.Worksheets[0];
                for (int row = 2, len = sheet1.Dimension.End.Row; row <= len; row++)
                {



                    //原始值
                    var productIdObj = sheet1.Cells[row, 1].Value;
                    var productNameObj = sheet1.Cells[row, 2].Value;
                    var productCategoryNameObj = sheet1.Cells[row, 3].Value;
                    var productDescriptionObj = sheet1.Cells[row, 4].Value;
                    var productPriceObj = sheet1.Cells[row, 5].Value;
                    var productPartnerPriceObj = sheet1.Cells[row, 6].Value;
                    var productPurchasePriceObj = sheet1.Cells[row, 7].Value;
                    var productUnitObj = sheet1.Cells[row, 8].Value;
                    var productBrandObj = sheet1.Cells[row, 9].Value;
                    var productTPIDObj = sheet1.Cells[row, 10].Value;


                    //if (productIdObj.ToString() == "8WUY8K7A5NW0JE")
                    //{

                    //}

                    //原始值转化
                    string productId = productIdObj != null ? productIdObj.ToString().Trim() : string.Empty;
                    string productName = productNameObj != null ? productNameObj.ToString().Trim() : string.Empty;
                    string productCategoryName = productCategoryNameObj != null ? productCategoryNameObj.ToString().Trim() : string.Empty;
                    string productDescription = productDescriptionObj != null ? productDescriptionObj.ToString().Trim() : string.Empty;
                    string productUnit = productUnitObj != null ? productUnitObj.ToString().Trim() : string.Empty;
                    string productBrand = productBrandObj != null ? productBrandObj.ToString().Trim() : string.Empty;
                    string productTPID = productTPIDObj != null ? productTPIDObj.ToString().Trim() : string.Empty;
                    decimal productPrice = 0;
                    decimal productPartnerPrice = 0;
                    decimal productPurchasePrice = 0;

                    ////
                    if (productPriceObj != null)
                        decimal.TryParse(productPriceObj.ToString().Trim(), out productPrice);
                    if (productPartnerPriceObj != null)
                        decimal.TryParse(productPartnerPriceObj.ToString().Trim(), out productPartnerPrice);
                    if (productPurchasePriceObj != null)
                        decimal.TryParse(productPurchasePriceObj.ToString().Trim(), out productPurchasePrice);

                    if (string.IsNullOrWhiteSpace(productId)) continue;
                    var product = _Repository._DbContext.Products.Include(x => x.Specifications).FirstOrDefault(x => x.Id == productId.ToString());
                    if (product == null) continue;
                    product.Description = productDescription;
                    product.Unit = productUnit;
                    product.Brand = productBrand;
                    product.Name = productName;
                    if (product.Specifications != null && product.Specifications.Count > 0)
                    {
                        product.Specifications[0].Name = productName;
                        product.Specifications[0].Price = productPrice;
                        product.Specifications[0].PartnerPrice = productPartnerPrice;
                        product.Specifications[0].PurchasePrice = productPurchasePrice;
                        product.Specifications[0].TPID = productTPID;
                    }

                    _Repository._DbContext.Products.Update(product);

                }
            }

            await _Repository._DbContext.SaveChangesAsync();

            return Ok();
        }
        #endregion

        #region ProductAndCategoryImportTemplate 导出批量分类产品模版
        /// <summary>
        /// 导出批量分类产品模版
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("ProductAndCategoryImportTemplate")]
        public IActionResult ProductAndCategoryImportTemplate()
        {
            var memorystream = new MemoryStream();
            using (var package = new ExcelPackage())
            {
                var workbox = package.Workbook;
                var sheet1 = workbox.Worksheets.Add("产品信息");
                sheet1.Cells[1, 1].Value = "ID";
                sheet1.Cells[1, 2].Value = "名称";
                sheet1.Cells[1, 3].Value = "分类";
                sheet1.Cells[1, 4].Value = "描述";
                sheet1.Cells[1, 5].Value = "零售价";
                sheet1.Cells[1, 6].Value = "渠道价";
                sheet1.Cells[1, 7].Value = "进货价";
                sheet1.Cells[1, 8].Value = "单位";
                sheet1.Cells[1, 9].Value = "品牌";

                package.SaveAs(memorystream);
                memorystream.Position = 0;
            }

            return new FileStreamResult(memorystream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet") { FileDownloadName = "产品导入模版.xlsx" };
        }
        #endregion

        #region ExportData 导出产品基本信息
        /// <summary>
        /// 导出产品基本信息
        /// </summary>
        /// <param name="model"></param>
        /// <param name="categoryId"></param>
        /// <param name="classify"></param>
        /// <returns></returns>
        [Route("Export")]
        [HttpGet]
        public async Task<IActionResult> ExportData([FromQuery] PagingRequestModel model, string categoryId = "", bool classify = true)
        {

            ////var advanceQuery = new Func<IQueryable<Product>, Task<IQueryable<Product>>>(async (query) =>
            ////{
            ////    if (classify)
            ////    {
            ////        if (!string.IsNullOrWhiteSpace(categoryId))
            ////        {
            ////            var curCategoryTree = await _Repository._DbContext.AssetCategoryTrees.FirstOrDefaultAsync(x => x.ObjId == categoryId);
            ////            //如果是根节点,把所有取出,不做分类过滤
            ////            //if (curCategoryTree != null && curCategoryTree.LValue > 1)
            ////            //{
            ////                var categoryQ = from it in _Repository._DbContext.AssetCategoryTrees
            ////                                where it.NodeType == curCategoryTree.NodeType && it.OrganizationId == curCategoryTree.OrganizationId
            ////                                && it.LValue >= curCategoryTree.LValue && it.RValue <= curCategoryTree.RValue
            ////                                select it;
            ////                query = from it in query
            ////                        join cat in categoryQ on it.CategoryId equals cat.ObjId
            ////                        select it;
            ////            //}
            ////        }
            ////    }
            ////    else
            ////    {
            ////        query = query.Where(x => string.IsNullOrWhiteSpace(x.CategoryId));
            ////    }
            ////    query = query.Where(x => x.ActiveFlag == AppConst.I_DataState_Active);
            ////    return query;
            ////});

            //var transMapping = new Func<ProductDTO, Task<ProductExportDataCSV>>(async (entity) =>
            //{
            //    var csData = new ProductExportDataCSV();
            //    csData.ProductName = entity.Name;
            //    csData.CategoryName = entity.CategoryName;
            //    csData.Description = entity.Description;
            //    csData.Price = entity.Price;
            //    csData.PartnerPrice = entity.PartnerPrice;
            //    csData.PurchasePrice = entity.PurchasePrice;
            //    csData.Unit = entity.Unit;
            //    csData.Brand = entity.Brand;
            //    //csData.CreatedTime = entity.CreatedTime.ToString("yyyy-MM-dd hh:mm:ss");
            //    //csData.ModifiedTime = entity.ModifiedTime.ToString("yyyy-MM-dd hh:mm:ss");
            //    //csData.Creator = entity.Creator;
            //    //csData.Modifier = entity.Modifier;
            //    return await Task.FromResult(csData);
            //});

            //return _ExportDataRequest(model, transMapping);



            var accid = AuthMan.GetAccountId(this);
            model.PageSize = int.MaxValue;
            var res = await _Repository.SimplePagedQueryAsync(model, accid);
            var result = RepositoryBase<Product, ProductDTO>.PageQueryDTOTransfer(res);

            var memorystream = new MemoryStream();
            using (var package = new ExcelPackage())
            {
                var workbox = package.Workbook;
                var sheet1 = workbox.Worksheets.Add("产品信息");
                sheet1.Cells[1, 1].Value = "ID";
                sheet1.Cells[1, 2].Value = "名称";
                sheet1.Cells[1, 3].Value = "分类";
                sheet1.Cells[1, 4].Value = "描述";
                sheet1.Cells[1, 5].Value = "零售价";
                sheet1.Cells[1, 6].Value = "渠道价";
                sheet1.Cells[1, 7].Value = "进货价";
                sheet1.Cells[1, 8].Value = "单位";
                sheet1.Cells[1, 9].Value = "品牌";
                sheet1.Cells[1, 10].Value = "品号";
                if (result != null && result.Data != null)
                {
                    for (int idx = 0, len = result.Data.Count(); idx < len; idx++)
                    {
                        var row = idx + 2;
                        var item = result.Data[idx];
                        sheet1.Cells[row, 1].Value = item.Id;
                        sheet1.Cells[row, 2].Value = item.Name;
                        sheet1.Cells[row, 3].Value = item.CategoryName;
                        sheet1.Cells[row, 4].Value = item.Description;
                        sheet1.Cells[row, 5].Value = item.Price;
                        sheet1.Cells[row, 6].Value = item.PartnerPrice;
                        sheet1.Cells[row, 7].Value = item.PurchasePrice;
                        sheet1.Cells[row, 8].Value = item.Unit;
                        sheet1.Cells[row, 9].Value = item.Brand;
                        sheet1.Cells[row, 10].Value = item.TPID;
                    }
                }
                package.SaveAs(memorystream);
                memorystream.Position = 0;
            }

            return new FileStreamResult(memorystream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet") { FileDownloadName = "产品信息.xlsx" };


            //return Ok();

        }

        #endregion

        #region  [ CSV Matedata ]
        class ProductAndCategoryImportCSV : ClassMap<ProductAndCategoryImportCSV>, ImportData
        {
            public ProductAndCategoryImportCSV()
                : base()
            {
                AutoMap();
            }
            public string ProductName { get; set; }
            public string CategoryName { get; set; }
            public string Description { get; set; }
            public decimal Price { get; set; }
            public decimal PartnerPrice { get; set; }
            public decimal PurchasePrice { get; set; }
            public string Unit { get; set; }
            public string Brand { get; set; }
            public string ErrorMsg { get; set; }
        }

        class ProductAndCategoryExportCSV : ClassMap<ProductAndCategoryImportCSV>
        {
            public ProductAndCategoryExportCSV()
                : base()
            {
                AutoMap();
            }

            public string ProductName { get; set; }
            public string CategoryName { get; set; }
            public string Description { get; set; }
            public decimal Price { get; set; }
            public decimal PartnerPrice { get; set; }
            public decimal PurchasePrice { get; set; }
            public string Unit { get; set; }
            public string Brand { get; set; }
        }

        class ProductExportDataCSV : ClassMap<ProductExportDataCSV>
        {
            public ProductExportDataCSV()
             : base()
            {
                AutoMap();
            }
            public string ProductName { get; set; }
            public string CategoryName { get; set; }
            public string Description { get; set; }
            public decimal Price { get; set; }
            public decimal PartnerPrice { get; set; }
            public decimal PurchasePrice { get; set; }
            public string Unit { get; set; }
            public string Brand { get; set; }
            //public string CreatedTime { get; set; }
            //public string ModifiedTime { get; set; }
            //public string Creator { get; set; }
            //public string Modifier { get; set; }

        }
        #endregion

        #region GetBriefById 根据Id获取产品简洁的信息
        /// <summary>
        /// 根据Id获取产品简洁的信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("Brief/{id}")]
        [ProducesResponseType(typeof(ProductDTO), 200)]
        public async Task<IActionResult> GetBriefById(string id)
        {
            var data = await _Repository._DbContext.Products.FirstOrDefaultAsync(x => x.Id == id);
            if (data == null)
                return NotFound();
            var dto = new ProductDTO();
            dto.Id = data.Id;
            dto.Name = data.Name;
            dto.Description = data.Description;
            dto.OrganizationId = data.OrganizationId;
            dto.CreatedTime = data.CreatedTime;
            dto.ModifiedTime = data.ModifiedTime;
            dto.Creator = data.Creator;
            dto.Modifier = data.Modifier;
            dto.Unit = data.Unit;
            dto.Brand = data.Brand;
            dto.CategoryId = data.CategoryId;
            if (!string.IsNullOrWhiteSpace(dto.CategoryId))
                dto.CategoryName = await _Repository._DbContext.AssetCategories.Where(x => x.Id == dto.CategoryId).Select(x => x.Name).FirstOrDefaultAsync();

            return Ok(dto);
        }
        #endregion

        #region UploadProductConfiguration 上传产品配置表
        /// <summary>
        /// 上传产品配置表
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [Route("ProductConfiguration")]
        [HttpPost]
        public async Task<IActionResult> UploadProductConfiguration(IFormFile file)
        {
            if (file == null)
                return BadRequest();

            var configPath = Path.Combine(env.WebRootPath, "upload", "产品配置.xlsx");

            using (FileStream fs = System.IO.File.Create(configPath))
            {
                file.CopyTo(fs);
                fs.Flush();
            }

            var dic = new Dictionary<string, string>();
            using (var package = new ExcelPackage(new FileInfo(configPath)))
            {
                var accessoriesSheet = package.Workbook.Worksheets.FirstOrDefault(x => x.Name == "五金配件");
                if (accessoriesSheet != null)
                {
                    var setting = new SettingsItem
                    {
                        Key = "Accessories",
                        Value = accessoriesSheet.ContentToString()
                    };
                    await settingRepository.CreateOrUpdateAsync(setting);
                }

                var panelMergeSheet = package.Workbook.Worksheets.FirstOrDefault(x => x.Name == "合并表");
                if (panelMergeSheet != null)
                {
                    var setting = new SettingsItem
                    {
                        Key = "PanelMerge",
                        Value = panelMergeSheet.ContentToString()
                    };
                    await settingRepository.CreateOrUpdateAsync(setting);
                }

                var panelProductCategorySheet = package.Workbook.Worksheets.FirstOrDefault(x => x.Name == "柜子类型");
                if (panelProductCategorySheet != null)
                {
                    var setting = new SettingsItem
                    {
                        Key = "PanelProductCategory",
                        Value = panelProductCategorySheet.ContentToString()
                    };
                    await settingRepository.CreateOrUpdateAsync(setting);
                }

                var panelProductsSheet = package.Workbook.Worksheets.FirstOrDefault(x => x.Name == "风格表");
                if (panelProductsSheet != null)
                {
                    var setting = new SettingsItem
                    {
                        Key = "PanelProducts",
                        Value = panelProductsSheet.ContentToString()
                    };
                    await settingRepository.CreateOrUpdateAsync(setting);
                }

                var panelReplaceProductsSheet = package.Workbook.Worksheets.FirstOrDefault(x => x.Name == "替换表");
                if (panelReplaceProductsSheet != null)
                {
                    var setting = new SettingsItem
                    {
                        Key = "PanelReplaceProducts",
                        Value = panelReplaceProductsSheet.ContentToString()
                    };
                    await settingRepository.CreateOrUpdateAsync(setting);
                }
            }

            return NoContent();
        }
        #endregion

    }
}