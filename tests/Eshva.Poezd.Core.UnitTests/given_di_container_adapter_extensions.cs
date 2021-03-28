#region Usings

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Eshva.Poezd.Core.Common;
using FluentAssertions;
using Moq;
using Xunit;

#endregion

namespace Eshva.Poezd.Core.UnitTests
{
  public class given_di_container_adapter_extensions
  {
    [Fact]
    public void when_get_known_service_it_should_return_instance()
    {
      var adapterMock = new Mock<IDiContainerAdapter>();
      adapterMock.Setup(adapter => adapter.GetService(typeof(KnownService))).Returns(() => new KnownService());
      adapterMock.Object.GetService<IKnownService>(typeof(KnownService)).Should().BeOfType<KnownService>();
    }

    [Fact]
    public void when_get_unknown_service_it_should_return_fail()
    {
      var adapterMock = new Mock<IDiContainerAdapter>();
      adapterMock.Setup(adapter => adapter.GetService(It.IsAny<Type>())).Throws<InvalidOperationException>();
      Action sut = () => adapterMock.Object.GetService<IUnknownService>(typeof(UnknownService));
      sut.Should().ThrowExactly<InvalidOperationException>();
    }

    [Fact]
    public void when_get_null_as_service_it_should_return_fail()
    {
      var adapterMock = new Mock<IDiContainerAdapter>();
      adapterMock.Setup(adapter => adapter.GetService(typeof(KnownService))).Returns(() => new KnownService());
      Action sut = () => adapterMock.Object.GetService<IKnownService>(serviceType: null);
      sut.Should().ThrowExactly<ArgumentNullException>();
    }

    [Fact]
    public void when_get_known_service_with_exception_factory_it_should_return_instance()
    {
      var adapterMock = new Mock<IDiContainerAdapter>();
      adapterMock.Setup(adapter => adapter.GetService(typeof(KnownService))).Returns(() => new KnownService());
      adapterMock.Object.GetService<IKnownService>(typeof(KnownService), exception => exception).Should().BeOfType<KnownService>();
    }

    [Fact]
    public void when_get_unknown_service_with_exception_factory_it_should_fail()
    {
      var adapterMock = new Mock<IDiContainerAdapter>();
      adapterMock.Setup(adapter => adapter.GetService(It.IsAny<Type>())).Throws<InvalidOperationException>();
      Action sut = () => adapterMock.Object.GetService<IKnownService>(typeof(KnownService), exception => new IOException());
      sut.Should().ThrowExactly<IOException>();
    }

    [Fact]
    [SuppressMessage("ReSharper", "AccessToModifiedClosure")]
    public void when_get_service_with_invalid_arguments_and_exception_factory_it_should_fail()
    {
      var adapterMock = new Mock<IDiContainerAdapter>();
      adapterMock.Setup(adapter => adapter.GetService(typeof(KnownService))).Returns(() => new KnownService());
      var serviceType = typeof(KnownService);
      Func<Exception, Exception> exceptionToThrowFactory = exception => exception;

      Action sut = () => adapterMock.Object.GetService<IKnownService>(serviceType, exceptionToThrowFactory);

      serviceType = null;
      sut.Should().ThrowExactly<ArgumentNullException>();
      serviceType = typeof(KnownService);

      exceptionToThrowFactory = null;
      sut.Should().ThrowExactly<ArgumentNullException>();
    }

    private interface IKnownService { }

    private class KnownService : IKnownService { }

    private interface IUnknownService { }

    private class UnknownService : IKnownService { }
  }
}
