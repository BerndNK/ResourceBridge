namespace ResourceBridge
{
    partial class StringFactory
    {
        private const string Content = nameof(Content);

        private const string InterfaceContent = nameof(InterfaceContent);

        private const string Namespace = nameof(Namespace);

        private const string FileTemplate = @"// GENERATED CLASS
using Microsoft.Extensions.Localization;
using System.Text;

namespace {Namespace}
{
{InterfaceContent}
{Content}
}";
        
        private const string FileName = nameof(FileName);
        private const string ClassModifier = nameof(ClassModifier);
        private const string TypeName = nameof(TypeName);
        private const string Inherits = nameof(Inherits);
        private const string SubClassProperties = nameof(SubClassProperties);
        private const string PropertyInitializer = nameof(PropertyInitializer);
        private const string Properties = nameof(Properties);
        private const string SubClasses = nameof(SubClasses);

        private const string ClassTemplate = @"{ClassModifier} class {TypeName}{Inherits}
{
    private readonly IStringLocalizer<{FileName}> _stringLocalizer;

{SubClassProperties}
{Properties}

    public {TypeName}(IStringLocalizer<{FileName}> stringLocalizer)
    {
        _stringLocalizer = stringLocalizer;
        {PropertyInitializer}
    }

{SubClasses}
}";

        private const string StringLocalizerKey = nameof(StringLocalizerKey);

        private const string TextPropertyTemplate = @"public string {PropertyName} => _stringLocalizer[""{StringLocalizerKey}""] as string ?? ""{StringLocalizerKey}"";";

        private const string MethodContent = nameof(MethodContent);
        private const string MethodParameter = nameof(MethodParameter);

        private const string TextPropertyMethodTemplate = @"public string {PropertyName}({MethodParameter}) {
    var sb = new StringBuilder(_stringLocalizer[""{StringLocalizerKey}""]);
{MethodContent}
     return sb.ToString();
}";
        

        private const string PropertyType = nameof(PropertyType);

        private const string PropertyName = nameof(PropertyName);

        private const string SubClassPropertyTemplate = @"public {PropertyType} {PropertyName} { get; }";


        private const string InterfaceProperties = nameof(InterfaceProperties);
        private const string SubInterfaces = nameof(SubInterfaces);

        private const string InterfaceTemplate = @"public interface {TypeName}
{
{InterfaceProperties}    
{SubInterfaces}
}";

        private const string InterfacePropertyTemplate = @"{PropertyType} {PropertyName} { get; }";

        private const string InterfacePropertyMethodTemplate = @"{PropertyType} {PropertyName} ({MethodParameter});";

        private const string InterfacePrefix = "ILocalized";
    }
}