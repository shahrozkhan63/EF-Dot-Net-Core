using Alphatech.Services.ProductAPI.Models;
using Alphatech.Services.ProductAPI.Models.Dto;
using Alphatech.Services.ProductAPI.RabbitMQ;
using Alphatech.Services.ProductAPI.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using NLog.Extensions.Logging;
using RabbitMQ.Client;

namespace Alphatech.Services.ProductAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var builder = new ConfigurationBuilder()
         .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
         .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true, reloadOnChange: true);

         

            Configuration = builder.Build();
            services.Configure<RabbitMqSettings>(Configuration.GetSection("RabbitMq"));
            services.AddTransient<ProductOrderPublisherService>();
            services.AddSingleton<IConnectionFactory>(sp =>
            {
                var rabbitMqSettings = sp.GetRequiredService<IOptions<RabbitMqSettings>>().Value;
                return new ConnectionFactory()
                {
                    HostName = rabbitMqSettings.HostName,
                    Port = rabbitMqSettings.Port,
                    UserName = rabbitMqSettings.UserName,
                    Password = rabbitMqSettings.Password
                };
            });

            services.AddSingleton<IConfiguration>(Configuration);


            // Add NLog as the logging provider
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                loggingBuilder.AddNLog(Configuration);
            });


            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("Alphatech.Services.ProductAPI"));
            });


            IMapper mapper = MappingConfig.RegisterMaps().CreateMapper();
            services.AddSingleton(mapper);
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            //  services.AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new NullableTimeOnlyConverter()));

            //services.AddControllers()
            //   .AddJsonOptions(options =>
            //   {
            //       options.JsonSerializerOptions.Converters.Add(new NullableTimeOnlyConverter());
            //   });

            
            services.AddScoped<IProductRepository, ProductRepository>();    

            services.AddHealthChecks();
           

            services.AddScoped<ApplicationDbContext>();




            //  services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();

            //  services.AddApplicationInsightsTelemetry(Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]);
            services.AddMvcCore().AddApiExplorer();

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Alpha tech Swagger API Documentation",
                    Description = "APIs are managed by Sharjah Free Zone, Version v1.1."
                });
                // options.SchemaFilter<HideSchemaFilter>();

                //options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                //{
                //    Name = "Authorization",
                //    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                //    Scheme = "Bearer",
                //    BearerFormat = "JWT",
                //    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                //    Description = "JWT Authorization header using the Bearer scheme."
                //});
                //options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement {
                //{
                //    new Microsoft.OpenApi.Models.OpenApiSecurityScheme {
                //            Reference = new Microsoft.OpenApi.Models.OpenApiReference {
                //                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                //                    Id = "Bearer"
                //            }
                //        },
                //        new string[] {}
                //}
                //});
            });

            //services.AddCors(option =>
            //{
            //    var frontendUrls = Configuration.GetSection("frontend_urls").Get<string[]>();

            //    option.AddDefaultPolicy(builder =>
            //    {
            //        builder.WithOrigins(frontendUrls)
            //            .AllowAnyMethod()
            //            .AllowAnyHeader()
            //            .WithExposedHeaders(new string[] { "Authorization" });
            //    });
            //    //var frontend_URL = Configuration.GetValue<string>("frontend_url");
            //    //option.AddDefaultPolicy(builder =>
            //    //{
            //    //    builder.WithOrigins(frontend_URL).AllowAnyMethod().AllowAnyHeader().WithExposedHeaders(new string[] { "Authorization" });
            //    //});
            //});
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Alphatech v1.1"));

            app.UseHttpsRedirection();

            app.UseRouting();

            //app.UseCors();

            //app.UseResponseCaching();

           // app.UseAuthentication();

            //app.UseAuthorization();

            //app.UseStaticFiles();

            // Add this to serve static files from the custom directory
            //app.UseStaticFiles(new StaticFileOptions
            //{
            //    FileProvider = new PhysicalFileProvider(
            //        Path.Combine(env.ContentRootPath, "Assets")),
            //    RequestPath = "/Assets"
            //});

           // app.UseMiddleware<ApiLoggingMiddleware>();

            //app.UseMiddleware<JwtMiddleware>();

            //app.UseMiddleware<CancellationTokenMiddleware>();

            //app.UseMiddleware<JwtTokenValidationMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
