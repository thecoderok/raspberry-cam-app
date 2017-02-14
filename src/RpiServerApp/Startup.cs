using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RpiServerApp.Models;
using RpiServerApp.Services;

namespace RpiServerApp
{
    using System.IO;
    using System.Text;
    using Auth;
    using Data;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Tokens;
    using Swashbuckle.AspNetCore.Swagger;
    using WebStuff;
    using Serilog;
    using Serilog.Events;

    public class Startup
    {
        private readonly bool isDevelopmentMode = false;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets();

                isDevelopmentMode = true;
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddMvc();

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
            services.AddSingleton<IConfiguration>(Configuration);

            if (isDevelopmentMode)
            {
                services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new Info {Title = "RaspberryPi WebCam Server", Version = "v1"});
                });
            }

            services.AddScoped<ValidateMimeMultipartContentFilter>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (!isDevelopmentMode)
            {
                app.Use(async (context, next) =>
                {
                    if (context.Request.IsHttps)
                    {
                        await next();
                    }
                    else
                    {
                        var sslPortStr = ":443";
                        var httpsUrl = $"https://{context.Request.Host.Host}{sslPortStr}{context.Request.Path}";
                        context.Response.Redirect(httpsUrl);
                    }
                });
            }

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory
                .AddDebug()
                .AddSerilog();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.RollingFile(Path.Combine(Directory.GetCurrentDirectory(), "logs/rpicamapp-{Date}.txt"))
                .WriteTo.RollingFile(Path.Combine(Directory.GetCurrentDirectory(), "logs/rpicamapp-errors-{Date}.txt"), LogEventLevel.Error)
                .CreateLogger();

            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            // Add external authentication middleware below. To configure them please see http://go.microsoft.com/fwlink/?LinkID=532715
            // Add JWT generation endpoint:
            // https://stormpath.com/blog/token-authentication-asp-net-core
            var secretKey = Configuration["JwtSectedKey"];
            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));

            var options = new TokenProviderOptions
            {
                Audience = "MyHome",
                Issuer = "VMG",
                SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256),
            };
            app.UseMiddleware<TokenProviderMiddleware>(Options.Create(options));

            var tokenValidationParameters = new TokenValidationParameters
            {
                // The signing key must match!
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,

                // Validate the JWT Issuer (iss) claim
                ValidateIssuer = true,
                ValidIssuer = "VMG",

                // Validate the JWT Audience (aud) claim
                ValidateAudience = true,
                ValidAudience = "MyHome",

                // Validate the token expiry
                ValidateLifetime = true,

                // If you want to allow a certain amount of clock drift, set that here:
                ClockSkew = TimeSpan.Zero
            };

            app.UseJwtBearerAuthentication(new JwtBearerOptions
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                TokenValidationParameters = tokenValidationParameters
            });

            app.UseIdentity();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            if (isDevelopmentMode)
            {
                app.UseSwagger();
                app.UseSwaggerUi(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "RaspberryPi WebCam Server");
                });
            }
        }
    }
}
