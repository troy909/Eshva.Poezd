using System;


namespace Eshva.Poezd.Core.Configuration
{
  public class MetadataHandlingConfigurator
  {
    public IMetadataHandler MetadataHandler { get; private set; }

    public MetadataHandlingConfigurator UseHandler(IMetadataHandler metadataHandler)
    {
      MetadataHandler = metadataHandler ?? throw new ArgumentNullException(nameof(metadataHandler));
      return this;
    }
  }
}