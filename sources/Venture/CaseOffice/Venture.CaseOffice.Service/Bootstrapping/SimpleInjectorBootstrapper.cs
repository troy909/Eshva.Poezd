#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Eshva.Poezd.Core.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using SimpleInjector;
using Venture.CaseOffice.Application;
using Venture.Common.Application.MessageHandling;
using Venture.Poezd.Coupling;

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

    private static void SetupPoezd(this Container container, IConfiguration configuration)
    {
      var poezdConfiguration = MessageRouter.Configure(
        configurator => configurator
          .WithMessageHandling(
            messageHandling => messageHandling.WithMessageHandlersFactory(new VentureMessageHandlerFactory(container))));
    }

    private static void SetupContainer(this Container container, IConfiguration configuration)
    {
      var messageHandlersAssemblies = new[] { typeof(CaseOfficeApplicationAssemblyTag).Assembly };
      var serviceTypes = container.GetMessageHandlersTypes(messageHandlersAssemblies).ToArray();
      container.Collection.Register(typeof(IHandleMessageOfType<>), serviceTypes);

      /*
      container.ConfigurePoezdRouter(
        router => router.AddBus(
                          bus => bus.WithName("lowers department Kafka bus")
                                    .WithMessageBroker(broker => broker.UseKafka("connection string"))
                                    .AddService(
                                      service => service.WithName("lowers service 1")
                                                        .WithSerialization(
                                                          serialization => serialization.UseSerializer(new FlatBuffersSerializer()))
                                                        .WithMetadataHandling(
                                                          metadataHandling => metadataHandling.UseHandler(new Service1MetadataHandler()))
                                                        .WithMessageTypingConvention(
                                                          messageTypingConfigurator =>
                                                            messageTypingConfigurator.UseConvention(new Service1MessageTypingConvention()))
                                                        .WithQueueNamingConvention(
                                                          queueNaming => queueNaming.UseConvention(new Service1QueueNamingConvention())))
                                    .AddService(
                                      service => service.WithName("lowers service 2")
                                                        .WithSerialization(
                                                          serialization => serialization.UseSerializer(new MyJsonSerializer()))
                                                        .WithMetadataHandling(
                                                          metadataHandling => metadataHandling.UseHandler(new Service2MetadataHandler()))
                                                        .WithMessageTypingConvention(
                                                          messageTypingConfigurator =>
                                                            messageTypingConfigurator.UseConvention(new ServiceMessageTypingConvention()))
                                                        .WithQueueNamingConvention(
                                                          queueNaming => queueNaming.UseConvention(new Service2QueueNamingConvention()))))
                        .AddBus(
                          bus => bus.WithName("accounting department RabbitMQ bus")
                                    .WithMessageBroker(broker => broker.UseRabbitMq("connection string"))
                                    .AddService(
                                      service => service.WithName("accountants main service")
                                                        .WithSerialization(
                                                          serialization => serialization.UseSerializer(new MyJsonSerializer()))
                                                        .WithMetadataHandling(
                                                          metadataHandling =>
                                                            metadataHandling.UseHandler(new MainAccountantsServiceMetadataHandler()))))
                        .AddBus(
                          bus => bus.WithName("accounting department Kafka bus")
                                    .WithMessageBroker(broker => broker.UseKafka("connection string"))
                                    .AddService(
                                      service => service.WithName("accountants audit service")
                                                        .WithSerialization(
                                                          serialization => serialization.UseSerializer(new MyXmlSerializer()))
                                                        .WithMetadataHandling(
                                                          metadataHandling =>
                                                            metadataHandling.UseHandler(new AccountantsAuditServiceMetadataHandler()))))
                        .WithLogging(logging => logging.UseSerilog())
                        .WithProcessManaging(processManaging => processManaging.UseInMemoryStorage())
                        .WithMessageHandling(messageHandling => messageHandling.WithBaseMessageHandlerType(typeof(IHandleMessageOfType<>)))
                        .Start());
    */
    }

    private static IEnumerable<Type> GetMessageHandlersTypes(this Container container, IEnumerable<Assembly> assemblies) =>
      container.GetTypesToRegister(
        typeof(IHandleMessageOfType<>),
        assemblies,
        new TypesToRegisterOptions
        {
          IncludeGenericTypeDefinitions = true,
          IncludeComposites = true
        });
  }
}
