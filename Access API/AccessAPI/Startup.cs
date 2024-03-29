using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;
using System;
using Access_API.Middleware;
using Access_API.SignalR;

namespace Access_API
{
    public class Startup
    {
        readonly string SignalRCors = "_SignalRCors";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR(options => { options.EnableDetailedErrors = true; });

            services.AddCors(options =>
            {
                options.AddPolicy(name: "UnsafeMode",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
                options.AddPolicy(name: SignalRCors,
                    builder =>
                    {
                        builder.SetIsOriginAllowed(origin => new Uri(origin).Host == "knox-master01.srv.aau.dk"
                                                             || new Uri(origin).Host == "localhost").AllowAnyHeader()
                            .AllowAnyMethod().AllowCredentials();
                    });
            });

            services.AddControllers();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "KNOX API",
                    Version = "v1",
                    Description = " This is documentation for the KNOX pipeline access API"
                });

                c.TagActionsBy(api =>
                {
                    if (api.GroupName != null)
                    {
                        return new[] { api.GroupName };
                    }

                    var controllerActionDescriptor = api.ActionDescriptor as ControllerActionDescriptor;
                    if (controllerActionDescriptor != null)
                    {
                        return new[] { controllerActionDescriptor.ControllerName };
                    }

                    throw new InvalidOperationException("Unable to determine tag for endpoint.");
                });
                c.DocInclusionPredicate((name, api) => true);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment() || env.IsProduction())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Access_API v1");
                    c.RoutePrefix = $"api/knox/swagger";
                });
            }

            app.UseRouting();

            // The logger has been commented out for now (this should not be a permanent fix) as it
            // sometimes throws strange errors during request handling. Perhaps a different logger could solve this.
            // app.UseMiddleware<MiddlewareLogger>();

            app.UseCors(SignalRCors);

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<SignalRHandler>("/suggestorHub");
            });
        }
    }
}
