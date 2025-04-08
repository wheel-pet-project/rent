using Application.Ports.Kafka;
using Application.Ports.Postgres;
using Application.Ports.Postgres.Repositories;
using Infrastructure.Adapters.Kafka;
using Infrastructure.Adapters.Postgres;
using Infrastructure.Adapters.Postgres.Inbox;
using Infrastructure.Adapters.Postgres.Outbox;
using Infrastructure.Adapters.Postgres.Repositories;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Quartz;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace Api;

public static class ServiceCollectionExtensions
{
    private static readonly Configuration Configuration;

    static ServiceCollectionExtensions()
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        Configuration = environment switch
        {
            "Development" => new Configuration
            {
                ApplicationName = "Rent#" + Environment.MachineName,
                PostgresHost = Environment.GetEnvironmentVariable("POSTGRES_HOST") ?? "localhost",
                PostgresPort = int.Parse(Environment.GetEnvironmentVariable("POSTGRES_PORT") ?? "5480"),
                PostgresDatabase = Environment.GetEnvironmentVariable("POSTGRES_DB") ?? "rent_db",
                PostgresUsername = Environment.GetEnvironmentVariable("POSTGRES_USER") ?? "postgres",
                PostgresPassword = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? "password",
                BootstrapServers = (Environment.GetEnvironmentVariable("BOOTSTRAP_SERVERS") ??
                                    "localhost:9092").Split("__"),
                BookingCompletedTopic = Environment.GetEnvironmentVariable("BOOKING_COMPLETED_TOPIC") ??
                                        "booking-completed-topic",
                MongoConnectionString = Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING") ??
                                        "mongodb://carsharing:password@localhost:27017/drivinglicense?authSource=admin",
                VehicleAddedTopic = Environment.GetEnvironmentVariable("VEHICLE_ADDED_TOPIC") ?? "vehicle-added-topic",
                VehicleDeletedTopic = Environment.GetEnvironmentVariable("VEHICLE_DELETED_TOPIC") ??
                                      "vehicle-deleted-topic",
                VehicleAddingProcessedTopic = Environment.GetEnvironmentVariable("VEHICLE_ADDING_TO_RENT_PROCESSED_TOPIC") ??
                                              "vehicle-adding-to-rent-processed-topic",
                ModelCreatedTopic = Environment.GetEnvironmentVariable("MODEL_CREATED_TOPIC") ??
                                    "model-created-topic",
                ModelCategoryUpdatedTopic = Environment.GetEnvironmentVariable("MODEL_CATEGORY_UPDATED_TOPIC") ??
                                            "model-category-updated-topic"
            },
            "Production" => new Configuration
            {
                ApplicationName = "Rent#" + Environment.MachineName,
                PostgresHost = GetEnvironmentOrThrow("POSTGRES_HOST"),
                PostgresPort = int.Parse(GetEnvironmentOrThrow("POSTGRES_PORT")),
                PostgresDatabase = GetEnvironmentOrThrow("POSTGRES_DB"),
                PostgresUsername = GetEnvironmentOrThrow("POSTGRES_USER"),
                PostgresPassword = GetEnvironmentOrThrow("POSTGRES_PASSWORD"),
                BootstrapServers = GetEnvironmentOrThrow("BOOTSTRAP_SERVERS")
                    .Split("__"),
                BookingCompletedTopic = GetEnvironmentOrThrow("BOOKING_COMPLETED_TOPIC"),
                MongoConnectionString = GetEnvironmentOrThrow("MONGO_CONNECTION_STRING"),
                VehicleAddedTopic = GetEnvironmentOrThrow("VEHICLE_ADDED_TOPIC"),
                VehicleDeletedTopic = GetEnvironmentOrThrow("VEHICLE_DELETED_TOPIC"),
                VehicleAddingProcessedTopic = GetEnvironmentOrThrow("VEHICLE_ADDING_TO_RENT_PROCESSED_TOPIC"),
                ModelCreatedTopic = GetEnvironmentOrThrow("MODEL_CREATED_TOPIC"),
                ModelCategoryUpdatedTopic = GetEnvironmentOrThrow("MODEL_CATEGORY_UPDATED_TOPIC")
            },
            _ => throw new ArgumentException("Unknown environment")
        };

        return;

