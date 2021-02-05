using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MasterTechDMO.API.Areas.Identity.Data;
using MasterTechDMO.API.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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

			services.AddIdentity<DMOUsers, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
				.AddEntityFrameworkStores<MTDMOContext>()
				.AddTokenProvider<DataProtectorTokenProvider<DMOUsers>>(TokenOptions.DefaultProvider);

			// Create Group policy
			services.AddAuthorization(option =>
			{
				option.AddPolicy("OnlyForOrganization", policy => policy.RequireRole("System_Admin", "Super_Admin", "Restaurant_Admin"));
			});

			services.AddControllers();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env,IServiceProvider serviceProvider)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseHttpsRedirection();

			app.UseRouting();

			app.UseAuthorization();
			app.UseAuthentication();
			
			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});

			// Create role on startup of the API Projet
			IdentityRoleService identityRoleService = new IdentityRoleService(serviceProvider);
			identityRoleService.CreateRolesAsync(Configuration.GetSection("RoleList")?.GetChildren()?.Select(x => x.Value)?.ToArray()).Wait();
		}
	}
}
