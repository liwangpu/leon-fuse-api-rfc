using ApiModel.Entities;
using ApiModel.Enums;
using ApiServer.Data;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ApiServer.Repositories
{
    public class MediaShareRepository : ListableRepository<MediaShareResource, MediaShareResourceDTO>
    {
        public MediaShareRepository(ApiDbContext context, ITreeRepository<PermissionTree> permissionTreeRep)
            : base(context, permissionTreeRep)
        {
        }

        public override ResourceTypeEnum ResourceTypeSetting
        {
            get
            {
                return ResourceTypeEnum.NoLimit;
            }
        }

        #region SatisfyCreateAsync 判断数据是否满足存储规范
        /// <summary>
        /// 判断数据是否满足存储规范
        /// </summary>
        /// <param name="accid"></param>
        /// <param name="data"></param>
        /// <param name="modelState"></param>
        /// <returns></returns>
        public override async Task SatisfyCreateAsync(string accid, MediaShareResource data, ModelStateDictionary modelState)
        {
            await Task.FromResult(string.Empty);
        }
        #endregion

        #region SatisfyUpdateAsync 判断数据是否满足更新规范
        /// <summary>
        /// 判断数据是否满足更新规范
        /// </summary>
        /// <param name="accid"></param>
        /// <param name="data"></param>
        /// <param name="modelState"></param>
        /// <returns></returns>
        public override async Task SatisfyUpdateAsync(string accid, MediaShareResource data, ModelStateDictionary modelState)
        {
            await Task.FromResult(string.Empty);
        }
        #endregion

        #region override GetByIdAsync 根据Id返回实体DTO数据信息
        /// <summary>
        /// 根据Id返回实体DTO数据信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override async Task<MediaShareResourceDTO> GetByIdAsync(string id)
        {
            var data = await _GetByIdAsync(id);
            var media = await _DbContext.Medias.FindAsync(data.MediaId);
            data.FileAssetId = media.FileAssetId;
            data.Rotation = media.Rotation;
            data.Location = media.Location;

            if (!string.IsNullOrWhiteSpace(data.Icon))
            {
                var fs = await _DbContext.Files.FirstOrDefaultAsync(x => x.Id == media.Icon);
                if (fs != null)
                {
                    data.IconFileAssetUrl = fs.Url;
                }
            }
            if (!string.IsNullOrWhiteSpace(data.FileAssetId))
            {
                var fs = await _DbContext.Files.FirstOrDefaultAsync(x => x.Id == media.FileAssetId);
                if (fs != null)
                {
                    data.FileAssetUrl = fs.Url;
                }
            }


            return data.ToDTO();
        }
        #endregion

        #region DeleteAsync 删除实体信息
        /// <summary>
        /// 删除实体信息
        /// </summary>
        /// <param name="accid"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public override async Task DeleteAsync(string accid, string id)
        {
            var data = await _GetByIdAsync(id);
            if (data != null)
            {
                _DbContext.MediaShareResources.Remove(data);
                await _DbContext.SaveChangesAsync();
            }
        }
        #endregion
    }
}
