# Message schemas for Work Planner service API
In this folder you should place FlatBuffers schemas of Work Planner's API message types. In your `.csproj` for every schema file you should add a line like in the following code sample.
```xml
<ItemGroup>
    <FlatSharpSchema Include="Schemas\AddSomethingCommand.fbs" />
    <FlatSharpSchema Include="Schemas\Event42.fbs" />
    ...
  </ItemGroup>
```
