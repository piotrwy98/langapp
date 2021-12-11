using LangApp.WebApi.Api.Repositories;
using LangApp.WebApi.Api.Repositories.Db;
using LangApp.WebApi.Api.Repositories.Local;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Text;
using System.Text.Json.Serialization;

namespace LangApp.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            LangAppContext.ConnectionString = Configuration.GetConnectionString("LangApp");
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContextPool<LangAppContext>(options =>
            {
                options.UseMySql(LangAppContext.ConnectionString,
                    ServerVersion.AutoDetect(LangAppContext.ConnectionString));
            });

            services.AddScoped<IUsersRepository, DbUsersRepository>();
            services.AddScoped<ITranslationsRepository, DbTranslationsRepository>();
            services.AddScoped<ICategoriesRepository, DbCategoriesRepository>();
            services.AddScoped<IFavouriteWordsRepository, DbFavouriteWordsRepository>();
            services.AddScoped<ILanguagesRepository, DbLanguagesRepository>();
            services.AddScoped<IPartsOfSpeechRepository, DbPartsOfSpeechRepository>();
            services.AddScoped<ISessionsRepository, DbSessionsRepository>();
            services.AddScoped<ISelectedCategoriesRepository, DbSelectedCategoriesRepository>();
            services.AddScoped<IAnswersRepository, DbAnswersRepository>();
            services.AddScoped<INewsRepository, DbNewsRepository>();

            services.AddControllers(options =>
                options.SuppressAsyncSuffixInActionNames = false
            )
            .AddJsonOptions(options =>
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve
            );

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "LangApp.WebApi", Version = "v1" });
            });

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters()
                {
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWTSettings:SecretKey"])),
                    ValidateLifetime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "LangApp.WebApi v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
