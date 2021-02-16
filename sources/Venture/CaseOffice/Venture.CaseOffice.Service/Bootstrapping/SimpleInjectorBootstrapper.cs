#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using SimpleInjector;
using Venture.CaseOffice.Application;
using Venture.Common.Application.MessageHandling;

#endregion

namespace Venture.CaseOffice.Service.Bootstrapping
{
  public static class SimpleInjectorBootstrapper
  {
    public static void ConfigureContainer(
      this IApplicationBuilder applicationBuilder,
      Container container,
      IConfiguration configuration)
    {
      applicationBuilder.UseSimpleInjector(container);
      container.SetupContainer(configuration);
      container.Verify(VerificationOption.VerifyAndDiagnose);
    }

    private static void SetupContainer(this Container container, IConfiguration configuration)
    {
      var messageHandlersAssemblies = new[] {typeof(CaseOfficeApplicationAssemblyTag).Assembly};
      var serviceTypes = container.GetMessageHandlersTypes(messageHandlersAssemblies).ToArray();
      container.Collection.Register(typeof(IHandleMessageOfType<>), serviceTypes);
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
