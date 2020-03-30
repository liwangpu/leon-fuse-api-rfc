using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace Gateway.API
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public string IdentityServerAuthority => Configuration["IdentityServer:Authority"];
        public string IdentityServerApiName => Configuration["IdentityServer:ApiName"];
        public string IdentityServerAPISecret => Configuration["IdentityServer:ApiSecret"];
        public bool IdentityServerRequireHttpsMetadata => Convert.ToBoolean(Configuration["IdentityServer:RequireHttpsMetadata"]);
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });

            var authenticationProviderKey = "OcelotClient";
            services.AddAuthentication()
                .AddIdentityServerAuthentication(authenticationProviderKey, o =>
                {
                    o.Authority = IdentityServerAuthority;
                    o.RequireHttpsMetadata = IdentityServerRequireHttpsMetadata;
                    o.SupportedTokens = SupportedTokens.Reference;
                    o.ApiName = IdentityServerApiName;
                    o.ApiSecret = IdentityServerAPISecret;
                });
            services.AddOcelot();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseCors("CorsPolicy");
            app.UseAuthorization();
            app.UseOcelot().Wait();
        }
    }
}
