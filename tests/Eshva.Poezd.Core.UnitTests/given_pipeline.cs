#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using Eshva.Common.Collections;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Pipeline;
using FluentAssertions;
using Xunit;

#endregion

namespace Eshva.Poezd.Core.UnitTests
{
  public class given_pipeline
  {
    [Fact]
    public async Task when_append_steps_it_should_execute_them()
    {
      var log = new List<int>();
      var sut = new Pipeline<TestContext>()
        .Append(new TestStep1(log))
        .Append(new TestStep2(log));
      await sut.Execute(new TestContext());
      log.Should().BeEquivalentTo(new[] {1, 2});
    }

    [Fact]
    public void when_append_same_step_it_should_fail()
    {
      var log = new List<int>();
      var pipeline = new Pipeline<TestContext>();
      var step = new TestStep1(log);
      Action sut = () => pipeline.Append(step);

      sut.Should().NotThrow();
      sut.Should().ThrowExactly<ArgumentException>();
    }

    [Fact]
    public void when_append_step_of_same_type_it_should_fail()
    {
      var log = new List<int>();
      var pipeline = new Pipeline<TestContext>();
      Action sut = () => pipeline.Append(new TestStep1(log));

      sut.Should().NotThrow();
      sut.Should().ThrowExactly<ArgumentException>();
    }

    [Fact]
    [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
    public void when_append_null_as_step_it_should_fail()
    {
      var pipeline = new Pipeline<TestContext>();
      Action sut = () => pipeline.Append(step: null);

      sut.Should().ThrowExactly<ArgumentNullException>();
    }

    [Fact]
    [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
    public void when_execute_without_context_it_should_fail()
    {
      var pipeline = new Pipeline<TestContext>();
      Func<Task> sut = () => pipeline.Execute(context: null);
      sut.Should().ThrowExactly<ArgumentNullException>();
    }

    [Fact]
    public void when_execute_with_throwing_step_it_should_fail()
    {
      var pipeline = new Pipeline<TestContext>().Append(new ThrowingStep());
      Func<Task> sut = () => pipeline.Execute(new TestContext());
      sut.Should().ThrowExactly<PoezdOperationException>().Where(exception => exception.Message.Contains(typeof(ThrowingStep).FullName));
    }

    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Local")]
    private class TestContext : ConcurrentPocket { }

    private class ThrowingStep : IStep<TestContext>
    {
      public Task Execute(TestContext context) => throw new IOException();
    }

    private class TestStep1 : IStep<TestContext>
    {
      public TestStep1(List<int> log)
      {
        _log = log;
      }

      public Task Execute(TestContext context)
      {
        _log.Add(item: 1);
        return Task.CompletedTask;
      }

      private readonly List<int> _log;
    }

    private class TestStep2 : IStep<TestContext>
    {
      public TestStep2(List<int> log)
      {
        _log = log;
      }

      public Task Execute(TestContext context)
      {
        _log.Add(item: 2);
        return Task.CompletedTask;
      }

      private readonly List<int> _log;
    }
  }
}
