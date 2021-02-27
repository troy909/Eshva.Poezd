#region Usings

using System;
using FluentAssertions;
using Venture.Common.Application.MessagePublishing;
using Xunit;

#endregion

namespace Venture.Common.Application.UnitTests
{
  public class given_message_id
  {
    [Fact]
    public void when_formatting_id_it_should_be_formatted_expected_way()
    {
      var id = Guid.NewGuid();
      MessageId.Format(id).Should().Be(id.ToString("N"), "message ID should be formatted expected way");
    }

    [Fact]
    public void when_generating_id_it_should_generate_as_new_guid_with_expected_format()
    {
      var id = MessageId.Generate();
      Guid.ParseExact(id, "N").ToString("N").Should().Be(id, "message ID should be generated as a new GUID with expected format");
    }
  }
}