        string GetEnvironmentOrThrow(string environmentName)
        {
            return Environment.GetEnvironmentVariable(environmentName) ??
                   throw new ArgumentNullException(environmentName, "not exist in environment variables");
        }
    }

    public static IServiceCollection RegisterPostgresContextAndDataSource(this IServiceCollection services)
    {
        services.AddScoped<NpgsqlDataSource>(_ =>
        {
            var dataSourceBuilder = new NpgsqlDataSourceBuilder
            {
                ConnectionStringBuilder =
                {
                    ApplicationName = Configuration.ApplicationName,
                    Host = Configuration.PostgresHost,
                    Port = Configuration.PostgresPort,
                    Database = Configuration.PostgresDatabase,
                    Username = Configuration.PostgresUsername,
                    Password = Configuration.PostgresPassword,
                    BrowsableConnectionString = false
                }
            };

            return dataSourceBuilder.Build();
        });

        var serviceProvider = services.BuildServiceProvider();
        var dataSource = serviceProvider.GetRequiredService<NpgsqlDataSource>();

        services.AddDbContext<DataContext>(optionsBuilder =>
        {
            optionsBuilder.UseNpgsql(dataSource,
                npgsqlOptions => npgsqlOptions.MigrationsAssembly(typeof(DataContext).Assembly));
            optionsBuilder.EnableSensitiveDataLogging();
        });

        return services;
    }

    public static IServiceCollection RegisterMediatorAndHandlers(this IServiceCollection services)
    {
        // services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        
        return services;
    }

    // public static IServiceCollection RegisterEnumMappers(this IServiceCollection services)
    // {
    //     services.AddScoped<EnumMapper>();
    //     
    //     return services;
    // }
    
    public static IServiceCollection RegisterSerilog(this IServiceCollection services)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console(theme: AnsiConsoleTheme.Sixteen)
            .WriteTo.MongoDBBson(Configuration.MongoConnectionString,
                "logs",
                LogEventLevel.Verbose,
                50,
                TimeSpan.FromSeconds(10))
            .CreateLogger();
        services.AddSerilog();

        return services;
    }

    public static IServiceCollection RegisterRepositories(this IServiceCollection services)
    {
        services.AddTransient<IBookingRepository, BookingRepository>();
        services.AddTransient<ICustomerRepository, CustomerRepository>();
        services.AddTransient<IRentRepository, RentRepository>();
        services.AddTransient<IVehicleRepository, VehicleRepository>();
        services.AddTransient<IVehicleModelRepository, VehicleModelRepository>();
        

        return services;
    }

    public static IServiceCollection RegisterUnitOfWork(this IServiceCollection services)
    {
        services.AddTransient<IUnitOfWork, UnitOfWork>();

        return services;
    }

    public static IServiceCollection RegisterInbox(this IServiceCollection services)
    {
        services.AddTransient<IInbox, Inbox>();

        return services;
    }

    public static IServiceCollection RegisterTimeProvider(this IServiceCollection services)
    {
        services.AddSingleton<TimeProvider>(TimeProvider.System);

        return services;
    }

    public static IServiceCollection RegisterMassTransit(this IServiceCollection services)
    {
        // services.Configure<KafkaTopicsConfiguration>(cfg =>
        // {
        //     cfg.VehicleCheckedTopic = Configuration.VehicleCheckedTopic;
        //     cfg.VehicleCheckingStartedTopic = Configuration.VehicleCheckingStartedTopic;
        // });
        
        services.AddTransient<IMessageBus, KafkaProducer>();
        
        services.AddMassTransit(x =>
        {
            x.UsingInMemory();
        
            x.AddRider(rider =>
            {
                // rider.AddConsumer<BookingCreatedConsumer>();
                //
                // rider.AddProducer<string, VehicleChecked>(Configuration.VehicleCheckedTopic);
                // rider.AddProducer<string, VehicleCheckingStarted>(Configuration.VehicleCheckingStartedTopic);
                //
                // rider.UsingKafka((context, k) =>
                // {
                //     k.TopicEndpoint<BookingCreated>(Configuration.BookingCompletedTopic,
                //         "vehicle-check-consumer-group",
                //         e =>
                //         {
                //             e.EnableAutoOffsetStore = false;
                //             e.EnablePartitionEof = true;
                //             e.AutoOffsetReset = AutoOffsetReset.Earliest;
                //             e.CreateIfMissing();
                //             e.UseKillSwitch(cfg =>
                //                 cfg.SetActivationThreshold(1)
                //                     .SetRestartTimeout(TimeSpan.FromMinutes(1))
                //                     .SetTripThreshold(0.05)
                //                     .SetTrackingPeriod(TimeSpan.FromMinutes(1)));
                //             e.UseMessageRetry(retry => retry.Interval(200, TimeSpan.FromSeconds(1)));
                //             e.ConfigureConsumer<BookingCreatedConsumer>(context);
                //         });
                //     
                //     
                //     k.Host(Configuration.BootstrapServers);
                // });
            });
        });

        return services;
    }

    public static IServiceCollection RegisterInboxAndOutboxBackgroundJobs(this IServiceCollection services)
    {
        services.AddQuartz(configure =>
        {
            var outboxJobKey = new JobKey(nameof(OutboxBackgroundJob));
            configure
                .AddJob<OutboxBackgroundJob>(j => j.WithIdentity(outboxJobKey))
                .AddTrigger(trigger => trigger.ForJob(outboxJobKey)
                    .WithSimpleSchedule(scheduleBuilder => scheduleBuilder.WithIntervalInSeconds(3).RepeatForever()));

            var inboxJobKey = new JobKey(nameof(InboxBackgroundJob));
            configure
                .AddJob<InboxBackgroundJob>(j => j.WithIdentity(inboxJobKey))
                .AddTrigger(trigger => trigger.ForJob(inboxJobKey)
                    .WithSimpleSchedule(scheduleBuilder => scheduleBuilder.WithIntervalInSeconds(3).RepeatForever()));
        });

        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

        return services;
    }

    public static IServiceCollection RegisterTelemetry(this IServiceCollection services)
    {
        services.AddOpenTelemetry()
            .WithMetrics(builder =>
            {
                builder.AddPrometheusExporter();

                builder.AddMeter("Microsoft.AspNetCore.Hosting",
                    "Microsoft.AspNetCore.Server.Kestrel");
                builder.AddView("http.server.request.duration",
                    new ExplicitBucketHistogramConfiguration
                    {
                        Boundaries =
                        [
                            0, 0.005, 0.01, 0.025, 0.05,
                            0.075, 0.1, 0.25, 0.5, 0.75, 1, 2.5, 5, 7.5, 10
                        ]
                    });
            })
            .WithTracing(builder =>
            {
                builder
                    .AddGrpcCoreInstrumentation()
                    .AddAspNetCoreInstrumentation()
                    .AddNpgsql()
                    .AddQuartzInstrumentation()
                    .SetResourceBuilder(ResourceBuilder.CreateDefault()
                        .AddService("Rent"))
                    .AddSource("Rent")
                    .AddSource("MassTransit")
                    .AddJaegerExporter();
            });

        return services;
    }

    public static IServiceCollection RegisterHealthCheckV1(this IServiceCollection services)
    {
        var getConnectionString = () =>
        {
            var connectionBuilder = new NpgsqlConnectionStringBuilder
            {
                ApplicationName = Configuration.ApplicationName,
                Host = Configuration.PostgresHost,
                Port = Configuration.PostgresPort,
                Database = Configuration.PostgresDatabase,
                Username = Configuration.PostgresUsername,
                Password = Configuration.PostgresPassword,
                BrowsableConnectionString = false
            };

            return connectionBuilder.ConnectionString;
        };

        services.AddGrpcHealthChecks()
            .AddNpgSql(getConnectionString(), timeout: TimeSpan.FromSeconds(10))
            .AddKafka(cfg =>
                    cfg.BootstrapServers = Configuration.BootstrapServers[0],
                timeout: TimeSpan.FromSeconds(10));

        return services;
    }
}

internal class Configuration
{
    public required string ApplicationName { get; init; }


    // Postgres
    public required string PostgresHost { get; init; }
    public required int PostgresPort { get; init; }
    public required string PostgresDatabase { get; init; }
    public required string PostgresUsername { get; init; }
    public required string PostgresPassword { get; init; }


    // Kafka
    public required string[] BootstrapServers { get; init; }
    public required string BookingCompletedTopic { get; init; }
    public required string VehicleAddedTopic { get; init; }
    public required string VehicleDeletedTopic { get; init; }
    public required string VehicleAddingProcessedTopic { get; init; }
    public required string ModelCreatedTopic { get; init; }
    public required string ModelCategoryUpdatedTopic { get; init; }


    // Mongo
    public required string MongoConnectionString { get; init; }
}