using APIGateway.Services.OrderService;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace APIGateway
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
            services.AddOcelot();
            services.AddHttpClient(); // Register IHttpClientFactory to make API calls
            services.AddHealthChecks(); // For health check endpoints
            services.AddTransient<IOrderService, OrderService>();

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            // Add Ocelot middleware after routing but before endpoints
            app.UseOcelot().Wait();

            app.UseEndpoints(endpoints =>
            {
                // No controllers are necessary, but if you plan to add custom endpoints in the future, they will go here
                endpoints.MapHealthChecks("/health"); // Health check endpoint for monitoring purposes
            });
        }
    }
}
