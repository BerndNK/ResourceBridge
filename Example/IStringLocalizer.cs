// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Localization
{
    // ReSharper disable once UnusedTypeParameter
    public interface IStringLocalizer<T>
    {
        public string this[string key] => key;
    }
}