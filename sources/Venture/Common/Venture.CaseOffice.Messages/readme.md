# Message schemas for Case Office service API
In this folder you should place FlatBuffers schemas of Work Planner's API message types. In your `.csproj` for every schema file you should add a line like in the following code sample.
```xml
<ItemGroup>
    <FlatSharpSchema Include="V1/Commands/AddSomethingCommand.fbs" />
    <FlatSharpSchema Include="V1/Events/Event42.fbs" />
    ...
  </ItemGroup>
```
