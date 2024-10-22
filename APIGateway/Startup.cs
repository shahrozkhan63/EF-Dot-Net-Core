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

            services.AddHttpClient("OrderAPI", client =>
            {
                client.BaseAddress = new Uri("https://localhost:7049"); // This is the base address for Ocelot
            });

            services.AddHealthChecks(); // Health check endpoints
            services.AddTransient<IOrderService, OrderService>();
            services.AddOcelot();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context => {
                    await context.Response.WriteAsync("E-Commerce Services - API Gateway");
                });
            });

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
