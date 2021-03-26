using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MasterTechDMO.API.Areas.Identity.Data;
using MasterTechDMO.API.Helpers;
using MasterTechDMO.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using MTSharedAccessToken.Model;
using MTSharedAccessToken.Services;

namespace MasterTechDMO.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<MTDMOContext>(options =>
                   options.UseSqlServer(
                       Configuration.GetConnectionString("MTDMOContextConnection")));

            services.AddDataProtection();

            services.AddIdentity<DMOUsers, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<MTDMOContext>()
                .AddTokenProvider<DataProtectorTokenProvider<DMOUsers>>(TokenOptions.DefaultProvider);

            services.Configure<DataProtectionTokenProviderOptions>(opt =>
            {
                opt.TokenLifespan = TimeSpan.FromHours(1);
            });

            // Create Group policy
            services.AddAuthorization(option =>
                {
                    option.AddPolicy("OnlyForOrganization", policy => policy.RequireRole("System_Admin", "Super_Admin", "Restaurant_Admin"));
                });

            var sharedTokenSettings = new SharedAccessTokenSettings();
            Configuration.Bind("JWTSettings", sharedTokenSettings);
            var tokenValidationParms = MTSharedAccessTokenService.VerifySharedTokenSettings(sharedTokenSettings);

            services.AddAuthentication(x =>
                    {
                        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

                    }).AddJwtBearer(x =>
        {
            x.RequireHttpsMetadata = false;
            x.TokenValidationParameters = tokenValidationParms;
        });

            services.AddTransient<ICipherService, CipherService>();

            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "MasterTechSolution DMO",
                    Version = "v1",
                    Description = "This API is all about DMO. How you can use our API and what parameters it takes and what will each API return",
                    Contact = new OpenApiContact
                    {
                        Name = "MasterTech Solution",
                        Email = "mastertech_dev@gmail.com",
                        Url = new Uri("https://mastertechsolution.com")
                    }
                });
            });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "MasterTechSolution DMO API");

                // To serve SwaggerUI at application's root page, set the RoutePrefix property to an empty string.
                c.RoutePrefix = string.Empty;
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();
            app.UseAuthentication();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // Create role on startup of the API Projet
            IdentityRoleService identityRoleService = new IdentityRoleService(serviceProvider, null);
            identityRoleService.CreateBaseRolesAsync(Configuration.GetSection("RoleList")?.GetChildren()?.Select(x => x.Value)?.ToArray()).Wait();
        }
    }
}
