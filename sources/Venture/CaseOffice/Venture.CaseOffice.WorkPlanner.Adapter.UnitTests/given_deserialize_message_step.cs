#region Usings

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Eshva.Common.Collections;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Routing;
using FlatSharp;
using FluentAssertions;
using Venture.Common.Poezd.Adapter;
using Venture.WorkPlanner.Messages.V1.Events;
using Xunit;

#endregion

namespace Venture.CaseOffice.WorkPlanner.Adapter.UnitTests
{
  [SuppressMessage("ReSharper", "InconsistentNaming")]
  public class given_deserialize_message_step
  {
    [Fact]
    public async Task when_executed_with_broker_message_and_message_type_name_withing_context_it_should_store_deserialized_message()
    {
      var sut = new DeserializeMessageStep();
      var context = new ConcurrentPocket();

      const string expectedTaskType = "DocumentCollectionTask";
      var expectedId = Guid.NewGuid();
      var serializedMessage = CreateSerializedMessage(expectedId, expectedTaskType);
      context.Put(ContextKeys.Broker.MessagePayload, serializedMessage);
      context.Put(ContextKeys.Application.MessageTypeName, typeof(TaskCreated).FullName!);

      await sut.Execute(context);

      var applicationMessage = context.TakeOrNull<TaskCreated>(ContextKeys.Application.MessagePayload);
      Guid messageTaskId = applicationMessage.TaskId;
      messageTaskId.Should().Be(expectedId);
      applicationMessage.TaskType.Should().Be(expectedTaskType);
    }

    [Fact]
    public void when_executed_with_broker_message_but_without_message_type_name_within_context_it_should_throw()
    {
      var step = new DeserializeMessageStep();
      var context = new ConcurrentPocket();

      const string expectedTaskType = "DocumentCollectionTask";
      var expectedId = Guid.NewGuid();
      var serializedMessage = CreateSerializedMessage(expectedId, expectedTaskType);
      context.Put(ContextKeys.Broker.MessagePayload, serializedMessage);

      Func<Task> sut = () => step.Execute(context);
      sut.Should().Throw<PoezdContextContentException>("impossible to deserialize message without to know its type")
        .Where(exception => exception.ContextItemName.Equals(ContextKeys.Application.MessageTypeName));
    }

    [Fact]
    public void when_executed_with_message_type_name_but_without_broker_message_within_context_it_should_throw()
    {
      var step = new DeserializeMessageStep();
      var context = new ConcurrentPocket();
      context.Put(ContextKeys.Application.MessageTypeName, typeof(TaskCreated).FullName!);

      Func<Task> sut = () => step.Execute(context);
      sut.Should().Throw<PoezdContextContentException>("impossible to deserialize message without its payload")
        .Where(exception => exception.ContextItemName.Equals(ContextKeys.Broker.MessagePayload));
    }

    [Fact]
    public void when_executed_without_context_it_should_throw()
    {
      // ReSharper disable once AssignNullToNotNullAttribute it's a test
      Func<Task> sut = async () => await new DeserializeMessageStep().Execute(context: null);
      sut.Should().Throw<ArgumentNullException>().Where(exception => exception.ParamName.Equals("context"), "context must be specified");
    }

    private static byte[] CreateSerializedMessage(Guid expectedId, string expectedTaskType)
    {
      var task = new TaskCreated {TaskId = expectedId, TaskType = expectedTaskType};
      var serializedMessage = new byte[TaskCreated.Serializer.GetMaxSize(task)];
      TaskCreated.Serializer.Write(serializedMessage, task);
      return serializedMessage;
    }
  }
}
