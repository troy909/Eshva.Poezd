#region Usings

using System;
using System.Collections.Generic;
using Eshva.Common.Collections;
using FluentAssertions;
using Xunit;

#endregion

namespace Eshva.Common.UnitTests
{
  public class given_pocket_extensions
  {
    [Fact]
    public void when_take_or_throw_stored_thing_it_should_be_found()
    {
      var sut = new ConcurrentPocket();
      const string nameOfThing = "name of the thing";
      const string thingItself = "the thing itself";
      sut.Put(nameOfThing, thingItself);

      sut.TakeOrThrow<string>(nameOfThing).Should().BeSameAs(thingItself, "the thing lies in the pocket");
    }

    [Fact]
    public void when_take_or_throw_something_from_empty_pocket_it_should_throw()
    {
      var pocket = new ConcurrentPocket();
      Action sut = () => pocket.TakeOrThrow<string>("some name");
      sut.Should().Throw<KeyNotFoundException>("the pocket is empty");
    }

    [Fact]
    public void when_take_or_throw_missing_thing_in_pocket_it_should_throw()
    {
      var pocket = new ConcurrentPocket();
      const string nameOfThing = "name of the thing";
      const string thingItself = "the thing itself";
      pocket.Put(nameOfThing, thingItself);

      Action sut = () => pocket.TakeOrThrow<string>("some other name");
      sut.Should().Throw<KeyNotFoundException>("there is no thing with such name in the pocket");
    }

    [Fact]
    public void when_take_or_throw_stored_thing_with_unexpected_type_it_should_throw()
    {
      var pocket = new ConcurrentPocket();
      const string nameOfThing = "name of the thing";
      const string thingItself = "the thing itself";
      pocket.Put(nameOfThing, thingItself);

      Action sut = () => pocket.TakeOrThrow<given_pocket_extensions>(nameOfThing);
      sut.Should().Throw<InvalidCastException>("impossible to cast a string to the different type");
    }

    [Fact]
    public void when_take_or_throw_thing_with_invalid_name_given_it_should_throw()
    {
      var pocket = new ConcurrentPocket();
      const string nameOfThing = "name of the thing";
      const string thingItself = "the thing itself";
      pocket.Put(nameOfThing, thingItself);

      Action sut = () => pocket.TakeOrThrow<string>(name: null);
      sut.Should().Throw<ArgumentNullException>().Where(
        exception => exception.ParamName == "name",
        "null is not a valid name");

      sut = () => pocket.TakeOrThrow<string>(string.Empty);
      sut.Should().Throw<ArgumentNullException>().Where(
        exception => exception.ParamName == "name",
        "an empty string is not a valid name");

      sut = () => pocket.TakeOrThrow<string>(WhiteSpaceString);
      sut.Should().Throw<ArgumentNullException>().Where(
        exception => exception.ParamName == "name",
        "a whitespace is not a valid name");
    }

    [Fact]
    public void when_take_or_null_stored_thing_it_should_be_found()
    {
      var sut = new ConcurrentPocket();
      const string nameOfThing = "name of the thing";
      const string thingItself = "the thing itself";
      sut.Put(nameOfThing, thingItself);

      sut.TakeOrNull<string>(nameOfThing).Should().BeSameAs(thingItself, "the thing lies in the pocket");
    }

    [Fact]
    public void when_take_or_null_something_from_empty_pocket_it_should_find_nothing()
    {
      var sut = new ConcurrentPocket();
      sut.TakeOrNull<string>("some name").Should().BeNull("the pocket is empty");
    }

    [Fact]
    public void when_take_or_null_missing_thing_in_pocket_it_find_nothing()
    {
      var sut = new ConcurrentPocket();
      const string nameOfThing = "name of the thing";
      const string thingItself = "the thing itself";
      sut.Put(nameOfThing, thingItself);

      sut.TakeOrNull<string>("some other name").Should().BeNull("there is no thing with such name in the pocket");
    }

    [Fact]
    public void when_take_or_null_stored_thing_with_unexpected_type_it_should_throw()
    {
      var pocket = new ConcurrentPocket();
      const string nameOfThing = "name of the thing";
      const string thingItself = "the thing itself";
      pocket.Put(nameOfThing, thingItself);

      Action sut = () => pocket.TakeOrNull<given_pocket_extensions>(nameOfThing);
      sut.Should().Throw<InvalidCastException>("impossible to cast a string to the different type");
    }

