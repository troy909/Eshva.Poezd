#region Usings

using Eshva.Poezd.SimpleInjectorCoupling;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using SimpleInjector;
using Venture.CaseOffice.Application;

#endregion


namespace Venture.CaseOffice.Service.Bootstrapping
{
  public static class SimpleInjectorBootstrapper
  {
    public static void ConfigureContainer(this IApplicationBuilder applicationBuilder, Container container, IConfiguration configuration)
    {
      applicationBuilder.UseSimpleInjector(container);
      container.SetupContainer(configuration);
      container.Verify(VerificationOption.VerifyAndDiagnose);
    }

    private static void SetupContainer(this Container container, IConfiguration configuration)
    {
      var messageHandlersAssemblies = new[] { typeof(CaseOfficeApplicationAssemblyTag).Assembly };
      //container.AddMessageBus
      container.ConfigurePoezdRouter(
        router => router.AddBus(
                          bus => bus.WithName("lowers department Kafka bus")
                                    .WithMessageBroker(broker => broker.UseKafka("connection string"))
                                    .AddService(
                                      service => service.WithName("lowers service 1")
                                                        .WithSerialization(new FlatBuffersSerializer())
                                                        .WithMetadataHandling(new Service1MetadataHandler())
                                                        .WithMessageTypingConvention(new Service1MessageTypingConvention())
                                                        .WithQueueNamingConvention(new Service1TopicNamingConvention()))
                                    .AddService(
                                      service => service.WithName("lowers service 2")
                                                        .WithSerialization(new MyJsonSerializer())
                                                        .WithMetadataHandling(new Service2MetadataHandler())
                                                        .WithMessageTypingConvention(new ServiceMessageTypingConvention())
                                                        .WithQueueNamingConvention(new Service2TopicNamingConvention())))
                        .AddBus(
                          bus => bus.WithName("accounting department RabbitMQ bus")
                                    .WithMessageBroker(broker => broker.UseRabbitMq("connection string"))
                                    .AddService(
                                      service => service.WithName("accountants main service")
                                                        .WithSerialization(new MyJsonSerializer())
                                                        .WithMetadataHandling(new MainAccountantsServiceMetadataHandler())))
                        .AddBus(
                          bus => bus.WithName("accounting department Kafka bus")
                                    .WithMessageBroker(broker => broker.UseKafka("connection string"))
                                    .AddService(
                                      service => service.WithName("accountants audit service")
                                                        .WithSerialization(new MyJsonSerializer())
                                                        .WithMetadataHandling(new AccountantsAuditServiceMetadataHandler())))
                        .WithLogging(logging => logging.UseSerilog())
                        .WithProcessManaging(processManaging => processManaging.));
    }
  }
}
