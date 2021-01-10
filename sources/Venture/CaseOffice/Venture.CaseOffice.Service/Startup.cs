#region Usings

using System.Text.Json.Serialization;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimpleInjector;

#endregion


namespace Venture.CaseOffice.Service
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    [UsedImplicitly(ImplicitUseKindFlags.Access)]
    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
      services.AddControllers().AddJsonOptions(opt => { opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); });
      services.AddLogging();
      services.AddSimpleInjector(
        _container,
        options => options.AddLogging().AddAspNetCore().AddControllerActivation());
    }

    [UsedImplicitly]
    public void Configure(IApplicationBuilder builder, IWebHostEnvironment environment)
    {
      builder.ConfigureContainer(_container, Configuration);
      //builder.UseSerilogRequestLogging();
      builder.UseHttpsRedirection();
      builder.UseRouting().UseEndpoints(endpoints => endpoints.MapControllers());
      builder.UseAuthorization();
    }

    private readonly Container _container = new Container();
  }
}
