using ApiServer.Data;
using ApiServer.MiddleWares;
using ApiServer.Repositories;
using ApiServer.Services;
using BambooCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Serialization;
using System;
using System.IO;

namespace ApiServer
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //init config
            SiteConfig.Instance.Init(Configuration);
            services.AddEntityFrameworkNpgsql();


            #region PGSQL Setting
            //services.AddEntityFrameworkNpgsql();
            var connStr = Configuration["ConnectionString"];
            services.AddDbContext<ApiDbContext>(options => options.UseNpgsql(connStr));
            services.Configure<AppConfig>(Configuration);
            RepositoryRegistry.Registry(services);
            Console.WriteLine("数据库链接参数:{0}", connStr);
            #endregion


            services.AddMvc()
                .AddJsonOptions(opts =>
                {
                    // Force Camel Case to JSON
                    opts.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    //ignore Entity framework Navigation property back reference problem. Blog >> Posts. Post >> Blog. Blog.post.blog will been ignored.
                    opts.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info { Title = "Damaozhu API", Version = "v1" });
                string dirPath = System.IO.Path.GetDirectoryName(typeof(Startup).Assembly.CodeBase);
                string xmlPath = Path.Combine(AppContext.BaseDirectory, "ApiServer.xml");
                if (System.IO.File.Exists(xmlPath))
                    c.IncludeXmlComments(xmlPath);
                else
                    Console.WriteLine("xml file for swagger api doc is missing. publish version need copy it manually, check path " + xmlPath);
            });
        }

        void hostStaticFileServer(IApplicationBuilder app, IHostingEnvironment env)
        {
            Console.WriteLine("=======", env.EnvironmentName);
            app.UseMiddleware(typeof(ErrorHandlingMiddleware));
            Console.WriteLine("content root path: " + env.ContentRootPath);
            Console.WriteLine("web root path: " + env.WebRootPath);
            if (Directory.Exists(env.WebRootPath) == false)
            {
                if (string.IsNullOrEmpty(env.WebRootPath))
                {
                    env.WebRootPath = Path.Combine(env.ContentRootPath, "wwwroot");
                }
                Console.WriteLine("web root path not exist. create " + env.WebRootPath);
                Directory.CreateDirectory(env.WebRootPath);
            }
            string uploadPath = Path.Combine(env.WebRootPath, "upload");
            Console.WriteLine("uploadpath: " + uploadPath);
            if (Directory.Exists(uploadPath) == false)
            {
                Console.WriteLine("upload path not exist. create " + uploadPath);
                Directory.CreateDirectory(uploadPath);
            }

            var appBackupPath = Path.Combine(env.WebRootPath, "AppBackup");
            if (Directory.Exists(appBackupPath) == false)
                Directory.CreateDirectory(appBackupPath);


            // default wwwroot directory
            app.UseStaticFiles(new StaticFileOptions
            {
                ServeUnknownFileTypes = true,
                OnPrepareResponse = ctx =>
                {
                    if (ctx.Context.Response.Headers.ContainsKey("Content-Type") == false)
                        ctx.Context.Response.Headers.Add("Content-Type", "application/octet-stream");
                }
            });
            app.UseStaticFiles(new StaticFileOptions
            {
                ServeUnknownFileTypes = true,
                OnPrepareResponse = ctx =>
                {
                    ctx.Context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                    if (ctx.Context.Response.Headers.ContainsKey("Content-Type") == false)
                        ctx.Context.Response.Headers.Add("Content-Type", "application/octet-stream");
                },
                FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(uploadPath),
                RequestPath = "/upload"
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider, IApplicationLifetime lifetime)
        {
            var settingsOptions = serviceProvider.GetService<IOptions<AppConfig>>();
            var appConfig = settingsOptions.Value;
            var dbContext = serviceProvider.GetService<ApiDbContext>();

            hostStaticFileServer(app, env);

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            #region Swagger
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Damaozhu API V1");
            });
            #endregion

            #region App Init
            {
                var serverId = Configuration["GuidSettings:ServerId"];
                var guidSalt = Configuration["GuidSettings:GuidSalt"];
                var guidMinLen = Configuration["GuidSettings:GuidMinLen"];
                GuidGen.Init(serverId, guidSalt, guidMinLen);
            }
            #endregion

            #region Database Init
            {
                dbContext.Database.Migrate();
                DatabaseInitTool.InitDatabase(app, env, serviceProvider, dbContext);
            }
            #endregion
        }
    }
}
