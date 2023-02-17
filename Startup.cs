using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using SelfDrvn.Feeds.Share.Configs;
using SelfDrvn.Feeds.Share.Data;
using SelfDrvn.Feeds.Share.Services;
using SelfDrvn.Feeds.Share.Tools;
using Swashbuckle.AspNetCore.Swagger;

namespace SelfDrvn.Feeds.Share
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
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddCors(options =>
                options.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
                                ));

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddJsonOptions(
                    options => options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                );

            // configure strongly typed settings objects
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            var appSettings = appSettingsSection.Get<AppSettings>().ApplyDockerEnvironment();

            // Register the Swagger generator, defining one or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Link Share API", Version = "v1" });
                c.CustomSchemaIds(schemaIdStrategy);
                c.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    In = "header",
                    Description =
                        "This field use for Authorization header using the Bearer scheme.\nExample input: \"Bearer {token}\"",
                    Name = "Authorization",
                    Type = "apiKey"
                });
                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
                {
                    {"Bearer", Enumerable.Empty<string>()}
                });
            });

            // Authenticate with IdentityServer4
            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(authoptions =>
                {
                    authoptions.Authority = appSettings.IdentityClientSetting.IdentityAuthority;
                    authoptions.ApiName = appSettings.IdentityClientSetting.IdentityClientId;
                    authoptions.ApiSecret = appSettings.IdentityClientSetting.IdentityClientSecret;
                    authoptions.SaveToken = true;
                });

            services.AddAuthorization(auth =>
            {
                auth.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser().Build());
            });

            services.AddSingleton<IAppSettingsService, AppSettingsService>(r => new AppSettingsService(appSettings));

            services.AddTransient<IPostShareContext, PostShareContext>(r => new PostShareContext(appSettings));
        }

        // Custom Swagger Schema Generation
        private string schemaIdStrategy(Type currentClass)
        {
            string returnedValue = currentClass.Name;
            // Remove ViewModel Suffix on Swagger UI Models Information
            if (returnedValue.EndsWith("ViewModel"))
                returnedValue = returnedValue.Replace("ViewModel", string.Empty);
            return returnedValue;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // Shows UseCors with named policy.
            app.UseCors("AllowAll");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            // Enable middleware to serve generated Swagger as a JSON endpoint.  
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.  
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Link Share API V1");
            });

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseStatusCodePages();
        }
    }
}
