using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Hello
{
    class Program
    {
        private static IServiceProvider _serviceProvider;
        private static IHelloProducer _producer;

        static void Main(string[] args)
        {
            var servers = Environment.GetEnvironmentVariable("KAFKA_SERVERS") ?? "localhost:9092,localhost:9093";

            SetupStaticLogger();

            try
            {
                RegisterServices(servers, "hello-topic");

                _producer = _serviceProvider.GetService<IHelloProducer>();

                System.Timers.Timer timer = new System.Timers.Timer();
                timer.Interval = 5000;
                timer.Elapsed += timer_Elapsed;
                timer.Start();


                var consumer = _serviceProvider.GetService<IHelloConsumer>();

                while (true)
                {
                    var message = consumer.ReadMessage();

                    Log.Information(message);
                }
            }
            finally
            {
                DisposeServices();
            }
        }

        private static void SetupStaticLogger()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }

        private static void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _producer.SendMessage("Hello World");
        }

        private static void RegisterServices(string servers, string topics)
        {
            var collection = new ServiceCollection();
            var builder = new ContainerBuilder();

            builder.RegisterType<BasicProducer>().As<IHelloProducer>().WithParameters(
                new[]
                {
                    new NamedParameter("servers",  servers),
                    new NamedParameter("topics",  topics)
                }
            );
            builder.RegisterType<BasicConsumer>().As<IHelloConsumer>().WithParameters(
                new[]
                {
                    new NamedParameter("groupId", Guid.NewGuid().ToString()),
                    new NamedParameter("servers",  servers),
                    new NamedParameter("topics",  topics)
                }
            );

            builder.Populate(collection);
            var appContainer = builder.Build();
            _serviceProvider = new AutofacServiceProvider(appContainer);
        }

        private static void DisposeServices()
        {
            if (_serviceProvider != null && _serviceProvider is IDisposable)
            {
                ((IDisposable)_serviceProvider).Dispose();
            }
        }
    }
}
