using FluentValidation;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RatingSystem.Application;
using RatingSystem.Application.CommandHandlers;
using RatingSystem.Application.Queries;
using RatingSystem.Data;
using RatingSystem.ExternalService;
using RatingSystem.PublishedLanguage.Commands;
using RatingSystem.PublishedLanguage.Events;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace RatingSystem
{
    class Program
    {
        static IConfiguration Configuration;
        static async Task Main(string[] args)
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            // setup
            var services = new ServiceCollection();

            var source = new CancellationTokenSource();
            var cancellationToken = source.Token;
            services.RegisterBusinessServices(Configuration);
            services.AddRatingDataAccess(Configuration);

            services.Scan(scan => scan
                .FromAssemblyOf<GetAvgRating>()
                .AddClasses(classes => classes.AssignableTo<IValidator>())
                .AsImplementedInterfaces()
                .WithScopedLifetime());


            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(RequestPreProcessorBehavior<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(RequestPostProcessorBehavior<,>));
            services.AddScoped(typeof(IRequestPreProcessor<>), typeof(ValidationPreProcessor<>));

            services.AddScopedContravariant<INotificationHandler<INotification>, AllEventsHandler>(typeof(AccountMade).Assembly);

            services.AddMediatR(new[] { typeof(AddRating).Assembly, typeof(AllEventsHandler).Assembly }); // get all IRequestHandler and INotificationHandler classes

            services.AddSingleton(Configuration);

            // build
            var serviceProvider = services.BuildServiceProvider();
            var database = serviceProvider.GetRequiredService<RatingDbContext>();
            var mediator = serviceProvider.GetRequiredService<IMediator>();


            //var makeAccountDetails = new AvarageRatingCommand
            //{
            //    UniqueIdentifier = "23",
            //    AccountType = "Debit",
            //    Valuta = "Eur"
            //};


            await mediator.Send(cancellationToken);



            //var query = new Application.Queries.AddRating.Query
            //{
            //    PersonId = 1
            //};

            //var result = await mediator.Send(query, cancellationToken);


        }
    }
}
