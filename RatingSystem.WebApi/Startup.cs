//using Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RatingSystem.Application;
using RatingSystem.WebApi.Swagger;
using MediatR;
using RatingSystem.Application.Queries;
using RatingSystem.ExternalService;
using RatingSystem.PublishedLanguage.Events;
using MediatR.Pipeline;
using FluentValidation;
using RatingSystem.WebApi.MediatorPipeline;
using RatingSystem.WebApi.Middleware;
using RatingSystem.Application.CommandHandlers;
using RatingSystem.Data;

namespace RatingSystem.WebApi
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddMvc(o => o.EnableEndpointRouting = false);
            services.AddRatingDataAccess(Configuration);

            services.Scan(scan => scan
                .FromAssemblyOf<AddRating>()
                .AddClasses(classes => classes.AssignableTo<IValidator>())
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            services.AddMediatR(new[] { typeof(AddRating).Assembly, typeof(AllEventsHandler).Assembly }); // get all IRequestHandler and INotificationHandler classes

            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(RequestPreProcessorBehavior<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(RequestPostProcessorBehavior<,>));
            services.AddScoped(typeof(IRequestPreProcessor<>), typeof(ValidationPreProcessor<>));

            services.AddScopedContravariant<INotificationHandler<INotification>, AllEventsHandler>(typeof(AccountMade).Assembly);

            services.RegisterBusinessServices(Configuration);
            services.AddSwagger(Configuration["Identity:Authority"]);

            // NEVER USE
            //services.BuildServiceProvider(); => serviceProvider...lista de "matrite"
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IConfiguration configuration)
        {
            app.UseMiddleware<ErrorMiddleware>(); // error 
            app.UseCors(cors =>
            {
                cors
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowAnyOrigin();
            });

#pragma warning disable MVC1005 // Cannot use UseMvc with Endpoint Routing.
            app.UseMvc();
#pragma warning restore MVC1005 // Cannot use UseMvc with Endpoint Routing.

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Payment gateway Api");
                //c.OAuthClientId("CharismaFinancialServices");
                //c.OAuthScopeSeparator(" ");
                c.EnableValidator(null);
            });

            app.UseRouting();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
