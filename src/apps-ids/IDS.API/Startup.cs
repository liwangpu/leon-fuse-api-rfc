using AutoMapper;
using Base.API;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using IDS.API.Infrastructure.Database;
using IDS.API.Infrastructure.IdentityServer;
using IDS.Domain.AggregateModels.IdentityServerAggregate;
using IDS.Domain.AggregateModels.UserAggregate;
using IDS.Infrastructure;
using IDS.Infrastructure.Repositories;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace IDS.API
{
    public class Startup
    {

        #region properties
        public IConfiguration Configuration { get; }
        public string Database => Configuration["DatabaseSettings:Database"];
        public string DBType => Configuration["DatabaseSettings:Type"];
        public string DBServer => Configuration["DatabaseSettings:Server"];
        public string DBPort => Configuration["DatabaseSettings:Port"];
        public string DBUserId => Configuration["DatabaseSettings:UserId"];
        public string DBPassword => Configuration["DatabaseSettings:Password"];
        public string IdentityServerIssuerUri => Configuration["IdentityServer:IssuerUri"];
        #endregion

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddMediatR(Assembly.GetExecutingAssembly());

            services.AddDbContext<IDSAppContext>(options =>
            {
                var connectionString = DbConnectionStringBuilder.Build(DBType, DBServer, DBPort, Database, DBUserId, DBPassword);
                options.UseNpgsql(connectionString, sqlContextOptionsBuilder =>
                {
                    sqlContextOptionsBuilder.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                });
            });
            //services.AddSingleton<IHostedService, DBMigrationService>();
            services.Configure<AppConfig>(Configuration);
            services.AddScoped<IPersistedGrantStore, PersistedGrantStore>();
            services.AddScoped<IIdentityRepository, IdentityRepository>();
            services.AddScoped<IIdentityGrantRepository, IdentityGrantRepository>();
            services.AddScoped<IOrganizationRepository, OrganizationRepository>();
            services.AddScoped<IResourceOwnerPasswordValidator, ResourceOwnerPasswordValidator>();
            services.AddScoped<IProfileService, ProfileService>();
            services.AddPrifileContext();
            services.AddAutoMapper(typeof(Startup));
            services.AddLocalization(o => o.ResourcesPath = "Resources");

            var builder = services.AddIdentityServer(options =>
            {
                options.IssuerUri = IdentityServerIssuerUri;
            })
                 .AddInMemoryIdentityResources(Resources.GetIdentityResources())
                 .AddInMemoryApiResources(Resources.GetApiResources())
                 .AddInMemoryClients(Clients.GetClients())
                 .AddResourceOwnerValidator<ResourceOwnerPasswordValidator>()
                 .AddPersistedGrantStore<PersistedGrantStore>()
                 .AddProfileService<ProfileService>();

            builder.AddDeveloperSigningCredential();

            services.AddControllers(options =>
            {
                options.Filters.Add(typeof(HttpGlobalExceptionFilter));
            })
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                }).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseIdentityServer();
            app.UseLocalization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
