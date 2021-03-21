using frznUpload.Web.Files;
using frznUpload.Web.Models;
using frznUpload.Web.Server;
using frznUpload.Web.Server.Certificates;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
			services.Configure<FormOptions>(x =>
			{
				x.ValueLengthLimit = int.MaxValue;
				x.MultipartBodyLengthLimit = int.MaxValue;
				x.MultipartHeadersLengthLimit = int.MaxValue;
			});

			services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
			services.AddAuthorization(options =>
			{
				options.AddPolicy("Admin", policy =>
					policy.RequireClaim(ClaimTypes.Role, UserRole.Admin.ToString()));
			});

			services.AddRazorPages().AddRazorPagesOptions(options =>
			{
				options.Conventions.AuthorizeAreaFolder("Admin", "/", "Admin");
				options.Conventions.AuthorizeAreaFolder("Account", "/");
			});

			services.AddControllers().AddRazorRuntimeCompilation();
			services.AddDbContext<Data.Database>(options => options.UseLazyLoadingProxies().UseSqlServer(Configuration.GetConnectionString("Database")));

			services.AddHttpClient();

			services.AddTransient<DatabaseHandler>();
			services.AddTransient<UserManager>();
			services.AddTransient<FileManager>();
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

				app.UseStaticFiles();
			}
			else
			{
				app.UseExceptionHandler("/Error");

				app.UseStaticFiles(new StaticFileOptions
				{
					OnPrepareResponse = ctx =>
					{
						const int durationInSeconds = 60 * 60 * 24 * 30;
						ctx.Context.Response.Headers[HeaderNames.CacheControl] =
							"public,max-age=" + durationInSeconds;
					}
				});
			}


			app.UseStaticFiles(new StaticFileOptions
			{
				OnPrepareResponse = ctx =>
				{
					const int durationInSeconds = 60 * 60 * 24;
					ctx.Context.Response.Headers[HeaderNames.CacheControl] =
						"public,max-age=" + durationInSeconds;
				}
			});

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
				endpoints.MapControllerRoute(
					name: "downloadFileAdmin",
					pattern: "/Admin/Files/d/{fileId}",
					defaults: new { controller = "Download", action = "DownloadFile" });
				endpoints.MapControllers();
				endpoints.MapRazorPages();
			});
		}
	}
}
