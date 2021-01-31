#region Usings

using System;
using Eshva.Common.Collections;
using FluentAssertions;
using Xunit;

#endregion

namespace Eshva.Common.UnitTests
{
  public class given_concurrent_pocket
  {
    [Fact]
    public void when_put_reference_thing_with_accepted_name_it_should_be_found_in_the_pocket()
    {
      var sut = new ConcurrentPocket();
      const string nameOfThing = "name of the thing";
      const string thingItself = "the thing itself";
      var pocket = sut.Put(nameOfThing, thingItself);

      pocket.Should().BeSameAs(sut, "returned pocket should stay the same");
      pocket.GetThings()[nameOfThing].Should().BeSameAs(thingItself, "thing should be stored by reference");
    }

    [Fact]
    public void when_put_struct_thing_with_accepted_name_it_should_be_found_in_the_pocket()
    {
      var sut = new ConcurrentPocket();

      const string nameOfThing = "name of the thing";
      var thingItself = DateTime.Now;
      var pocket = sut.Put(nameOfThing, thingItself);

      pocket.Should().BeSameAs(sut, "returned pocket should stay the same");
      sut.GetThings()[nameOfThing].Should().Be(thingItself, "thing should be stored");
    }

    [Fact]
    public void when_put_thing_with_same_name_as_already_present_it_should_replace_it()
    {
      var sut = new ConcurrentPocket();

      const string nameOfThing = "name of the thing";
      var secondThing = DateTime.Now;
      sut.Put(nameOfThing, "first thing")
        .Put(nameOfThing, secondThing);

      sut.GetThings().Count.Should().Be(expected: 1, "same name means to replace the stored thing");
      sut.GetThings()[nameOfThing].Should().Be(secondThing, "the thing should be replaced");
    }

    [Fact]
    public void when_put_nothing_with_valid_name_given_it_should_throw()
    {
      var pocket = new ConcurrentPocket();

      Action sut = () => pocket.Put("a valid name", thing: null);

      sut.Should().Throw<ArgumentNullException>().Where(exception => exception.ParamName.Equals("thing"));
    }

    [Fact]
    public void when_put_something_with_invalid_name_it_should_throw()
    {
      var pocket = new ConcurrentPocket();

      Action sut = () => pocket.Put(name: null, "something");
      sut.Should().Throw<ArgumentNullException>().Where(
        exception => exception.ParamName.Equals("name"),
        "null is not a valid name");

      sut = () => pocket.Put(string.Empty, "something");
      sut.Should().Throw<ArgumentNullException>().Where(
        exception => exception.ParamName.Equals("name"),
        "an empty string is not a valid name");

      sut = () => pocket.Put(WhiteSpaceString, "something");
      sut.Should().Throw<ArgumentNullException>().Where(
        exception => exception.ParamName.Equals("name"),
        "a whitespace string is not a valid name");
    }

    [Fact]
    public void when_try_to_take_stored_thing_by_name_it_should_return_it_and_success_status()
    {
      var sut = new ConcurrentPocket();
      const string nameOfThing = "name of the thing";
      const string thingItself = "the thing itself";
      sut.Put(nameOfThing, thingItself);

      sut.TryTake<string>(nameOfThing, out var foundThing).Should().BeTrue("the requested thing lies in the pocket");
      foundThing.Should().BeSameAs(thingItself, "thing should be stored by reference");
    }

    [Fact]
    public void when_try_to_take_missing_reference_thing_it_should_return_null_and_fail_status()
    {
      var sut = new ConcurrentPocket();
      const string nameOfThing = "name of the thing";
      const string thingItself = "the thing itself";
      sut.Put(nameOfThing, thingItself);

      sut.TryTake<string>("a different name", out var foundThing).Should().BeFalse("there is no a thing with this name in the pocket");
      foundThing.Should().BeNull("null is expected 'nothing' for a reference type");
    }

