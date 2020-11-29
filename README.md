# ResourceBridge

```
Install-Package ResourceBridge 
```

Generates strongly typed wrapper classes. Similar to Visual Studios native ResX generation, but with Asp.Net core in mind.

![alt summary](https://github.com/BerndNK/ResourceBridge/blob/master/Resources/summary.png?raw=true)


Resource names are defined with nesting. These groups then get generated as nested classes and may then be used like:
```C#
var examples = new ExampleResources(new StringLocalizer<ExampleResources>());
Console.WriteLine(examples.GroupA.Hello);
```

However, the generator was designed with Blazor and the tooling of .NET 5 in mind:
```html
@inject ILocalizedExampleResources Resources;

<span>@Resources.GroupA.Hello</span>
```

As the designated solution for localization is to use [IStringLocalizer](https://docs.microsoft.com/dotnet/api/microsoft.extensions.localization.istringlocalizer-1).

# Usage
- Install the package 
```
Install-Package ResourceBridge 
```
- Define .resx files ([Example](https://github.com/BerndNK/ResourceBridge/blob/master/Example/ExampleResources.de.resx))
- Edit your .csproj file and include the .resx files for which you want to generate the wrapper ([Example](https://github.com/BerndNK/ResourceBridge/blob/master/Example/Example.csproj))
```
<ItemGroup>
  <AdditionalFiles Include="YourResourceFile.resx" />
</ItemGroup>
```
Note: *Only define one language per resource.*

# Behind the scenes
Instead of using static classes to avoid poluting the list of types, the only exposed types are interfaces, all starting with a "ILocalized" prefix.
Therefore resources keys like "Key.Name", will not interfer with a class also named "Key".

For the example above, the following code is produced:
```C#
using Microsoft.Extensions.Localization;
using System.Text;

namespace BridgeSource
{
    public interface ILocalizedExampleResources
    {
        ILocalizedGroupA GroupA { get; }
        ILocalizedGroupB GroupB { get; }
        
        public interface ILocalizedGroupA
        {
            string Hello { get; }
            string World { get; }
            
        
        }
        public interface ILocalizedGroupB
        {
            ILocalizedChild Child { get; }
            
            public interface ILocalizedChild
            {
                string Hello (string name);
                
            
            }
        
        }
    
    }

    public sealed class ExampleResources : ILocalizedExampleResources
    {
        private readonly IStringLocalizer<ExampleResources> _stringLocalizer;
    
        public ILocalizedExampleResources.ILocalizedGroupA GroupA { get; }
        public ILocalizedExampleResources.ILocalizedGroupB GroupB { get; }
    
    
    
        public ExampleResources(IStringLocalizer<ExampleResources> stringLocalizer)
        {
            _stringLocalizer = stringLocalizer;
                GroupA = new GroupAClass(stringLocalizer);
        GroupB = new GroupBClass(stringLocalizer);
    
        }
    
        private class GroupAClass : ILocalizedExampleResources.ILocalizedGroupA
        {
            private readonly IStringLocalizer<ExampleResources> _stringLocalizer;
        
        
            public string Hello => _stringLocalizer["GroupA.Hello"];
            public string World => _stringLocalizer["GroupA.World"];
        
        
            public GroupAClass(IStringLocalizer<ExampleResources> stringLocalizer)
            {
                _stringLocalizer = stringLocalizer;
                
            }
        
        
        }
        private class GroupBClass : ILocalizedExampleResources.ILocalizedGroupB
        {
            private readonly IStringLocalizer<ExampleResources> _stringLocalizer;
        
            public ILocalizedExampleResources.ILocalizedGroupB.ILocalizedChild Child { get; }
        
        
        
            public GroupBClass(IStringLocalizer<ExampleResources> stringLocalizer)
            {
                _stringLocalizer = stringLocalizer;
                    Child = new ChildClass(stringLocalizer);
        
            }
        
            private class ChildClass : ILocalizedExampleResources.ILocalizedGroupB.ILocalizedChild
            {
                private readonly IStringLocalizer<ExampleResources> _stringLocalizer;
            
            
                public string Hello(string name) {
                    var sb = new StringBuilder(_stringLocalizer["GroupB.Child.Hello"]);
                    sb.Replace("name", name);
                
                     return sb.ToString();
                }
            
            
                public ChildClass(IStringLocalizer<ExampleResources> stringLocalizer)
                {
                    _stringLocalizer = stringLocalizer;
                    
                }
            
            
            }
        
        }
    
    }

}
```

