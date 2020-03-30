using ApiModel.Entities;
using ApiModel.Enums;
using ApiModel.Extension;
using ApiServer.Data;
using ApiServer.Services;
using BambooCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace ApiServer.Repositories
{
    public class MediaRepository : ListableRepository<Media, MediaDTO>
    {
        public AppConfig appConfig { get; }
        public MediaRepository(ApiDbContext context, ITreeRepository<PermissionTree> permissionTreeRep, IOptions<AppConfig> settingsOptions)
            : base(context, permissionTreeRep)
        {
            appConfig = settingsOptions.Value;
        }

        public override ResourceTypeEnum ResourceTypeSetting
        {
            get
            {
                return ResourceTypeEnum.Personal;
            }
        }

        #region override GetByIdAsync 根据Id返回实体DTO数据信息
        /// <summary>
        /// 根据Id返回实体DTO数据信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override async Task<MediaDTO> GetByIdAsync(string id)
        {
            var data = await _DbContext.Medias.Include(x => x.MediaShareResources).FirstOrDefaultAsync(x => x.Id == id);

            if (!string.IsNullOrWhiteSpace(data.Icon))
            {
                var fs = await _DbContext.Files.FirstOrDefaultAsync(x => x.Id == data.Icon);
                if (fs != null)
                {
                    data.IconFileAssetUrl = fs.Url;
                }
            }
            if (!string.IsNullOrWhiteSpace(data.FileAssetId))
            {
                var fs = await _DbContext.Files.FirstOrDefaultAsync(x => x.Id == data.FileAssetId);
                if (fs != null)
                {
                    data.FileAssetUrl = fs.Url;
                }
            }

            data.Server = appConfig.Plugins.MediaShare;

            return data.ToDTO();
        }
        #endregion

        public override async Task CreateAsync(string accid, Media data)
        {
            await base.CreateAsync(accid, data);
            var currentAcc = await _DbContext.Accounts.FindAsync(accid);
            #region 创建默认分享
            var defaultShare = new MediaShareResource();
            defaultShare.Media = data;
            defaultShare.MediaId = data.Id;
            defaultShare.Id = GuidGen.NewGUID();
            defaultShare.Name = "默认分享";
            defaultShare.StartShareTimeStamp = DateTime.UtcNow.ReferUnixTimestampFromDateTime();
            defaultShare.StopShareTimeStamp = DateTime.UtcNow.AddDays(2).ReferUnixTimestampFromDateTime();
            defaultShare.Creator = accid;
            defaultShare.Modifier = accid;
            defaultShare.OrganizationId = currentAcc.OrganizationId;
            defaultShare.CreatedTime = DateTime.UtcNow;
            defaultShare.ModifiedTime = DateTime.UtcNow;
            _DbContext.MediaShareResources.Add(defaultShare);
            await _DbContext.SaveChangesAsync();
            #endregion
        }
    }
}
