#region Usings

using System;
using System.Collections.Generic;
using Eshva.Poezd.Core.Pipeline;

#endregion

namespace Eshva.Poezd.Core.Configuration
{
  public class EgressApiConfiguration : IMessageRouterConfigurationPart
  {
    public string Id { get; set; }

    public Type PipeFitterType { get; set; }

    public Type MessageTypesRegistryType { get; set; }

    public static EgressApiConfiguration Empty { get; } = CreateValidEmpty();

    /// <inheritdoc />
    public IEnumerable<string> Validate()
    {
      if (string.IsNullOrWhiteSpace(Id))
        yield return "ID of egress API should be specified.";
      if (PipeFitterType == null)
        yield return $"The egress pipe fitter type should be set for the API with ID '{Id}'.";
      if (MessageTypesRegistryType == null)
        yield return $"The message registry type should be set for the API with ID '{Id}'.";
    }

    private static EgressApiConfiguration CreateValidEmpty() => new()
    {
      Id = "empty egress API configuration",
      MessageTypesRegistryType = typeof(EmptyEgressMessageTypesRegistry),
      PipeFitterType = typeof(EmptyPipeFitter)
    };
  }
}