    [Fact]
    public void when_take_or_null_thing_with_invalid_name_given_it_should_throw()
    {
      var pocket = new ConcurrentPocket();
      const string nameOfThing = "name of the thing";
      const string thingItself = "the thing itself";
      pocket.Put(nameOfThing, thingItself);

      Action sut = () => pocket.TakeOrNull<string>(name: null);
      sut.Should().Throw<ArgumentNullException>().Where(
        exception => exception.ParamName == "name",
        "null is not a valid name");

      sut = () => pocket.TakeOrNull<string>(string.Empty);
      sut.Should().Throw<ArgumentNullException>().Where(
        exception => exception.ParamName == "name",
        "an empty string is not a valid name");

      sut = () => pocket.TakeOrNull<string>(WhiteSpaceString);
      sut.Should().Throw<ArgumentNullException>().Where(
        exception => exception.ParamName == "name",
        "a whitespace is not a valid name");
    }

    [Fact]
    public void when_take_or_put_stored_thing_it_should_be_found()
    {
      var sut = new ConcurrentPocket();
      const string nameOfThing = "name of the thing";
      const string thingItself = "the thing itself";
      var isFactoryCalled = false;
      Func<string> thingFactory = () =>
      {
        isFactoryCalled = true;
        return thingItself;
      };
      sut.Put(nameOfThing, thingItself);

      sut.TakeOrPut(nameOfThing, thingFactory).Should().BeSameAs(thingItself, "the thing lies in the pocket");
      isFactoryCalled.Should().BeFalse("the think was already stored in the pocket");
    }

    [Fact]
    public void when_take_or_put_thing_from_empty_pocket_it_should_make_thing_put_it_into_pocket_and_return_it()
    {
      var sut = new ConcurrentPocket();
      const string nameOfThing = "name of the thing";
      const string thingItself = "the thing itself";
      var isFactoryCalled = false;
      Func<string> thingFactory = () =>
      {
        isFactoryCalled = true;
        return thingItself;
      };

      var thing = sut.TakeOrPut(nameOfThing, thingFactory);

      sut.GetThings().Count.Should().Be(expected: 1, "the made thing should be stored in the pocket");
      isFactoryCalled.Should().BeTrue("it was an empty pocket and the thing should be made from scratch");
      thing.Should().BeSameAs(thingItself, "exactly this thing was made");
    }

    [Fact]
    public void when_take_or_put_missing_thing_pocket_it_should_make_thing_put_it_into_pocket_and_return_it()
    {
      var sut = new ConcurrentPocket();
      sut.Put("some name", "something");
      const string nameOfThing = "name of the thing";
      const string thingItself = "the thing itself";
      var isFactoryCalled = false;
      Func<string> thingFactory = () =>
      {
        isFactoryCalled = true;
        return thingItself;
      };

      var thing = sut.TakeOrPut(nameOfThing, thingFactory);

      sut.GetThings().Count.Should().Be(expected: 2, "the made thing should be stored in the pocket");
      isFactoryCalled.Should().BeTrue("it was an empty pocket and the thing should be made from scratch");
      thing.Should().BeSameAs(thingItself, "exactly this thing was made");
    }

    [Fact]
    public void when_take_or_put_stored_thing_with_unexpected_type_it_should_throw()
    {
      var pocket = new ConcurrentPocket();
      const string nameOfThing = "name of the thing";
      const string thingItself = "the thing itself";
      pocket.Put(nameOfThing, thingItself);

      Action sut = () => pocket.TakeOrPut(nameOfThing, () => new given_pocket_extensions());
      sut.Should().Throw<InvalidCastException>("impossible to cast a string to the different type");
    }

    [Fact]
    public void when_take_or_put_thing_with_invalid_name_given_it_should_throw()
    {
      var pocket = new ConcurrentPocket();
      const string nameOfThing = "name of the thing";
      const string thingItself = "the thing itself";
      pocket.Put(nameOfThing, thingItself);

      Action sut = () => pocket.TakeOrPut(name: null, () => "not relevant");
      sut.Should().Throw<ArgumentNullException>().Where(
        exception => exception.ParamName == "name",
        "null is not a valid name");

      sut = () => pocket.TakeOrPut(string.Empty, () => "not relevant");
      sut.Should().Throw<ArgumentNullException>().Where(
        exception => exception.ParamName == "name",
        "an empty string is not a valid name");

      sut = () => pocket.TakeOrPut(WhiteSpaceString, () => "not relevant");
      sut.Should().Throw<ArgumentNullException>().Where(
        exception => exception.ParamName == "name",
        "a whitespace is not a valid name");
    }

    [Fact]
    public void when_take_or_put_thing_with_invalid_factory_it_should_throw()
    {
      var pocket = new ConcurrentPocket();
      const string nameOfThing = "name of the thing";
      const string thingItself = "the thing itself";
      pocket.Put(nameOfThing, thingItself);

      Action sut = () => pocket.TakeOrPut<string>(nameOfThing, thingFactory: null);
      sut.Should().Throw<ArgumentNullException>().Where(
        exception => exception.ParamName == "thingFactory",
        "null is not a valid thing factory");
    }

    private const string WhiteSpaceString = " \n\t\r";
  }
}
