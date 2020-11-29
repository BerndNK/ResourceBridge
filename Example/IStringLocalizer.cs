// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Localization
{
    public interface IStringLocalizer<T>
    {
        public string this[string key] => key;
    }
}