    [Fact]
    public void when_try_to_take_missing_struct_thing_it_should_return_default_value_and_fail_status()
    {
      var sut = new ConcurrentPocket();
      const string nameOfThing = "name of the thing";
      var thingItself = DateTime.Now;
      sut.Put(nameOfThing, thingItself);

      sut.TryTake<DateTime>("a different name", out var foundThing).Should().BeFalse("there is no a thing with this name in the pocket");
      foundThing.Should().Be(new DateTime(), "default DateTime is expected 'nothing' for a struct DateTime");
    }

    [Fact]
    public void when_try_to_take_stored_thing_with_unexpected_type_it_should_throw()
    {
      var pocket = new ConcurrentPocket();
      const string nameOfThing = "name of the thing";
      const string thingItself = "the thing itself";
      pocket.Put(nameOfThing, thingItself);

      Action sut = () => pocket.TryTake<DateTime>(nameOfThing, out _);
      sut.Should().Throw<InvalidCastException>("impossible to cast a string to the different type");
    }

    [Fact]
    public void when_try_to_take_thing_with_invalid_name_given_it_should_throw()
    {
      var pocket = new ConcurrentPocket();
      const string nameOfThing = "name of the thing";
      const string thingItself = "the thing itself";
      pocket.Put(nameOfThing, thingItself);

      Action sut = () => pocket.TryTake<string>(name: null, out var _);
      sut.Should().Throw<ArgumentNullException>().Where(
        exception => exception.ParamName == "name",
        "null is not a valid name");

      sut = () => pocket.TryTake<string>(string.Empty, out var _);
      sut.Should().Throw<ArgumentNullException>().Where(
        exception => exception.ParamName == "name",
        "an empty string is not a valid name");

      sut = () => pocket.TryTake<string>(WhiteSpaceString, out var _);
      sut.Should().Throw<ArgumentNullException>().Where(
        exception => exception.ParamName == "name",
        "a whitespace is not a valid name");
    }

    [Fact]
    public void when_try_to_remove_stored_thing_it_should_remove_it_and_return_success_status()
    {
      var sut = new ConcurrentPocket();
      const string nameOfThing = "name of the thing";
      const string thingItself = "the thing itself";
      sut.Put(nameOfThing, thingItself);

      sut.TryRemove(nameOfThing).Should().BeTrue("thing was removed from the pocket");
      sut.GetThings().Count.Should().Be(expected: 0, "the pocket is empty");
    }

    [Fact]
    public void when_try_to_remove_thing_not_stored_in_pocket_it_should_leave_pocket_content_unchanged_and_return_fail_status()
    {
      var sut = new ConcurrentPocket();
      const string nameOfThing = "name of the thing";
      const string thingItself = "the thing itself";
      sut.Put(nameOfThing, thingItself);

      sut.TryRemove("a different name").Should().BeFalse("a thing with such name not stored in the pocket");
      sut.GetThings().Count.Should().Be(expected: 1, "number of things should be the same");
      sut.GetThings()[nameOfThing].Should().BeSameAs(thingItself, "the pocket content should stay the same");
    }

    [Fact]
    public void when_try_to_remove_thing_with_invalid_name_given_it_should_throw()
    {
      var pocket = new ConcurrentPocket();

      Action sut = () => pocket.TryRemove(name: null);
      sut.Should().Throw<ArgumentNullException>().Where(
        exception => exception.ParamName.Equals("name"),
        "null is not a valid name");

      sut = () => pocket.TryRemove(string.Empty);
      sut.Should().Throw<ArgumentNullException>().Where(
        exception => exception.ParamName.Equals("name"),
        "an empty string is not a valid name");

      sut = () => pocket.TryRemove(WhiteSpaceString);
      sut.Should().Throw<ArgumentNullException>().Where(
        exception => exception.ParamName.Equals("name"),
        "a whitespace is not a valid name");
    }

    [Fact]
    public void when_try_get_things_from_empty_pocket_it_should_return_nothing()
    {
      var sut = new ConcurrentPocket();
      sut.GetThings().Count.Should().Be(expected: 0, "no any thing in the pocket");
    }

    private const string WhiteSpaceString = " \n\t\r";
  }
}
