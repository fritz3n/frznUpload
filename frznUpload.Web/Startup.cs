using frznUpload.Web.Server;
using frznUpload.Web.Server.Certificates;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace frznUpload.Web
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
			services.Configure<CookiePolicyOptions>(options =>
			{
				// This lambda determines whether user consent for non-essential cookies is needed for a given request.
				options.CheckConsentNeeded = context => true;
			});

			services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();

			services.AddRazorPages().AddRazorPagesOptions(options =>
			{
				options.Conventions.AuthorizeAreaFolder("Account", "/");
			});
			services.AddControllers().AddRazorRuntimeCompilation();
			services.AddDbContext<Data.Database>(options => options.UseLazyLoadingProxies().UseSqlServer(Configuration.GetConnectionString("Database")));

			services.AddTransient<DatabaseHandler>();
			services.AddTransient<UserManager>();
			services.AddSingleton<CertificateHandler>();
			services.AddHostedService<ServerService>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseBrowserLink();
				app.UseHttpsRedirection();
			}
			else
			{
				app.UseExceptionHandler("/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				//app.UseHsts();
			}

			
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();


			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute(
					name: "downloadShare",
					pattern: "/d/{shareId}",
					defaults: new { controller = "Download", action = "DownloadShare" });
				endpoints.MapControllerRoute(
					name: "downloadFile",
					pattern: "/Account/Files/d/{fileId}",
					defaults: new { controller = "Download", action = "DownloadFile" });
				endpoints.MapControllers();
				endpoints.MapRazorPages();
			});
		}
	}
}