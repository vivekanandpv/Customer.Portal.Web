using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Customer.Portal.Web.Configurations;
using Customer.Portal.Web.Context;
using Customer.Portal.Web.Filters;
using Customer.Portal.Web.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Customer.Portal.Web {
    public class Startup {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration) {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services) {
            var allowedOrigins = _configuration.GetSection("AllowedOrigins").Get<string[]>();

            services.AddControllers(options => { options.Filters.Add(new GeneralExceptionHandler()); })
                .AddNewtonsoftJson(options => {
                    options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                    options.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                    options.SerializerSettings.Converters.Add(new StringEnumConverter());
                });

            services.AddDbContext<CustomerPortalContext>(options => { options.UseSqlServer(_configuration.GetConnectionString("SqlServer")); });

            services.AddCors(options => {
                options.AddPolicy("App_Cors_Policy", builder => {
                    builder
                        .WithOrigins(allowedOrigins)
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });

            services.AddAutoMapper(config => { config.AddProfile<CustomerPortalAutoMapperProfile>(); });

            services.AddScoped<IBankCustomerService, BankCustomerService>();
            services.AddScoped<IAccountService, AccountService>();

            services.AddSwaggerGen(config => { config.SwaggerDoc("v1.0.0", new OpenApiInfo { Title = "Customer Portal API Documentation" }); });

            services.Configure<AppSettings>(_configuration.GetSection("AppSettings"));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => {
                    options.TokenValidationParameters = new TokenValidationParameters {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:JwtSecret").Value)),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

            services.AddAuthorization(options => {
                options.AddPolicy("Admin", policy => policy.RequireClaim("Roles", "Admin"));
                options.AddPolicy("User", policy => policy.RequireClaim("Roles", "User", "Admin"));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(config => {
                    config.SwaggerEndpoint("/swagger/v1.0.0/swagger.json", "Customer Portal");
                });
            }

            app.UseCors("App_Cors_Policy");

            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}