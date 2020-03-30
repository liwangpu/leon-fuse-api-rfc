using IDS.Domain.AggregateModels.UserAggregate;
using IDS.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IDS.API.Infrastructure.Database
{
    public static class MigrationManager
    {
        public static IHost MigrateDatabase(this IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                using (IDSAppContext appContext = scope.ServiceProvider.GetRequiredService<IDSAppContext>())
                {
                    try
                    {
                        appContext.Database.Migrate();
                        //var identityRepository = scope.ServiceProvider.GetRequiredService<IIdentityRepository>();
                        //var options = scope.ServiceProvider.GetRequiredService<IOptions<AppConfig>>();
                        //var appConfig = options.Value;

                        //#region 创建默认用户
                        //{
                        //    var adminId = "admin";
                        //    var admin = identityRepository.FindAsync(adminId);
                        //    if (admin == null && appConfig.SoftwareProviderSettings != null)
                        //    {
                        //        //admin = new Identity(appConfig.SoftwareProviderSettings.Username, appConfig.SoftwareProviderSettings.Password, appConfig.SoftwareProviderSettings.Name, appConfig.SoftwareProviderSettings.Mail, appConfig.SoftwareProviderSettings.Phone, adminId);
                        //        //admin.InitializeId(adminId);
                        //        //await identityRepository.AddAsync(admin);
                        //    }
                        //}


                    }
                    catch (Exception ex)
                    {
                        //Log errors or do anything you think it's needed
                        throw;
                    }
                }
            }
            return host;
        }
    }
}
