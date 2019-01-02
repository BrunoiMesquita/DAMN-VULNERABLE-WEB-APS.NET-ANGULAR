using data.damn;
using logic.damn;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using model.damn;
using Newtonsoft.Json.Serialization;
using System;
using System.Text;
using System.Threading.Tasks;

namespace api.damn
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
            var settings = Configuration.Get<ApplicationSettings>();
            services.AddSingleton(settings);

            //When an access token is sent to the server, use these rules to validate the token.
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(settings.SigningKey)),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
                ClockSkew = TimeSpan.Zero
            };

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.ExpireTimeSpan = new TimeSpan(1000, 0, 0, 0);
                options.Cookie = new CookieBuilder()
                {
                    Name = "access",
                    HttpOnly = true,
                    SameSite = SameSiteMode.Strict
                };
                options.Events.OnRedirectToLogin = (context) =>
                {
                    context.Response.StatusCode = 401;
                    return Task.CompletedTask;
                };
                options.LoginPath = PathString.Empty;
                options.TicketDataFormat = new CustomJwtDataFormat(SecurityAlgorithms.HmacSha512, tokenValidationParameters);
            });

            services.AddMvc(options =>
            {
                options.CacheProfiles.Add("no-cache", new CacheProfile()
                {
                    NoStore = true,
                });
            })
            .AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            });

            services.AddSingleton<MongoContext>();
            services.AddSingleton<HasherLogic>();
            services.AddSingleton<ValidationLogic>();
            services.AddSingleton<TokenLogic>();
            services.AddSingleton<UserLogic>();
            services.AddSingleton<TodoLogic>();
            services.AddSingleton<UtilityLogic>();

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {

            app.UseAuthentication();
            app.UseCors("AllowAll");
            app.UseMvcWithDefaultRoute();
        }
    }
}
