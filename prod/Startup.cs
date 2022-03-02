using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Runtime;
using Amazon.S3;
using BLL.Interfaces;
using BLL.Services;
using DAL.Context;
using DAL.Entities;
using DAL.HelpModels;
using DAL.Interfaces;
using DAL.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using prod.Filters;
using prod.Logger;
using prod.SignalR;
using StackExchange.Redis;
using Stripe;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prod
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var tokenValidationParameters =  new TokenValidationParameters()
            {
                RequireAudience = false,
                ValidateAudience = false,
                ValidateIssuer = false,         // TODO: Issue validation should be perofromed with the Host name the request is sent,
                                                // HTTP context is not available in scope of IssueValidator
                RequireExpirationTime = true, // min 5 min  -  
                ValidateLifetime = true,
                RequireSignedTokens = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration["AdminJwtInfo:keyString"]))
            };

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = tokenValidationParameters;
                });

            services.AddSingleton(tokenValidationParameters);

            services.Configure<JwtInfo>(Configuration.GetSection("AdminJwtInfo"));

            services.AddSignalR();

            services.AddControllers(opt => opt.CacheProfiles.Add("Caching", new CacheProfile() 
            {
                Duration = 120,
                Location = ResponseCacheLocation.Any
            }))
                .AddNewtonsoftJson(options 
                => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            services.AddCors();
            services.AddOptions();
            services.AddMemoryCache(); 
            services.AddScoped<ExceptionFilter>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<IOrderService, BLL.Services.OrderService>();
            services.AddTransient<IScreenshotService, ScreenshotService>();
            services.AddScoped<ITokenService, BLL.Services.TokenService>();
            services.AddScoped<ICacheService, BLL.Services.CacheService>();

            var server = Configuration["DBHOST"] ?? "localhost";
            var port = Configuration["DBPORT"] ?? "3306";
            var user = Configuration["DBUSER"] ?? "root";
            var password = Configuration["DBPASSWORD"] ?? "root";
            var name = Configuration["DBNAME"] ?? "ProdDb";

            services.AddDbContext<DatabaseContext>(a => a.UseMySql($"server={server};port={port};database={name};user={user};password={password}", new MySqlServerVersion("8.0.26")));

            //services.AddDbContext<DatabaseContext>(a => a.UseSqlServer(Configuration.GetConnectionString("ProdDb")));
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "prod", Version = "v1" });
            });

            //var redis = ConnectionMultiplexer.Connect("172.28.240.1");
            //services.AddScoped<IDatabase>(s => redis.GetDatabase()); 

            var credentials = new BasicAWSCredentials("AKIA2YMTTIUJCOLGDO7R", "Gms4tauK1m4UoMz+zD++8G3Jjony3fLknObK1kmk");
            var config = new AmazonDynamoDBConfig()
            {
                RegionEndpoint = Amazon.RegionEndpoint.EUNorth1
            };
            var client = new AmazonDynamoDBClient(credentials, config);
            services.AddSingleton<IAmazonDynamoDB>(client);
            services.AddSingleton<IDynamoDBContext, DynamoDBContext>();
            //
            var awsS3Client = new AmazonS3Client("AKIA2YMTTIUJCOLGDO7R", "Gms4tauK1m4UoMz+zD++8G3Jjony3fLknObK1kmk", Amazon.RegionEndpoint.EUNorth1);
            services.AddSingleton<IAmazonS3>(awsS3Client);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "prod v1"));
                app.UseCors(builder => builder.WithOrigins("http://localhost:3000").AllowAnyHeader().AllowAnyMethod().AllowCredentials());
            }
            StripeConfiguration.SetApiKey(Configuration.GetSection("Stripe")["SecretKey"]);
            loggerFactory.AddFile(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Logs", "logger.txt"));
            app.UseDefaultFiles();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCors("ClientPermission");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<ChatHub>("/hubs/chat");
                endpoints.MapControllers();
            });
        }
    }
}
