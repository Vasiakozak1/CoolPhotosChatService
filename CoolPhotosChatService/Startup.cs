using CoolPhotosChatService.BL;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CoolPhotosChatService.Web
{
    public class Startup
    {
        private const string ALLOWED_CORS_ORIGINS_CONFIG_KEY = "AllowedCorsOrigins";

        private IConfiguration _configuration;

        private string[] _allowedCorsOrigins;
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
            _allowedCorsOrigins = configuration.GetSection(ALLOWED_CORS_ORIGINS_CONFIG_KEY)
                                               .GetChildren()
                                               .Select(sec => sec.Value)
                                               .ToArray();
        }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)))
                .SetApplicationName("CoolPhotosApp");

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(Constants.AUTH_SCHEME, options =>
                {
                    options.Cookie.Path = "/";
                    options.Cookie.Name = Constants.COOKIES_NAME;
                    options.DataProtectionProvider = DataProtectionProvider
                        .Create(new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)));

                    options.Events.OnRedirectToLogin = context =>
                    {
                        context.Response.StatusCode = (int)System.Net.HttpStatusCode.Unauthorized;
                        return Task.CompletedTask;
                    };
                });

            services.AddCors(options => options.AddPolicy("default"
                , config => config.WithOrigins(_allowedCorsOrigins)
                                  .AllowCredentials()
                                  .AllowAnyMethod()
                                  .AllowAnyHeader()));

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();
            app.UseCors("default");

            app.UseMvc();

        }
    }
}
