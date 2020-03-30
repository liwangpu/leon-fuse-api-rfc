using ApiModel.Entities;
using ApiServer.Data;
using System.Threading.Tasks;

namespace ApiServer.Repositories
{
    public interface ISettingRepository
    {
        Task<SettingsItem> GetByKey(string key);
        Task CreateOrUpdateAsync(SettingsItem data);
    }


    public class SettingRepository : ISettingRepository
    {
        public ApiDbContext Context { get; }

        public SettingRepository(ApiDbContext context)
        {
            Context = context;
        }

        public async Task CreateOrUpdateAsync(SettingsItem data)
        {
            //规范化,key为全小写
            data.Key = data.Key.ToLower();
            var entity = await Context.Set<SettingsItem>().FindAsync(data.Key);
            if (entity == null)
                Context.Set<SettingsItem>().Add(data);
            else
            {
                entity.Value = data.Value;
                Context.Set<SettingsItem>().Update(entity);
            }

            await Context.SaveChangesAsync();
        }

        public async Task<SettingsItem> GetByKey(string key)
        {
            return await Context.Set<SettingsItem>().FindAsync(key.ToLower());
        }
    }
}
