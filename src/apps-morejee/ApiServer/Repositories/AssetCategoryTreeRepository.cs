
using ApiModel.Entities;
using ApiServer.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiServer.Repositories
{

    public class AssetCategoryTreeRepository : TreeRepository<AssetCategoryTree>, ITreeRepository<AssetCategoryTree>
    {
        public AssetCategoryTreeRepository(ApiDbContext context)
            : base(context)
        {

        }

        #region GetFlatCategory 获取扁平结构的类别信息
        /// <summary>
        /// 获取扁平结构的类别信息
        /// </summary>
        /// <param name="type"></param>
        /// <param name="organId"></param>
        /// <returns></returns>
        public async Task<List<AssetCategoryDTO>> GetFlatCategory(string type, string organId)
        {
            var categoryQ = from cat in _DbContext.Set<AssetCategory>()
                            where cat.OrganizationId == organId && cat.Type == type
                            select cat;
            return await categoryQ.Select(x => x.ToDTO()).ToListAsync();
        }
        #endregion
    }
}
