using ApiModel.Entities;
using ApiModel.Enums;
using ApiServer.Data;
using ApiServer.Models;
using BambooCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiServer.Repositories
{
    public class ProductReplaceGroupRepository : ListableRepository<ProductReplaceGroup, ProductReplaceGroupDTO>
    {
        public ProductReplaceGroupRepository(ApiDbContext context, ITreeRepository<PermissionTree> permissionTreeRep)
          : base(context, permissionTreeRep)
        {
        }

        public override ResourceTypeEnum ResourceTypeSetting
        {
            get
            {
                return ResourceTypeEnum.Organizational;
            }
        }


        public override async Task<PagedData<ProductReplaceGroup>> SimplePagedQueryAsync(PagingRequestModel model, string accid, Func<IQueryable<ProductReplaceGroup>, Task<IQueryable<ProductReplaceGroup>>> advanceQuery = null)
        {
            var result = await base.SimplePagedQueryAsync(model, accid, advanceQuery);
            if (result.Total > 0)
            {
                for (int idx = result.Data.Count - 1; idx >= 0; idx--)
                {
                    var curData = result.Data[idx];

                    #region 产品组组成项
                    var groupItems = new List<Product>();
                    var idsArr = curData.GroupItemIds.Split(",", StringSplitOptions.RemoveEmptyEntries);
                    foreach (var productId in idsArr)
                    {
                        var product = await _DbContext.Products.FirstOrDefaultAsync(x => x.Id == productId);
                        if (product != null)
                        {
                            //默认项,取出icon信息做为展示
                            if (productId == curData.DefaultItemId)
                            {
                                if (!string.IsNullOrWhiteSpace(product.Icon))
                                {
                                    var fs = await _DbContext.Files.FirstOrDefaultAsync(x => x.Id == product.Icon);
                                    if (fs != null)
                                    {
                                        curData.IconFileAssetUrl = fs.Url;
                                        product.IconFileAssetUrl = fs.Url;
                                    }
                                }
                                curData.DefaultItem = product;
                                groupItems.Insert(0, product);
                            }
                            else
                            {
                                groupItems.Add(product);
                            }
                        }
                    }
                    curData.GroupItems = groupItems;
                    #endregion

                    #region 产品组分类信息
                    if (!string.IsNullOrWhiteSpace(curData.CategoryId))
                    {
                        var cat = await _DbContext.AssetCategories.FirstOrDefaultAsync(x => x.Id == curData.CategoryId);
                        if (cat != null)
                            curData.CategoryName = cat.Name;
                    }
                    #endregion
                }
            }
            return result;
        }


        public override async Task<ProductReplaceGroupDTO> GetByIdAsync(string id)
        {
            var data = await _DbContext.ProductReplaceGroups.FirstOrDefaultAsync(x => x.Id == id);
            if (data == null)
                return null;

            #region 产品组组成项
            var groupItems = new List<Product>();
            var idsArr = data.GroupItemIds.Split(",", StringSplitOptions.RemoveEmptyEntries);
            foreach (var productId in idsArr)
            {
                var product = await _DbContext.Products.FirstOrDefaultAsync(x => x.Id == productId);
                if (product != null)
                {
                    //默认项,取出icon信息做为展示
                    if (productId == data.DefaultItemId)
                    {
                        //if (!string.IsNullOrWhiteSpace(product.Icon))
                        //{
                        //    data.IconFileAsset = await _DbContext.Files.FindAsync(product.Icon);
                        //    product.IconFileAsset = data.IconFileAsset;
                        //}

                        if (!string.IsNullOrWhiteSpace(product.Icon))
                        {
                            var fs = await _DbContext.Files.FirstOrDefaultAsync(x => x.Id == product.Icon);
                            if (fs != null)
                            {
                                data.IconFileAssetUrl = fs.Url;
                                product.IconFileAssetUrl = fs.Url;
                            }
                        }

                        data.DefaultItem = product;
                        groupItems.Insert(0, product);
                    }
                    else
                    {
                        groupItems.Add(product);
                    }
                }
            }
            data.GroupItems = groupItems;
            #endregion

            #region 产品组分类信息
            if (!string.IsNullOrWhiteSpace(data.CategoryId))
            {
                var cat = await _DbContext.AssetCategories.FirstOrDefaultAsync(x => x.Id == data.CategoryId);
                if (cat != null)
                    data.CategoryName = cat.Name;
            }
            #endregion
            return data.ToDTO();
        }
    }
}
