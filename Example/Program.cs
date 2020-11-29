using System;
using BridgeSource;
using Microsoft.Extensions.Localization;

namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            var examples = new ExampleResources(new StringLocalizerDummy<ExampleResources>());
            Console.WriteLine(examples.GroupA.Hello);
            Console.WriteLine(examples.GroupA.World);

            Console.WriteLine(examples.GroupB.Child.Hello("World"));

            Console.ReadKey();
        }
    }

    internal class StringLocalizerDummy<T> : IStringLocalizer<T>
    {

    }
}
